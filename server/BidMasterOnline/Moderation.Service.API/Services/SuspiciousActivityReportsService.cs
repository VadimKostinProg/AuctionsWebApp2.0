using BidMasterOnline.Core.DTO;
using BidMasterOnline.Core.RepositoryContracts;
using BidMasterOnline.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Moderation.Service.API.Constants;
using Moderation.Service.API.DTO.Gemini;
using Moderation.Service.API.Enums;
using Moderation.Service.API.Extensions;
using Moderation.Service.API.Models;
using Moderation.Service.API.ServiceContracts;
using MongoDB.Driver;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Moderation.Service.API.Services
{
    public class SuspiciousActivityReportsService : ISuspiciousActivityReportsService
    {
        private readonly IRepository _repository;
        private readonly HttpClient _httpClient;
        private readonly ILogger<SuspiciousActivityReportsService> _logger;

        IMongoCollection<SuspiciousActivityReport> _reportsCollection;

        private readonly string _geminiApiUrl;
        private readonly string _geminiApiKey;

        public SuspiciousActivityReportsService(IRepository repository,
            IConfiguration configuration,
            ILogger<SuspiciousActivityReportsService> logger)
        {
            _geminiApiUrl = configuration["Gemini:ApiUrl"]!;
            _geminiApiKey = configuration["Gemini:ApiKey"]!;

            MongoClient client = new(configuration["ConnectionStrings:MongoDBConnection"]!);
            _reportsCollection = client.GetDatabase(configuration["MongoDBSettings:DatabaseName"]!)
                .GetCollection<SuspiciousActivityReport>("suspicious-activity-reports");

            _repository = repository;

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("x-goog-api-key", _geminiApiKey);

            _logger = logger;
        }

        public async Task<ServiceResult<SuspiciousActivityReport>> GetSuspiciousActivityReportAsync(SuspiciousActivityReportPeriod period)
        {
            ServiceResult<SuspiciousActivityReport> result = new();

            DateTime endRange = DateTime.UtcNow;
            DateTime startRange = endRange.AddDays(-1);

            SuspiciousActivityReport report = await _reportsCollection
                .Find(e => e.Period == period.ToString() && e.CreatedAt >= startRange && e.CreatedAt <= endRange)
                .FirstOrDefaultAsync();

            if (report == null)
            {
                ServiceResult<SuspiciousActivityReport> generatedReportResult = await GenerateSuspiciousActivityReportAsync(period);

                if (!generatedReportResult.IsSuccessfull)
                {
                    return generatedReportResult;
                }

                report = generatedReportResult.Data!;

                await _reportsCollection.InsertOneAsync(report);
            }

            result.Data = report;

            return result;
        }

        private async Task<ServiceResult<SuspiciousActivityReport>> GenerateSuspiciousActivityReportAsync(
            SuspiciousActivityReportPeriod period)
        {
            ServiceResult<SuspiciousActivityReport> result = new();

            try
            {
                // Prepare prompt
                List<Auction> auctions = await FetchAuctionsForSpecifiedPeriodAsync(period);

                GeminiInputPayload payload = GetGeminiInputPayload(auctions);

                string payloadSerialized = JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = false, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });

                string fullPrompt = AIPrompts.GenerateSuspiciousActivityReportPrompt.Replace("$$inputPayload$$", payloadSerialized);

                // Prepare prompt request
                GeminiRequest request = new()
                {
                    Contents = [
                        new GeminiContent
                        {
                            Parts = [new GeminiPart { Text = fullPrompt }]
                        }
                    ]
                };

                string requestSerialized = JsonSerializer.Serialize(request, new JsonSerializerOptions { WriteIndented = false });

                StringContent content = new(requestSerialized, Encoding.UTF8, "application/json");

                // Send prompt request to AI
                HttpResponseMessage response = await _httpClient.PostAsync($"{_geminiApiUrl}?key={_geminiApiKey}", content);

                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();

                GeminiResponse geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(jsonResponse)!;

                if (geminiResponse?.Candidates?.Any() == true)
                {
                    string responseText = geminiResponse.Candidates.First().Content.Parts.First().Text;

                    GeminiOutputPayload outputPayload = JsonSerializer.Deserialize<GeminiOutputPayload>(CleanGeminiJsonResponse(responseText))!;

                    SuspiciousActivityReport report = outputPayload.ToModel();
                    report.Period = period.ToString();
                    result.Data = report;
                }
                else
                {
                    result.IsSuccessfull = false;
                    result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                    result.Errors.Add("Response from AI is successfull, but it could not be read.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate suspicious activity report.");

                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                result.Errors.Add("Failed to generate suspicious activity report.");
            }

            return result;
        }

        private Task<List<Auction>> FetchAuctionsForSpecifiedPeriodAsync(SuspiciousActivityReportPeriod period)
        {
            DateTime startPeriod;
            DateTime endPeriod = DateTime.UtcNow;

            switch (period)
            {
                case SuspiciousActivityReportPeriod.LastDay:
                    startPeriod = endPeriod.AddDays(-1);
                    break;
                case SuspiciousActivityReportPeriod.LastWeek:
                    startPeriod = endPeriod.AddDays(-7);
                    break;
                default:
                    startPeriod = endPeriod.AddDays(-30);
                    break;
            }

            return _repository.GetFiltered<Auction>(e => e.Bids!.Any(b => b.CreatedAt >= startPeriod && b.CreatedAt <= endPeriod),
                    includeQuery: query => query.Include(e => e.Type)
                                                .Include(e => e.Auctioneer)
                                                .Include(e => e.Bids)!
                                                    .ThenInclude(e => e.Bidder)!)
                .ToListAsync();
        }

        private GeminiInputPayload GetGeminiInputPayload(List<Auction> auctions)
            => new()
            {
                AuctionsToAnalyze = auctions.Select(auction => new AuctionData
                {
                    AuctionDetails = new()
                    {
                        Id = auction.Id,
                        AuctioneerId = auction.AuctioneerId,
                        Auctioneer = new()
                        {
                            Id = auction.Auctioneer!.Id,
                            Username = auction.Auctioneer.Username,
                            UserRegistrationDate = auction.Auctioneer.CreatedAt,
                            TotalWins = auction.Auctioneer.TotalWins,
                            TotalAuctionsOrganized = auction.Auctioneer.TotalAuctions,
                            TotalAuctionsCompleted = auction.Auctioneer.CompletedAuctions
                        },
                        LotTitle = auction.LotTitle,
                        StartTime = auction.StartTime,
                        FinishTime = auction.FinishTime,
                        BidAmountInterval = auction.BidAmountInterval,
                        Type = auction.Type!.Name,
                        Status = auction.Status.ToString(),
                        WinnerId = auction.WinnerId,
                        Winner = auction.Winner == null
                            ? null
                            : new()
                            {
                                Id = auction.Winner.Id,
                                Username = auction.Winner.Username,
                                UserRegistrationDate = auction.Winner.CreatedAt,
                                TotalWins = auction.Winner!.TotalWins,
                                TotalAuctionsOrganized = auction.Winner.TotalAuctions,
                                TotalAuctionsCompleted = auction.Winner.CompletedAuctions
                            },
                        FinishPrice = auction.FinishPrice,
                    },
                    BidHistory = auction.Bids!.Where(b => !b.Deleted).Select(bid => new DTO.Gemini.Bid
                    {
                        Id = bid.Id,
                        BidderId = bid.BidderId,
                        Bidder = new()
                        {
                            Id = bid.Bidder!.Id,
                            Username = bid.Bidder.Username,
                            UserRegistrationDate = bid.Bidder.CreatedAt,
                            TotalWins = bid.Bidder.TotalWins,
                            TotalAuctionsOrganized = bid.Bidder.TotalAuctions,
                            TotalAuctionsCompleted = bid.Bidder.CompletedAuctions
                        },
                        Amount = bid.Amount,
                        PlacedAt = bid.CreatedAt
                    })
                    .ToList(),
                })
                .ToList()
            };

        private GeminiInputPayload GetDummyPayload()
        {
            var auctionsToAnalyze = new List<AuctionData>
            {
                new AuctionData
                {
                    AuctionDetails = new AuctionDetails
                    {
                        Id = 12345,
                        AuctioneerId = 101,
                        Auctioneer = new UserStats { Id = 101, Username = "seller_user", UserRegistrationDate = new DateTime(2024, 1, 1), TotalWins = 5, TotalAuctionsOrganized = 50, TotalAuctionsCompleted = 45 },
                        LotTitle = "Rare Comic Book #1",
                        StartTime = new DateTime(2025, 6, 1, 10, 0, 0, DateTimeKind.Utc),
                        FinishTime = new DateTime(2025, 6, 5, 15, 30, 0, DateTimeKind.Utc),
                        BidAmountInterval = 5.00m,
                        Type = "English auction",
                        Status = "Completed",
                        WinnerId = 204,
                        Winner = new UserStats { Id = 204, Username = "winner_user_X", UserRegistrationDate = new DateTime(2024, 3, 15), TotalWins = 25, TotalAuctionsOrganized = 0, TotalAuctionsCompleted = 0 },
                        FinishPrice = 250.00m
                    },
                    BidHistory = new List<DTO.Gemini.Bid>
                    {
                        new () { Id = 1, BidderId = 201, Bidder = new UserStats { Id = 201, Username = "bidder_A", UserRegistrationDate = new DateTime(2024, 2, 1), TotalWins = 2, TotalAuctionsOrganized = 0, TotalAuctionsCompleted = 0 }, Amount = 105.00m, PlacedAt = new DateTime(2025, 6, 1, 10, 5, 0, DateTimeKind.Utc) },
                        new () { Id = 2, BidderId = 202, Bidder = new UserStats { Id = 202, Username = "bidder_B", UserRegistrationDate = new DateTime(2024, 4, 1), TotalWins = 1, TotalAuctionsOrganized = 0, TotalAuctionsCompleted = 0 }, Amount = 110.00m, PlacedAt = new DateTime(2025, 6, 1, 10, 10, 0, DateTimeKind.Utc) },
                        new () { Id = 3, BidderId = 201, Bidder = new UserStats { Id = 201, Username = "bidder_A", UserRegistrationDate = new DateTime(2024, 2, 1), TotalWins = 2, TotalAuctionsOrganized = 0, TotalAuctionsCompleted = 0 }, Amount = 120.00m, PlacedAt = new DateTime(2025, 6, 1, 10, 11, 0, DateTimeKind.Utc) }, // Shill-like activity
                        new () { Id = 4, BidderId = 202, Bidder = new UserStats { Id = 202, Username = "bidder_B", UserRegistrationDate = new DateTime(2024, 4, 1), TotalWins = 1, TotalAuctionsOrganized = 0, TotalAuctionsCompleted = 0 }, Amount = 125.00m, PlacedAt = new DateTime(2025, 6, 1, 10, 11, 30, DateTimeKind.Utc) }, // Ping-pong
                        new () { Id = 5, BidderId = 203, Bidder = new UserStats { Id = 203, Username = "bidder_C", UserRegistrationDate = new DateTime(2024, 1, 1), TotalWins = 10, TotalAuctionsOrganized = 5, TotalAuctionsCompleted = 4 }, Amount = 240.00m, PlacedAt = new DateTime(2025, 6, 5, 15, 29, 50, DateTimeKind.Utc) },
                        new () { Id = 6, BidderId = 204, Bidder = new UserStats { Id = 204, Username = "winner_user_X", UserRegistrationDate = new DateTime(2024, 3, 15), TotalWins = 25, TotalAuctionsOrganized = 0, TotalAuctionsCompleted = 0 }, Amount = 245.00m, PlacedAt = new DateTime(2025, 6, 5, 15, 29, 58, DateTimeKind.Utc) }, // Sniping
                        new () { Id = 7, BidderId = 204, Bidder = new UserStats { Id = 204, Username = "winner_user_X", UserRegistrationDate = new DateTime(2024, 3, 15), TotalWins = 25, TotalAuctionsOrganized = 0, TotalAuctionsCompleted = 0 }, Amount = 250.00m, PlacedAt = new DateTime(2025, 6, 5, 15, 29, 59, DateTimeKind.Utc) }  // Winning Sniping
                    }
                    .OrderByDescending(x => x.Id).ToList()
                },
                new AuctionData
                {
                    AuctionDetails = new AuctionDetails
                    {
                        Id = 12346,
                        AuctioneerId = 101,
                        Auctioneer = new UserStats { Id = 101, Username = "seller_user", UserRegistrationDate = new DateTime(2024, 1, 1), TotalWins = 5, TotalAuctionsOrganized = 50, TotalAuctionsCompleted = 45 },
                        LotTitle = "Rare Stamp Collection",
                        StartTime = new DateTime(2025, 6, 2, 9, 0, 0, DateTimeKind.Utc),
                        FinishTime = new DateTime(2025, 6, 6, 14, 0, 0, DateTimeKind.Utc),
                        BidAmountInterval = 10.00m,
                        Type = "English auction",
                        Status = "Active",
                        WinnerId = null,
                        Winner = null,
                        FinishPrice = null
                    },
                    BidHistory = new List<DTO.Gemini.Bid>
                    {
                        new () { Id = 10, BidderId = 201, Bidder = new UserStats { Id = 201, Username = "bidder_A", UserRegistrationDate = new DateTime(2024, 2, 1), TotalWins = 2, TotalAuctionsOrganized = 0, TotalAuctionsCompleted = 0 }, Amount = 80.00m, PlacedAt = new DateTime(2025, 6, 2, 9, 11, 0, DateTimeKind.Utc) },
                        new () { Id = 9, BidderId = 205, Bidder = new UserStats { Id = 205, Username = "bidder_D", UserRegistrationDate = new DateTime(2025, 5, 20), TotalWins = 0, TotalAuctionsOrganized = 0, TotalAuctionsCompleted = 0 }, Amount = 70.00m, PlacedAt = new DateTime(2025, 6, 2, 9, 10, 0, DateTimeKind.Utc) }, // New user aggressive
                        new () { Id = 8, BidderId = 201, Bidder = new UserStats { Id = 201, Username = "bidder_A", UserRegistrationDate = new DateTime(2024, 2, 1), TotalWins = 2, TotalAuctionsOrganized = 0, TotalAuctionsCompleted = 0 }, Amount = 60.00m, PlacedAt = new DateTime(2025, 6, 2, 9, 5, 0, DateTimeKind.Utc) },
                    }
                }
            };

            return new()
            {
                AuctionsToAnalyze = auctionsToAnalyze
            };
        }

        private string CleanGeminiJsonResponse(string rawResponseText)
        {
            string cleaned = rawResponseText.Trim();

            
            if (cleaned.StartsWith("```json", StringComparison.OrdinalIgnoreCase))
            {
                cleaned = Regex.Replace(cleaned, @"^```json\s*\n?", "", RegexOptions.IgnoreCase).TrimStart();
            }
            if (cleaned.EndsWith("```"))
            {
                cleaned = Regex.Replace(cleaned, @"\n?s*```$", "").TrimEnd();
            }

            return cleaned;
        }
    }
}

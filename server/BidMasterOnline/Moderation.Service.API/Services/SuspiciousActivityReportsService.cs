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

            SuspiciousActivityReport? report = null;

            if (period != SuspiciousActivityReportPeriod.LastDay)
            {
                report = await _reportsCollection
                    .Find(e => e.Period == period.ToString() && e.CreatedAt >= startRange && e.CreatedAt <= endRange)
                    .FirstOrDefaultAsync();
            }

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

                if (!auctions.Any())
                {
                    result.IsSuccessfull = false;
                    result.StatusCode = System.Net.HttpStatusCode.NotFound;
                    result.Errors.Add("Could not find any auction within selected period.");

                    return result;
                }

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
                        AimPrice = auction.AimPrice,
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

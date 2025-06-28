using Auctions.Service.API.ServiceContracts.Participant;
using BidMasterOnline.Core.RepositoryContracts;
using BidMasterOnline.Domain.Enums;
using BidMasterOnline.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace Auctions.Service.API.BackgroundJobs
{
    public class AuctionsStatusCheckBackgroundJob : IJob
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IRepository _repository;
        private readonly ILogger<AuctionsStatusCheckBackgroundJob> _logger;

        public AuctionsStatusCheckBackgroundJob(IServiceScopeFactory scopeFactory,
            IRepository repository,
            ILogger<AuctionsStatusCheckBackgroundJob> logger)
        {
            _scopeFactory = scopeFactory;
            _repository = repository;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("AuctionsStatusCheckBackgroundService: started work...");

            try
            {
                List<long> finishedAuctionIds = await _repository
                    .GetFiltered<Auction>(e => e.Status == AuctionStatus.Active && e.FinishTime <= DateTime.UtcNow)
                    .Select(e => e.Id)
                    .ToListAsync();

                List<long> pendingAuctionIds = await _repository
                    .GetFiltered<Auction>(e => e.Status == AuctionStatus.Pending && e.StartTime <= DateTime.UtcNow)
                    .Select(e => e.Id)
                    .ToListAsync();

                string message = $"AuctionsStatusCheckBackgroundService: " +
                    $"fetched {finishedAuctionIds.Count} finished and {pendingAuctionIds} pending auctions.";

                if (finishedAuctionIds.Count > 0 || pendingAuctionIds.Count > 0)
                {
                    message += " Started chenging the status of auctions...";
                }
                else
                {
                    message += " Skiped changing auctions status for now.";
                }

                _logger.LogInformation(message);

                if (finishedAuctionIds.Count > 0)
                {
                    int succeededAuctions = 0;
                    int failedAuctions = 0;

                    await Parallel.ForEachAsync(finishedAuctionIds, async (auctionId, token) =>
                    {
                        using IServiceScope scope = _scopeFactory.CreateScope();

                        IAuctionsService service = scope.ServiceProvider.GetRequiredService<IAuctionsService>();

                        if (await service.FinishAuctionAsync(auctionId, token))
                            Interlocked.Increment(ref succeededAuctions);
                        else
                            Interlocked.Increment(ref failedAuctions);
                    });

                    _logger.LogInformation($"AuctionsStatusCheckBackgroundService: successfully changed status for " +
                        $"{succeededAuctions} of {finishedAuctionIds.Count} finishing auctions. " +
                        $"Failed {failedAuctions} of {finishedAuctionIds.Count} finishing auctions.");
                }

                if (pendingAuctionIds.Count > 0)
                {
                    int succeededAuctions = 0;
                    int failedAuctions = 0;

                    await Parallel.ForEachAsync(pendingAuctionIds, async (auctionId, token) =>
                    {
                        using IServiceScope scope = _scopeFactory.CreateScope();

                        IAuctionsService service = scope.ServiceProvider.GetRequiredService<IAuctionsService>();

                        if (await service.StartPendingAuctionAsync(auctionId))
                            Interlocked.Increment(ref succeededAuctions);
                        else
                            Interlocked.Increment(ref failedAuctions);
                    });

                    _logger.LogInformation($"AuctionsStatusCheckBackgroundService: successfully changed status for " +
                        $"{succeededAuctions} of {finishedAuctionIds.Count} pending auctions. " +
                        $"Failed {failedAuctions} of {finishedAuctionIds.Count} pending auctions.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured in AuctionsStatusCheckBackgroundJob.");
            }
        }
    }
}

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
        private readonly IRepository _repository;
        private readonly IAuctionsService _service;
        private readonly ILogger<AuctionsStatusCheckBackgroundJob> _logger;

        public AuctionsStatusCheckBackgroundJob(IRepository repository,
            IAuctionsService service,
            ILogger<AuctionsStatusCheckBackgroundJob> logger)
        {
            _repository = repository;
            _service = service;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("AuctionsStatusCheckBackgroundService: started work...");

            List<long> finishedAuctionIds = await _repository
                .GetFiltered<Auction>(e => e.Status == AuctionStatus.Active && e.FinishTime >= DateTime.UtcNow)
                .Select(e => e.Id)
                .ToListAsync();

            List<long> pendingAuctionIds = await _repository
                .GetFiltered<Auction>(e => e.Status == AuctionStatus.Pending && e.StartTime >= DateTime.UtcNow)
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
                    if (await _service.FinishAuctionAsync(auctionId, token))
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
                    if (await _service.StartPendingAuctionAsync(auctionId))
                        Interlocked.Increment(ref succeededAuctions);
                    else
                        Interlocked.Increment(ref failedAuctions);
                });

                _logger.LogInformation($"AuctionsStatusCheckBackgroundService: successfully changed status for " +
                    $"{succeededAuctions} of {finishedAuctionIds.Count} pending auctions. " +
                    $"Failed {failedAuctions} of {finishedAuctionIds.Count} pending auctions.");
            }
        }
    }
}

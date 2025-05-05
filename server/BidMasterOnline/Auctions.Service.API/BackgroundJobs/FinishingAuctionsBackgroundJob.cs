using Auctions.Service.API.ServiceContracts.Participant;
using BidMasterOnline.Core.RepositoryContracts;
using BidMasterOnline.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace Auctions.Service.API.BackgroundJobs
{
    public class FinishingAuctionsBackgroundJob : IJob
    {
        private readonly IRepository _repository;
        private readonly IParticipantAuctionsService _service;

        public FinishingAuctionsBackgroundJob(IRepository repository, IParticipantAuctionsService service)
        {
            _repository = repository;
            _service = service;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            List<long> finishedAuctionIds = await _repository
                .GetFiltered<Auction>(e => e.FinishTime >= DateTime.UtcNow)
                .Select(e => e.Id)
                .ToListAsync();

            await Parallel.ForEachAsync(finishedAuctionIds, async (auctionId, token) =>
            {
                await _service.FinishAuctionAsync(auctionId); 
            });
        }
    }
}

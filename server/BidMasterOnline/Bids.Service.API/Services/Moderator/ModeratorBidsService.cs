using BidMasterOnline.Core.DTO;
using BidMasterOnline.Core.RepositoryContracts;
using Bids.Service.API.DTO.Moderator;
using Bids.Service.API.ServiceContracts.Moderator;

namespace Bids.Service.API.Services.Moderator
{
    public class ModeratorBidsService : IModeratorBidsService
    {
        private readonly IRepository _repository;
        private readonly ILogger<ModeratorBidsService> _logger;

        public ModeratorBidsService(IRepository repository, ILogger<ModeratorBidsService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<bool> ClearAllBidsForAuctionAsync(long auctionId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult<PaginatedList<AuctionBidDTO>>> GetAuctionBidsAsync(long auctionId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult<PaginatedList<UserBidDTO>>> GetUserBidsAsync(long userId)
        {
            throw new NotImplementedException();
        }
    }
}

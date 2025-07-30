using Auctions.Service.API.DTO.Participant;
using Auctions.Service.API.Extensions;
using Auctions.Service.API.ServiceContracts.Participant;
using BidMasterOnline.Core.DTO;
using BidMasterOnline.Core.RepositoryContracts;
using BidMasterOnline.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Auctions.Service.API.Services.Participant
{
    public class AuctionFinishMethodsService : IAuctionFinishMethodsService
    {
        private readonly IRepository _repository;

        public AuctionFinishMethodsService(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResult<List<AuctionFinishMethodDTO>>> GetAuctionFinishMethodsAsync()
        {
            ServiceResult<List<AuctionFinishMethodDTO>> result = new();

            List<AuctionFinishMethod> entities = await _repository.GetFiltered<AuctionFinishMethod>(e => !e.Deleted)
                .OrderBy(e => e.Name)
                .ToListAsync();

            result.Data = entities.Select(e => e.ToParticipantDTO()).ToList();

            return result;
        }
    }
}

using Auctions.Service.API.DTO.Participant;
using Auctions.Service.API.Extensions;
using Auctions.Service.API.ServiceContracts.Participant;
using BidMasterOnline.Core.DTO;
using BidMasterOnline.Core.RepositoryContracts;
using BidMasterOnline.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Auctions.Service.API.Services.Participant
{
    public class AuctionTypesService : IAuctionTypesService
    {
        private readonly IRepository _repository;

        public AuctionTypesService(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResult<List<AuctionTypeDTO>>> GetAuctionTypesAsync()
        {
            ServiceResult<List<AuctionTypeDTO>> result = new();

            List<AuctionType> entities = await _repository.GetFiltered<AuctionType>(e => !e.Deleted)
                .OrderBy(e => e.Name)
                .ToListAsync();

            result.Data = entities.Select(e => e.ToParticipantDTO()).ToList();

            return result;
        }
    }
}

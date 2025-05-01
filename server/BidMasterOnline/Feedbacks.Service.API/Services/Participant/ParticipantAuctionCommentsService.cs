using BidMasterOnline.Core.DTO;
using BidMasterOnline.Core.RepositoryContracts;
using BidMasterOnline.Core.Specifications;
using BidMasterOnline.Domain.Models;
using BidMasterOnline.Domain.Models.Entities;
using Feedbacks.Service.API.DTO.Participant;
using Feedbacks.Service.API.Extensions;
using Feedbacks.Service.API.ServiceContracts.Participant;

namespace Feedbacks.Service.API.Services.Participant
{
    public class ParticipantAuctionCommentsService : IParticipantAuctionCommentsService
    {
        private readonly IRepository _repository;
        private readonly ILogger<ParticipantAuctionCommentsService> _logger;

        public ParticipantAuctionCommentsService(IRepository repository,
            ILogger<ParticipantAuctionCommentsService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ServiceResult<PaginatedList<ParticipantAuctionCommentDTO>>> GetAuctionCommentsAsync(long auctionId, PaginationRequestDTO pagination)
        {
            ServiceResult<PaginatedList<ParticipantAuctionCommentDTO>> result = new();

            ISpecification<AuctionComment> specification = new SpecificationBuilder<AuctionComment>()
                .With(e => e.AuctionId == auctionId)
                .WithPagination(pagination.PageSize, pagination.PageNumber)
                .OrderBy(e => e.CreatedAt, BidMasterOnline.Core.Enums.SortDirection.DESC)
                .Build();

            ListModel<AuctionComment> entitiesList = await _repository.GetFilteredAndPaginated(specification);

            result.Data = new PaginatedList<ParticipantAuctionCommentDTO>
            {
                Items = entitiesList.Items.Select(e => e.ToParticipantDTO()).ToList(),
                Pagination = new()
                {
                    PageSize = entitiesList.PageSize,
                    CurrentPage = entitiesList.CurrentPage,
                    TotalPages = entitiesList.TotalPages,
                    TotalCount = entitiesList.TotalCount                    
                }
            };

            return result;
        }

        public async Task<ServiceResult> PostAuctionCommentAsync(ParticipantPostCommentDTO comment)
        {
            ServiceResult result = new();

            try
            {
                AuctionComment entity = comment.ToDomain();

                await _repository.AddAsync(entity);
                await _repository.SaveChangesAsync();

                //TODO: recalculate auction average score
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured during posting the auction comment.");

                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                result.Errors.Add("An error occured during posting the auction comment.");
            }

            return result;
        }
    }
}

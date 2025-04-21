using Auctions.Service.API.DTO;
using Auctions.Service.API.DTO.Moderator;
using Auctions.Service.API.Extensions;
using Auctions.Service.API.ServiceContracts.Moderator;
using BidMasterOnline.Core.DTO;
using BidMasterOnline.Core.RepositoryContracts;
using BidMasterOnline.Core.ServiceContracts;
using BidMasterOnline.Core.Specifications;
using BidMasterOnline.Domain.Enums;
using BidMasterOnline.Domain.Models;
using BidMasterOnline.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore.Storage;
using System.Net;

namespace Auctions.Service.API.Services.Moderator
{
    public class ModeratorAuctionRequestsService : IModeratorAuctionRequestsService
    {
        private readonly IRepository _repository;
        private readonly ITransactionsService _transactionsService;
        private readonly ILogger<ModeratorAuctionRequestsService> _logger;

        public ModeratorAuctionRequestsService(IRepository repository,
            ITransactionsService transactionsService, 
            ILogger<ModeratorAuctionRequestsService> logger)
        {
            _repository = repository;
            _transactionsService = transactionsService;
            _logger = logger;
        }

        public async Task<bool> ApproveAuctionRequestAsync(long auctionRequestId)
        {
            IDbContextTransaction transaction = _transactionsService.BeginTransaction();

            try
            {
                AuctionRequest auctionRequest = await _repository.GetByIdAsync<AuctionRequest>(auctionRequestId);

                if (auctionRequest.Status != AuctionRequestStatus.Pending)
                {
                    await transaction.RollbackAsync();

                    return false;
                }

                auctionRequest.Status = AuctionRequestStatus.Approved;

                DateTime auctionStartTime = auctionRequest.RequestedStartTime ?? DateTime.UtcNow;

                Auction newAuction = new()
                {
                    AuctionistId = auctionRequest.RequestedByUserId,
                    AuctionCategoryId = auctionRequest.AuctionCategoryId,
                    AuctionTypeId = auctionRequest.AuctionTypeId,
                    AuctionFinishMethodId = auctionRequest.AuctionFinishMethodId,
                    LotTitle = auctionRequest.LotTitle,
                    LotDescription = auctionRequest.LotDescription,
                    StartPrice = auctionRequest.StartPrice,
                    CurrentPrice = auctionRequest.StartPrice,
                    FinishTimeIntervalInTicks = auctionRequest.FinishTimeIntervalInTicks,
                    BidAmountInterval = auctionRequest.BidAmountInterval,
                    StartTime = auctionStartTime,
                    FinishTime = auctionStartTime.AddTicks(auctionRequest.RequestedAuctionTimeInTicks),
                    Status = auctionRequest.RequestedStartTime == null
                        ? AuctionStatus.Active
                        : AuctionStatus.Pending,
                };

                await _repository.AddAsync(newAuction);
                await _repository.SaveChangesAsync();

                // TODO: notify auctionist

                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "An error occured during declining auction request.");

                return false;
            }
        }

        public async Task<bool> DeclineAuctionRequestAsync(long auctionRequestId, string reason)
        {
            IDbContextTransaction transaction = _transactionsService.BeginTransaction();

            try
            {
                AuctionRequest auctionRequest = await _repository.GetByIdAsync<AuctionRequest>(auctionRequestId);

                if (auctionRequest.Status != AuctionRequestStatus.Pending)
                {
                    return false;
                }

                auctionRequest.Status = AuctionRequestStatus.Declined;
                auctionRequest.ReasonDeclined = reason;

                _repository.Update(auctionRequest);

                await _repository.SaveChangesAsync();

                // TODO: notify auctionist

                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "An error occured during declining auction request.");

                return false;
            }
        }

        public async Task<ServiceResult<PaginatedList<AuctionRequestSummaryDTO>>> GetAllAuctionRequestAsync(AuctionRequestSpecificationsDTO specifications)
        {
            ServiceResult<PaginatedList<AuctionRequestSummaryDTO>> result = new();

            ISpecification<AuctionRequest> specification = new SpecificationBuilder<AuctionRequest>()
                .With(e => e.Status == specifications.Status)
                .OrderBy(e => e.CreatedAt, specifications.SortDirection)
                .WithPagination(specifications.PageSize, specifications.PageNumber)
                .Build();

            ListModel<AuctionRequest> auctionRequestsList = await _repository.GetFilteredAndPaginated(specification);

            result.Data = new PaginatedList<AuctionRequestSummaryDTO>
            {
                Items = auctionRequestsList.Items.Select(e => e.ToModeratorSummaryDTO()).ToList(),
                Pagination = new()
                {
                    TotalCount = auctionRequestsList.TotalCount,
                    TotalPages = auctionRequestsList.TotalPages,
                    CurrentPage = auctionRequestsList.CurrentPage,
                    PageSize = auctionRequestsList.PageSize
                }
            };

            return result;
        }

        public async Task<ServiceResult<AuctionRequestDTO>> GetAuctionRequestById(long id)
        {
            AuctionRequest? auctionRequest = await _repository
                .GetFirstOrDefaultAsync<AuctionRequest>(x => x.Id == id);

            ServiceResult<AuctionRequestDTO> result = new();

            if (auctionRequest == null)
            {
                result.StatusCode = HttpStatusCode.NotFound;
                result.Errors.Add("Auction request not found.");

                return result;
            }

            result.Data = auctionRequest.ToModeratorDTO();

            return result;
        }
    }
}

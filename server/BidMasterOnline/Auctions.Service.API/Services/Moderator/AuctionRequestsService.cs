using Auctions.Service.API.DTO.Moderator;
using Auctions.Service.API.Extensions;
using Auctions.Service.API.GrpcServices.Client;
using Auctions.Service.API.ServiceContracts;
using Auctions.Service.API.ServiceContracts.Moderator;
using BidMasterOnline.Core.DTO;
using BidMasterOnline.Core.Enums;
using BidMasterOnline.Core.Extensions;
using BidMasterOnline.Core.RepositoryContracts;
using BidMasterOnline.Core.ServiceContracts;
using BidMasterOnline.Core.Specifications;
using BidMasterOnline.Domain.Enums;
using BidMasterOnline.Domain.Models;
using BidMasterOnline.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Net;

namespace Auctions.Service.API.Services.Moderator
{
    public class AuctionRequestsService : IAuctionRequestsService
    {
        private readonly IRepository _repository;
        private readonly ITransactionsService _transactionsService;
        private readonly ILogger<AuctionRequestsService> _logger;
        private readonly ModerationGrpcClient _moderationClient;
        private readonly INotificationsService _notificationsService;

        public AuctionRequestsService(IRepository repository,
            ITransactionsService transactionsService,
            ILogger<AuctionRequestsService> logger,
            ModerationGrpcClient moderationClient,
            INotificationsService notificationsService)
        {
            _repository = repository;
            _transactionsService = transactionsService;
            _logger = logger;
            _moderationClient = moderationClient;
            _notificationsService = notificationsService;
        }

        public async Task<ServiceResult> ApproveAuctionRequestAsync(ApproveAuctionRequestDTO requestDTO)
        {
            ServiceResult result = new();

            IDbContextTransaction transaction = _transactionsService.BeginTransaction();

            try
            {
                AuctionRequest auctionRequest = await _repository.GetByIdAsync<AuctionRequest>(requestDTO.AuctionRequestId,
                    includeQuery: query => query.Include(e => e.Images)!);

                if (auctionRequest.Status != AuctionRequestStatus.Pending)
                {
                    await transaction.RollbackAsync();

                    result.IsSuccessfull = false;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    result.Errors.Add("Auction request has been already approved or declined.");
                    return result;
                }

                auctionRequest.Status = AuctionRequestStatus.Approved;
                _repository.Update(auctionRequest);

                User auctionist = await _repository.GetByIdAsync<User>(auctionRequest.RequestedByUserId);
                auctionist.TotalAuctions++;
                _repository.Update(auctionist);

                DateTime auctionStartTime = auctionRequest.RequestedStartTime.HasValue && auctionRequest.RequestedStartTime > DateTime.UtcNow
                    ? auctionRequest.RequestedStartTime.Value
                    : DateTime.UtcNow;

                Auction newAuction = new()
                {
                    AuctioneerId = auctionRequest.RequestedByUserId,
                    AuctionCategoryId = auctionRequest.AuctionCategoryId,
                    AuctionTypeId = auctionRequest.AuctionTypeId,
                    AuctionFinishMethodId = auctionRequest.AuctionFinishMethodId,
                    LotTitle = auctionRequest.LotTitle,
                    LotDescription = auctionRequest.LotDescription,
                    StartPrice = auctionRequest.StartPrice,
                    CurrentPrice = auctionRequest.StartPrice,
                    FinishTimeIntervalInTicks = auctionRequest.FinishTimeIntervalInTicks,
                    BidAmountInterval = auctionRequest.BidAmountInterval,
                    AimPrice = auctionRequest.AimPrice,
                    AuctionTimeInTicks = auctionRequest.RequestedAuctionTimeInTicks,
                    StartTime = auctionStartTime,
                    FinishTime = auctionStartTime.AddTicks(auctionRequest.RequestedAuctionTimeInTicks),
                    Status = auctionRequest.RequestedStartTime == null || auctionRequest.RequestedStartTime <= auctionStartTime
                        ? AuctionStatus.Active
                        : AuctionStatus.Pending,
                    Images = auctionRequest.Images?.Select(image => new AuctionImage
                    {
                        PublicId = image.PublicId,
                        Url = image.Url
                    })
                    .ToList()
                };

                await _repository.AddAsync(newAuction);
                await _repository.SaveChangesAsync();

                await transaction.CommitAsync();

                await _notificationsService.SendMessageOfApprovalAuctionRequestToAuctioneer(auctionRequest);

                result.Message = "Auction request has been approved successfully.";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "An error occured during approving auction request.");

                result.IsSuccessfull = false;
                result.StatusCode = HttpStatusCode.InternalServerError;
                result.Errors.Add("An error occured during approving auction request.");
            }

            await _moderationClient.LogModerationAction(ModerationAction.ApprovingAuctionRequest, requestDTO.AuctionRequestId);

            return result;
        }

        public async Task<ServiceResult> DeclineAuctionRequestAsync(DeclineAuctionRequestDTO requestDTO)
        {
            ServiceResult result = new();

            try
            {
                AuctionRequest auctionRequest = await _repository.GetByIdAsync<AuctionRequest>(requestDTO.AuctionRequestId);

                if (auctionRequest.Status != AuctionRequestStatus.Pending)
                {
                    result.IsSuccessfull = false;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    result.Errors.Add("Auction request has been already approved or declined.");
                    return result;
                }

                auctionRequest.Status = AuctionRequestStatus.Declined;
                auctionRequest.ReasonDeclined = requestDTO.Reason;

                _repository.Update(auctionRequest);

                await _repository.SaveChangesAsync();

                await _notificationsService.SendMessageOfDecliningAuctionRequestToUser(auctionRequest);

                result.Message = "Auction request has been successfully declined!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured during declining auction request.");

                result.IsSuccessfull = false;
                result.StatusCode = HttpStatusCode.InternalServerError;
                result.Errors.Add("An error occured during declining auction request.");
            }

            await _moderationClient.LogModerationAction(ModerationAction.DecliningAuctionRequest, requestDTO.AuctionRequestId);

            return result;
        }

        public async Task<ServiceResult<PaginatedList<AuctionRequestSummaryDTO>>> GetAllAuctionRequestAsync(
            AuctionRequestSpecificationsDTO specifications)
        {
            ServiceResult<PaginatedList<AuctionRequestSummaryDTO>> result = new();

            SpecificationBuilder<AuctionRequest> specificationBuilder = new SpecificationBuilder<AuctionRequest>();

            specificationBuilder
                .With(e => e.Status == specifications.Status)
                .OrderBy(e => e.CreatedAt, SortDirection.DESC)
                .WithPagination(specifications.PageSize, specifications.PageNumber);

            if (!string.IsNullOrEmpty(specifications.SearchTerm))
                specificationBuilder.With(e => e.LotTitle.Contains(specifications.SearchTerm) || 
                                               e.LotDescription.Contains(specifications.SearchTerm));

            ListModel<AuctionRequest> auctionRequestsList = await _repository.GetFilteredAndPaginated(specificationBuilder.Build(),
                includeQuery: query => query.Include(e => e.Images)!);

            result.Data = auctionRequestsList.ToPaginatedList(e => e.ToModeratorSummaryDTO());

            return result;
        }

        public async Task<ServiceResult<AuctionRequestDTO>> GetAuctionRequestById(long id)
        {
            AuctionRequest? auctionRequest = await _repository
                .GetFirstOrDefaultAsync<AuctionRequest>(x => x.Id == id,
                    includeQuery: query => query.Include(e => e.Category)
                                                .Include(e => e.Type)
                                                .Include(e => e.FinishMethod)
                                                .Include(e => e.RequestedByUser)
                                                .Include(e => e.Images)!);

            ServiceResult<AuctionRequestDTO> result = new();

            if (auctionRequest == null)
            {
                result.StatusCode = HttpStatusCode.NotFound;
                result.IsSuccessfull = false;
                result.Errors.Add("Auction request not found.");

                return result;
            }

            result.Data = auctionRequest.ToModeratorDTO();

            return result;
        }
    }
}

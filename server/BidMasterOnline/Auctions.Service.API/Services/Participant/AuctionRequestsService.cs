using Auctions.Service.API.DTO.Participant;
using Auctions.Service.API.Extensions;
using Auctions.Service.API.ServiceContracts.Participant;
using BidMasterOnline.Core.DTO;
using BidMasterOnline.Core.Enums;
using BidMasterOnline.Core.Extensions;
using BidMasterOnline.Core.RepositoryContracts;
using BidMasterOnline.Core.ServiceContracts;
using BidMasterOnline.Core.Specifications;
using BidMasterOnline.Domain.Enums;
using BidMasterOnline.Domain.Models;
using BidMasterOnline.Domain.Models.Entities;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Net;

namespace Auctions.Service.API.Services.Participant
{
    public class AuctionRequestsService : IAuctionRequestsService
    {
        private readonly IRepository _repository;
        private readonly IUserAccessor _userAccessor;
        private readonly IImagesService _imagesService;
        private readonly ITransactionsService _transactionsService;
        private readonly ILogger<AuctionRequestsService> _logger;

        public AuctionRequestsService(IRepository repository,
            IUserAccessor userAccessor,
            IImagesService imagesService,
            ITransactionsService transactionsService,
            ILogger<AuctionRequestsService> logger)
        {
            _repository = repository;
            _userAccessor = userAccessor;
            _imagesService = imagesService;
            _transactionsService = transactionsService;
            _logger = logger;
        }

        public async Task<ServiceResult<PaginatedList<AuctionRequestSummaryDTO>>> GetUserAuctionRequestsAsync(PaginationRequestDTO pagination)
        {
            ServiceResult<PaginatedList<AuctionRequestSummaryDTO>> result = new();

            long userId = _userAccessor.UserId;

            ISpecification<AuctionRequest> specification = new SpecificationBuilder<AuctionRequest>()
                .With(e => e.RequestedByUserId == userId)
                .WithPagination(pagination.PageSize, pagination.PageNumber)
                .Build();

            ListModel<AuctionRequest> auctionRequestsList = await _repository.GetFilteredAndPaginated(specification,
                includeQuery: query => query.Include(e => e.Images)!);

            result.Data = auctionRequestsList.ToPaginatedList(e => e.ToParticipantSummaryDTO());

            return result;
        }

        public async Task<ServiceResult<AuctionRequestDTO>> GetAuctionRequestByIdAsync(long id)
        {
            long userId = _userAccessor.UserId;

            AuctionRequest? auctionRequest = await _repository
                .GetFirstOrDefaultAsync<AuctionRequest>(x => x.Id == id && x.RequestedByUserId == userId,
                    includeQuery: query => query.Include(e => e.Category)
                                                .Include(e => e.Type)
                                                .Include(e => e.FinishMethod)
                                                .Include(e => e.Images)!);

            ServiceResult<AuctionRequestDTO> result = new();

            if (auctionRequest == null)
            {
                result.StatusCode = HttpStatusCode.NotFound;
                result.IsSuccessfull = false;
                result.Errors.Add("Auction request not found or you have not permission to access it.");

                return result;
            }

            result.Data = auctionRequest.ToParticipantDTO();

            return result;
        }

        public async Task<ServiceResult> PostAuctionRequestAsync(PostAuctionRequestDTO requestDTO)
        {
            ServiceResult result = new();

            IDbContextTransaction transaction = _transactionsService.BeginTransaction();

            try
            {
                AuctionRequest entity = requestDTO.ToParticipantDomain();
                entity.Status = AuctionRequestStatus.Pending;
                entity.RequestedByUserId = _userAccessor.UserId;

                await _repository.AddAsync(entity);
                await _repository.SaveChangesAsync();

                foreach (IFormFile image in requestDTO.Images)
                {
                    ImageUploadResult uploadResult = await _imagesService.AddImageAsync(image, ImageType.ImageForAuction);

                    if (uploadResult.StatusCode != HttpStatusCode.OK)
                        throw new ArgumentException();

                    AuctionImage auctionImage = new()
                    {
                        AuctionRequestId = entity.Id,
                        PublicId = uploadResult.PublicId,
                        Url = uploadResult.Url.AbsoluteUri
                    };

                    await _repository.AddAsync(auctionImage);
                }

                await _repository.SaveChangesAsync();

                await transaction.CommitAsync();

                result.Message = "Your auction request has been submitted!";
            }
            catch (Exception ex)
            {
                // TODO: handle image deleting
                await transaction.RollbackAsync();

                _logger.LogError(ex, "Something went wrong during processing your request.");

                result.StatusCode = HttpStatusCode.BadRequest;
                result.IsSuccessfull = false;
                result.Errors.Add("Something went wrong during processing your request.");
            }

            return result;
        }

        public async Task<ServiceResult> CancelAuctionRequestByIdAsync(long id)
        {
            ServiceResult result = new();

            long userId = _userAccessor.UserId;

            AuctionRequest? entity = await _repository
                .GetFirstOrDefaultAsync<AuctionRequest>(x => x.Id == id && x.RequestedByUserId == userId);

            if (entity == null)
            {
                result.StatusCode = HttpStatusCode.NotFound;
                result.IsSuccessfull = false;
                result.Errors.Add("Auction request not found or you have not permission to access it.");
            }
            else if (entity.Status == AuctionRequestStatus.CanceledByUser)
            {
                result.StatusCode = HttpStatusCode.BadRequest;
                result.IsSuccessfull = false;
                result.Errors.Add("Auction request has been already cancelled before.");
            }
            else if (entity.Status != AuctionRequestStatus.Pending)
            {
                result.StatusCode = HttpStatusCode.BadRequest;
                result.IsSuccessfull = false;
                result.Errors.Add("Could not cancel this auction request, as it has been already approved or rejected.");
            }
            else
            {
                entity.Status = AuctionRequestStatus.CanceledByUser;
                _repository.Update(entity);

                await _repository.SaveChangesAsync();

                result.Message = "Auction request has been cancelled successfully!";
            }

            return result;
        }
    }
}

using Auctions.Service.API.DTO;
using Auctions.Service.API.Extensions;
using Auctions.Service.API.ServiceContracts.Participant;
using BidMasterOnline.Core.DTO;
using BidMasterOnline.Core.Enums;
using BidMasterOnline.Core.RepositoryContracts;
using BidMasterOnline.Core.ServiceContracts;
using BidMasterOnline.Domain.Entities;
using BidMasterOnline.Domain.Enums;
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

        public AuctionRequestsService(IRepository repository,
            IUserAccessor userAccessor,
            IImagesService imagesService,
            ITransactionsService transactionsService)
        {
            _repository = repository;
            _userAccessor = userAccessor;
            _imagesService = imagesService;
            _transactionsService = transactionsService;
        }

        public async Task<ServiceResult<List<AuctionRequestSummaryDTO>>> GetUserAuctionRequests()
        {
            long userId = _userAccessor.UserId;

            List<AuctionRequest> auctionRequests = await _repository.GetFiltered<AuctionRequest>(x => x.RequestedByUserId == userId)
                .ToListAsync();

            ServiceResult<List<AuctionRequestSummaryDTO>> result = new()
            {
                Data = auctionRequests.Select(e => e.ToSummaryDTO()).ToList(),
            };

            return result;
        }

        public async Task<ServiceResult<AuctionRequestDTO>> GetUserAuctionRequestById(long id)
        {
            long userId = _userAccessor.UserId;

            AuctionRequest? auctionRequest = await _repository
                .GetFirstOrDefaultAsync<AuctionRequest>(x => x.Id == id && x.RequestedByUserId == userId);

            ServiceResult<AuctionRequestDTO> result = new();

            if (auctionRequest == null)
            {
                result.StatusCode = HttpStatusCode.NotFound;
                result.Errors.Add("Auction request not found or you have not permission to access it.");

                return result;
            }

            result.Data = auctionRequest.ToDTO();

            return result;
        }

        public async Task<ServiceResult> PostAuctionRequest(PostAuctionRequestDTO requestDTO)
        {
            ServiceResult result = new();

            IDbContextTransaction transaction = _transactionsService.BeginTransaction();

            try
            {
                AuctionRequest entity = requestDTO.ToDomain();
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
            catch (Exception)
            {
                // TODO: handle image deleting
                await transaction.RollbackAsync();

                result.StatusCode = HttpStatusCode.BadRequest;
                result.Errors.Add("Something went wrong during processing your request.");
            }

            return result;
        }

        public async Task<ServiceResult> CancelAuctionRequestById(long id)
        {
            ServiceResult result = new();

            long userId = _userAccessor.UserId;

            AuctionRequest? entity = await _repository
                .GetFirstOrDefaultAsync<AuctionRequest>(x => x.Id == id && x.RequestedByUserId == userId);

            if (entity == null)
            {
                result.StatusCode = HttpStatusCode.NotFound;
                result.Errors.Add("Auction request not found or you have not permission to access it.");
            }
            else if (entity.Status == AuctionRequestStatus.CanceledByUser)
            {
                result.StatusCode = HttpStatusCode.BadRequest;
                result.Errors.Add("Auction request has been already cancelled before.");
            }
            else if (entity.Status != AuctionRequestStatus.Pending)
            {
                result.StatusCode = HttpStatusCode.BadRequest;
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

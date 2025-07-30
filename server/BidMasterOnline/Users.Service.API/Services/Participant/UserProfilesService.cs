using BidMasterOnline.Core.DTO;
using BidMasterOnline.Core.Helpers;
using BidMasterOnline.Core.RepositoryContracts;
using BidMasterOnline.Core.ServiceContracts;
using BidMasterOnline.Domain.Enums;
using BidMasterOnline.Domain.Models.Entities;
using Users.Service.API.DTO.Participant;
using Users.Service.API.Extensions;
using Users.Service.API.GrpcServices.Client;
using Users.Service.API.ServiceContracts;
using Users.Service.API.ServiceContracts.Participant;

namespace Users.Service.API.Services.Participant
{
    public class UserProfilesService : IUserProfilesService
    {
        private readonly IRepository _repository;
        private readonly IUserAccessor _userAccessor;
        private readonly ILogger<UserProfilesService> _logger;
        private readonly IUserStatusValidationService _userValidationService;
        private readonly INotificationsService _notificationsService;

        private readonly UserAuctionsGrpcClient _auctionsClient;
        private readonly UserBidsGrpcClient _bidsClient;

        public UserProfilesService(IRepository repository,
            IUserAccessor userAccessor,
            ILogger<UserProfilesService> logger,
            IUserStatusValidationService userValidationService,
            INotificationsService notificationsService,
            UserAuctionsGrpcClient auctionsClient,
            UserBidsGrpcClient bidsClient)
        {
            _repository = repository;
            _userAccessor = userAccessor;
            _logger = logger;
            _userValidationService = userValidationService;
            _notificationsService = notificationsService;
            _auctionsClient = auctionsClient;
            _bidsClient = bidsClient;
        }

        public async Task<ServiceResult> DeleteProfileAsync()
        {
            ServiceResult result = new();

            long userId = _userAccessor.UserId;

            if (await _repository.AnyAsync<Auction>(a =>
                a.Status == AuctionStatus.Finished &&
                (a.AuctioneerId == userId || a.WinnerId == userId) &&
                (!a.IsPaymentPerformed || !a.IsDeliveryPerformed)))
            {
                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.BadRequest;
                result.Errors.Add("Could not delete profile, while payment and delivery are not performed on your auctions yet.");

                return result;
            }

            try
            {
                User user = await _repository.GetByIdAsync<User>(userId);

                user.Status = UserStatus.Deleted;

                _repository.Update(user);

                await _repository.SaveChangesAsync();

                await _notificationsService.SendMessageOfDeletingAccountToUser(user);

                result.Message = "Your profile has been successfully deleted.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while deleting profile.");

                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                result.Errors.Add("An error occured while deleting profile.");
            }

            return result;
        }

        public async Task<ServiceResult<ExtendedUserProfileInfoDTO>> GetOwnUserProfileInfoAsync()
        {
            ServiceResult<ExtendedUserProfileInfoDTO> result = new();

            User? user = await _repository.GetFirstOrDefaultAsync<User>(e => e.Id == _userAccessor.UserId);

            if (user == null)
            {
                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.NotFound;
                result.Errors.Add("User not found.");

                return result;
            }

            result.Data = user.ToExtendedParticipantUserProfileDTO();

            return result;
        }

        public async Task<ServiceResult<UserProfileInfoDTO>> GetUserProfileInfoAsync(long userId)
        {
            ServiceResult<UserProfileInfoDTO> result = new();

            User? user = await _repository.GetFirstOrDefaultAsync<User>(e => e.Id == userId);

            if (user == null)
            {
                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.NotFound;
                result.Errors.Add("User not found.");

                return result;
            }

            result.Data = user.ToParticipantUserProfileDTO();

            return result;
        }

        public async Task<string> GetPaymentAttachmentStatusAsync()
        {
            return await _userValidationService.IsPaymentMethodAttachedAsync()
                ? "Attached"
                : "NotAttached";
        }

        public async Task<ServiceResult> ResetPasswordAsync(ResetPasswordDTO request)
        {
            ServiceResult result = new();

            long userId = _userAccessor.UserId;

            User user = await _repository.GetByIdAsync<User>(userId);

            if (user.PasswordHashed != CryptographyHelper.Hash(request.CurrentPassword, user.PasswordSalt))
            {
                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.BadRequest;
                result.Errors.Add("Incorrect current password");

                return result;
            }

            if (!PasswordFormatValidationHelper.ValidatePasswordFormat(request.NewPassword, out string errors))
            {
                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.BadRequest;
                result.Errors.Add(errors);

                return result;
            }

            try
            {
                string passwordSalt = CryptographyHelper.GenerateSalt(size: 128);
                string passwordHashed = CryptographyHelper.Hash(request.NewPassword, passwordSalt);

                user.PasswordHashed = passwordHashed;
                user.PasswordSalt = passwordSalt;

                _repository.Update(user);
                await _repository.SaveChangesAsync();

                result.Message = "Password has been successfully reset.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while reseting user's password.");

                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                result.Errors.Add("Error while reseting user's password.");
            }

            return result;
        }
    }
}

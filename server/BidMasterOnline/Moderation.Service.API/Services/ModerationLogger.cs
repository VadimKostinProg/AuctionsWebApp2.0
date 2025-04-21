using BidMasterOnline.Core.DTO;
using BidMasterOnline.Core.RepositoryContracts;
using BidMasterOnline.Domain.Enums;
using BidMasterOnline.Domain.Models.Entities;
using Moderation.Service.API.ServiceContracts;

namespace Moderation.Service.API.Services
{
    public class ModerationLogger : IModerationLogger
    {
        private readonly IRepository _repository;

        public ModerationLogger(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResult> LogAction(ModerationAction action,
            long? userId = null,
            long? auctionId = null,
            long? auctionRequestId = null,
            long? commentId = null,
            long? complaintId = null,
            long? technicalRequestId = null)
        {
            ServiceResult result = new();

            ModerationLog log = new()
            {
                Action = action,
                UserId = userId,
                AuctionId = auctionId,
                AuctionRequestId = auctionRequestId,
                AuctionCommentId = commentId,
                ComplaintId = complaintId,
                TechnicalSupportRequestId = technicalRequestId
            };

            if (!ValidateModerationLog(log, out string errors))
            {
                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.BadRequest;
                result.Errors.Add(errors);

                return result;
            }

            await _repository.AddAsync(log);
            await _repository.SaveChangesAsync();

            result.Message = "Moderation actions has been successfully logged!";

            return result;
        }

        private bool ValidateModerationLog(ModerationLog log, out string errors)
        {
            // TODO: add validation

            errors = string.Empty;

            return true;
        }
    }
}

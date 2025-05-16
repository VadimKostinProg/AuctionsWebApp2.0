using BidMasterOnline.Core.RepositoryContracts;
using BidMasterOnline.Domain.Enums;
using BidMasterOnline.Domain.Models.Entities;
using Moderation.Service.API.GrpcServices.Server;
using Moderation.Service.API.ServiceContracts;

namespace Moderation.Service.API.Services
{
    public class ModerationLogsService : IModerationLogsService
    {
        private readonly IRepository _repository;
        private readonly ILogger<GrpcModerationLoggerService> _logger;

        public ModerationLogsService(IRepository repository, ILogger<GrpcModerationLoggerService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<bool> LogModerationAction(ModerationAction action, long resourceId)
        {
            try
            {
                ModerationLog log = new();
                log.Action = action;

                SetResourceId(log, resourceId);

                await _repository.AddAsync(log);
                await _repository.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during logging moderation action.");
                return false;
            }
        }

        private void SetResourceId(ModerationLog log, long resourceId)
        {
            switch (log.Action)
            {
                case ModerationAction.BlockingUser:
                case ModerationAction.UnblockingUser:
                    log.UserId = resourceId;
                    break;
                case ModerationAction.ApprovingAuctionRequest:
                case ModerationAction.DecliningAuctionRequest:
                    log.AuctionRequestId = resourceId;
                    break;
                case ModerationAction.CancelingAuction:
                case ModerationAction.RecoveringAuction:
                    log.AuctionId = resourceId;
                    break;
                case ModerationAction.DeletingAuctionComment:
                    log.AuctionCommentId = resourceId;
                    break;
                case ModerationAction.DeletingUserFeedback:
                    log.UserFeedbackId = resourceId;
                    break;
            }
        }
    }
}

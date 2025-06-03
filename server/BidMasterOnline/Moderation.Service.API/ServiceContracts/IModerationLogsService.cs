using BidMasterOnline.Domain.Enums;

namespace Moderation.Service.API.ServiceContracts
{
    public interface IModerationLogsService
    {
        Task<bool> LogModerationAction(ModerationAction action, long resourceId, long moderatorId);
    }
}

using BidMasterOnline.Domain.Enums;
using Grpc.Core;
using Moderation.Service.API.ServiceContracts;
using ModerationGrpc;

namespace Moderation.Service.API.GrpcServices.Server
{
    public class GrpcModerationLoggerService : ModerationLogger.ModerationLoggerBase
    {
        private readonly IModerationLogsService _logService;

        public GrpcModerationLoggerService(IModerationLogsService logService)
        {
            _logService = logService;
        }

        public override async Task<ModerationActionResponse> LogModeration(ModerationLogRequest request, ServerCallContext context)
        {
            ModerationActionResponse response = new();

            response.Success = await _logService.LogModerationAction((ModerationAction)request.ActionCode, request.ResourceId);

            return response;
        }
    }
}

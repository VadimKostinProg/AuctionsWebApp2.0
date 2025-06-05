using BidMasterOnline.Core.ServiceContracts;
using BidMasterOnline.Domain.Enums;
using Grpc.Net.Client;
using ModerationGrpc;

namespace Auctions.Service.API.GrpcServices.Client
{
    public class ModerationGrpcClient
    {
        private readonly IUserAccessor _userAccessor;
        private readonly string _moderationHost;
        private readonly ILogger<ModerationGrpcClient> _logger;

        public ModerationGrpcClient(IConfiguration configuration,
            IUserAccessor userAccessor,
            ILogger<ModerationGrpcClient> logger)
        {
            _moderationHost = configuration["GrpcChannels:Moderation"]!;
            _userAccessor = userAccessor;
            _logger = logger;
        }

        public async Task LogModerationAction(ModerationAction action, long resourceId)
        {
            using GrpcChannel channel = GrpcChannel.ForAddress(_moderationHost);
            ModerationLogger.ModerationLoggerClient client = new(channel);

            ModerationLogRequest request = new()
            {
                ActionCode = (int)action,
                ResourceId = resourceId,
                ModeratorId = _userAccessor.UserId
            };

            ModerationActionResponse responce = await client.LogModerationAsync(request);

            if (!responce.Success)
            {
                _logger.LogError($"Internal error on Moderation.Service while " +
                    $"logging action {action} on resource {resourceId}.");
            }
        }
    }
}

using BidMasterOnline.Domain.Enums;
using Grpc.Net.Client;
using ModerationGrpc;

namespace Feedbacks.Service.API.GrpcServices.Client
{
    public class ModerationClient
    {
        private readonly string _moderationHost;
        private readonly ILogger<ModerationClient> _logger;

        public ModerationClient(IConfiguration configuration, ILogger<ModerationClient> logger)
        {
            _moderationHost = configuration["GrpcChannels:Moderation"]!;
            _logger = logger;
        }

        public async Task LogModerationAction(ModerationAction action, long resourceId)
        {
            using GrpcChannel channel = GrpcChannel.ForAddress(_moderationHost);
            ModerationLogger.ModerationLoggerClient client = new(channel);

            ModerationLogRequest request = new()
            {
                ActionCode = (int)action,
                ResourceId = resourceId
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

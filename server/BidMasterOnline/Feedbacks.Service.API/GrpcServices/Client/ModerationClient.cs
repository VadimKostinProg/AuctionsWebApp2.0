using BidMasterOnline.Domain.Enums;
using Grpc.Net.Client;
using ModerationGrpc;

namespace Feedbacks.Service.API.GrpcServices.Client
{
    public class ModerationClient
    {
        private readonly string _moderationHost;

        public ModerationClient(IConfiguration configuration)
        {
            _moderationHost = configuration["GrpcChannels:Moderation"]!;
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
                throw new InvalidOperationException($"Internal error on Moderation.Service while " +
                    $"logging action {action} on resource {resourceId}.");
            }
        }
    }
}

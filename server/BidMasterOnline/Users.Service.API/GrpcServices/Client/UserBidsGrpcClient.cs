using Grpc.Net.Client;
using UserBidsGrpc;

namespace Users.Service.API.GrpcServices.Client
{
    public class UserBidsGrpcClient
    {
        private readonly string _auctionsHost;
        private readonly ILogger<UserBidsGrpcClient> _logger;

        public UserBidsGrpcClient(IConfiguration configuration, ILogger<UserBidsGrpcClient> logger)
        {
            _auctionsHost = configuration["GrpcChannels:Bids"]!;
            _logger = logger;
        }

        public async Task CancelUserWinningBidsAsync(long userId)
        {
            using GrpcChannel channel = GrpcChannel.ForAddress(_auctionsHost);
            UserBids.UserBidsClient client = new(channel);

            UserInfoRequest request = new UserInfoRequest()
            {
                UserId = userId,
            };

            ActionResponse responce = await client.CancelWinningBidsAfterUserBlockingAsync(request);

            if (!responce.Success)
            {
                _logger.LogError($"Internal error on Bids.Service while " +
                    $"canceling winning bids of user {userId}.");
            }
        }
    }
}

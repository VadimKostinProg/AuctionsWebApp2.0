using Grpc.Net.Client;
using UsersGrpc;

namespace Users.Service.API.GrpcServices.Client
{
    public class UserAuctionsGrpcClient
    {
        private readonly string _auctionsHost;
        private readonly ILogger<UserAuctionsGrpcClient> _logger;

        public UserAuctionsGrpcClient(IConfiguration configuration, ILogger<UserAuctionsGrpcClient> logger)
        {
            _auctionsHost = configuration["GrpcChannels:Auctions"]!;
            _logger = logger;
        }

        public async Task CancelUserAuctionsAsync(long userId)
        {
            using GrpcChannel channel = GrpcChannel.ForAddress(_auctionsHost);
            UsersGrpc.UserAuctions.UserAuctionsClient client = new(channel);

            UserInfoRequest request = new UserInfoRequest()
            {
                UserId = userId,
            };

            ActionResponse responce = await client.CancelAuctionsAfterUserBlockingAsync(request);

            if (!responce.Success)
            {
                _logger.LogError($"Internal error on Auctions.Service while " +
                    $"canceling auctions of user {userId}.");
            }
        }
    }
}

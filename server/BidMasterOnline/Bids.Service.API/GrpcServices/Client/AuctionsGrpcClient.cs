using AuctionsGrpc;
using Grpc.Net.Client;

namespace Bids.Service.API.GrpcServices.Client
{
    public class AuctionsGrpcClient
    {
        private readonly string _auctionsHost;
        private readonly ILogger<AuctionsGrpcClient> _logger;

        public AuctionsGrpcClient(IConfiguration configuration, ILogger<AuctionsGrpcClient> logger)
        {
            _auctionsHost = configuration["GrcpChannels:Auctions"]!;
            _logger = logger;
        }

        public async Task FinishAuctionAsync(long auctionId)
        {
            using GrpcChannel channel = GrpcChannel.ForAddress(_auctionsHost);
            AuctionsGrpc.Auctions.AuctionsClient client = new(channel);

            FinishAuctionRequest request = new()
            {
                AuctionId = auctionId
            };

            ActionResponse responce = await client.FinishAuctionAsync(request);

            if (!responce.Success)
            {
                _logger.LogError($"Internal error on Auctions.Service while " +
                    $"finishing auction {auctionId}.");
            }
        }
    }
}

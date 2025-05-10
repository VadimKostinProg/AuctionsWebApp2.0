using AuctionsGrpc;
using Grpc.Net.Client;

namespace Bids.Service.API.GrpcServices.Client
{
    public class AuctionsGrpcClient
    {
        private readonly string _auctionsHost;

        public AuctionsGrpcClient(IConfiguration configuration)
        {
            _auctionsHost = configuration["GrcpChannels:Auctions"]!;
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
                throw new InvalidOperationException($"Internal error on Auctions.Service while " +
                    $"finishing auction {auctionId}.");
            }
        }
    }
}

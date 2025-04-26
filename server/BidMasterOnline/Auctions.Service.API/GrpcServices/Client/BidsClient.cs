using BidsGrpc;
using Grpc.Net.Client;

namespace Auctions.Service.API.GrpcServices.Client
{
    public class BidsClient
    {
        private readonly string _bidsHost;

        public BidsClient(IConfiguration configuration)
        {
            _bidsHost = configuration["GrcpChannels:Bids"]!;
        }

        public async Task ClearAllBidsForAuctionAsync(long auctionId)
        {
            using GrpcChannel channel = GrpcChannel.ForAddress(_bidsHost);
            Bids.BidsClient client = new(channel);

            ClearBidsRequest request = new()
            {
                AuctionId = auctionId
            };

            BidActionResponse responce = await client.ClearBidsForAuctionAsync(request);

            if (!responce.Success)
            {
                throw new InvalidOperationException($"Internal error on Bids.Service while " +
                    $"clearing bids for auction {auctionId}.");
            }
        }
    }
}

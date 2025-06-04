using BidsGrpc;
using Grpc.Net.Client;

namespace Auctions.Service.API.GrpcServices.Client
{
    public class BidsClient
    {
        private readonly string _bidsHost;
        private readonly ILogger<BidsClient> _logger;

        public BidsClient(IConfiguration configuration, ILogger<BidsClient> logger)
        {
            _bidsHost = configuration["GrpcChannels:Bids"]!;
            _logger = logger;
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
                _logger.LogError($"Internal error on Bids.Service while " +
                    $"clearing bids for auction {auctionId}.");
            }
        }
    }
}

using AuctionsGrpcClient;
using Grpc.Net.Client;
using Moderation.Service.API.ServiceContracts;

namespace Moderation.Service.API.Services
{
    public class AuctionsClient : IAuctionsClient
    {
        private readonly string _auctionServiceAddress;

        public AuctionsClient(IConfiguration configuration)
        {
            _auctionServiceAddress = configuration["GrpcChannels:Auctions"]!;
        }

        public async Task<bool> ApproveAuctionRequestAsync(long auctionRequestId)
        {
            using GrpcChannel channel = GrpcChannel.ForAddress(_auctionServiceAddress);
            AuctionRequests.AuctionRequestsClient client = new(channel);

            ApproveAuctionRequest request = new()
            {
                AuctionRequestId = auctionRequestId,
            };

            AuctionActionResponse response = await client.ApproveAsync(request);

            return response.Success;
        }

        public async Task<bool> DeclineAuctionRequestAsync(long auctionRequestId, string reason)
        {
            using GrpcChannel channel = GrpcChannel.ForAddress(_auctionServiceAddress);
            AuctionRequests.AuctionRequestsClient client = new(channel);

            DeclineAuctionRequest request = new()
            {
                AuctionRequestId = auctionRequestId,
                Reason = reason
            };

            AuctionActionResponse response = await client.DeclineAsync(request);

            return response.Success;
        }

        public async Task<bool> CancelAuctionAsync(long auctionId, string reason)
        {
            using GrpcChannel channel = GrpcChannel.ForAddress(_auctionServiceAddress);
            Auctions.AuctionsClient client = new(channel);

            CancelRequest request = new()
            {
                AuctionId = auctionId,
                Reason = reason
            };

            AuctionActionResponse response = await client.CancelAsync(request);

            return response.Success;
        }

        public async Task<bool> RecoverAuctionAsync(long auctionId)
        {
            using GrpcChannel channel = GrpcChannel.ForAddress(_auctionServiceAddress);
            Auctions.AuctionsClient client = new(channel);

            RecoverRequest request = new()
            {
                AuctionId = auctionId
            };

            AuctionActionResponse response = await client.RecoverAsync(request);

            return response.Success;
        }
    }
}

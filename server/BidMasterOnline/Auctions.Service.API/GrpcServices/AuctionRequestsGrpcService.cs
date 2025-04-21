using Auctions.Service.API.ServiceContracts.Moderator;
using AuctionsGrpc;
using Grpc.Core;

namespace Auctions.Service.API.GrpcServices
{
    public class AuctionRequestsGrpcService : AuctionRequests.AuctionRequestsBase
    {
        private readonly IModeratorAuctionRequestsService _service;

        public AuctionRequestsGrpcService(IModeratorAuctionRequestsService service)
        {
            _service = service;
        }

        public override async Task<AuctionActionResponse> Approve(ApproveAuctionRequest request, ServerCallContext context)
        {
            AuctionActionResponse response = new();

            response.Success = await _service.ApproveAuctionRequestAsync(request.AuctionRequestId);

            return response;
        }

        public override async Task<AuctionActionResponse> Decline(DeclineAuctionRequest request, ServerCallContext context)
        {
            AuctionActionResponse response = new();

            response.Success = await _service.DeclineAuctionRequestAsync(request.AuctionRequestId, request.Reason);

            return response;
        }
    }
}

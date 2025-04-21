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

        public override async Task<ActionResponse> Approve(ApproveAuctionRequest request, ServerCallContext context)
        {
            ActionResponse response = new();

            response.Success = await _service.ApproveAuctionRequestAsync(request.AuctionRequestId);

            return response;
        }

        public override async Task<ActionResponse> Decline(DeclineAuctionRequest request, ServerCallContext context)
        {
            ActionResponse response = new();

            response.Success = await _service.DeclineAuctionRequestAsync(request.AuctionRequestId, request.Reason);

            return response;
        }
    }
}

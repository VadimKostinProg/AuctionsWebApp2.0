using Auctions.Service.API.ServiceContracts.Moderator;
using AuctionsGrpc;
using Grpc.Core;

namespace Auctions.Service.API.GrpcServices
{
    public class AuctionsGrpcService : AuctionsGrpc.Auctions.AuctionsBase
    {
        private readonly IModeratorAuctionsService _service;

        public AuctionsGrpcService(IModeratorAuctionsService service)
        {
            _service = service;
        }

        public override async Task<AuctionActionResponse> Cancel(CancelRequest request, ServerCallContext context)
        {
            AuctionActionResponse response = new();

            response.Success = await _service.CancelAuctionAsync(request.AuctionId, request.Reason);

            return response;
        }

        public override async Task<AuctionActionResponse> Recover(RecoverRequest request, ServerCallContext context)
        {
            AuctionActionResponse response = new();

            response.Success = await _service.RecoverAuctionAsync(request.AuctionId);

            return response;
        }
    }
}

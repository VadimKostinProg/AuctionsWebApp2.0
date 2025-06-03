using Auctions.Service.API.ServiceContracts.Participant;
using AuctionsGrpc;
using Grpc.Core;

namespace Auctions.Service.API.GrpcServices.Server
{
    public class AuctionsGrpcService : AuctionsGrpc.Auctions.AuctionsBase
    {
        private readonly IAuctionsService _service;

        public AuctionsGrpcService(IAuctionsService service)
        {
            _service = service;
        }

        public override async Task<ActionResponse> FinishAuction(FinishAuctionRequest request, ServerCallContext context)
        {
            ActionResponse response = new();

            response.Success = await _service.FinishAuctionAsync(request.AuctionId);

            return response;
        }
    }
}

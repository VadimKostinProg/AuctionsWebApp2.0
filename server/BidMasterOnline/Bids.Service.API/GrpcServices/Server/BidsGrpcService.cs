using Bids.Service.API.ServiceContracts.Moderator;
using BidsGrpc;
using Grpc.Core;

namespace Bids.Service.API.GrpcServices.Server
{
    public class BidsGrpcService : BidsGrpc.Bids.BidsBase
    {
        private readonly IModeratorBidsService _moderatorBidsService;

        public BidsGrpcService(IModeratorBidsService moderatorBidsService)
        {
            _moderatorBidsService = moderatorBidsService;
        }

        public override async Task<BidActionResponse> ClearBidsForAuction(ClearBidsRequest request, ServerCallContext context)
        {
            BidActionResponse response = new();

            response.Success = await _moderatorBidsService.ClearAllBidsForAuctionAsync(request.AuctionId);

            return response;
        }
    }
}

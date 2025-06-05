using Auctions.Service.API.ServiceContracts.Moderator;
using Grpc.Core;
using UsersGrpc;

namespace Auctions.Service.API.GrpcServices.Server
{
    public class UserAuctionsGrpcService : UsersGrpc.UserAuctions.UserAuctionsBase
    {
        private readonly IAuctionsService _auctionsService;

        public UserAuctionsGrpcService(IAuctionsService auctionsService)
        {
            _auctionsService = auctionsService;
        }

        public override async Task<ActionResponse> CancelAuctionsAfterUserBlocking(UserInfoRequest request, ServerCallContext context)
        {
            ActionResponse response = new();

            response.Success = await _auctionsService.CancelAllUserAuctionsAfterBlockingAsync(request.UserId);

            return response;
        }
    }
}

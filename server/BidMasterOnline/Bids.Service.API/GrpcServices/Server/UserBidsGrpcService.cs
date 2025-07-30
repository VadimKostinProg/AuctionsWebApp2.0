using Bids.Service.API.ServiceContracts.Moderator;
using Grpc.Core;
using UserBidsGrpc;

namespace Bids.Service.API.GrpcServices.Server
{
    public class UserBidsGrpcService : UserBidsGrpc.UserBids.UserBidsBase
    {
        private readonly IBidsService _service;

        public UserBidsGrpcService(IBidsService service)
        {
            _service = service;
        }

        public override async Task<ActionResponse> CancelWinningBidsAfterUserBlocking(UserInfoRequest request, ServerCallContext context)
        {
            ActionResponse response = new();

            response.Success = await _service.CancelUserWinningBidsAsync(request.UserId);

            return response;
        }
    }
}

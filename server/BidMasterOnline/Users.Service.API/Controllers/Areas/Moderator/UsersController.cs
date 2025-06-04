using BidMasterOnline.Core.Constants;
using BidMasterOnline.Core.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Users.Service.API.DTO.Moderator;
using Users.Service.API.ServiceContracts.Moderator;

namespace Users.Service.API.Controllers.Areas.Moderator
{
    [Route("api/moderator/users")]
    [ApiController]
    [Authorize(Roles = UserRoles.Moderator)]
    public class UsersController : BaseController
    {
        private readonly IUsersService _service;

        public UsersController(IUsersService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsersList([FromQuery] UserSpecificationsDTO specs)
        {
            ServiceResult<PaginatedList<UserProfileSummaryInfoDTO>> result = 
                await _service.GetUsersListAsync(specs);

            return FromResult(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserDetails([FromRoute] long id)
        {
            ServiceResult<UserProfileInfoDTO> result = await _service.GetUserProfileInfoAsync(id);

            return FromResult(result);
        }

        [HttpPut("{id}/block")]
        public async Task<IActionResult> BlockUser([FromRoute] long id, [FromBody] BlockUserDTO request)
        {
            ServiceResult result = await _service.BlockUserAsync(id, request);

            return FromResult(result);
        }

        [HttpPut("{id}/unblock")]
        public async Task<IActionResult> UnblockUser([FromRoute] long id)
        {
            ServiceResult result = await _service.UnblockUserAsync(id);

            return FromResult(result);
        }
    }
}

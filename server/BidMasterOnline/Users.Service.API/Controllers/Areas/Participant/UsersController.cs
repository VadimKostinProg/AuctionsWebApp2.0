using Azure.Core;
using BidMasterOnline.Core.Constants;
using BidMasterOnline.Core.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Users.Service.API.DTO.Participant;
using Users.Service.API.ServiceContracts.Participant;

namespace Users.Service.API.Controllers.Areas.Participant
{
    [Route("api/participant/users")]
    [ApiController]
    [Authorize(Roles = UserRoles.Participant)]
    public class UsersController : BaseController
    {
        private readonly IUserProfilesService _service;

        public UsersController(IUserProfilesService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserProfileInfo([FromRoute] long id)
        {
            ServiceResult<UserProfileInfoDTO> result = await _service.GetUserProfileInfoAsync(id);

            return FromResult(result);
        }

        [HttpPut("password")]
        public async Task<IActionResult> UpdatePassword([FromBody] ResetPasswordDTO request)
        {
            ServiceResult result = await _service.ResetPasswordAsync(request);

            return FromResult(result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProfile()
        {
            ServiceResult result = await _service.DeleteProfileAsync();

            return FromResult(result);
        }
    }
}

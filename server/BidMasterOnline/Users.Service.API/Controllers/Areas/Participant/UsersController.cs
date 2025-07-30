using Azure.Core;
using BidMasterOnline.Core.Constants;
using BidMasterOnline.Core.DTO;
using BidMasterOnline.Core.ServiceContracts;
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
        private readonly IUserAccessor _userAccessor;

        public UsersController(IUserProfilesService service,
            IUserAccessor userAccessor)
        {
            _service = service;
            _userAccessor = userAccessor;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserProfileInfo([FromRoute] long id)
        {
            if (id == _userAccessor.UserId)
            {
                ServiceResult<ExtendedUserProfileInfoDTO> result = await _service.GetOwnUserProfileInfoAsync();

                return FromResult(result);
            }
            else
            {
                ServiceResult<UserProfileInfoDTO> result = await _service.GetUserProfileInfoAsync(id);

                return FromResult(result);
            }
        }

        [HttpGet("payment-method-attachment-status")]
        public async Task<IActionResult> GetPaymentAttachedStatus()
        {
            string status = await _service.GetPaymentAttachmentStatusAsync();

            return Ok(new { Status = status });
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

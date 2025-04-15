using BidMasterOnline.Application.Constants;
using BidMasterOnline.Application.DTO;
using BidMasterOnline.Application.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BidMasterOnline.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class UsersController : ControllerBase
    {
        private readonly IUserManager _userManager;
        private readonly IBidsService _bidsService;

        public UsersController(IUserManager userManager, IBidsService bidsService)
        {
            _userManager = userManager;
            _bidsService = bidsService;
        }

        [HttpGet("staff/list")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<ListModel<UserDTO>>> GetStuffList([FromQuery] SpecificationsDTO specifications)
        {
            return Ok(await _userManager.GetStaffListAsync(specifications));
        }

        [HttpGet("customers/list")]
        [Authorize(Roles = UserRoles.TechnicalSupportSpecialist)]
        public async Task<ActionResult<ListModel<UserDTO>>> GetCustomersList([FromQuery] UserSpecificationsDTO specifications)
        {
            return Ok(await _userManager.GetCustomersListAsync(specifications));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProfileDTO>> GetUserProfileById([FromRoute] Guid id)
        {
            return Ok(await _userManager.GetUserProfileByIdAsync(id));
        }

        [HttpPost("customers")]
        [AllowAnonymous]
        public async Task<ActionResult> CreateCustomer([FromForm] CreateUserDTO user)
        {
            await _userManager.CreateUserAsync(user, Application.Enums.UserRole.Customer);

            return Ok(new { Message = "Account has been successfully created." });
        }

        [HttpPost("technical-support-specialists")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult> CreateTechnicalSupportSpecialist([FromForm] CreateUserDTO user)
        {
            await _userManager.CreateUserAsync(user, Application.Enums.UserRole.TechnicalSupportSpecialist);

            return Ok(new { Message = "Account has been successfully created." });
        }

        [HttpPost("admins")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult> CreateAdmin([FromForm] CreateUserDTO user)
        {
            await _userManager.CreateUserAsync(user, Application.Enums.UserRole.Admin);

            return Ok(new { Message = "Account has been successfully created." });
        }

        [HttpPut("{id}/confirm-email")]
        [Authorize]
        public async Task<ActionResult> ConfirmEmail([FromRoute] Guid id)
        {
            await _userManager.ConfirmEmailAsync(id);

            return Ok(new { Message = "Your email has been successfully confirmed." });
        }

        [HttpPut("passwords")]
        [Authorize]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDTO changePassword)
        {
            await _userManager.ChangePasswordAsync(changePassword);

            return Ok(new { Message = "Password has been successfully changed." });
        }

        [HttpDelete]
        [Authorize]
        public async Task<ActionResult> DeleteOwnAccount()
        {
            await _userManager.DeleteUserAsync();

            return Ok(new { Message = "Your account has been deleted successfully." });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult> DeleteAccount([FromRoute] Guid id)
        {
            await _userManager.DeleteUserAsync(id);

            return Ok(new { Message = "User account has been deleted successfully." });
        }

        [HttpGet("{id}/bids/list")]
        [AllowAnonymous]
        public async Task<ActionResult<ListModel<BidDTO>>> GetBidsOfUser([FromRoute] Guid id, [FromQuery] SpecificationsDTO specifications)
        {
            return Ok(await _bidsService.GetBidsListForUserAsync(id, specifications));
        }

        [HttpPut("block")]
        [Authorize(Roles = $"{UserRoles.TechnicalSupportSpecialist}")]
        public async Task<ActionResult> BlockUser([FromBody] BlockUserDTO request)
        {
            await _userManager.BlockUserAsync(request);

            return Ok(new { Message = "User has been blocked successfully." });
        }

        [HttpPut("{id}/unblock")]
        [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.TechnicalSupportSpecialist}")]
        public async Task<ActionResult> UnblockUser([FromRoute] Guid id)
        {
            await _userManager.UnblockUserAsync(id);

            return Ok(new { Message = "User has been unblocked successfully." });
        }
    }
}

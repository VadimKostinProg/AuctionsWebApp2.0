using BidMasterOnline.Core.Constants;
using BidMasterOnline.Domain.Models.Entities;
using IdentityServer.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServer.Pages.Admin
{
    [Authorize(Roles = UserRoles.Admin)]
    public class DeleteModeratorModel : PageModel
    {
        private readonly IUserManager _userManager;

        public DeleteModeratorModel(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public User? Moderator { get; set; }

        public async Task OnGetAsync(long id)
        {
            Moderator = await _userManager.GetByIdAsync(id);
        }

        public async Task<IActionResult> OnPostAsync(long id)
        {
            await _userManager.DeleteUserAsync(id);

            return RedirectToPage("Index");
        }
    }
}

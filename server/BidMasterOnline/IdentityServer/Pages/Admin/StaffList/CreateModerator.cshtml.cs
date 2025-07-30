using BidMasterOnline.Core.Constants;
using IdentityServer.Models;
using IdentityServer.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Pages.Admin
{
    [Authorize(Roles = UserRoles.Admin)]
    public class CreateModeratorModel : PageModel
    {
        private readonly IUserManager _userManager;

        public CreateModeratorModel(IUserManager userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Moderator { get; set; } = new();

        public class InputModel
        {
            [Required]
            public string Username { get; set; } = string.Empty;

            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            public string FullName { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Date)]
            public DateTime DateOfBirth { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (await _userManager.ExistsWithUsernameAsync(Moderator.Username!))
            {
                ModelState.AddModelError("Input.Username", "User with this username already exists");
            }

            if (await _userManager.ExistsWithEmailAsync(Moderator.Email!))
            {
                ModelState.AddModelError("Input.Username", "User with this email already exists");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            CreateUserModel user = new()
            {
                Username = Moderator.Username,
                Email = Moderator.Email,
                FullName = Moderator.FullName,
                DateOfBirth = Moderator.DateOfBirth,
                Password = Moderator.Password
            };

            await _userManager.CreateUserAsync(user, UserRoles.Moderator);

            return RedirectToPage("Index");
        }
    }
}

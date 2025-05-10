using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Pages.Account.ForceChangePassword
{
    public class InputModel
    {
        public string? ReturnUrl { get; set; }

        [Required]
        public string? NewPassword { get; set; }

        [Required]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string? RepeatNewPassword { get; set; }
    }
}

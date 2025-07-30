using BidMasterOnline.Core.Constants;
using BidMasterOnline.Domain.Models.Entities;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityServer.Services.Contracts;
using IdentityServerHost.Pages;
using IdentityServerHost.Pages.Account.EmailConfirmation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace IdentityServer.Pages.Account.EmailConfirmation
{
    [Authorize(Roles = UserRoles.Participant)]
    public class IndexModel : PageModel
    {
        private readonly IUserManager _userManager;
        private readonly INotificationsService _notificationsService;
        private readonly IIdentityServerInteractionService _interaction;

        public IndexModel(IUserManager userManager,
            INotificationsService notificationsService,
            IIdentityServerInteractionService interaction)
        {
            _userManager = userManager;
            _notificationsService = notificationsService;
            _interaction = interaction;
        }

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public async Task OnGetAsync(string? returnUrl)
        {
            long userId = long.Parse(User.GetSubjectId());

            User user = await _userManager.GetByIdAsync(userId);

            string code = _userManager.GenerateEmailConfirmationCodeForUser(userId);

            await _notificationsService.SendEmailConfirmationMessageAsync(user.Email, user.FullName, code);

            Input = new InputModel()
            {
                ReturnUrl = returnUrl,
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // check if we are in the context of an authorization request
            AuthorizationRequest? context = await _interaction.GetAuthorizationContextAsync(Input.ReturnUrl);

            if (ModelState.IsValid)
            {
                long userId = long.Parse(User.GetSubjectId());

                bool result = await _userManager.ConfirmEmailAsync(userId, Input.Code!);

                if (!result)
                {
                    Input = new InputModel()
                    {
                        ReturnUrl = Input.ReturnUrl,
                        Errors = "Invalid code"
                    };

                    return Page();
                }

                if (context != null)
                {
                    // This "can't happen", because if the ReturnUrl was null, then the context would be null
                    ArgumentNullException.ThrowIfNull(Input.ReturnUrl, nameof(Input.ReturnUrl));

                    if (context.IsNativeClient())
                    {
                        // The client is native, so this change in how to
                        // return the response is for better UX for the end user.
                        return this.LoadingPage(Input.ReturnUrl);
                    }

                    // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                    return Redirect(Input.ReturnUrl ?? "~/");
                }

                // request for a local page
                if (Url.IsLocalUrl(Input.ReturnUrl))
                {
                    return Redirect(Input.ReturnUrl);
                }
                else if (string.IsNullOrEmpty(Input.ReturnUrl))
                {
                    return Redirect("~/");
                }
                else
                {
                    // user might have clicked on a malicious link - should be logged
                    throw new ArgumentException("invalid return URL");
                }
            }

            Input = new InputModel()
            {
                ReturnUrl = Input.ReturnUrl,
            };

            return Page();
        }
    }
}

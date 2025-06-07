// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

using BidMasterOnline.Core.Constants;
using BidMasterOnline.Domain.Models.Entities;
using Duende.IdentityModel;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityServer.Models;
using IdentityServer.Services.Contracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace IdentityServerHost.Pages.Create;

[SecurityHeaders]
[AllowAnonymous]
public class Index : PageModel
{
    private readonly IIdentityServerInteractionService _interaction;
    private readonly IUserManager _userManager;

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public Index(IIdentityServerInteractionService interaction, IUserManager userManager)
    {
        _interaction = interaction;
        _userManager = userManager;
    }

    public IActionResult OnGet(string? returnUrl)
    {
        Input = new InputModel { ReturnUrl = returnUrl };
        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        // check if we are in the context of an authorization request
        AuthorizationRequest? context = await _interaction.GetAuthorizationContextAsync(Input.ReturnUrl);

        // the user clicked the "cancel" button
        if (Input.Button != "create")
        {
            if (context != null)
            {
                // if the user cancels, send a result back into IdentityServer as if they 
                // denied the consent (even if this client does not require consent).
                // this will send back an access denied OIDC error response to the client.
                await _interaction.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied);

                // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                if (context.IsNativeClient())
                {
                    // The client is native, so this change in how to
                    // return the response is for better UX for the end user.
                    return this.LoadingPage(Input.ReturnUrl);
                }

                return Redirect(Input.ReturnUrl ?? "~/");
            }
            else
            {
                // since we don't have a valid context, then we just go back to the home page
                return Redirect("~/");
            }
        }

        if (await _userManager.ExistsWithUsernameAsync(Input.Username!))
        {
            ModelState.AddModelError("Input.Username", "User with this username already exists");
        }

        if (await _userManager.ExistsWithEmailAsync(Input.Email!))
        {
            ModelState.AddModelError("Input.Username", "User with this email already exists");
        }

        if (ModelState.IsValid)
        {
            CreateUserModel userModel = new()
            {
                Username = Input.Username!,
                FullName = Input.Name!,
                Email = Input.Email!,
                DateOfBirth = Input.DateOfBirth!.Value,
                Password = Input.Password!
            };

            User user = await _userManager.CreateUserAsync(userModel, UserRoles.Participant);

            // issue authentication cookie with subject ID and username
            IdentityServerUser isuser = new(user.Id.ToString())
            {
                DisplayName = user.Username,
                AdditionalClaims = [new Claim(JwtClaimTypes.Role, UserRoles.Participant)]
            };

            await HttpContext.SignInAsync(isuser);

            return RedirectToPage("/Account/EmailConfirmation/Index", new
            {
                returnUrl = Input.ReturnUrl,
            });

        }

        return Page();
    }
}

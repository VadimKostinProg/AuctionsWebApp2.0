// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

using Duende.IdentityServer;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BidMasterOnline.Core.Constants;

namespace IdentityServerHost.Pages.Home;

[Authorize]
public class Index : PageModel
{
    private readonly IConfiguration _configuration;

    public Index(IConfiguration configuration, IdentityServerLicense? license = null)
    {
        _configuration = configuration;
        License = license;
    }

    public string Version
    {
        get => typeof(Duende.IdentityServer.Hosting.IdentityServerMiddleware).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion.Split('+').First()
            ?? "unavailable";
    }

    public IdentityServerLicense? License { get; }


    public string? ClientURL { get; set; }

    public async Task OnGet()
    {
        if (User.IsInRole(UserRoles.Participant))
            ClientURL = _configuration["Clients:ParticipantUI"];
        else if (User.IsInRole(UserRoles.Moderator))
            ClientURL = _configuration["Clients:ModeratorUI"];
    }
}

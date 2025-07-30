// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace IdentityServerHost.Pages.Account.EmailConfirmation;

public class InputModel
{
    [Required]
    public string? Code { get; set; }
    public string? ReturnUrl { get; set; }
    public string? Errors { get; set; }
}
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using WhatShouldIWorkOnToday.Auth;
using WhatShouldIWorkOnToday.Events;

namespace WhatShouldIWorkOnToday.Pages;

[AllowAnonymous]
public sealed class LoginModel(
    SingleUserAuthService authService,
    WsiwotLoginEventPublisher loginEventPublisher) : PageModel
{
    [BindProperty]
    [Required]
    public string Username { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public IActionResult OnGet()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return LocalRedirect(GetReturnUrl());
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        if (!authService.Validate(
                Username,
                Password))
        {
            await loginEventPublisher.TryPublishFailedAsync(
                Username,
                HttpContext.Connection
                    .RemoteIpAddress?
                    .ToString(),
                HttpContext.RequestAborted);

            ModelState.AddModelError(
                string.Empty,
                "Invalid username or password.");

            Password = string.Empty;

            return Page();
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, authService.Username),
            new Claim(ClaimTypes.Name, authService.Username)
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        var principal =
            new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                AllowRefresh = true
            });

        await loginEventPublisher.TryPublishSucceededAsync(
            authService.Username,
            HttpContext.Connection
                .RemoteIpAddress?
                .ToString(),
            HttpContext.RequestAborted);

        return LocalRedirect(GetReturnUrl());
    }

    private string GetReturnUrl()
    {
        return Url.IsLocalUrl(ReturnUrl)
            ? ReturnUrl!
            : "/";
    }
}
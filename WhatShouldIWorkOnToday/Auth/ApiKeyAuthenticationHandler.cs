using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;

namespace WhatShouldIWorkOnToday.Auth;

public sealed class ApiKeyAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IOptions<ApiKeyOptions> apiKeyOptions)
    : AuthenticationHandler<AuthenticationSchemeOptions>(
        options,
        logger,
        encoder)
{
    protected override Task<AuthenticateResult>
        HandleAuthenticateAsync()
    {
        var authorization = Request.Headers.Authorization.ToString();

        if (string.IsNullOrWhiteSpace(authorization))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (!AuthenticationHeaderValue.TryParse(
                authorization,
                out var header) ||
            !string.Equals(
                header.Scheme,
                "Bearer",
                StringComparison.OrdinalIgnoreCase) ||
            string.IsNullOrWhiteSpace(header.Parameter))
        {
            return Task.FromResult(
                AuthenticateResult.Fail("Invalid Authorization header."));
        }

        if (!KeysMatch(
                header.Parameter,
                apiKeyOptions.Value.Key))
        {
            return Task.FromResult(
                AuthenticateResult.Fail("Invalid API key."));
        }

        var claims = new[]
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                "api-client"),

            new Claim(
                ClaimTypes.Name,
                "API Client")
        };

        var identity = new ClaimsIdentity(
            claims,
            Scheme.Name);

        var principal = new ClaimsPrincipal(identity);

        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    protected override Task HandleChallengeAsync(
        AuthenticationProperties properties)
    {
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        Response.Headers["WWW-Authenticate"] = "Bearer";

        return Task.CompletedTask;
    }

    private static bool KeysMatch(
        string provided,
        string expected)
    {
        var providedBytes = Encoding.UTF8.GetBytes(provided);
        var expectedBytes = Encoding.UTF8.GetBytes(expected);

        return providedBytes.Length ==
               expectedBytes.Length &&
               CryptographicOperations.FixedTimeEquals(
                   providedBytes,
                   expectedBytes);
    }
}
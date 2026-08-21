using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace WhatShouldIWorkOnToday.Auth;

public static class AuthenticationServiceExtensions
{
    public static IServiceCollection AddApplicationAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddAuthentication(
                CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/login";

                options.Cookie.Name = "__Host-wsiwot-auth";
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy =
                    CookieSecurePolicy.Always;
                options.Cookie.Path = "/";

                options.ExpireTimeSpan =
                    TimeSpan.FromDays(30);

                options.SlidingExpiration = true;
            })
            .AddScheme<
                AuthenticationSchemeOptions,
                ApiKeyAuthenticationHandler>(
                ApiKeyDefaults.AuthenticationScheme,
                _ => { });

        services
            .AddOptions<ApiKeyOptions>()
            .Bind(
                configuration.GetSection(ApiKeyOptions.SectionName))
            .Validate(
                options =>
                    !string.IsNullOrWhiteSpace(options.Key),
                "Api:Key is required.")
            .ValidateOnStart();

        services
            .AddOptions<SingleUserOptions>()
            .Bind(configuration.GetSection("Auth"))
            .Validate(
                options =>
                    !string.IsNullOrWhiteSpace(options.Username),
                "Auth:Username is required.")
            .Validate(
                options =>
                    !string.IsNullOrWhiteSpace(options.Password),
                "Auth:Password is required.")
            .ValidateOnStart();

        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                ApiKeyDefaults.AuthorizationPolicy,
                policy =>
                {
                    policy.AddAuthenticationSchemes(ApiKeyDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                });
        });

        services.AddCascadingAuthenticationState();

        services.AddSingleton<
            IPasswordHasher<SingleUser>,
            PasswordHasher<SingleUser>>();

        services.AddSingleton<SingleUserAuthService>();

        return services;
    }
}
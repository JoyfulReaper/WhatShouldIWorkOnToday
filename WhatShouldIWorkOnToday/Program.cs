using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WhatShouldIWorkOnToday.Api;
using WhatShouldIWorkOnToday.Auth;
using WhatShouldIWorkOnToday.Components;
using WhatShouldIWorkOnToday.Data;
using WhatShouldIWorkOnToday.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRazorPages();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";

        options.Cookie.Name = "__Host-wsiwot-auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.Path = "/";

        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.SlidingExpiration = true;
    })
    .AddScheme<
        AuthenticationSchemeOptions,
        ApiKeyAuthenticationHandler>(
        ApiKeyDefaults.AuthenticationScheme,
        _ => { });

builder.Services
    .AddOptions<ApiKeyOptions>()
    .Bind(
        builder.Configuration.GetSection(
            ApiKeyOptions.SectionName))
    .Validate(
        options =>
            !string.IsNullOrWhiteSpace(options.Key),
        "Api:Key is required.")
    .ValidateOnStart();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        ApiKeyDefaults.AuthorizationPolicy,
        policy =>
        {
            policy.AddAuthenticationSchemes(
                ApiKeyDefaults.AuthenticationScheme);

            policy.RequireAuthenticatedUser();
        });
});

builder.Services.AddCascadingAuthenticationState();

builder.Services
    .AddOptions<SingleUserOptions>()
    .Bind(builder.Configuration.GetSection("Auth"))
    .Validate(
        options =>
            !string.IsNullOrWhiteSpace(options.Username),
        "Auth:Username is required.")
    .Validate(
        options =>
            !string.IsNullOrWhiteSpace(options.Password),
        "Auth:Password is required.")
    .ValidateOnStart();

builder.Services.AddSingleton<
    IPasswordHasher<SingleUser>,
    PasswordHasher<SingleUser>>();

builder.Services.AddSingleton<SingleUserAuthService>();
builder.Services.AddScoped<WorkChooser>();

var configuredDatabasePath =
    builder.Configuration["Database:Path"]
    ?? DatabasePath.DefaultRelativePath;

var databasePath = DatabasePath.Resolve(
    configuredDatabasePath,
    builder.Environment.ContentRootPath);

builder.Services.AddDbContextFactory<AppDbContext>(
    options =>
        options.UseSqlite($"Data Source={databasePath}"));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(
        "/Error",
        createScopeForErrors: true);

    app.UseHsts();
}

app.UseWhen(
    context =>
        !context.Request.Path
            .StartsWithSegments("/api"),
    branch =>
    {
        branch.UseStatusCodePagesWithReExecute(
            "/not-found",
            createScopeForStatusCodePages: true);
    });

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorPages();

app.MapApiEndpoints();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .RequireAuthorization();

app.Run();
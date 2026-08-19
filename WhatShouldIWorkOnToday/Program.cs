using WhatShouldIWorkOnToday.Api;
using WhatShouldIWorkOnToday.Auth;
using WhatShouldIWorkOnToday.Components;
using WhatShouldIWorkOnToday.Data;
using WhatShouldIWorkOnToday.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRazorPages();

builder.Services.AddApplicationAuthentication(builder.Configuration);
builder.Services.AddApplicationDatabase(
    builder.Configuration,
    builder.Environment);

builder.Services.AddScoped<WorkChooser>();

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

if (!app.Environment.IsDevelopment())
{
    app.UseWhen(
        context =>
            !context.Request.Path
                .StartsWithSegments("/api") &&
            !context.Request.Path
                .StartsWithSegments("/health"),
        branch =>
        {
            branch.UseHttpsRedirection();
        });
}

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorPages();

app.MapGet(
        "/health/live",
        () => Results.Ok(
            new
            {
                status = "ok"
            }))
    .AllowAnonymous();

app.MapApiEndpoints();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .RequireAuthorization();

app.Run();
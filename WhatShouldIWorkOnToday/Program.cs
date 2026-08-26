using JoyfulReaperLib.MissionControl;
using WhatShouldIWorkOnToday.Api;
using WhatShouldIWorkOnToday.Auth;
using WhatShouldIWorkOnToday.Components;
using WhatShouldIWorkOnToday.Data;
using WhatShouldIWorkOnToday.Events;
using WhatShouldIWorkOnToday.GitHubSync;
using WhatShouldIWorkOnToday.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRazorPages();

builder.Services.AddApplicationAuthentication(builder.Configuration);
builder.Services.AddApplicationDatabase(builder.Configuration, builder.Environment);
builder.Services.AddGitHubSync(builder.Configuration);

builder.Services.AddMissionControlClient(builder.Configuration.GetSection(MissionControlClientOptions.SectionName));

builder.Services.AddScoped<WsiwotLoginEventPublisher>();
builder.Services.Configure<PlanningOptions>(builder.Configuration.GetSection(PlanningOptions.SectionName));

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<PlanningClock>();

builder.Services.AddScoped<WorkChooser>();
builder.Services.AddSingleton<PlanningMutationService>();

var app = builder.Build();

await app.Services.MigrateApplicationDatabaseAsync();

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

public partial class Program
{
}

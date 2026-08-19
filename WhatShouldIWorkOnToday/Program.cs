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
    app.UseHttpsRedirection();
}

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
using System.Net.Http.Headers;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using WhatShouldIWorkOnToday.Data;

namespace WhatShouldIWorkOnToday.Tests;

public sealed class WorkItemsApiFactory
    : WebApplicationFactory<Program>
{
    public const string ApiKey =
        "work-items-api-test-key";

    private readonly string _databaseDirectory;
    private readonly string _databasePath;

    public WorkItemsApiFactory()
    {
        _databaseDirectory = Path.Combine(
            Path.GetTempPath(),
            "WhatShouldIWorkOnToday.Tests",
            Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(_databaseDirectory);

        _databasePath = Path.Combine(
            _databaseDirectory,
            "work-items.db");
    }

    public HttpClient CreateAuthenticatedClient()
    {
        var client = CreateClient(
            new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                ApiKey);

        return client;
    }

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration(
            (_, configuration) =>
                configuration.AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["Api:Key"] = ApiKey,
                        ["Auth:Username"] = "test-user",
                        ["Auth:Password"] = "test-password",
                        ["GitHubSync:Enabled"] = "false",
                        ["MissionControl:Enabled"] = "false",
                        ["Logging:EventLog:LogLevel:Default"] = "None"
                    }));

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<
                IDbContextFactory<AppDbContext>>();

            services.AddSingleton<
                IDbContextFactory<AppDbContext>>(
                new TestDbContextFactory(
                    _databasePath));

            services
                .AddDataProtection()
                .UseEphemeralDataProtectionProvider();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing &&
            Directory.Exists(_databaseDirectory))
        {
            Directory.Delete(
                _databaseDirectory,
                recursive: true);
        }
    }

}

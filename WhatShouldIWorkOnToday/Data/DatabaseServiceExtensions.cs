using Microsoft.EntityFrameworkCore;

namespace WhatShouldIWorkOnToday.Data;

public static class DatabaseServiceExtensions
{
    public static IServiceCollection AddApplicationDatabase(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var configuredPath =
            configuration["Database:Path"]
            ?? DatabasePath.DefaultRelativePath;

        var databasePath = DatabasePath.Resolve(configuredPath, environment.ContentRootPath);

        services.AddDbContextFactory<AppDbContext>(
            options =>
                options.UseSqlite($"Data Source={databasePath}"));

        return services;
    }

    public static async Task
        MigrateApplicationDatabaseAsync(
            this IServiceProvider services,
            CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();

        var factory =
            scope.ServiceProvider
                .GetRequiredService<IDbContextFactory<AppDbContext>>();

        await using AppDbContext database = await factory.CreateDbContextAsync(cancellationToken);

        await database.Database.MigrateAsync(cancellationToken);
    }
}
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

        var databasePath = DatabasePath.Resolve(
            configuredPath,
            environment.ContentRootPath);

        services.AddDbContextFactory<AppDbContext>(
            options =>
                options.UseSqlite($"Data Source={databasePath}"));

        return services;
    }
}
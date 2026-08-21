using Microsoft.EntityFrameworkCore;
using WhatShouldIWorkOnToday.Data;

namespace WhatShouldIWorkOnToday.Tests;

internal sealed class TestDbContextFactory(
    string databasePath)
    : IDbContextFactory<AppDbContext>
{
    private readonly DbContextOptions<AppDbContext>
        _options =
            new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(
                    $"Data Source={databasePath};Pooling=False")
                .Options;

    public AppDbContext CreateDbContext()
    {
        return new AppDbContext(_options);
    }

    public Task<AppDbContext> CreateDbContextAsync(
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            CreateDbContext());
    }
}

internal sealed class TemporarySqliteDatabase
    : IAsyncDisposable
{
    private TemporarySqliteDatabase(
        string directory)
    {
        Directory = directory;
        Factory = new TestDbContextFactory(
            Path.Combine(
                directory,
                "test.db"));
    }

    private string Directory { get; }

    public IDbContextFactory<AppDbContext> Factory { get; }

    public static async Task<TemporarySqliteDatabase>
        CreateAsync()
    {
        var directory = Path.Combine(
            Path.GetTempPath(),
            "WhatShouldIWorkOnToday.Tests",
            Guid.NewGuid().ToString("N"));

        System.IO.Directory.CreateDirectory(directory);

        var database =
            new TemporarySqliteDatabase(directory);

        await using var db = await database.Factory
            .CreateDbContextAsync();

        await db.Database.MigrateAsync();

        return database;
    }

    public ValueTask DisposeAsync()
    {
        if (System.IO.Directory.Exists(Directory))
        {
            System.IO.Directory.Delete(
                Directory,
                recursive: true);
        }

        return ValueTask.CompletedTask;
    }
}

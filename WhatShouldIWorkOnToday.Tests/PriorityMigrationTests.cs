using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using WhatShouldIWorkOnToday.Data;
using WhatShouldIWorkOnToday.Models;
using Xunit;

namespace WhatShouldIWorkOnToday.Tests;

public sealed class PriorityMigrationTests
{
    [Fact]
    public async Task ExistingRowsMigrateToNormalPriority()
    {
        var directory = Path.Combine(
            Path.GetTempPath(),
            "WhatShouldIWorkOnToday.Tests",
            Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        var path = Path.Combine(directory, "migration.db");
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite($"Data Source={path};Pooling=False")
            .Options;

        try
        {
            await using (var db = new AppDbContext(options))
            {
                var migrator = db.GetService<IMigrator>();
                await migrator.MigrateAsync("20260821164744_AddProcessedSyncCommands");

                await db.Database.ExecuteSqlRawAsync(
                    """
                    INSERT INTO WorkItems
                        (Name, Kind, CreatedAt)
                    VALUES
                        ('Existing parent', 0, '2026-08-21T12:00:00+00:00');
                    INSERT INTO TodoItems
                        (WorkItemId, Task, Energy, Effort, CreatedAt)
                    VALUES
                        (last_insert_rowid(), 'Existing todo', 1, 1, '2026-08-21T12:00:00+00:00');
                    """);

                await migrator.MigrateAsync();
            }

            await using var verifyDb = new AppDbContext(options);
            Assert.Equal(
                PriorityLevel.Normal,
                (await verifyDb.WorkItems.SingleAsync()).Priority);
            Assert.Equal(
                PriorityLevel.Normal,
                (await verifyDb.TodoItems.SingleAsync()).Priority);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }
}

using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using WhatShouldIWorkOnToday.GitHubSync;
using WhatShouldIWorkOnToday.Models;
using Xunit;

namespace WhatShouldIWorkOnToday.Tests;

public sealed class SyncSnapshotTests
{
    [Fact]
    public async Task SnapshotContainsPlanningState_AndHashTracksMeaningfulChanges()
    {
        await using var database =
            await TemporarySqliteDatabase.CreateAsync();

        await using (var db = await database.Factory
                         .CreateDbContextAsync())
        {
            var workItem = new WorkItem
            {
                Name = "Snapshot project",
                Kind = WorkItemKind.Learning,
                Priority = PriorityLevel.High,
                Description = "Useful state",
                Url = "https://example.com/project"
            };

            workItem.Todos.Add(
                new TodoItem
                {
                    Task = "Read documentation",
                    Energy = EnergyLevel.Low,
                    Effort = EffortLevel.Short,
                    Priority = PriorityLevel.Low
                });

            db.WorkItems.Add(workItem);
            await db.SaveChangesAsync();
        }

        var builder = new SyncSnapshotBuilder(
            database.Factory);

        var first = await builder.BuildAsync();
        var unchanged = await builder.BuildAsync();

        var workItemSnapshot = Assert.Single(
            first.WorkItems);

        Assert.Equal(
            "Snapshot project",
            workItemSnapshot.Name);
        Assert.Equal(
            "Learning",
            workItemSnapshot.Kind);
        Assert.Equal("High", workItemSnapshot.Priority);
        Assert.Equal(
            "Low",
            Assert.Single(workItemSnapshot.Todos).Priority);
        Assert.Equal(
            "Read documentation",
            Assert.Single(workItemSnapshot.Todos).Task);
        Assert.Equal(
            first.StateHash,
            unchanged.StateHash);

        var serialized = JsonSerializer.Serialize(
            first,
            GitHubSyncJson.Compact);

        Assert.DoesNotContain(
            "password",
            serialized,
            StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(
            "apiKey",
            serialized,
            StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(
            "token",
            serialized,
            StringComparison.OrdinalIgnoreCase);

        await using (var db = await database.Factory
                         .CreateDbContextAsync())
        {
            var todo = await db.TodoItems.SingleAsync();
            todo.Task = "Read updated documentation";
            await db.SaveChangesAsync();
        }

        var changed = await builder.BuildAsync();

        Assert.NotEqual(
            first.StateHash,
            changed.StateHash);

        await using (var db = await database.Factory
                         .CreateDbContextAsync())
        {
            var todo = await db.TodoItems.SingleAsync();
            todo.Priority = PriorityLevel.High;
            await db.SaveChangesAsync();
        }

        var priorityChanged = await builder.BuildAsync();
        Assert.NotEqual(changed.StateHash, priorityChanged.StateHash);

        await using (var db = await database.Factory
                         .CreateDbContextAsync())
        {
            var workItem = await db.WorkItems.SingleAsync();
            workItem.LastWorkedAt = DateTimeOffset.UtcNow;
            await db.SaveChangesAsync();
        }

        var workedOn = await builder.BuildAsync();
        Assert.NotEqual(priorityChanged.StateHash, workedOn.StateHash);
    }

    [Fact]
    public async Task UnchangedState_DoesNotRewriteRemoteSnapshot()
    {
        await using var database =
            await TemporarySqliteDatabase.CreateAsync();

        var client = new FakeGitHubSyncClient();
        var publisher = new SyncSnapshotPublisher(
            new SyncSnapshotBuilder(database.Factory),
            client);

        Assert.True(
            await publisher.PublishIfChangedAsync());
        Assert.False(
            await publisher.PublishIfChangedAsync());

        Assert.Equal(
            1,
            client.Operations.Count(operation =>
                operation ==
                $"write:{SyncSnapshotPublisher.SnapshotPath}"));

        var remote = client.Files[
            SyncSnapshotPublisher.SnapshotPath];

        var snapshot = JsonSerializer.Deserialize<SyncSnapshot>(
            Encoding.UTF8.GetString(remote.Content),
            GitHubSyncJson.Compact);

        Assert.NotNull(snapshot);
        Assert.Equal(1, snapshot.SchemaVersion);
    }
}

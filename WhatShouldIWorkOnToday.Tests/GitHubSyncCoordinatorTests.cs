using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using WhatShouldIWorkOnToday.GitHubSync;
using WhatShouldIWorkOnToday.Services;
using Xunit;

namespace WhatShouldIWorkOnToday.Tests;

public sealed class GitHubSyncCoordinatorTests
{
    [Fact]
    public async Task SuccessfulCommand_WritesReceiptBeforeDeletingPendingFile()
    {
        await using var database =
            await TemporarySqliteDatabase.CreateAsync();

        var id = Guid.NewGuid();
        var pending = SyncTestCommands.CreateFile(
            id,
            SyncCommandTypes.CreateWorkItem,
            new
            {
                name = "Coordinated project"
            });

        var client = new FakeGitHubSyncClient();
        client.SetFile(pending.Path, pending.Content);

        var coordinator = CreateCoordinator(
            database,
            client);

        await coordinator.RunCycleAsync();

        var receiptPath =
            $"commands/applied/{id:D}.json";

        Assert.True(client.Files.ContainsKey(receiptPath));
        Assert.False(client.Files.ContainsKey(pending.Path));

        var writeIndex = client.Operations.IndexOf(
            $"write:{receiptPath}");
        var deleteIndex = client.Operations.IndexOf(
            $"delete:{pending.Path}");

        Assert.True(writeIndex >= 0);
        Assert.True(deleteIndex > writeIndex);
    }

    [Fact]
    public async Task MissingRemoteReceipt_RecoversWithoutDuplicatingMutation()
    {
        await using var database =
            await TemporarySqliteDatabase.CreateAsync();

        var id = Guid.NewGuid();
        var pending = SyncTestCommands.CreateFile(
            id,
            SyncCommandTypes.CreateWorkItem,
            new
            {
                name = "Crash-safe project"
            });

        var receiptPath =
            $"commands/applied/{id:D}.json";

        var client = new FakeGitHubSyncClient();
        client.SetFile(pending.Path, pending.Content);
        client.WriteFailuresRemaining[receiptPath] = 1;

        var coordinator = CreateCoordinator(
            database,
            client);

        await coordinator.RunCycleAsync();

        Assert.True(client.Files.ContainsKey(pending.Path));
        Assert.False(client.Files.ContainsKey(receiptPath));

        await using (var db = await database.Factory
                         .CreateDbContextAsync())
        {
            Assert.Equal(
                1,
                await db.WorkItems.CountAsync());
            Assert.Equal(
                1,
                await db.ProcessedSyncCommands.CountAsync());
        }

        await coordinator.RunCycleAsync();

        Assert.True(client.Files.ContainsKey(receiptPath));
        Assert.False(client.Files.ContainsKey(pending.Path));

        await using var verifyDb = await database.Factory
            .CreateDbContextAsync();

        Assert.Equal(
            1,
            await verifyDb.WorkItems.CountAsync());
        Assert.Equal(
            1,
            await verifyDb.ProcessedSyncCommands.CountAsync());
    }

    private static GitHubSyncCoordinator CreateCoordinator(
        TemporarySqliteDatabase database,
        FakeGitHubSyncClient client)
    {
        var snapshotBuilder = new SyncSnapshotBuilder(
            database.Factory);

        return new GitHubSyncCoordinator(
            client,
            new SyncSnapshotPublisher(
                snapshotBuilder,
                client),
            new SyncCommandParser(),
            new SyncCommandProcessor(
                database.Factory,
                new PlanningMutationService()),
            NullLogger<GitHubSyncCoordinator>.Instance);
    }
}

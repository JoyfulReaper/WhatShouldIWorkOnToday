using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using WhatShouldIWorkOnToday.GitHubSync;
using WhatShouldIWorkOnToday.Services;
using Xunit;

namespace WhatShouldIWorkOnToday.Tests;

public sealed class GitHubSyncCoordinatorTests
{
    [Fact]
    public async Task InvalidFilename_IsQuarantinedByteForByteBeforePendingDeletion()
    {
        await using var database =
            await TemporarySqliteDatabase.CreateAsync();

        var pending = InvalidFilenameCommand();
        var rejectedPath =
            GitHubSyncCoordinator.GetRejectedCommandPath(
                pending);

        var client = new FakeGitHubSyncClient();
        client.SetFile(pending.Path, pending.Content);

        await CreateCoordinator(database, client)
            .RunCycleAsync();

        Assert.False(client.Files.ContainsKey(pending.Path));
        Assert.True(client.Files.ContainsKey(rejectedPath));
        Assert.True(
            pending.Content.AsSpan().SequenceEqual(
                client.Files[rejectedPath].Content));
        Assert.Matches(
            "^commands/rejected/invalid-filename-[0-9a-f]{64}\\.json$",
            rejectedPath);
        Assert.DoesNotContain(
            "not a guid",
            rejectedPath,
            StringComparison.Ordinal);

        var writeIndex = client.Operations.IndexOf(
            $"write:{rejectedPath}");
        var deleteIndex = client.Operations.IndexOf(
            $"delete:{pending.Path}");

        Assert.True(writeIndex >= 0);
        Assert.True(deleteIndex > writeIndex);
    }

    [Fact]
    public async Task QuarantineWriteFailure_LeavesPending_AndRetrySucceeds()
    {
        await using var database =
            await TemporarySqliteDatabase.CreateAsync();

        var pending = InvalidFilenameCommand();
        var rejectedPath =
            GitHubSyncCoordinator.GetRejectedCommandPath(
                pending);

        var client = new FakeGitHubSyncClient();
        client.SetFile(pending.Path, pending.Content);
        client.WriteFailuresRemaining[rejectedPath] = 1;

        var coordinator = CreateCoordinator(
            database,
            client);

        await coordinator.RunCycleAsync();

        Assert.True(client.Files.ContainsKey(pending.Path));
        Assert.False(client.Files.ContainsKey(rejectedPath));
        Assert.DoesNotContain(
            $"delete:{pending.Path}",
            client.Operations);

        await coordinator.RunCycleAsync();

        Assert.False(client.Files.ContainsKey(pending.Path));
        Assert.True(client.Files.ContainsKey(rejectedPath));
        Assert.True(
            pending.Content.AsSpan().SequenceEqual(
                client.Files[rejectedPath].Content));
    }

    [Fact]
    public async Task QuarantineDeleteFailure_ReusesExistingRejectedCopy()
    {
        await using var database =
            await TemporarySqliteDatabase.CreateAsync();

        var pending = InvalidFilenameCommand();
        var rejectedPath =
            GitHubSyncCoordinator.GetRejectedCommandPath(
                pending);

        var client = new FakeGitHubSyncClient();
        client.SetFile(pending.Path, pending.Content);
        client.DeleteFailuresRemaining[pending.Path] = 1;

        var coordinator = CreateCoordinator(
            database,
            client);

        await coordinator.RunCycleAsync();

        Assert.True(client.Files.ContainsKey(pending.Path));
        Assert.True(client.Files.ContainsKey(rejectedPath));
        Assert.Equal(
            1,
            client.Operations.Count(operation =>
                operation == $"write:{rejectedPath}"));

        await coordinator.RunCycleAsync();

        Assert.False(client.Files.ContainsKey(pending.Path));
        Assert.Single(
            client.Files.Keys,
            path =>
                path.StartsWith(
                    GitHubSyncCoordinator.RejectedCommandsPath + "/",
                    StringComparison.Ordinal));
        Assert.Equal(
            1,
            client.Operations.Count(operation =>
                operation == $"write:{rejectedPath}"));
    }

    [Fact]
    public async Task SuccessfullyQuarantinedFile_IsNotProcessedAgain()
    {
        await using var database =
            await TemporarySqliteDatabase.CreateAsync();

        var pending = InvalidFilenameCommand();
        var rejectedPath =
            GitHubSyncCoordinator.GetRejectedCommandPath(
                pending);

        var client = new FakeGitHubSyncClient();
        client.SetFile(pending.Path, pending.Content);

        var coordinator = CreateCoordinator(
            database,
            client);

        await coordinator.RunCycleAsync();

        var pendingReads = client.Operations.Count(operation =>
            operation == $"get:{pending.Path}");
        var rejectedWrites = client.Operations.Count(operation =>
            operation == $"write:{rejectedPath}");
        var pendingDeletes = client.Operations.Count(operation =>
            operation == $"delete:{pending.Path}");

        await coordinator.RunCycleAsync();

        Assert.Equal(
            pendingReads,
            client.Operations.Count(operation =>
                operation == $"get:{pending.Path}"));
        Assert.Equal(
            rejectedWrites,
            client.Operations.Count(operation =>
                operation == $"write:{rejectedPath}"));
        Assert.Equal(
            pendingDeletes,
            client.Operations.Count(operation =>
                operation == $"delete:{pending.Path}"));
    }

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
        Assert.DoesNotContain(
            client.Files.Keys,
            path => path.StartsWith(
                GitHubSyncCoordinator.RejectedCommandsPath + "/",
                StringComparison.Ordinal));

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

    private static GitHubSyncFile InvalidFilenameCommand()
    {
        return new GitHubSyncFile(
            "commands/pending/not a guid @!.json",
            "pending-sha",
            [
                0,
                255,
                10,
                123,
                34,
                120,
                34,
                58,
                49,
                125
            ]);
    }
}

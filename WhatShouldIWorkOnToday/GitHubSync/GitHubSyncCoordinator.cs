namespace WhatShouldIWorkOnToday.GitHubSync;

public sealed class GitHubSyncCoordinator(
    IGitHubSyncClient gitHubClient,
    SyncSnapshotPublisher snapshotPublisher,
    SyncCommandParser commandParser,
    SyncCommandProcessor commandProcessor,
    ILogger<GitHubSyncCoordinator> logger)
{
    public const string PendingCommandsPath =
        "commands/pending";

    public const string AppliedCommandsPath =
        "commands/applied";

    public async Task RunCycleAsync(
        CancellationToken cancellationToken = default)
    {
        await TryPublishSnapshotAsync(cancellationToken);

        IReadOnlyList<GitHubSyncFileEntry> pendingFiles;

        try
        {
            pendingFiles = await gitHubClient.ListFilesAsync(
                PendingCommandsPath,
                cancellationToken);
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            logger.LogWarning(
                exception,
                "Unable to list pending GitHub sync commands. The next cycle will retry.");

            return;
        }

        var stateChanged = false;

        foreach (var entry in pendingFiles
                     .Where(file =>
                         file.Name.EndsWith(
                             ".json",
                             StringComparison.OrdinalIgnoreCase))
                     .OrderBy(
                         file => file.Path,
                         StringComparer.Ordinal))
        {
            try
            {
                var file = await gitHubClient.GetFileAsync(
                    entry.Path,
                    cancellationToken);

                if (file is null)
                {
                    continue;
                }

                var parseResult = commandParser.Parse(file);

                if (parseResult.CommandId is null)
                {
                    logger.LogWarning(
                        "Ignoring GitHub sync command with invalid filename: {CommandPath}",
                        entry.Path);

                    continue;
                }

                var outcome = await commandProcessor
                    .ProcessAsync(
                        parseResult,
                        cancellationToken);

                stateChanged |= outcome.StateChanged;

                await EnsureAppliedReceiptAsync(
                    outcome,
                    cancellationToken);

                await gitHubClient.DeleteFileAsync(
                    file.Path,
                    file.Sha,
                    $"Remove processed WSIWOT command {outcome.Receipt.Id}",
                    cancellationToken);
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                logger.LogWarning(
                    exception,
                    "Unable to finish GitHub sync command {CommandPath}. The next cycle will retry safely.",
                    entry.Path);
            }
        }

        if (stateChanged)
        {
            await TryPublishSnapshotAsync(cancellationToken);
        }
    }

    private async Task EnsureAppliedReceiptAsync(
        SyncCommandProcessingOutcome outcome,
        CancellationToken cancellationToken)
    {
        var receiptPath =
            $"{AppliedCommandsPath}/{outcome.Receipt.Id:D}.json";

        var existing = await gitHubClient.GetFileAsync(
            receiptPath,
            cancellationToken);

        if (existing is not null &&
            existing.Content.AsSpan()
                .SequenceEqual(outcome.ReceiptContent))
        {
            return;
        }

        await gitHubClient.WriteFileAsync(
            receiptPath,
            outcome.ReceiptContent,
            $"Record WSIWOT command {outcome.Receipt.Id}",
            existing?.Sha,
            cancellationToken);
    }

    private async Task TryPublishSnapshotAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            await snapshotPublisher.PublishIfChangedAsync(
                cancellationToken);
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            logger.LogWarning(
                exception,
                "Unable to publish the GitHub sync snapshot. The next cycle will retry.");
        }
    }
}

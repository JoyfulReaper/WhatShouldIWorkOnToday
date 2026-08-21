using System.Text.Json;

namespace WhatShouldIWorkOnToday.GitHubSync;

public sealed class SyncSnapshotPublisher(
    SyncSnapshotBuilder snapshotBuilder,
    IGitHubSyncClient gitHubClient)
{
    public const string SnapshotPath =
        "state/snapshot.json";

    public async Task<bool> PublishIfChangedAsync(
        CancellationToken cancellationToken = default)
    {
        var snapshot = await snapshotBuilder.BuildAsync(
            cancellationToken);

        var existing = await gitHubClient.GetFileAsync(
            SnapshotPath,
            cancellationToken);

        if (existing is not null &&
            TryReadStateHash(
                existing.Content,
                out var existingHash) &&
            string.Equals(
                snapshot.StateHash,
                existingHash,
                StringComparison.Ordinal))
        {
            return false;
        }

        var content = JsonSerializer.SerializeToUtf8Bytes(
            snapshot,
            GitHubSyncJson.Indented);

        await gitHubClient.WriteFileAsync(
            SnapshotPath,
            content,
            "Update WSIWOT state snapshot",
            existing?.Sha,
            cancellationToken);

        return true;
    }

    private static bool TryReadStateHash(
        byte[] content,
        out string? stateHash)
    {
        try
        {
            using var document = JsonDocument.Parse(content);

            if (document.RootElement.TryGetProperty(
                    "stateHash",
                    out var property) &&
                property.ValueKind == JsonValueKind.String)
            {
                stateHash = property.GetString();
                return !string.IsNullOrWhiteSpace(stateHash);
            }
        }
        catch (JsonException)
        {
        }

        stateHash = null;
        return false;
    }
}

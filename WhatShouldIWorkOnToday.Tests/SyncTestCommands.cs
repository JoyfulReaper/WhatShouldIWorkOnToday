using System.Text.Json;
using WhatShouldIWorkOnToday.GitHubSync;

namespace WhatShouldIWorkOnToday.Tests;

internal static class SyncTestCommands
{
    public static GitHubSyncFile CreateFile(
        Guid id,
        string type,
        object payload)
    {
        var content = JsonSerializer.SerializeToUtf8Bytes(
            new
            {
                schemaVersion = 1,
                id,
                type,
                createdAtUtc =
                    DateTimeOffset.Parse(
                        "2026-08-21T12:00:00Z"),
                payload
            },
            GitHubSyncJson.Compact);

        return new GitHubSyncFile(
            $"commands/pending/{id:D}.json",
            $"sha-{id:N}",
            content);
    }
}

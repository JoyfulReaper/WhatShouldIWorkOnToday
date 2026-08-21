using System.Text.Json;
using WhatShouldIWorkOnToday.GitHubSync;
using Xunit;

namespace WhatShouldIWorkOnToday.Tests;

public sealed class SyncCommandParserTests
{
    private readonly SyncCommandParser _parser = new();

    [Fact]
    public void ValidCommand_IsAccepted()
    {
        var id = Guid.NewGuid();
        var result = _parser.Parse(
            CommandFile(
                id,
                id.ToString(),
                schemaVersion: 1,
                SyncCommandTypes.CreateTodo));

        Assert.True(result.Succeeded);
        Assert.Equal(id, result.CommandId);
        Assert.Equal(
            SyncCommandTypes.CreateTodo,
            result.Command!.Type);
    }

    [Fact]
    public void MalformedGuid_IsRejected()
    {
        var result = _parser.Parse(
            new GitHubSyncFile(
                "commands/pending/not-a-guid.json",
                "sha",
                JsonSerializer.SerializeToUtf8Bytes(
                    new
                    {
                        schemaVersion = 1,
                        id = "not-a-guid",
                        type = SyncCommandTypes.CreateTodo,
                        createdAtUtc = DateTimeOffset.UtcNow,
                        payload = new { workItemId = 1, task = "Task" }
                    })));

        Assert.False(result.Succeeded);
        Assert.Null(result.CommandId);
    }

    [Fact]
    public void FilenameAndCommandIdMismatch_IsRejected()
    {
        var result = _parser.Parse(
            CommandFile(
                Guid.NewGuid(),
                Guid.NewGuid().ToString(),
                schemaVersion: 1,
                SyncCommandTypes.CreateTodo));

        Assert.False(result.Succeeded);
        Assert.Contains(
            "must match",
            result.Error,
            StringComparison.Ordinal);
    }

    [Fact]
    public void UnsupportedSchemaVersion_IsRejected()
    {
        var id = Guid.NewGuid();
        var result = _parser.Parse(
            CommandFile(
                id,
                id.ToString(),
                schemaVersion: 2,
                SyncCommandTypes.CreateTodo));

        Assert.False(result.Succeeded);
        Assert.Contains(
            "schemaVersion 1",
            result.Error,
            StringComparison.Ordinal);
    }

    [Fact]
    public void UnsupportedCommandType_IsRejected()
    {
        var id = Guid.NewGuid();
        var result = _parser.Parse(
            CommandFile(
                id,
                id.ToString(),
                schemaVersion: 1,
                "deleteWorkItem"));

        Assert.False(result.Succeeded);
        Assert.Contains(
            "Unsupported command type",
            result.Error,
            StringComparison.Ordinal);
    }

    private static GitHubSyncFile CommandFile(
        Guid filenameId,
        string bodyId,
        int schemaVersion,
        string type)
    {
        var content = JsonSerializer.SerializeToUtf8Bytes(
            new
            {
                schemaVersion,
                id = bodyId,
                type,
                createdAtUtc = DateTimeOffset.UtcNow,
                payload = new
                {
                    workItemId = 1,
                    task = "Task"
                }
            },
            GitHubSyncJson.Compact);

        return new GitHubSyncFile(
            $"commands/pending/{filenameId:D}.json",
            "sha",
            content);
    }
}

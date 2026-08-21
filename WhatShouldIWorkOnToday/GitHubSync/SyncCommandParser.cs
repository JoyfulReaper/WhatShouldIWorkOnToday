using System.Globalization;
using System.Text.Json;

namespace WhatShouldIWorkOnToday.GitHubSync;

public sealed class SyncCommandParser
{
    public SyncCommandParseResult Parse(
        GitHubSyncFile file)
    {
        if (!string.Equals(
                Path.GetExtension(file.Path),
                ".json",
                StringComparison.OrdinalIgnoreCase))
        {
            return new SyncCommandParseResult(
                null,
                "unknown",
                null,
                "Command filename must be a valid GUID followed by .json.");
        }

        var filenameId = Path.GetFileNameWithoutExtension(
            file.Path);

        if (!Guid.TryParse(
                filenameId,
                out var commandId))
        {
            return new SyncCommandParseResult(
                null,
                "unknown",
                null,
                "Command filename must be a valid GUID followed by .json.");
        }

        RawSyncCommand? raw;

        try
        {
            raw = JsonSerializer.Deserialize<RawSyncCommand>(
                file.Content,
                GitHubSyncJson.Compact);
        }
        catch (JsonException)
        {
            return Reject(
                commandId,
                "unknown",
                "Command file contains malformed JSON.");
        }

        if (raw is null)
        {
            return Reject(
                commandId,
                "unknown",
                "Command file is empty.");
        }

        var commandType = string.IsNullOrWhiteSpace(raw.Type)
            ? "unknown"
            : raw.Type;

        if (!Guid.TryParse(raw.Id, out var bodyId))
        {
            return Reject(
                commandId,
                commandType,
                "Command id must be a valid GUID.");
        }

        if (bodyId != commandId)
        {
            return Reject(
                commandId,
                commandType,
                "Command filename id must match the JSON command id.");
        }

        if (raw.SchemaVersion != 1)
        {
            return Reject(
                commandId,
                commandType,
                "Only command schemaVersion 1 is supported.");
        }

        if (!SyncCommandTypes.IsSupported(raw.Type))
        {
            return Reject(
                commandId,
                commandType,
                $"Unsupported command type '{commandType}'.");
        }

        if (!DateTimeOffset.TryParse(
                raw.CreatedAtUtc,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal,
                out var createdAtUtc))
        {
            return Reject(
                commandId,
                commandType,
                "createdAtUtc must be a valid timestamp.");
        }

        if (raw.Payload.ValueKind !=
            JsonValueKind.Object)
        {
            return Reject(
                commandId,
                commandType,
                "Command payload must be a JSON object.");
        }

        return new SyncCommandParseResult(
            commandId,
            commandType,
            new ParsedSyncCommand(
                commandId,
                commandType,
                createdAtUtc.ToUniversalTime(),
                raw.Payload),
            null);
    }

    private static SyncCommandParseResult Reject(
        Guid commandId,
        string commandType,
        string error)
    {
        return new SyncCommandParseResult(
            commandId,
            commandType,
            null,
            error);
    }

    private sealed record RawSyncCommand(
        int SchemaVersion,
        string? Id,
        string? Type,
        string? CreatedAtUtc,
        JsonElement Payload);
}

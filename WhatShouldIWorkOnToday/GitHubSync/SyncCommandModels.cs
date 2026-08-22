using System.Text.Json;

namespace WhatShouldIWorkOnToday.GitHubSync;

public static class SyncCommandTypes
{
    public const string CreateWorkItem = "createWorkItem";
    public const string CreateTodo = "createTodo";
    public const string CompleteTodo = "completeTodo";
    public const string MarkWorkItemWorkedOn = "markWorkItemWorkedOn";
    public const string SetWorkItemPriority = "setWorkItemPriority";
    public const string SetTodoPriority = "setTodoPriority";
    public const string RenameWorkItem = "renameWorkItem";

    public static bool IsSupported(string? type)
    {
        return type is
            CreateWorkItem or
            CreateTodo or
            CompleteTodo or
            MarkWorkItemWorkedOn or
            SetWorkItemPriority or
            SetTodoPriority or
            RenameWorkItem;
    }
}

public sealed record ParsedSyncCommand(
    Guid Id,
    string Type,
    DateTimeOffset CreatedAtUtc,
    JsonElement Payload);

public sealed record SyncCommandParseResult(
    Guid? CommandId,
    string CommandType,
    ParsedSyncCommand? Command,
    string? Error)
{
    public bool Succeeded => Command is not null;
}

public sealed record CreateWorkItemCommandPayload(
    string? Name,
    string? Kind = null,
    string? Description = null,
    string? Url = null,
    string? Priority = null,
    IReadOnlyList<CreateInitialTodoCommandPayload?>? Todos = null);

public sealed record CreateInitialTodoCommandPayload(
    string? Task,
    string? Energy = null,
    string? Effort = null,
    string? Priority = null);

public sealed record CreateTodoCommandPayload(
    int WorkItemId,
    string? Task,
    string? Energy = null,
    string? Effort = null,
    string? Priority = null);

public sealed record CompleteTodoCommandPayload(
    int TodoId);

public sealed record MarkWorkItemWorkedOnCommandPayload(
    int WorkItemId,
    string? Note = null);

public sealed record SetWorkItemPriorityCommandPayload(
    int WorkItemId,
    string? Priority);

public sealed record SetTodoPriorityCommandPayload(
    int TodoId,
    string? Priority);

public sealed record SyncCommandReceipt(
    int SchemaVersion,
    Guid Id,
    string Type,
    string Status,
    DateTimeOffset AppliedAtUtc,
    SyncCommandResult? Result = null,
    string? Error = null);

public sealed record SyncCommandResult(
    int? WorkItemId = null,
    int? TodoId = null,
    IReadOnlyList<int>? TodoIds = null);

public sealed record SyncCommandProcessingOutcome(
    SyncCommandReceipt Receipt,
    byte[] ReceiptContent,
    bool StateChanged,
    bool AlreadyProcessed);

public sealed record RenameWorkItemCommandPayload(
    int WorkItemId,
    string? Name);

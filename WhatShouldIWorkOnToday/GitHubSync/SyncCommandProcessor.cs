using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using WhatShouldIWorkOnToday.Data;
using WhatShouldIWorkOnToday.Models;
using WhatShouldIWorkOnToday.Services;

namespace WhatShouldIWorkOnToday.GitHubSync;

public sealed class SyncCommandProcessor(
    IDbContextFactory<AppDbContext> dbContextFactory,
    PlanningMutationService mutationService)
{
    public async Task<SyncCommandProcessingOutcome>
        ProcessAsync(
            SyncCommandParseResult parseResult,
            CancellationToken cancellationToken = default)
    {
        if (parseResult.CommandId is null)
        {
            throw new InvalidOperationException(
                "A command with an invalid filename cannot be processed durably.");
        }

        var commandId = parseResult.CommandId.Value;

        await using var db =
            await dbContextFactory.CreateDbContextAsync(
                cancellationToken);

        var existing = await db.ProcessedSyncCommands
            .AsNoTracking()
            .SingleOrDefaultAsync(
                x => x.CommandId == commandId,
                cancellationToken);

        if (existing is not null)
        {
            return FromProcessedCommand(existing);
        }

        await using var transaction =
            await db.Database.BeginTransactionAsync(
                cancellationToken);

        var appliedAtUtc = DateTimeOffset.UtcNow;
        SyncCommandReceipt receipt;
        var stateChanged = false;

        if (!parseResult.Succeeded)
        {
            receipt = RejectedReceipt(
                commandId,
                parseResult.CommandType,
                appliedAtUtc,
                parseResult.Error ?? "Command is invalid.");
        }
        else
        {
            (receipt, stateChanged) =
                await ApplyCommandAsync(
                    db,
                    parseResult.Command!,
                    appliedAtUtc,
                    cancellationToken);
        }

        var receiptContent =
            JsonSerializer.SerializeToUtf8Bytes(
                receipt,
                GitHubSyncJson.Indented);

        db.ProcessedSyncCommands.Add(
            new ProcessedSyncCommand
            {
                CommandId = commandId,
                CommandType = receipt.Type,
                ProcessedAtUtc = receipt.AppliedAtUtc,
                Status = receipt.Status,
                ReceiptJson = Encoding.UTF8.GetString(
                    receiptContent)
            });

        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new SyncCommandProcessingOutcome(
            receipt,
            receiptContent,
            stateChanged,
            AlreadyProcessed: false);
    }

    private async Task<(SyncCommandReceipt Receipt, bool StateChanged)>
        ApplyCommandAsync(
            AppDbContext db,
            ParsedSyncCommand command,
            DateTimeOffset appliedAtUtc,
            CancellationToken cancellationToken)
    {
        try
        {
            return command.Type switch
            {
                SyncCommandTypes.CreateWorkItem =>
                    await ApplyCreateWorkItemAsync(
                        db,
                        command,
                        appliedAtUtc,
                        cancellationToken),

                SyncCommandTypes.CreateTodo =>
                    await ApplyCreateTodoAsync(
                        db,
                        command,
                        appliedAtUtc,
                        cancellationToken),

                SyncCommandTypes.CompleteTodo =>
                    await ApplyCompleteTodoAsync(
                        db,
                        command,
                        appliedAtUtc,
                        cancellationToken),

                SyncCommandTypes.MarkWorkItemWorkedOn =>
                    await ApplyMarkWorkItemWorkedOnAsync(
                        db,
                        command,
                        appliedAtUtc,
                        cancellationToken),

                SyncCommandTypes.SetWorkItemPriority =>
                    await ApplySetWorkItemPriorityAsync(
                        db,
                        command,
                        appliedAtUtc,
                        cancellationToken),

                SyncCommandTypes.SetTodoPriority =>
                    await ApplySetTodoPriorityAsync(
                        db,
                        command,
                        appliedAtUtc,
                        cancellationToken),

                _ =>
                    (
                        RejectedReceipt(
                            command.Id,
                            command.Type,
                            appliedAtUtc,
                            $"Unsupported command type '{command.Type}'."),
                        false
                    )
            };
        }
        catch (JsonException)
        {
            return (
                RejectedReceipt(
                    command.Id,
                    command.Type,
                    appliedAtUtc,
                    "Command payload is malformed."),
                false
            );
        }
    }

    private async Task<(SyncCommandReceipt, bool)>
        ApplyCreateWorkItemAsync(
            AppDbContext db,
            ParsedSyncCommand command,
            DateTimeOffset appliedAtUtc,
            CancellationToken cancellationToken)
    {
        var payload = command.Payload
            .Deserialize<CreateWorkItemCommandPayload>(
                GitHubSyncJson.Compact);

        if (payload is null)
        {
            return (
                RejectedReceipt(
                    command.Id,
                    command.Type,
                    appliedAtUtc,
                    "Command payload is required."),
                false
            );
        }

        var todoInputs = payload.Todos?
            .Select(todo =>
                todo is null
                    ? new CreateTodoInput(null)
                    : new CreateTodoInput(
                        todo.Task,
                        todo.Energy,
                        todo.Effort,
                        todo.Priority))
            .ToList();

        var result = mutationService.CreateWorkItemWithTodos(
            db,
            new CreateWorkItemInput(
                payload.Name,
                payload.Kind,
                payload.Description,
                payload.Url,
                payload.Priority),
            todoInputs);

        if (!result.Succeeded)
        {
            return (
                RejectedReceipt(
                    command.Id,
                    command.Type,
                    appliedAtUtc,
                    DescribeFailure(result)),
                false
            );
        }

        await db.SaveChangesAsync(cancellationToken);

        var created = result.Value!;

        return (
            AppliedReceipt(
                command,
                appliedAtUtc,
                new SyncCommandResult(
                    WorkItemId: created.WorkItem.Id,
                    TodoIds: created.Todos
                        .Select(todo => todo.Id)
                        .ToList())),
            true
        );
    }

    private async Task<(SyncCommandReceipt, bool)>
        ApplyCreateTodoAsync(
            AppDbContext db,
            ParsedSyncCommand command,
            DateTimeOffset appliedAtUtc,
            CancellationToken cancellationToken)
    {
        var payload = command.Payload
            .Deserialize<CreateTodoCommandPayload>(
                GitHubSyncJson.Compact);

        if (payload is null)
        {
            return (
                RejectedReceipt(
                    command.Id,
                    command.Type,
                    appliedAtUtc,
                    "Command payload is required."),
                false
            );
        }

        var result = await mutationService.CreateTodoAsync(
            db,
            payload.WorkItemId,
            new CreateTodoInput(
                payload.Task,
                payload.Energy,
                payload.Effort,
                payload.Priority),
            cancellationToken);

        if (!result.Succeeded)
        {
            return (
                RejectedReceipt(
                    command.Id,
                    command.Type,
                    appliedAtUtc,
                    DescribeFailure(result)),
                false
            );
        }

        await db.SaveChangesAsync(cancellationToken);

        var todo = result.Value!.Todo;

        return (
            AppliedReceipt(
                command,
                appliedAtUtc,
                new SyncCommandResult(
                    todo.WorkItemId,
                    todo.Id)),
            true
        );
    }

    private async Task<(SyncCommandReceipt, bool)>
        ApplyCompleteTodoAsync(
            AppDbContext db,
            ParsedSyncCommand command,
            DateTimeOffset appliedAtUtc,
            CancellationToken cancellationToken)
    {
        var payload = command.Payload
            .Deserialize<CompleteTodoCommandPayload>(
                GitHubSyncJson.Compact);

        if (payload is null)
        {
            return (
                RejectedReceipt(
                    command.Id,
                    command.Type,
                    appliedAtUtc,
                    "Command payload is required."),
                false
            );
        }

        var result = await mutationService.CompleteTodoAsync(
            db,
            payload.TodoId,
            cancellationToken);

        if (!result.Succeeded)
        {
            return (
                RejectedReceipt(
                    command.Id,
                    command.Type,
                    appliedAtUtc,
                    DescribeFailure(result)),
                false
            );
        }

        await db.SaveChangesAsync(cancellationToken);

        var todo = result.Value!;

        return (
            AppliedReceipt(
                command,
                appliedAtUtc,
                new SyncCommandResult(
                    todo.WorkItemId,
                    todo.Id)),
            result.Changed
        );
    }

    private async Task<(SyncCommandReceipt, bool)>
        ApplyMarkWorkItemWorkedOnAsync(
            AppDbContext db,
            ParsedSyncCommand command,
            DateTimeOffset appliedAtUtc,
            CancellationToken cancellationToken)
    {
        var payload = command.Payload
            .Deserialize<MarkWorkItemWorkedOnCommandPayload>(
                GitHubSyncJson.Compact);

        if (payload is null)
        {
            return MissingPayload(command, appliedAtUtc);
        }

        var result = await mutationService.MarkWorkItemWorkedOnAsync(
            db,
            payload.WorkItemId,
            payload.Note,
            cancellationToken);

        if (!result.Succeeded)
        {
            return RejectedMutation(
                command,
                appliedAtUtc,
                result);
        }

        await db.SaveChangesAsync(cancellationToken);

        return (
            AppliedReceipt(
                command,
                appliedAtUtc,
                new SyncCommandResult(
                    WorkItemId: result.Value!.Id)),
            result.Changed
        );
    }

    private async Task<(SyncCommandReceipt, bool)>
        ApplySetWorkItemPriorityAsync(
            AppDbContext db,
            ParsedSyncCommand command,
            DateTimeOffset appliedAtUtc,
            CancellationToken cancellationToken)
    {
        var payload = command.Payload
            .Deserialize<SetWorkItemPriorityCommandPayload>(
                GitHubSyncJson.Compact);

        if (payload is null)
        {
            return MissingPayload(command, appliedAtUtc);
        }

        var result = await mutationService.SetWorkItemPriorityAsync(
            db,
            payload.WorkItemId,
            payload.Priority,
            cancellationToken);

        if (!result.Succeeded)
        {
            return RejectedMutation(
                command,
                appliedAtUtc,
                result);
        }

        await db.SaveChangesAsync(cancellationToken);

        return (
            AppliedReceipt(
                command,
                appliedAtUtc,
                new SyncCommandResult(
                    WorkItemId: result.Value!.Id)),
            result.Changed
        );
    }

    private async Task<(SyncCommandReceipt, bool)>
        ApplySetTodoPriorityAsync(
            AppDbContext db,
            ParsedSyncCommand command,
            DateTimeOffset appliedAtUtc,
            CancellationToken cancellationToken)
    {
        var payload = command.Payload
            .Deserialize<SetTodoPriorityCommandPayload>(
                GitHubSyncJson.Compact);

        if (payload is null)
        {
            return MissingPayload(command, appliedAtUtc);
        }

        var result = await mutationService.SetTodoPriorityAsync(
            db,
            payload.TodoId,
            payload.Priority,
            cancellationToken);

        if (!result.Succeeded)
        {
            return RejectedMutation(
                command,
                appliedAtUtc,
                result);
        }

        await db.SaveChangesAsync(cancellationToken);

        var todo = result.Value!;

        return (
            AppliedReceipt(
                command,
                appliedAtUtc,
                new SyncCommandResult(
                    todo.WorkItemId,
                    todo.Id)),
            result.Changed
        );
    }

    private static (SyncCommandReceipt, bool) MissingPayload(
        ParsedSyncCommand command,
        DateTimeOffset appliedAtUtc)
    {
        return (
            RejectedReceipt(
                command.Id,
                command.Type,
                appliedAtUtc,
                "Command payload is required."),
            false
        );
    }

    private static (SyncCommandReceipt, bool) RejectedMutation<T>(
        ParsedSyncCommand command,
        DateTimeOffset appliedAtUtc,
        PlanningMutationResult<T> result)
        where T : class
    {
        return (
            RejectedReceipt(
                command.Id,
                command.Type,
                appliedAtUtc,
                DescribeFailure(result)),
            false
        );
    }

    private static SyncCommandProcessingOutcome
        FromProcessedCommand(
            ProcessedSyncCommand processed)
    {
        var content = Encoding.UTF8.GetBytes(
            processed.ReceiptJson);

        var receipt = JsonSerializer.Deserialize<SyncCommandReceipt>(
            content,
            GitHubSyncJson.Compact)
            ?? throw new InvalidOperationException(
                $"Stored receipt for command {processed.CommandId} is invalid.");

        return new SyncCommandProcessingOutcome(
            receipt,
            content,
            StateChanged: false,
            AlreadyProcessed: true);
    }

    private static SyncCommandReceipt AppliedReceipt(
        ParsedSyncCommand command,
        DateTimeOffset appliedAtUtc,
        SyncCommandResult result)
    {
        return new SyncCommandReceipt(
            1,
            command.Id,
            command.Type,
            "applied",
            appliedAtUtc,
            result);
    }

    private static SyncCommandReceipt RejectedReceipt(
        Guid commandId,
        string commandType,
        DateTimeOffset appliedAtUtc,
        string error)
    {
        return new SyncCommandReceipt(
            1,
            commandId,
            commandType,
            "rejected",
            appliedAtUtc,
            Error: error);
    }

    private static string DescribeFailure<T>(
        PlanningMutationResult<T> result)
        where T : class
    {
        if (result.ValidationErrors is not null)
        {
            return string.Join(
                " ",
                result.ValidationErrors
                    .OrderBy(pair => pair.Key)
                    .SelectMany(pair =>
                        pair.Value.Select(message =>
                            $"{pair.Key}: {message}")));
        }

        return result.Error ??
               "The command could not be applied.";
    }
}

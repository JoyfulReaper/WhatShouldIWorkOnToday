using Microsoft.EntityFrameworkCore;
using WhatShouldIWorkOnToday.GitHubSync;
using WhatShouldIWorkOnToday.Models;
using WhatShouldIWorkOnToday.Services;
using Xunit;

namespace WhatShouldIWorkOnToday.Tests;

public sealed class SyncCommandProcessorTests
{
    [Fact]
    public async Task CreateWorkItem_IsValidatedNormalizedAndIdempotent()
    {
        await using var database =
            await TemporarySqliteDatabase.CreateAsync();

        var id = Guid.NewGuid();
        var command = Parse(
            SyncTestCommands.CreateFile(
                id,
                SyncCommandTypes.CreateWorkItem,
                new
                {
                    name = "  Synced project  ",
                    description = "  Created remotely  ",
                    priority = "high"
                }));

        var processor = CreateProcessor(database);

        var first = await processor.ProcessAsync(command);
        var second = await processor.ProcessAsync(command);

        Assert.Equal("applied", first.Receipt.Status);
        Assert.NotNull(first.Receipt.Result?.WorkItemId);
        Assert.False(first.AlreadyProcessed);
        Assert.True(second.AlreadyProcessed);

        await using var db = await database.Factory
            .CreateDbContextAsync();

        var workItem = await db.WorkItems.SingleAsync();

        Assert.Equal("Synced project", workItem.Name);
        Assert.Equal(
            "Created remotely",
            workItem.Description);
        Assert.Equal(
            WorkItemKind.Project,
            workItem.Kind);
        Assert.Equal(PriorityLevel.High, workItem.Priority);
        Assert.Equal(
            first.Receipt.Result!.WorkItemId,
            workItem.Id);
        Assert.Equal(
            1,
            await db.ProcessedSyncCommands.CountAsync());
    }

    [Fact]
    public async Task CreateWorkItem_BlankName_IsRejectedWithApiValidation()
    {
        await using var database =
            await TemporarySqliteDatabase.CreateAsync();

        var outcome = await CreateProcessor(database)
            .ProcessAsync(
                Parse(
                    SyncTestCommands.CreateFile(
                        Guid.NewGuid(),
                        SyncCommandTypes.CreateWorkItem,
                        new
                        {
                            name = "   "
                        })));

        Assert.Equal("rejected", outcome.Receipt.Status);
        Assert.Contains(
            "name: Name is required.",
            outcome.Receipt.Error,
            StringComparison.Ordinal);

        await using var db = await database.Factory
            .CreateDbContextAsync();

        Assert.Equal(
            0,
            await db.WorkItems.CountAsync());
    }

    [Fact]
    public async Task CreateTodo_IsValidatedNormalizedAndIdempotent()
    {
        await using var database =
            await TemporarySqliteDatabase.CreateAsync();

        var workItemId = await CreateWorkItemAsync(
            database,
            active: true);

        var id = Guid.NewGuid();
        var command = Parse(
            SyncTestCommands.CreateFile(
                id,
                SyncCommandTypes.CreateTodo,
                new
                {
                    workItemId,
                    task = "  Synced todo  ",
                    priority = "low"
                }));

        var processor = CreateProcessor(database);

        var first = await processor.ProcessAsync(command);
        var second = await processor.ProcessAsync(command);

        Assert.Equal("applied", first.Receipt.Status);
        Assert.Equal(
            workItemId,
            first.Receipt.Result?.WorkItemId);
        Assert.NotNull(first.Receipt.Result?.TodoId);
        Assert.True(second.AlreadyProcessed);

        await using var db = await database.Factory
            .CreateDbContextAsync();

        var todo = await db.TodoItems.SingleAsync();

        Assert.Equal("Synced todo", todo.Task);
        Assert.Equal(EnergyLevel.Medium, todo.Energy);
        Assert.Equal(EffortLevel.Medium, todo.Effort);
        Assert.Equal(PriorityLevel.Low, todo.Priority);
        Assert.Equal(
            first.Receipt.Result!.TodoId,
            todo.Id);
    }

    [Fact]
    public async Task CreateTodo_InvalidTask_IsRejectedWithApiValidation()
    {
        await using var database =
            await TemporarySqliteDatabase.CreateAsync();

        var workItemId = await CreateWorkItemAsync(
            database,
            active: true);

        var outcome = await CreateProcessor(database)
            .ProcessAsync(
                Parse(
                    SyncTestCommands.CreateFile(
                        Guid.NewGuid(),
                        SyncCommandTypes.CreateTodo,
                        new
                        {
                            workItemId,
                            task = "   "
                        })));

        Assert.Equal("rejected", outcome.Receipt.Status);
        Assert.Contains(
            "task: Task is required.",
            outcome.Receipt.Error,
            StringComparison.Ordinal);

        await using var db = await database.Factory
            .CreateDbContextAsync();

        Assert.Equal(
            0,
            await db.TodoItems.CountAsync());
    }

    [Fact]
    public async Task OldCreationCommands_DefaultBothPrioritiesToNormal()
    {
        await using var database = await TemporarySqliteDatabase.CreateAsync();
        var processor = CreateProcessor(database);

        var workOutcome = await processor.ProcessAsync(
            Parse(
                SyncTestCommands.CreateFile(
                    Guid.NewGuid(),
                    SyncCommandTypes.CreateWorkItem,
                    new { name = "Old command parent" })));

        var workItemId = workOutcome.Receipt.Result!.WorkItemId!.Value;

        await processor.ProcessAsync(
            Parse(
                SyncTestCommands.CreateFile(
                    Guid.NewGuid(),
                    SyncCommandTypes.CreateTodo,
                    new { workItemId, task = "Old command todo" })));

        await using var db = await database.Factory.CreateDbContextAsync();
        Assert.Equal(
            PriorityLevel.Normal,
            (await db.WorkItems.SingleAsync()).Priority);
        Assert.Equal(
            PriorityLevel.Normal,
            (await db.TodoItems.SingleAsync()).Priority);
    }

    [Fact]
    public async Task CreateTodo_OnInactiveWorkItem_IsRejected()
    {
        await using var database =
            await TemporarySqliteDatabase.CreateAsync();

        var workItemId = await CreateWorkItemAsync(
            database,
            active: false);

        var outcome = await CreateProcessor(database)
            .ProcessAsync(
                Parse(
                    SyncTestCommands.CreateFile(
                        Guid.NewGuid(),
                        SyncCommandTypes.CreateTodo,
                        new
                        {
                            workItemId,
                            task = "Cannot add this"
                        })));

        Assert.Equal("rejected", outcome.Receipt.Status);
        Assert.Equal(
            "Cannot add a todo to an inactive work item.",
            outcome.Receipt.Error);

        await using var db = await database.Factory
            .CreateDbContextAsync();

        Assert.Equal(
            0,
            await db.TodoItems.CountAsync());
    }

    [Fact]
    public async Task CompleteTodo_PreservesCompletionAndHistorySemantics()
    {
        await using var database =
            await TemporarySqliteDatabase.CreateAsync();

        int todoId;

        await using (var db = await database.Factory
                         .CreateDbContextAsync())
        {
            var seededWorkItem = new WorkItem
            {
                Name = "Completion project"
            };

            var seededTodo = new TodoItem
            {
                Task = "Finish this"
            };

            seededWorkItem.Todos.Add(seededTodo);
            db.WorkItems.Add(seededWorkItem);
            await db.SaveChangesAsync();
            todoId = seededTodo.Id;
        }

        var outcome = await CreateProcessor(database)
            .ProcessAsync(
                Parse(
                    SyncTestCommands.CreateFile(
                        Guid.NewGuid(),
                        SyncCommandTypes.CompleteTodo,
                        new
                        {
                            todoId
                        })));

        Assert.Equal("applied", outcome.Receipt.Status);
        Assert.Equal(todoId, outcome.Receipt.Result?.TodoId);

        await using var verifyDb = await database.Factory
            .CreateDbContextAsync();

        var todo = await verifyDb.TodoItems.SingleAsync();
        var workItem = await verifyDb.WorkItems.SingleAsync();
        var history = await verifyDb.WorkHistoryEntries
            .SingleAsync();

        Assert.NotNull(todo.CompletedAt);
        Assert.Equal(todo.CompletedAt, workItem.LastWorkedAt);
        Assert.Equal(todo.Id, history.TodoItemId);
        Assert.Equal(todo.Task, history.TaskSnapshot);
        Assert.Equal(todo.CompletedAt, history.WorkedAt);
    }

    private static SyncCommandProcessor CreateProcessor(
        TemporarySqliteDatabase database)
    {
        return new SyncCommandProcessor(
            database.Factory,
            new PlanningMutationService());
    }

    private static SyncCommandParseResult Parse(
        GitHubSyncFile file)
    {
        var result = new SyncCommandParser().Parse(file);
        Assert.True(result.Succeeded, result.Error);
        return result;
    }

    private static async Task<int> CreateWorkItemAsync(
        TemporarySqliteDatabase database,
        bool active)
    {
        await using var db = await database.Factory
            .CreateDbContextAsync();

        var workItem = new WorkItem
        {
            Name = "Parent",
            CompletedAt = active
                ? null
                : DateTimeOffset.UtcNow
        };

        db.WorkItems.Add(workItem);
        await db.SaveChangesAsync();

        return workItem.Id;
    }
}

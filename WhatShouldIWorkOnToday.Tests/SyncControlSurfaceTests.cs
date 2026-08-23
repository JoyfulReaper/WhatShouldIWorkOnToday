using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WhatShouldIWorkOnToday.GitHubSync;
using WhatShouldIWorkOnToday.Models;
using WhatShouldIWorkOnToday.Services;
using Xunit;

namespace WhatShouldIWorkOnToday.Tests;

public sealed class SyncControlSurfaceTests
{
    [Fact]
    public async Task CreateWorkItemWithTodos_IsAtomic_ReturnsIds_AppliesDefaults_AndReplaysSafely()
    {
        await using var database = await TemporarySqliteDatabase.CreateAsync();
        var command = Command(
            SyncCommandTypes.CreateWorkItem,
            new
            {
                name = "Project with backlog",
                priority = "high",
                todos = new object[]
                {
                    new
                    {
                        task = "Explicit todo",
                        energy = "low",
                        effort = "short",
                        priority = "high"
                    },
                    new
                    {
                        task = "Default todo"
                    }
                }
            });
        var processor = CreateProcessor(database);

        var first = await processor.ProcessAsync(command);
        var replay = await processor.ProcessAsync(command);

        Assert.Equal("applied", first.Receipt.Status);
        Assert.True(first.StateChanged);
        Assert.True(replay.AlreadyProcessed);
        Assert.False(replay.StateChanged);

        await using var db = await database.Factory.CreateDbContextAsync();
        var workItem = await db.WorkItems
            .Include(x => x.Todos)
            .SingleAsync();
        var todoIds = first.Receipt.Result?.TodoIds;

        Assert.Equal(workItem.Id, first.Receipt.Result?.WorkItemId);
        Assert.NotNull(todoIds);
        Assert.Equal(2, todoIds.Count);
        Assert.Equal(
            workItem.Todos.Select(x => x.Id).Order().ToArray(),
            todoIds.Order().ToArray());
        Assert.Equal(PriorityLevel.High, workItem.Priority);

        var explicitTodo = workItem.Todos.Single(x => x.Task == "Explicit todo");
        Assert.Equal(EnergyLevel.Low, explicitTodo.Energy);
        Assert.Equal(EffortLevel.Short, explicitTodo.Effort);
        Assert.Equal(PriorityLevel.High, explicitTodo.Priority);

        var defaultTodo = workItem.Todos.Single(x => x.Task == "Default todo");
        Assert.Equal(EnergyLevel.Medium, defaultTodo.Energy);
        Assert.Equal(EffortLevel.Medium, defaultTodo.Effort);
        Assert.Equal(PriorityLevel.Normal, defaultTodo.Priority);
        Assert.Equal(1, await db.ProcessedSyncCommands.CountAsync());
    }

    [Fact]
    public async Task CreateWorkItemWithInvalidChild_RollsBackEntireAggregate()
    {
        await using var database = await TemporarySqliteDatabase.CreateAsync();

        var outcome = await CreateProcessor(database).ProcessAsync(
            Command(
                SyncCommandTypes.CreateWorkItem,
                new
                {
                    name = "Must not survive",
                    todos = new object[]
                    {
                        new { task = "Valid child" },
                        new { task = "   " }
                    }
                }));

        Assert.Equal("rejected", outcome.Receipt.Status);
        Assert.Contains("todos[1].task", outcome.Receipt.Error);

        await using var db = await database.Factory.CreateDbContextAsync();
        Assert.Equal(0, await db.WorkItems.CountAsync());
        Assert.Equal(0, await db.TodoItems.CountAsync());
    }

    [Fact]
    public async Task CreateWorkItem_RejectsMoreThanMaximumInitialTodos()
    {
        await using var database = await TemporarySqliteDatabase.CreateAsync();
        var todos = Enumerable.Range(
                1,
                PlanningMutationService.MaximumInitialTodoCount + 1)
            .Select(index => new { task = $"Todo {index}" })
            .ToArray();

        var outcome = await CreateProcessor(database).ProcessAsync(
            Command(
                SyncCommandTypes.CreateWorkItem,
                new { name = "Too many children", todos }));

        Assert.Equal("rejected", outcome.Receipt.Status);
        Assert.Contains("A maximum of 100 todos", outcome.Receipt.Error);

        await using var db = await database.Factory.CreateDbContextAsync();
        Assert.Equal(0, await db.WorkItems.CountAsync());
    }

    [Fact]
    public async Task CreateWorkItemWithoutTodos_RemainsValid()
    {
        await using var database = await TemporarySqliteDatabase.CreateAsync();

        var outcome = await CreateProcessor(database).ProcessAsync(
            Command(
                SyncCommandTypes.CreateWorkItem,
                new { name = "Legacy shape" }));

        Assert.Equal("applied", outcome.Receipt.Status);
        Assert.Empty(outcome.Receipt.Result!.TodoIds!);

        await using var db = await database.Factory.CreateDbContextAsync();
        Assert.Equal(PriorityLevel.Normal, (await db.WorkItems.SingleAsync()).Priority);
        Assert.Equal(0, await db.TodoItems.CountAsync());
    }

    [Fact]
    public async Task MarkWorkedOn_UpdatesTimestamp_StoresTrimmedNote_AndReplayDoesNotDuplicateHistory()
    {
        await using var database = await TemporarySqliteDatabase.CreateAsync();
        var (workItemId, _) = await SeedAsync(database);
        var command = Command(
            SyncCommandTypes.MarkWorkItemWorkedOn,
            new
            {
                workItemId,
                note = "  Worked on listener support  "
            });
        var processor = CreateProcessor(database);

        var first = await processor.ProcessAsync(command);
        var replay = await processor.ProcessAsync(command);

        Assert.Equal("applied", first.Receipt.Status);
        Assert.Equal(workItemId, first.Receipt.Result?.WorkItemId);
        Assert.True(first.StateChanged);
        Assert.True(replay.AlreadyProcessed);

        await using var db = await database.Factory.CreateDbContextAsync();
        var workItem = await db.WorkItems.SingleAsync();
        var history = await db.WorkHistoryEntries.SingleAsync();

        Assert.NotNull(workItem.LastWorkedAt);
        Assert.Equal(workItem.LastWorkedAt, history.WorkedAt);
        Assert.Equal("Worked on listener support", history.Note);
        Assert.Equal(workItemId, history.WorkItemId);
        Assert.Null(history.TodoItemId);
        Assert.Null(history.TaskSnapshot);
        Assert.Equal(0, await db.TodoItems.CountAsync(x => x.CompletedAt != null));
    }

    [Fact]
    public async Task MarkWorkedOn_BlankNoteBecomesNull()
    {
        await using var database = await TemporarySqliteDatabase.CreateAsync();
        var (workItemId, _) = await SeedAsync(database);

        var outcome = await CreateProcessor(database).ProcessAsync(
            Command(
                SyncCommandTypes.MarkWorkItemWorkedOn,
                new { workItemId, note = "   " }));

        Assert.Equal("applied", outcome.Receipt.Status);

        await using var db = await database.Factory.CreateDbContextAsync();
        Assert.Null((await db.WorkHistoryEntries.SingleAsync()).Note);
    }

    [Fact]
    public async Task MarkWorkedOn_RejectsMissingWorkItem()
    {
        await using var database = await TemporarySqliteDatabase.CreateAsync();

        var outcome = await CreateProcessor(database).ProcessAsync(
            Command(
                SyncCommandTypes.MarkWorkItemWorkedOn,
                new { workItemId = 999999 }));

        Assert.Equal("rejected", outcome.Receipt.Status);
        Assert.Equal("Work item does not exist.", outcome.Receipt.Error);
    }

    [Fact]
    public async Task MarkWorkedOn_RejectsTooLongNote()
    {
        await using var database = await TemporarySqliteDatabase.CreateAsync();
        var (workItemId, _) = await SeedAsync(database);

        var outcome = await CreateProcessor(database).ProcessAsync(
            Command(
                SyncCommandTypes.MarkWorkItemWorkedOn,
                new { workItemId, note = new string('n', 2001) }));

        Assert.Equal("rejected", outcome.Receipt.Status);
        Assert.Contains("Note cannot exceed 2000 characters", outcome.Receipt.Error);

        await using var db = await database.Factory.CreateDbContextAsync();
        Assert.Equal(0, await db.WorkHistoryEntries.CountAsync());
        Assert.Null((await db.WorkItems.SingleAsync()).LastWorkedAt);
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async Task MarkWorkedOn_RejectsInactiveWorkItem(
        bool completed,
        bool archived)
    {
        await using var database = await TemporarySqliteDatabase.CreateAsync();
        var (workItemId, _) = await SeedAsync(database, completed, archived);

        var outcome = await CreateProcessor(database).ProcessAsync(
            Command(
                SyncCommandTypes.MarkWorkItemWorkedOn,
                new { workItemId }));

        Assert.Equal("rejected", outcome.Receipt.Status);
        Assert.Equal(
            "Cannot mark an inactive work item as worked on.",
            outcome.Receipt.Error);
    }

    [Fact]
    public async Task SetWorkItemPriority_ChangesValue_AndSameValueIsSuccessfulNoOp()
    {
        await using var database = await TemporarySqliteDatabase.CreateAsync();
        var (workItemId, _) = await SeedAsync(database);
        var processor = CreateProcessor(database);

        var changed = await processor.ProcessAsync(
            Command(
                SyncCommandTypes.SetWorkItemPriority,
                new { workItemId, priority = "HIGH" }));
        var unchanged = await processor.ProcessAsync(
            Command(
                SyncCommandTypes.SetWorkItemPriority,
                new { workItemId, priority = "high" }));

        Assert.Equal("applied", changed.Receipt.Status);
        Assert.True(changed.StateChanged);
        Assert.Equal(workItemId, changed.Receipt.Result?.WorkItemId);
        Assert.Equal("applied", unchanged.Receipt.Status);
        Assert.False(unchanged.StateChanged);

        await using var db = await database.Factory.CreateDbContextAsync();
        Assert.Equal(PriorityLevel.High, (await db.WorkItems.SingleAsync()).Priority);
    }

    [Theory]
    [InlineData("invalid", 1, "priority: Priority must be Low, Normal, or High.")]
    [InlineData("High", 999999, "Work item does not exist.")]
    public async Task SetWorkItemPriority_RejectsInvalidOrMissingTarget(
        string priority,
        int workItemId,
        string expectedError)
    {
        await using var database = await TemporarySqliteDatabase.CreateAsync();
        if (workItemId == 1)
        {
            await SeedAsync(database);
        }

        var outcome = await CreateProcessor(database).ProcessAsync(
            Command(
                SyncCommandTypes.SetWorkItemPriority,
                new { workItemId, priority }));

        Assert.Equal("rejected", outcome.Receipt.Status);
        Assert.Contains(expectedError, outcome.Receipt.Error);
    }

    [Fact]
    public async Task SetTodoPriority_ChangesValue_AndSameValueIsSuccessfulNoOp()
    {
        await using var database = await TemporarySqliteDatabase.CreateAsync();
        var (workItemId, todoId) = await SeedAsync(database, withTodo: true);
        var processor = CreateProcessor(database);

        var changed = await processor.ProcessAsync(
            Command(
                SyncCommandTypes.SetTodoPriority,
                new { todoId, priority = "low" }));
        var unchanged = await processor.ProcessAsync(
            Command(
                SyncCommandTypes.SetTodoPriority,
                new { todoId, priority = "LOW" }));

        Assert.Equal("applied", changed.Receipt.Status);
        Assert.True(changed.StateChanged);
        Assert.Equal(workItemId, changed.Receipt.Result?.WorkItemId);
        Assert.Equal(todoId, changed.Receipt.Result?.TodoId);
        Assert.Equal("applied", unchanged.Receipt.Status);
        Assert.False(unchanged.StateChanged);

        await using var db = await database.Factory.CreateDbContextAsync();
        Assert.Equal(PriorityLevel.Low, (await db.TodoItems.SingleAsync()).Priority);
    }

    [Theory]
    [InlineData("urgent", 1, "priority: Priority must be Low, Normal, or High.")]
    [InlineData("Low", 999999, "Todo does not exist.")]
    public async Task SetTodoPriority_RejectsInvalidOrMissingTarget(
        string priority,
        int todoId,
        string expectedError)
    {
        await using var database = await TemporarySqliteDatabase.CreateAsync();
        if (todoId == 1)
        {
            await SeedAsync(database, withTodo: true);
        }

        var outcome = await CreateProcessor(database).ProcessAsync(
            Command(
                SyncCommandTypes.SetTodoPriority,
                new { todoId, priority }));

        Assert.Equal("rejected", outcome.Receipt.Status);
        Assert.Contains(expectedError, outcome.Receipt.Error);
    }

    [Fact]
    public void OlderReceiptWithoutTodoIds_StillDeserializes()
    {
        const string json =
            """
            {
              "schemaVersion": 1,
              "id": "77777777-7777-4777-8777-777777777777",
              "type": "createWorkItem",
              "status": "applied",
              "appliedAtUtc": "2026-08-21T12:00:00Z",
              "result": { "workItemId": 4 }
            }
            """;

        var receipt = JsonSerializer.Deserialize<SyncCommandReceipt>(
            json,
            GitHubSyncJson.Compact);

        Assert.NotNull(receipt);
        Assert.Equal(4, receipt.Result?.WorkItemId);
        Assert.Null(receipt.Result?.TodoIds);
    }

    private static SyncCommandParseResult Command(
        string type,
        object payload)
    {
        var file = SyncTestCommands.CreateFile(
            Guid.NewGuid(),
            type,
            payload);
        var result = new SyncCommandParser().Parse(file);
        Assert.True(result.Succeeded, result.Error);
        return result;
    }

    private static SyncCommandProcessor CreateProcessor(
        TemporarySqliteDatabase database)
    {
        return new SyncCommandProcessor(
            database.Factory,
            new PlanningMutationService());
    }

    private static async Task<(int WorkItemId, int TodoId)> SeedAsync(
        TemporarySqliteDatabase database,
        bool completed = false,
        bool archived = false,
        bool withTodo = false)
    {
        await using var db = await database.Factory.CreateDbContextAsync();
        var workItem = new WorkItem
        {
            Name = "Target",
            CompletedAt = completed ? DateTimeOffset.UtcNow : null,
            ArchivedAt = archived ? DateTimeOffset.UtcNow : null
        };
        var todo = new TodoItem { Task = "Target todo" };

        if (withTodo)
        {
            workItem.Todos.Add(todo);
        }

        db.WorkItems.Add(workItem);
        await db.SaveChangesAsync();
        return (workItem.Id, withTodo ? todo.Id : 0);
    }
}

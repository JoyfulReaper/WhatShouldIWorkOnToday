using Microsoft.EntityFrameworkCore;
using WhatShouldIWorkOnToday.Models;
using WhatShouldIWorkOnToday.Services;
using Xunit;

namespace WhatShouldIWorkOnToday.Tests;

public sealed class DailyPickTests
{
    [Fact]
    public async Task ConcurrentRequestsForSameDate_ReturnSamePick()
    {
        await using var database = await TemporarySqliteDatabase.CreateAsync();
        await SeedTodosAsync(database);

        var chooser = new WorkChooser(database.Factory);
        var date = new DateOnly(2026, 8, 25);

        var firstTask = chooser.ChooseDailyAsync(date);
        var secondTask = chooser.ChooseDailyAsync(date);

        var results = await Task.WhenAll(firstTask, secondTask);

        Assert.NotNull(results[0]);
        Assert.NotNull(results[1]);
        Assert.Equal(results[0]!.Id, results[1]!.Id);

        await using var db = await database.Factory.CreateDbContextAsync();

        Assert.Equal(
            1,
            await db.DailyPicks.CountAsync(x => x.Date == date));
    }

    [Fact]
    public async Task SameDate_ReturnsPersistedPick()
    {
        await using var database = await TemporarySqliteDatabase.CreateAsync();
        await SeedTodosAsync(database);

        var chooser = new WorkChooser(database.Factory);
        var date = new DateOnly(2026, 8, 25);

        var first = await chooser.ChooseDailyAsync(date);

        Assert.NotNull(first);

        var firstId = first.Id;

        await using (var db = await database.Factory.CreateDbContextAsync())
        {
            var otherTodo = await db.TodoItems
                .Include(x => x.WorkItem)
                .SingleAsync(x => x.Id != firstId);

            otherTodo.Priority = PriorityLevel.High;
            otherTodo.WorkItem.Priority = PriorityLevel.High;

            var selectedTodo = await db.TodoItems
                .Include(x => x.WorkItem)
                .SingleAsync(x => x.Id == firstId);

            selectedTodo.Priority = PriorityLevel.Low;
            selectedTodo.WorkItem.Priority = PriorityLevel.Low;

            await db.SaveChangesAsync();
        }

        var second = await chooser.ChooseDailyAsync(date);

        Assert.NotNull(second);
        Assert.Equal(firstId, second.Id);
    }

    [Fact]
    public async Task DailyPick_IsPersistedToDatabase()
    {
        await using var database = await TemporarySqliteDatabase.CreateAsync();
        await SeedTodosAsync(database);

        var chooser = new WorkChooser(database.Factory);
        var date = new DateOnly(2026, 8, 25);

        var selected = await chooser.ChooseDailyAsync(date);

        Assert.NotNull(selected);

        await using var db = await database.Factory.CreateDbContextAsync();

        var dailyPick = await db.DailyPicks.SingleAsync();

        Assert.Equal(date, dailyPick.Date);
        Assert.Equal(selected.Id, dailyPick.TodoItemId);
    }

    [Fact]
    public async Task DifferentDates_CreateSeparateDailyPicks()
    {
        await using var database = await TemporarySqliteDatabase.CreateAsync();
        await SeedTodosAsync(database);

        var chooser = new WorkChooser(database.Factory);

        await chooser.ChooseDailyAsync(new DateOnly(2026, 8, 25));
        await chooser.ChooseDailyAsync(new DateOnly(2026, 8, 26));

        await using var db = await database.Factory.CreateDbContextAsync();

        Assert.Equal(2, await db.DailyPicks.CountAsync());
    }

    private static async Task SeedTodosAsync(
        TemporarySqliteDatabase database)
    {
        await using var db =
            await database.Factory.CreateDbContextAsync();

        var first = new WorkItem
        {
            Name = "First",
            Priority = PriorityLevel.Normal
        };

        first.Todos.Add(new TodoItem
        {
            Task = "First todo",
            Energy = EnergyLevel.Medium,
            Effort = EffortLevel.Medium,
            Priority = PriorityLevel.Normal
        });

        var second = new WorkItem
        {
            Name = "Second",
            Priority = PriorityLevel.Normal
        };

        second.Todos.Add(new TodoItem
        {
            Task = "Second todo",
            Energy = EnergyLevel.Medium,
            Effort = EffortLevel.Medium,
            Priority = PriorityLevel.Normal
        });

        db.WorkItems.AddRange(first, second);
        await db.SaveChangesAsync();
    }
}
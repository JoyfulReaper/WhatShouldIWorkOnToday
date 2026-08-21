using Microsoft.EntityFrameworkCore;
using WhatShouldIWorkOnToday.Models;
using WhatShouldIWorkOnToday.Services;
using Xunit;

namespace WhatShouldIWorkOnToday.Tests;

public sealed class PriorityTests
{
    [Fact]
    public void NewEntities_DefaultToNormalPriority()
    {
        Assert.Equal(PriorityLevel.Normal, new WorkItem().Priority);
        Assert.Equal(PriorityLevel.Normal, new TodoItem().Priority);
    }

    [Theory]
    [InlineData("low", PriorityLevel.Low)]
    [InlineData("NORMAL", PriorityLevel.Normal)]
    [InlineData("High", PriorityLevel.High)]
    [InlineData(null, PriorityLevel.Normal)]
    [InlineData("   ", PriorityLevel.Normal)]
    public void PriorityParsing_AcceptsSupportedValuesAndDefaults(
        string? value,
        PriorityLevel expected)
    {
        Assert.True(PlanningMutationService.TryParsePriority(value, out var actual));
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void PriorityParsing_RejectsInvalidValue()
    {
        Assert.False(PlanningMutationService.TryParsePriority("Urgent", out _));

        var errors = new Dictionary<string, string[]>();
        PlanningMutationService.ValidateAndNormalizeTodo(
            "Task",
            null,
            null,
            "Urgent",
            string.Empty,
            errors);

        Assert.Equal(
            "Priority must be Low, Normal, or High.",
            Assert.Single(errors["priority"]));
    }

    [Theory]
    [InlineData(PriorityLevel.Normal, PriorityLevel.Normal, 0)]
    [InlineData(PriorityLevel.High, PriorityLevel.Normal, 45)]
    [InlineData(PriorityLevel.High, PriorityLevel.High, 90)]
    [InlineData(PriorityLevel.High, PriorityLevel.Low, 0)]
    [InlineData(PriorityLevel.Low, PriorityLevel.Low, -90)]
    public void ParentAndTodoPriorityBiases_Combine(
        PriorityLevel workItemPriority,
        PriorityLevel todoPriority,
        int expected)
    {
        var todo = new TodoItem
        {
            Priority = todoPriority,
            WorkItem = new WorkItem { Priority = workItemPriority }
        };

        Assert.Equal(expected, WorkChooser.GetPriorityScore(todo));
    }

    [Fact]
    public async Task HighPriorityImprovesRank_AndLowPriorityReducesRank()
    {
        await using var database = await TemporarySqliteDatabase.CreateAsync();
        var now = DateTimeOffset.UtcNow;

        await SeedAsync(database, "High", PriorityLevel.High, now);
        await SeedAsync(database, "Older normal", PriorityLevel.Normal, now.AddDays(-14));

        var chooser = new WorkChooser(database.Factory);
        var highRanked = await chooser.ChooseAsync(
            EnergyLevel.High,
            EffortLevel.Long,
            count: 2);

        Assert.Equal("High", highRanked[0].Task);

        await using (var db = await database.Factory.CreateDbContextAsync())
        {
            var high = await db.TodoItems.SingleAsync(x => x.Task == "High");
            high.Priority = PriorityLevel.Low;
            await db.SaveChangesAsync();
        }

        var lowRanked = await chooser.ChooseAsync(
            EnergyLevel.High,
            EffortLevel.Long,
            count: 2);

        Assert.Equal("Older normal", lowRanked[0].Task);
    }

    [Fact]
    public async Task DailySelectionIsDeterministic_AndRandomSelectionStillAllowsLowPriority()
    {
        await using var database = await TemporarySqliteDatabase.CreateAsync();
        var todoId = await SeedAsync(
            database,
            "Low priority candidate",
            PriorityLevel.Low,
            DateTimeOffset.UtcNow,
            PriorityLevel.Low);

        var chooser = new WorkChooser(database.Factory);
        var date = new DateOnly(2026, 8, 21);

        var first = await chooser.ChooseDailyAsync(date);
        var second = await chooser.ChooseDailyAsync(date);
        var random = await chooser.ChooseRandomAsync();

        Assert.Equal(todoId, first?.Id);
        Assert.Equal(first?.Id, second?.Id);
        Assert.Equal(todoId, random?.Id);
    }

    private static async Task<int> SeedAsync(
        TemporarySqliteDatabase database,
        string task,
        PriorityLevel todoPriority,
        DateTimeOffset lastWorkedAt,
        PriorityLevel workItemPriority = PriorityLevel.Normal)
    {
        await using var db = await database.Factory.CreateDbContextAsync();
        var workItem = new WorkItem
        {
            Name = $"Parent for {task}",
            Priority = workItemPriority,
            LastWorkedAt = lastWorkedAt
        };
        var todo = new TodoItem
        {
            Task = task,
            Priority = todoPriority,
            Energy = EnergyLevel.Low,
            Effort = EffortLevel.Short
        };
        workItem.Todos.Add(todo);
        db.WorkItems.Add(workItem);
        await db.SaveChangesAsync();
        return todo.Id;
    }
}

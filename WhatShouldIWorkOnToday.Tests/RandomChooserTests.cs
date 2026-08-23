using Microsoft.EntityFrameworkCore;
using WhatShouldIWorkOnToday.Models;
using WhatShouldIWorkOnToday.Services;
using Xunit;

namespace WhatShouldIWorkOnToday.Tests;

public sealed class RandomChooserTests
{
    [Theory]
    [InlineData(PriorityLevel.Low, PriorityLevel.Low, 1)]
    [InlineData(PriorityLevel.Low, PriorityLevel.Normal, 2)]
    [InlineData(PriorityLevel.Low, PriorityLevel.High, 4)]
    [InlineData(PriorityLevel.Normal, PriorityLevel.Normal, 4)]
    [InlineData(PriorityLevel.Normal, PriorityLevel.High, 8)]
    [InlineData(PriorityLevel.High, PriorityLevel.High, 16)]
    public void RandomPriorityWeight_CombinesParentAndTodoMultipliers(
        PriorityLevel workItemPriority,
        PriorityLevel todoPriority,
        int expectedWeight)
    {
        Assert.Equal(
            expectedWeight,
            WorkChooser.GetRandomPriorityWeight(
                Candidate(
                    "Candidate",
                    workItemPriority,
                    todoPriority)));
    }

    [Fact]
    public void EveryPriorityCombination_HasPositiveWeight()
    {
        foreach (var workItemPriority in Enum.GetValues<PriorityLevel>())
        {
            foreach (var todoPriority in Enum.GetValues<PriorityLevel>())
            {
                Assert.True(
                    WorkChooser.GetRandomPriorityWeight(
                        Candidate(
                            "Candidate",
                            workItemPriority,
                            todoPriority)) > 0);
            }
        }
    }

    [Fact]
    public void WeightedSelection_HonorsCumulativeBoundaries()
    {
        var lowLow = Candidate(
            "Low/Low",
            PriorityLevel.Low,
            PriorityLevel.Low);
        var lowNormal = Candidate(
            "Low/Normal",
            PriorityLevel.Low,
            PriorityLevel.Normal);
        var lowHigh = Candidate(
            "Low/High",
            PriorityLevel.Low,
            PriorityLevel.High);
        TodoItem[] candidates =
        [
            lowLow,
            lowNormal,
            lowHigh
        ];

        Assert.Same(lowLow, WorkChooser.SelectWeightedCandidate(candidates, 0));
        Assert.Same(lowNormal, WorkChooser.SelectWeightedCandidate(candidates, 1));
        Assert.Same(lowNormal, WorkChooser.SelectWeightedCandidate(candidates, 2));
        Assert.Same(lowHigh, WorkChooser.SelectWeightedCandidate(candidates, 3));
        Assert.Same(lowHigh, WorkChooser.SelectWeightedCandidate(candidates, 6));
    }

    [Fact]
    public void LowLowRemainsSelectable_AndHighHighGetsMoreSelectionSpace()
    {
        var lowLow = Candidate(
            "Low/Low",
            PriorityLevel.Low,
            PriorityLevel.Low);
        var normalNormal = Candidate(
            "Normal/Normal",
            PriorityLevel.Normal,
            PriorityLevel.Normal);
        var highHigh = Candidate(
            "High/High",
            PriorityLevel.High,
            PriorityLevel.High);

        Assert.Same(
            lowLow,
            WorkChooser.SelectWeightedCandidate(
                [lowLow, normalNormal, highHigh],
                0));
        Assert.True(
            WorkChooser.GetRandomPriorityWeight(highHigh) >
            WorkChooser.GetRandomPriorityWeight(normalNormal));

        TodoItem[] candidates = [normalNormal, highHigh];
        Assert.Same(
            normalNormal,
            WorkChooser.SelectWeightedCandidate(candidates, 3));
        Assert.Same(
            highHigh,
            WorkChooser.SelectWeightedCandidate(candidates, 4));
        Assert.Same(
            highHigh,
            WorkChooser.SelectWeightedCandidate(candidates, 19));
    }

    [Fact]
    public void UniformSelection_UsesOnlyCandidateIndex_NotPriorityWeight()
    {
        var highHigh = Candidate(
            "High/High",
            PriorityLevel.High,
            PriorityLevel.High);
        var lowLow = Candidate(
            "Low/Low",
            PriorityLevel.Low,
            PriorityLevel.Low);
        TodoItem[] candidates = [highHigh, lowLow];

        Assert.Same(
            highHigh,
            WorkChooser.SelectUniformCandidate(candidates, 0));
        Assert.Same(
            lowLow,
            WorkChooser.SelectUniformCandidate(candidates, 1));
    }

    [Fact]
    public async Task DefaultAndExplicitUniformModes_PreserveRandomEligibility()
    {
        await using var database = await TemporarySqliteDatabase.CreateAsync();
        await SeedSingleTodoAsync(
            database,
            todoCompleted: false,
            parentCompleted: false,
            parentArchived: false);
        var chooser = new WorkChooser(database.Factory);

        var defaultPick = await chooser.ChooseRandomAsync();
        var uniformPick = await chooser.ChooseRandomAsync(
            favorPriority: false);
        var weightedPick = await chooser.ChooseRandomAsync(
            favorPriority: true);

        Assert.Equal("Eligible", defaultPick?.Task);
        Assert.Equal("Eligible", uniformPick?.Task);
        Assert.Equal("Eligible", weightedPick?.Task);
        Assert.Equal(EnergyLevel.High, weightedPick?.Energy);
        Assert.Equal(EffortLevel.Long, weightedPick?.Effort);
    }

    [Fact]
    public async Task CompletedTodos_AreExcludedFromBothRandomModes()
    {
        await using var database = await TemporarySqliteDatabase.CreateAsync();
        await SeedSingleTodoAsync(
            database,
            todoCompleted: true,
            parentCompleted: false,
            parentArchived: false);
        var chooser = new WorkChooser(database.Factory);

        Assert.Null(await chooser.ChooseRandomAsync());
        Assert.Null(await chooser.ChooseRandomAsync(favorPriority: true));
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async Task InactiveWorkItems_AreExcludedFromBothRandomModes(
        bool completed,
        bool archived)
    {
        await using var database = await TemporarySqliteDatabase.CreateAsync();
        await SeedSingleTodoAsync(
            database,
            todoCompleted: false,
            parentCompleted: completed,
            parentArchived: archived);
        var chooser = new WorkChooser(database.Factory);

        Assert.Null(await chooser.ChooseRandomAsync());
        Assert.Null(await chooser.ChooseRandomAsync(favorPriority: true));
    }

    private static TodoItem Candidate(
        string task,
        PriorityLevel workItemPriority,
        PriorityLevel todoPriority)
    {
        return new TodoItem
        {
            Task = task,
            Priority = todoPriority,
            WorkItem = new WorkItem
            {
                Priority = workItemPriority
            }
        };
    }

    private static async Task SeedSingleTodoAsync(
        TemporarySqliteDatabase database,
        bool todoCompleted,
        bool parentCompleted,
        bool parentArchived)
    {
        await using var db = await database.Factory.CreateDbContextAsync();

        var active = new WorkItem
        {
            Name = "Active",
            Priority = PriorityLevel.Low,
            CompletedAt = parentCompleted
                ? DateTimeOffset.UtcNow
                : null,
            ArchivedAt = parentArchived
                ? DateTimeOffset.UtcNow
                : null
        };
        active.Todos.Add(
            new TodoItem
            {
                Task = "Eligible",
                Energy = EnergyLevel.High,
                Effort = EffortLevel.Long,
                Priority = PriorityLevel.Low,
                CompletedAt = todoCompleted
                    ? DateTimeOffset.UtcNow
                    : null
            });

        db.WorkItems.Add(active);
        await db.SaveChangesAsync();
    }
}

using Microsoft.EntityFrameworkCore;
using WhatShouldIWorkOnToday.Data;
using WhatShouldIWorkOnToday.Models;

namespace WhatShouldIWorkOnToday.Services;

public sealed class WorkChooser(
    IDbContextFactory<AppDbContext> dbContextFactory)
{
    public async Task<List<TodoItem>> ChooseAsync(
        EnergyLevel energy,
        EffortLevel effort,
        int count = 3,
        CancellationToken cancellationToken = default)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var candidates = await db.TodoItems
            .AsNoTracking()
            .Include(x => x.WorkItem)
            .Where(x =>
                x.CompletedAt == null &&
                x.WorkItem.CompletedAt == null &&
                x.WorkItem.ArchivedAt == null &&
                x.Energy <= energy &&
                x.Effort <= effort)
            .ToListAsync(cancellationToken);

        return candidates
            .OrderByDescending(GetNeglectScore)
            .Take(count)
            .ToList();
    }

    public async Task<TodoItem?> ChooseRandomAsync(
        bool favorPriority = false,
        CancellationToken cancellationToken = default)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var candidates = await db.TodoItems
            .AsNoTracking()
            .Include(x => x.WorkItem)
            .Where(x =>
                x.CompletedAt == null &&
                x.WorkItem.CompletedAt == null &&
                x.WorkItem.ArchivedAt == null)
            .ToListAsync(cancellationToken);

        if (candidates.Count == 0)
        {
            return null;
        }

        if (!favorPriority)
        {
            return SelectUniformCandidate(
                candidates,
                Random.Shared.Next(candidates.Count));
        }

        var totalWeight = candidates.Sum(
            GetRandomPriorityWeight);
        var draw = Random.Shared.Next(totalWeight);

        return SelectWeightedCandidate(candidates, draw);
    }

    internal static TodoItem SelectUniformCandidate(
        IReadOnlyList<TodoItem> candidates,
        int index)
    {
        return candidates[index];
    }

    internal static int GetRandomPriorityWeight(
        TodoItem todo)
    {
        return GetRandomPriorityMultiplier(
                   todo.WorkItem.Priority) *
               GetRandomPriorityMultiplier(todo.Priority);
    }

    internal static TodoItem SelectWeightedCandidate(
        IReadOnlyList<TodoItem> candidates,
        int draw)
    {
        if (draw < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(draw));
        }

        var cumulativeWeight = 0;

        foreach (var candidate in candidates)
        {
            cumulativeWeight += GetRandomPriorityWeight(
                candidate);

            if (draw < cumulativeWeight)
            {
                return candidate;
            }
        }

        throw new ArgumentOutOfRangeException(
            nameof(draw),
            "Draw must be within the total candidate weight.");
    }

    private static int GetRandomPriorityMultiplier(
        PriorityLevel priority)
    {
        return priority switch
        {
            PriorityLevel.Low => 1,
            PriorityLevel.High => 4,
            _ => 2
        };
    }

    private static double GetNeglectScore(
        TodoItem todo)
    {
        return GetAgeScore(todo) +
               Random.Shared.NextDouble() * 30 +
               GetPriorityScore(todo);
    }

    private static double GetDailyScore(
        TodoItem todo,
        DateOnly date)
    {
        return GetAgeScore(todo) +
               GetDailyRandomScore(todo, date) * 30 +
               GetPriorityScore(todo);
    }

    internal static int GetPriorityScore(TodoItem todo)
    {
        return GetPriorityBias(todo.WorkItem.Priority) +
               GetPriorityBias(todo.Priority);
    }

    private static int GetPriorityBias(PriorityLevel priority)
    {
        return priority switch
        {
            PriorityLevel.Low => -45,
            PriorityLevel.High => 45,
            _ => 0
        };
    }

    private static double GetAgeScore(
        TodoItem todo)
    {
        return todo.WorkItem.LastWorkedAt is null
            ? 365
            : Math.Min(
                (DateTimeOffset.UtcNow -
                    todo.WorkItem.LastWorkedAt.Value)
                .TotalDays,
                365);
    }

    private static double GetDailyRandomScore(
        TodoItem todo,
        DateOnly date)
    {
        unchecked
        {
            var value = (uint)date.DayNumber;

            value ^= (uint)todo.Id * 0x9E3779B9u;
            value ^= (uint)todo.WorkItemId * 0x85EBCA6Bu;
            value ^= value >> 16;
            value *= 0x7FEB352Du;
            value ^= value >> 15;
            value *= 0x846CA68Bu;
            value ^= value >> 16;

            return value / (double)uint.MaxValue;
        }
    }

    public async Task<TodoItem?> ChooseDailyAsync(
        DateOnly date,
        EnergyLevel energy = EnergyLevel.Medium,
        EffortLevel effort = EffortLevel.Medium,
        CancellationToken cancellationToken = default)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var existingPick = await db.DailyPicks
            .AsNoTracking()
            .Include(x => x.TodoItem)
            .ThenInclude(x => x.WorkItem)
            .SingleOrDefaultAsync(x => x.Date == date, cancellationToken);

        if (existingPick is not null)
        {
            return existingPick.TodoItem;
        }

        var candidates = await db.TodoItems
            .AsNoTracking()
            .Include(x => x.WorkItem)
            .Where(x =>
                x.CompletedAt == null &&
                x.WorkItem.CompletedAt == null &&
                x.WorkItem.ArchivedAt == null &&
                x.Energy <= energy &&
                x.Effort <= effort)
            .ToListAsync(cancellationToken);

        var todo = candidates
            .OrderByDescending(todo => GetDailyScore(todo, date))
            .FirstOrDefault();

        if (todo is null)
        {
            return null;
        }

        db.DailyPicks.Add(new DailyPick
        {
            Date = date,
            TodoItemId = todo.Id
        });

        await db.SaveChangesAsync(cancellationToken);

        return todo;
    }
}

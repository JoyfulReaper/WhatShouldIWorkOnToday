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

        return candidates.Count == 0
            ? null
            : candidates[Random.Shared.Next(candidates.Count)];
    }

    private static double GetNeglectScore(
        TodoItem todo)
    {
        return GetAgeScore(todo) +
               Random.Shared.NextDouble() * 30;
    }

    private static double GetDailyScore(
        TodoItem todo,
        DateOnly date)
    {
        return GetAgeScore(todo) +
               GetDailyRandomScore(todo, date) * 30;
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
            .OrderByDescending(
                todo => GetDailyScore(todo, date))
            .FirstOrDefault();
    }
}
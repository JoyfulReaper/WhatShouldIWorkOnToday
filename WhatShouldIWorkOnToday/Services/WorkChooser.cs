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
        var ageScore =
            todo.WorkItem.LastWorkedAt is null
                ? 365
                : Math.Min((DateTimeOffset.UtcNow -
                        todo.WorkItem.LastWorkedAt.Value)
                    .TotalDays, 365);

        var randomScore = Random.Shared.NextDouble() * 30;
        return ageScore + randomScore;
    }
}
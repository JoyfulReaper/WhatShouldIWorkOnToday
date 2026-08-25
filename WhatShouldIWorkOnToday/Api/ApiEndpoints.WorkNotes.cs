using Microsoft.EntityFrameworkCore;
using WhatShouldIWorkOnToday.Data;
using WhatShouldIWorkOnToday.Models;

namespace WhatShouldIWorkOnToday.Api;

public static partial class ApiEndpoints
{
    private static async Task<IResult> GetWorkNotesAsync(
        int? count,
        IDbContextFactory<AppDbContext> dbContextFactory,
        CancellationToken cancellationToken)
    {
        var take = Math.Clamp(count ?? 50, 1, 200);

        await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var entries = await db.WorkHistoryEntries
            .AsNoTracking()
            .Include(x => x.WorkItem)
            .ToListAsync(cancellationToken);

        var notes = entries
            .OrderByDescending(x => x.WorkedAt)
            .Take(take)
            .Select(x => new WorkNoteDto(
                x.Id,
                x.WorkItemId,
                x.WorkItem.Name,
                x.TodoItemId,
                x.TaskSnapshot,
                x.Note,
                x.WorkedAt))
            .ToList();

        return Results.Ok(notes);
    }

    private static async Task<IResult> GetWorkItemWorkNotesAsync(
        int id,
        IDbContextFactory<AppDbContext> dbContextFactory,
        CancellationToken cancellationToken)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var workItemExists = await db.WorkItems
            .AsNoTracking()
            .AnyAsync(
                x => x.Id == id,
                cancellationToken);

        if (!workItemExists)
        {
            return Results.NotFound();
        }

        var entries = await db.WorkHistoryEntries
            .AsNoTracking()
            .Include(x => x.WorkItem)
            .Where(x => x.WorkItemId == id)
            .ToListAsync(cancellationToken);

        var notes = entries
            .OrderByDescending(x => x.WorkedAt)
            .Select(x => new WorkNoteDto(
                x.Id,
                x.WorkItemId,
                x.WorkItem.Name,
                x.TodoItemId,
                x.TaskSnapshot,
                x.Note,
                x.WorkedAt))
            .ToList();

        return Results.Ok(notes);
    }

    private static async Task<IResult> CreateWorkNoteAsync(
        int id,
        CreateWorkNoteRequest request,
        IDbContextFactory<AppDbContext> dbContextFactory,
        CancellationToken cancellationToken)
    {
        await using var db =
            await dbContextFactory.CreateDbContextAsync(
                cancellationToken);

        var workItem = await db.WorkItems
            .SingleOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        if (workItem is null)
        {
            return Results.NotFound();
        }

        TodoItem? todo = null;

        if (request.TodoItemId is not null)
        {
            todo = await db.TodoItems
                .SingleOrDefaultAsync(
                    x =>
                        x.Id == request.TodoItemId &&
                        x.WorkItemId == id,
                    cancellationToken);

            if (todo is null)
            {
                return Results.ValidationProblem(
                    new Dictionary<string, string[]>
                    {
                        ["todoItemId"] =
                        [
                            "Todo does not belong to this work item."
                        ]
                    });
            }
        }

        var workedAt = DateTimeOffset.UtcNow;

        workItem.LastWorkedAt = workedAt;

        var entry = new WorkHistoryEntry
        {
            WorkItemId = workItem.Id,
            TodoItemId = todo?.Id,
            TaskSnapshot = todo?.Task,
            Note = NormalizeNote(request.Note),
            WorkedAt = workedAt
        };

        db.WorkHistoryEntries.Add(entry);

        await db.SaveChangesAsync(cancellationToken);

        return Results.Created(
            $"/api/work-notes/{entry.Id}",
            new WorkNoteDto(
                entry.Id,
                workItem.Id,
                workItem.Name,
                entry.TodoItemId,
                entry.TaskSnapshot,
                entry.Note,
                entry.WorkedAt));
    }

    private static async Task<IResult> UpdateWorkNoteAsync(
        int id,
        UpdateWorkNoteRequest request,
        IDbContextFactory<AppDbContext> dbContextFactory,
        CancellationToken cancellationToken)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var entry = await db.WorkHistoryEntries
            .Include(x => x.WorkItem)
            .SingleOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        if (entry is null)
        {
            return Results.NotFound();
        }

        entry.Note = NormalizeNote(request.Note);

        await db.SaveChangesAsync(cancellationToken);

        return Results.Ok(
            new WorkNoteDto(
                entry.Id,
                entry.WorkItemId,
                entry.WorkItem.Name,
                entry.TodoItemId,
                entry.TaskSnapshot,
                entry.Note,
                entry.WorkedAt));
    }

    private static string? NormalizeNote(string? note)
    {
        if (string.IsNullOrWhiteSpace(note))
        {
            return null;
        }

        return note.Trim();
    }
}
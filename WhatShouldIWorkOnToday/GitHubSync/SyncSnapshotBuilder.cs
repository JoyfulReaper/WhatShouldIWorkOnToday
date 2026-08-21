using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using WhatShouldIWorkOnToday.Data;

namespace WhatShouldIWorkOnToday.GitHubSync;

public sealed class SyncSnapshotBuilder(
    IDbContextFactory<AppDbContext> dbContextFactory)
{
    public async Task<SyncSnapshot> BuildAsync(
        CancellationToken cancellationToken = default)
    {
        await using var db =
            await dbContextFactory.CreateDbContextAsync(
                cancellationToken);

        var workItems = await db.WorkItems
            .AsNoTracking()
            .Include(x => x.Todos)
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);

        var snapshotItems = workItems
            .Select(workItem =>
                new SyncWorkItemSnapshot(
                    workItem.Id,
                    workItem.Name,
                    workItem.Kind.ToString(),
                    workItem.Priority.ToString(),
                    workItem.Description,
                    workItem.Url,
                    workItem.CreatedAt,
                    workItem.LastWorkedAt,
                    workItem.CompletedAt,
                    workItem.ArchivedAt,
                    workItem.Todos
                        .OrderBy(todo => todo.Id)
                        .Select(todo =>
                            new SyncTodoSnapshot(
                                todo.Id,
                                todo.Task,
                                todo.Energy.ToString(),
                                todo.Effort.ToString(),
                                todo.Priority.ToString(),
                                todo.CreatedAt,
                                todo.CompletedAt))
                        .ToList()))
            .ToList();

        var stateBytes = JsonSerializer.SerializeToUtf8Bytes(
            snapshotItems,
            GitHubSyncJson.Compact);

        var stateHash = Convert.ToHexString(
                SHA256.HashData(stateBytes))
            .ToLowerInvariant();

        return new SyncSnapshot(
            1,
            DateTimeOffset.UtcNow,
            stateHash,
            snapshotItems);
    }
}

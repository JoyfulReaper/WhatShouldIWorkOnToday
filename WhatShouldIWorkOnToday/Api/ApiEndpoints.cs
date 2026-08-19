using Microsoft.EntityFrameworkCore;
using WhatShouldIWorkOnToday.Data;

namespace WhatShouldIWorkOnToday.Api;

public static class ApiEndpoints
{
    public static IEndpointRouteBuilder MapApiEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var api = endpoints.MapGroup("/api");

        api.MapGet(
            "/work-items",
            GetWorkItemsAsync);

        api.MapGet(
            "/work-items/{id:int}",
            GetWorkItemAsync);

        api.MapGet(
            "/todos",
            GetTodosAsync);

        api.MapGet(
            "/todos/{id:int}",
            GetTodoAsync);

        return endpoints;
    }

    private static async Task<IResult> GetWorkItemsAsync(
        IDbContextFactory<AppDbContext> dbContextFactory,
        CancellationToken cancellationToken)
    {
        await using var db =
            await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var workItems = await db.WorkItems
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new WorkItemDto(
                x.Id,
                x.Name,
                x.Description,
                x.Url,
                x.Kind.ToString(),
                x.CreatedAt,
                x.LastWorkedAt,
                x.CompletedAt,
                x.ArchivedAt,
                x.Todos.Count,
                x.Todos.Count(todo =>
                    todo.CompletedAt == null)))
            .ToListAsync(cancellationToken);

        return Results.Ok(workItems);
    }

    private static async Task<IResult> GetWorkItemAsync(
        int id,
        IDbContextFactory<AppDbContext> dbContextFactory,
        CancellationToken cancellationToken)
    {
        await using var db =
            await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var workItem = await db.WorkItems
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new WorkItemDto(
                x.Id,
                x.Name,
                x.Description,
                x.Url,
                x.Kind.ToString(),
                x.CreatedAt,
                x.LastWorkedAt,
                x.CompletedAt,
                x.ArchivedAt,
                x.Todos.Count,
                x.Todos.Count(todo =>
                    todo.CompletedAt == null)))
            .SingleOrDefaultAsync(cancellationToken);

        return workItem is null
            ? Results.NotFound()
            : Results.Ok(workItem);
    }

    private static async Task<IResult> GetTodosAsync(
        int? workItemId,
        bool? includeCompleted,
        IDbContextFactory<AppDbContext> dbContextFactory,
        CancellationToken cancellationToken)
    {
        await using var db =
            await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var query = db.TodoItems
            .AsNoTracking()
            .AsQueryable();

        if (workItemId.HasValue)
        {
            query = query.Where(
                x => x.WorkItemId == workItemId.Value);
        }

        if (includeCompleted is not true)
        {
            query = query.Where(
                x => x.CompletedAt == null);
        }

        var todos = await query
            .OrderBy(x => x.WorkItem.Name)
            .ThenBy(x => x.Task)
            .Select(x => new TodoItemDto(
                x.Id,
                x.WorkItemId,
                x.WorkItem.Name,
                x.Task,
                x.Energy.ToString(),
                x.Effort.ToString(),
                x.CreatedAt,
                x.CompletedAt))
            .ToListAsync(cancellationToken);

        return Results.Ok(todos);
    }

    private static async Task<IResult> GetTodoAsync(
        int id,
        IDbContextFactory<AppDbContext> dbContextFactory,
        CancellationToken cancellationToken)
    {
        await using var db =
            await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var todo = await db.TodoItems
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new TodoItemDto(
                x.Id,
                x.WorkItemId,
                x.WorkItem.Name,
                x.Task,
                x.Energy.ToString(),
                x.Effort.ToString(),
                x.CreatedAt,
                x.CompletedAt))
            .SingleOrDefaultAsync(cancellationToken);

        return todo is null
            ? Results.NotFound()
            : Results.Ok(todo);
    }
}
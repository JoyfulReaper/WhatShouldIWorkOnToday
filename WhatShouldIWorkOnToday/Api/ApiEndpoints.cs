using Microsoft.EntityFrameworkCore;
using WhatShouldIWorkOnToday.Auth;
using WhatShouldIWorkOnToday.Data;
using WhatShouldIWorkOnToday.Models;

namespace WhatShouldIWorkOnToday.Api;

public static class ApiEndpoints
{
    public static IEndpointRouteBuilder MapApiEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var api = endpoints
            .MapGroup("/api")
            .RequireAuthorization(ApiKeyDefaults.AuthorizationPolicy);

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

        api.MapPost(
            "/work-items/{workItemId:int}/todos",
            CreateTodoAsync);

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

    private static async Task<IResult> CreateTodoAsync(
        int workItemId,
        CreateTodoRequest request,
        IDbContextFactory<AppDbContext> dbContextFactory,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Task))
        {
            return Results.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    ["task"] =
                    [
                        "Task is required."
                    ]
                });
        }

        var task = request.Task.Trim();
        if (task.Length > 500)
        {
            return Results.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    ["task"] =
                    [
                        "Task cannot exceed 500 characters."
                    ]
                });
        }

        if (!TryParseEnergy(request.Energy, out var energy))
        {
            return Results.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    ["energy"] =
                    [
                        "Energy must be Low, Medium, or High."
                    ]
                });
        }

        if (!TryParseEffort(request.Effort, out var effort))
        {
            return Results.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    ["effort"] =
                    [
                        "Effort must be Short, Medium, or Long."
                    ]
                });
        }

        await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var workItem = await db.WorkItems
            .SingleOrDefaultAsync(x => x.Id == workItemId, cancellationToken);

        if (workItem is null)
        {
            return Results.NotFound();
        }

        if (workItem.CompletedAt is not null ||
            workItem.ArchivedAt is not null)
        {
            return Results.Conflict(
                new
                {
                    error = "Cannot add a todo to an inactive work item."
                });
        }

        var todo = new TodoItem
        {
            WorkItemId = workItem.Id,
            Task = task,
            Energy = energy,
            Effort = effort
        };

        db.TodoItems.Add(todo);

        await db.SaveChangesAsync(cancellationToken);

        var response = new TodoItemDto(
            todo.Id,
            todo.WorkItemId,
            workItem.Name,
            todo.Task,
            todo.Energy.ToString(),
            todo.Effort.ToString(),
            todo.CreatedAt,
            todo.CompletedAt);

        return Results.Created($"/api/todos/{todo.Id}", response);
    }

    private static bool TryParseEnergy(
        string? value,
        out EnergyLevel energy)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            energy = EnergyLevel.Medium;
            return true;
        }

        return Enum.TryParse(value, ignoreCase: true, out energy) &&
               Enum.IsDefined(energy);
    }

    private static bool TryParseEffort(
        string? value,
        out EffortLevel effort)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            effort = EffortLevel.Medium;
            return true;
        }

        return Enum.TryParse(value, ignoreCase: true, out effort) &&
               Enum.IsDefined(effort);
    }
}
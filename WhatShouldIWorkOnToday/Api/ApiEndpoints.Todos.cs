using Microsoft.EntityFrameworkCore;
using WhatShouldIWorkOnToday.Data;
using WhatShouldIWorkOnToday.Models;
using WhatShouldIWorkOnToday.Services;

namespace WhatShouldIWorkOnToday.Api;

public static partial class ApiEndpoints
{
    private static async Task<IResult> GetTodosAsync(
        int? workItemId,
        bool? includeCompleted,
        IDbContextFactory<AppDbContext> dbContextFactory,
        CancellationToken cancellationToken)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var query = db.TodoItems
            .AsNoTracking()
            .AsQueryable();

        if (workItemId.HasValue)
        {
            query = query.Where(x => x.WorkItemId == workItemId.Value);
        }

        if (includeCompleted is not true)
        {
            query = query.Where(x => x.CompletedAt == null);
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
        PlanningMutationService mutationService,
        CancellationToken cancellationToken)
    {
        await using var db =
            await dbContextFactory.CreateDbContextAsync(
                cancellationToken);

        var result = await mutationService.CreateTodoAsync(
            db,
            workItemId,
            new CreateTodoInput(
                request.Task,
                request.Energy,
                request.Effort),
            cancellationToken);

        if (!result.Succeeded)
        {
            return result.Failure switch
            {
                PlanningMutationFailure.Validation =>
                    Results.ValidationProblem(
                        result.ValidationErrors!),

                PlanningMutationFailure.NotFound =>
                    Results.NotFound(),

                _ => Results.Conflict(
                    new
                    {
                        error = result.Error
                    })
            };
        }

        await db.SaveChangesAsync(cancellationToken);

        var created = result.Value!;
        var todo = created.Todo;

        var response = new TodoItemDto(
            todo.Id,
            todo.WorkItemId,
            created.WorkItem.Name,
            todo.Task,
            todo.Energy.ToString(),
            todo.Effort.ToString(),
            todo.CreatedAt,
            todo.CompletedAt);

        return Results.Created($"/api/todos/{todo.Id}", response);
    }

    private static async Task<IResult> CreateTodosBulkAsync(
    BulkCreateTodosRequest request,
    IDbContextFactory<AppDbContext> dbContextFactory,
    CancellationToken cancellationToken)
    {
        if (request.Items is null ||
            request.Items.Count == 0)
        {
            return Results.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    ["items"] =
                    [
                        "At least one todo is required."
                    ]
                });
        }

        if (request.Items.Count > 100)
        {
            return Results.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    ["items"] =
                    [
                        "A maximum of 100 todos can be created at once."
                    ]
                });
        }

        var errors =
            new Dictionary<string, string[]>();

        var parsedItems =
            new List<(
                int WorkItemId,
                string Task,
                EnergyLevel Energy,
                EffortLevel Effort)>();

        for (var i = 0;
             i < request.Items.Count;
             i++)
        {
            var item = request.Items[i];
            var prefix = $"items[{i}].";

            if (item.WorkItemId <= 0)
            {
                errors[$"{prefix}workItemId"] =
                [
                    "WorkItemId must be greater than zero."
                ];
            }

            var task = item.Task?.Trim()
                       ?? string.Empty;

            if (task.Length == 0)
            {
                errors[$"{prefix}task"] =
                [
                    "Task is required."
                ];
            }
            else if (task.Length > 500)
            {
                errors[$"{prefix}task"] =
                [
                    "Task cannot exceed 500 characters."
                ];
            }

            if (!TryParseEnergy(
                    item.Energy,
                    out var energy))
            {
                errors[$"{prefix}energy"] =
                [
                    "Energy must be Low, Medium, or High."
                ];
            }

            if (!TryParseEffort(
                    item.Effort,
                    out var effort))
            {
                errors[$"{prefix}effort"] =
                [
                    "Effort must be Short, Medium, or Long."
                ];
            }

            parsedItems.Add(
                (
                    item.WorkItemId,
                    task,
                    energy,
                    effort
                ));
        }

        if (errors.Count > 0)
        {
            return Results.ValidationProblem(errors);
        }

        await using var db =
            await dbContextFactory.CreateDbContextAsync(
                cancellationToken);

        var workItemIds = parsedItems
            .Select(x => x.WorkItemId)
            .Distinct()
            .ToArray();

        var workItems = await db.WorkItems
            .Where(x => workItemIds.Contains(x.Id))
            .ToDictionaryAsync(
                x => x.Id,
                cancellationToken);

        for (var i = 0;
             i < parsedItems.Count;
             i++)
        {
            var item = parsedItems[i];
            var key = $"items[{i}].workItemId";

            if (!workItems.TryGetValue(
                    item.WorkItemId,
                    out var workItem))
            {
                errors[key] =
                [
                    "Work item does not exist."
                ];

                continue;
            }

            if (workItem.CompletedAt is not null ||
                workItem.ArchivedAt is not null)
            {
                errors[key] =
                [
                    "Cannot add a todo to an inactive work item."
                ];
            }
        }

        if (errors.Count > 0)
        {
            return Results.ValidationProblem(errors);
        }

        var todos = parsedItems
            .Select(item => new TodoItem
            {
                WorkItemId = item.WorkItemId,
                Task = item.Task,
                Energy = item.Energy,
                Effort = item.Effort
            })
            .ToList();

        db.TodoItems.AddRange(todos);

        await db.SaveChangesAsync(cancellationToken);

        var response = todos
            .Select(todo => new TodoItemDto(
                todo.Id,
                todo.WorkItemId,
                workItems[todo.WorkItemId].Name,
                todo.Task,
                todo.Energy.ToString(),
                todo.Effort.ToString(),
                todo.CreatedAt,
                todo.CompletedAt))
            .ToList();

        return Results.Created(
            "/api/todos",
            response);
    }
}

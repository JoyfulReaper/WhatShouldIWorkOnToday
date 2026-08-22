using Microsoft.EntityFrameworkCore;
using WhatShouldIWorkOnToday.Data;
using WhatShouldIWorkOnToday.Models;
using WhatShouldIWorkOnToday.Services;

namespace WhatShouldIWorkOnToday.Api;

public static partial class ApiEndpoints
{
    private static async Task<IResult> GetWorkItemsAsync(
        IDbContextFactory<AppDbContext> dbContextFactory,
        CancellationToken cancellationToken)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var workItems = await db.WorkItems
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new WorkItemDto(
                x.Id,
                x.Name,
                x.Description,
                x.Url,
                x.Kind.ToString(),
                x.Priority.ToString(),
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
        await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var workItem = await db.WorkItems
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new WorkItemDto(
                x.Id,
                x.Name,
                x.Description,
                x.Url,
                x.Kind.ToString(),
                x.Priority.ToString(),
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

    private static async Task<IResult> CreateWorkItemAsync(
        CreateWorkItemRequest request,
        IDbContextFactory<AppDbContext> dbContextFactory,
        PlanningMutationService mutationService,
        CancellationToken cancellationToken)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var result = mutationService.CreateWorkItem(
            db,
            new CreateWorkItemInput(
                request.Name,
                request.Kind,
                request.Description,
                request.Url,
                request.Priority));

        if (!result.Succeeded)
        {
            return Results.ValidationProblem(result.ValidationErrors!);
        }

        var workItem = result.Value!;
        await db.SaveChangesAsync(cancellationToken);

        var response = new WorkItemDto(
            workItem.Id,
            workItem.Name,
            workItem.Description,
            workItem.Url,
            workItem.Kind.ToString(),
            workItem.Priority.ToString(),
            workItem.CreatedAt,
            workItem.LastWorkedAt,
            workItem.CompletedAt,
            workItem.ArchivedAt,
            0,
            0);

        return Results.Created(
            $"/api/work-items/{workItem.Id}",
            response);
    }

    private static async Task<IResult> CreateWorkItemsBulkAsync(
        BulkCreateWorkItemsRequest request,
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
                        "At least one work item is required."
                    ]
                });
        }

        if (request.Items.Count > 50)
        {
            return Results.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    ["items"] =
                    [
                        "A maximum of 50 work items can be created at once."
                    ]
                });
        }

        var totalTodoCount = request.Items
            .Sum(x => x.Todos?.Count ?? 0);

        if (totalTodoCount > 100)
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

        var errors = new Dictionary<string, string[]>();

        var parsedItems =
            new List<(
                string Name,
                WorkItemKind Kind,
                string? Description,
                string? Url,
                PriorityLevel Priority,
                List<(
                    string Task,
                    EnergyLevel Energy,
                    EffortLevel Effort,
                    PriorityLevel Priority)> Todos)>();

        for (var i = 0;
             i < request.Items.Count;
             i++)
        {
            var item = request.Items[i];
            var prefix = $"items[{i}].";

            var parsedItem =
                ValidateAndNormalizeWorkItem(
                    item.Name,
                    item.Kind,
                    item.Description,
                    item.Url,
                    item.Priority,
                    prefix,
                    errors);

            var parsedTodos =
                new List<(
                    string Task,
                    EnergyLevel Energy,
                    EffortLevel Effort,
                    PriorityLevel Priority)>();

            var todos = item.Todos ?? [];

            for (var todoIndex = 0;
                 todoIndex < todos.Count;
                 todoIndex++)
            {
                var todo = todos[todoIndex];

                var todoPrefix =
                    $"{prefix}todos[{todoIndex}].";

                var parsedTodo = ValidateAndNormalizeTodo(
                    todo.Task,
                    todo.Energy,
                    todo.Effort,
                    todo.Priority,
                    todoPrefix,
                    errors);

                parsedTodos.Add(
                    (
                        parsedTodo.Task,
                        parsedTodo.Energy,
                        parsedTodo.Effort,
                        parsedTodo.Priority
                    ));
            }

            parsedItems.Add(
                (
                    parsedItem.Name,
                    parsedItem.Kind,
                    parsedItem.Description,
                    parsedItem.Url,
                    parsedItem.Priority,
                    parsedTodos
                ));
        }

        if (errors.Count > 0)
        {
            return Results.ValidationProblem(errors);
        }

        await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var workItems = parsedItems
            .Select(item =>
            {
                var workItem = new WorkItem
                {
                    Name = item.Name,
                    Kind = item.Kind,
                    Description = item.Description,
                    Url = item.Url,
                    Priority = item.Priority
                };

                foreach (var todo in item.Todos)
                {
                    workItem.Todos.Add(
                        new TodoItem
                        {
                            Task = todo.Task,
                            Energy = todo.Energy,
                            Effort = todo.Effort,
                            Priority = todo.Priority
                        });
                }

                return workItem;
            })
            .ToList();

        db.WorkItems.AddRange(workItems);
        await db.SaveChangesAsync(cancellationToken);

        var response = workItems
            .Select(workItem =>
                new BulkCreatedWorkItemDto(
                    new WorkItemDto(
                        workItem.Id,
                        workItem.Name,
                        workItem.Description,
                        workItem.Url,
                        workItem.Kind.ToString(),
                        workItem.Priority.ToString(),
                        workItem.CreatedAt,
                        workItem.LastWorkedAt,
                        workItem.CompletedAt,
                        workItem.ArchivedAt,
                        workItem.Todos.Count,
                        workItem.Todos.Count(todo =>
                            todo.CompletedAt == null)),
                    workItem.Todos
                        .Select(todo =>
                            new TodoItemDto(
                                todo.Id,
                                workItem.Id,
                                workItem.Name,
                                todo.Task,
                                todo.Energy.ToString(),
                                todo.Effort.ToString(),
                                todo.Priority.ToString(),
                                todo.CreatedAt,
                                todo.CompletedAt))
                        .ToList()))
            .ToList();

        return Results.Created(
            "/api/work-items",
            response);
    }

    private static async Task<IResult> RenameWorkItemAsync(
        int id,
        RenameWorkItemRequest request,
        IDbContextFactory<AppDbContext> dbContextFactory,
        PlanningMutationService mutationService,
        CancellationToken cancellationToken)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var result = await mutationService.RenameWorkItemAsync(
            db,
            id,
            request.Name,
            cancellationToken);

        if (!result.Succeeded)
        {
            return result.Failure switch
            {
                PlanningMutationFailure.Validation =>
                    Results.ValidationProblem(result.ValidationErrors!),

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

        var workItem = result.Value!;

        return Results.Ok(new RenameWorkItemResponse(workItem.Id, workItem.Name));
    }
}

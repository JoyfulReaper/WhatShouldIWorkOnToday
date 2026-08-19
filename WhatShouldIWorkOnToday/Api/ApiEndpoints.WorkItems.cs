using Microsoft.EntityFrameworkCore;
using WhatShouldIWorkOnToday.Data;
using WhatShouldIWorkOnToday.Models;

namespace WhatShouldIWorkOnToday.Api;

public static partial class ApiEndpoints
{
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

        var errors =
            new Dictionary<string, string[]>();

        var parsedItems =
            new List<(
                string Name,
                WorkItemKind Kind,
                string? Description,
                string? Url,
                List<(
                    string Task,
                    EnergyLevel Energy,
                    EffortLevel Effort)> Todos)>();

        for (var i = 0;
             i < request.Items.Count;
             i++)
        {
            var item = request.Items[i];
            var prefix = $"items[{i}].";

            var name = item.Name?.Trim()
                       ?? string.Empty;

            if (name.Length == 0)
            {
                errors[$"{prefix}name"] =
                [
                    "Name is required."
                ];
            }
            else if (name.Length > 200)
            {
                errors[$"{prefix}name"] =
                [
                    "Name cannot exceed 200 characters."
                ];
            }

            if (!TryParseWorkItemKind(
                    item.Kind,
                    out var kind))
            {
                errors[$"{prefix}kind"] =
                [
                    "Kind must be Project, Maintenance, Learning, Idea, or ExternalIssue."
                ];
            }

            var description =
                string.IsNullOrWhiteSpace(
                    item.Description)
                    ? null
                    : item.Description.Trim();

            if (description?.Length > 2000)
            {
                errors[$"{prefix}description"] =
                [
                    "Description cannot exceed 2000 characters."
                ];
            }

            var url =
                string.IsNullOrWhiteSpace(item.Url)
                    ? null
                    : item.Url.Trim();

            if (url?.Length > 2048)
            {
                errors[$"{prefix}url"] =
                [
                    "URL cannot exceed 2048 characters."
                ];
            }

            var parsedTodos =
                new List<(
                    string Task,
                    EnergyLevel Energy,
                    EffortLevel Effort)>();

            var todos = item.Todos ?? [];

            for (var todoIndex = 0;
                 todoIndex < todos.Count;
                 todoIndex++)
            {
                var todo = todos[todoIndex];

                var todoPrefix =
                    $"{prefix}todos[{todoIndex}].";

                var task = todo.Task?.Trim()
                           ?? string.Empty;

                if (task.Length == 0)
                {
                    errors[$"{todoPrefix}task"] =
                    [
                        "Task is required."
                    ];
                }
                else if (task.Length > 500)
                {
                    errors[$"{todoPrefix}task"] =
                    [
                        "Task cannot exceed 500 characters."
                    ];
                }

                if (!TryParseEnergy(
                        todo.Energy,
                        out var energy))
                {
                    errors[$"{todoPrefix}energy"] =
                    [
                        "Energy must be Low, Medium, or High."
                    ];
                }

                if (!TryParseEffort(
                        todo.Effort,
                        out var effort))
                {
                    errors[$"{todoPrefix}effort"] =
                    [
                        "Effort must be Short, Medium, or Long."
                    ];
                }

                parsedTodos.Add(
                    (
                        task,
                        energy,
                        effort
                    ));
            }

            parsedItems.Add(
                (
                    name,
                    kind,
                    description,
                    url,
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
                    Url = item.Url
                };

                foreach (var todo in item.Todos)
                {
                    workItem.Todos.Add(
                        new TodoItem
                        {
                            Task = todo.Task,
                            Energy = todo.Energy,
                            Effort = todo.Effort
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
                                todo.CreatedAt,
                                todo.CompletedAt))
                        .ToList()))
            .ToList();

        return Results.Created(
            "/api/work-items",
            response);
    }
}
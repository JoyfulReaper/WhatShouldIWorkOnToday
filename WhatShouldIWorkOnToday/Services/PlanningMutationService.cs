using Microsoft.EntityFrameworkCore;
using WhatShouldIWorkOnToday.Data;
using WhatShouldIWorkOnToday.Models;

namespace WhatShouldIWorkOnToday.Services;

public sealed class PlanningMutationService
{
    public PlanningMutationResult<WorkItem> CreateWorkItem(
        AppDbContext db,
        CreateWorkItemInput input)
    {
        var errors = new Dictionary<string, string[]>();

        var item = ValidateAndNormalizeWorkItem(
            input.Name,
            input.Kind,
            input.Description,
            input.Url,
            input.Priority,
            string.Empty,
            errors);

        if (errors.Count > 0)
        {
            return PlanningMutationResult<WorkItem>
                .ValidationFailure(errors);
        }

        var workItem = new WorkItem
        {
            Name = item.Name,
            Kind = item.Kind,
            Description = item.Description,
            Url = item.Url,
            Priority = item.Priority
        };

        db.WorkItems.Add(workItem);

        return PlanningMutationResult<WorkItem>
            .Success(workItem);
    }

    public async Task<PlanningMutationResult<CreatedTodo>>
        CreateTodoAsync(
            AppDbContext db,
            int workItemId,
            CreateTodoInput input,
            CancellationToken cancellationToken = default)
    {
        var errors = new Dictionary<string, string[]>();

        var item = ValidateAndNormalizeTodo(
            input.Task,
            input.Energy,
            input.Effort,
            input.Priority,
            string.Empty,
            errors);

        if (errors.Count > 0)
        {
            return PlanningMutationResult<CreatedTodo>
                .ValidationFailure(errors);
        }

        var workItem = await db.WorkItems
            .SingleOrDefaultAsync(
                x => x.Id == workItemId,
                cancellationToken);

        if (workItem is null)
        {
            return PlanningMutationResult<CreatedTodo>
                .NotFound("Work item does not exist.");
        }

        if (workItem.CompletedAt is not null ||
            workItem.ArchivedAt is not null)
        {
            return PlanningMutationResult<CreatedTodo>
                .Conflict(
                    "Cannot add a todo to an inactive work item.");
        }

        var todo = new TodoItem
        {
            WorkItemId = workItem.Id,
            Task = item.Task,
            Energy = item.Energy,
            Effort = item.Effort,
            Priority = item.Priority
        };

        db.TodoItems.Add(todo);

        return PlanningMutationResult<CreatedTodo>
            .Success(new CreatedTodo(todo, workItem));
    }

    public async Task<PlanningMutationResult<TodoItem>>
        CompleteTodoAsync(
            AppDbContext db,
            int todoId,
            CancellationToken cancellationToken = default)
    {
        var todo = await db.TodoItems
            .SingleOrDefaultAsync(
                x => x.Id == todoId,
                cancellationToken);

        if (todo is null)
        {
            return PlanningMutationResult<TodoItem>
                .NotFound("Todo does not exist.");
        }

        if (todo.CompletedAt is not null)
        {
            return PlanningMutationResult<TodoItem>
                .Success(todo, changed: false);
        }

        var workItem = await db.WorkItems
            .SingleOrDefaultAsync(
                x => x.Id == todo.WorkItemId,
                cancellationToken);

        if (workItem is null)
        {
            return PlanningMutationResult<TodoItem>
                .NotFound("Work item does not exist.");
        }

        CompleteTrackedTodo(db, todo, workItem);

        return PlanningMutationResult<TodoItem>
            .Success(todo);
    }

    public async Task<PlanningMutationResult<TodoItem>>
        ToggleTodoAsync(
            AppDbContext db,
            int todoId,
            CancellationToken cancellationToken = default)
    {
        var todo = await db.TodoItems
            .SingleOrDefaultAsync(
                x => x.Id == todoId,
                cancellationToken);

        if (todo is null)
        {
            return PlanningMutationResult<TodoItem>
                .NotFound("Todo does not exist.");
        }

        if (todo.CompletedAt is not null)
        {
            todo.CompletedAt = null;

            return PlanningMutationResult<TodoItem>
                .Success(todo);
        }

        var workItem = await db.WorkItems
            .SingleOrDefaultAsync(
                x => x.Id == todo.WorkItemId,
                cancellationToken);

        if (workItem is null)
        {
            return PlanningMutationResult<TodoItem>
                .NotFound("Work item does not exist.");
        }

        CompleteTrackedTodo(db, todo, workItem);

        return PlanningMutationResult<TodoItem>
            .Success(todo);
    }

    public static NormalizedWorkItem
        ValidateAndNormalizeWorkItem(
            string? nameValue,
            string? kindValue,
            string? descriptionValue,
            string? urlValue,
            string? priorityValue,
            string keyPrefix,
            Dictionary<string, string[]> errors)
    {
        var name = nameValue?.Trim()
                   ?? string.Empty;

        if (name.Length == 0)
        {
            errors[$"{keyPrefix}name"] =
            [
                "Name is required."
            ];
        }
        else if (name.Length > 200)
        {
            errors[$"{keyPrefix}name"] =
            [
                "Name cannot exceed 200 characters."
            ];
        }

        if (!TryParseWorkItemKind(
                kindValue,
                out var kind))
        {
            errors[$"{keyPrefix}kind"] =
            [
                "Kind must be Project, Maintenance, Learning, Idea, or ExternalIssue."
            ];
        }

        var description =
            string.IsNullOrWhiteSpace(descriptionValue)
                ? null
                : descriptionValue.Trim();

        if (description?.Length > 2000)
        {
            errors[$"{keyPrefix}description"] =
            [
                "Description cannot exceed 2000 characters."
            ];
        }

        var url = string.IsNullOrWhiteSpace(urlValue)
            ? null
            : urlValue.Trim();

        if (url?.Length > 2048)
        {
            errors[$"{keyPrefix}url"] =
            [
                "URL cannot exceed 2048 characters."
            ];
        }

        if (!TryParsePriority(priorityValue, out var priority))
        {
            errors[$"{keyPrefix}priority"] =
            [
                "Priority must be Low, Normal, or High."
            ];
        }

        return new NormalizedWorkItem(
            name,
            kind,
            description,
            url,
            priority);
    }

    public static NormalizedTodo ValidateAndNormalizeTodo(
        string? taskValue,
        string? energyValue,
        string? effortValue,
        string? priorityValue,
        string keyPrefix,
        Dictionary<string, string[]> errors)
    {
        var task = taskValue?.Trim()
                   ?? string.Empty;

        if (task.Length == 0)
        {
            errors[$"{keyPrefix}task"] =
            [
                "Task is required."
            ];
        }
        else if (task.Length > 500)
        {
            errors[$"{keyPrefix}task"] =
            [
                "Task cannot exceed 500 characters."
            ];
        }

        if (!TryParseEnergy(
                energyValue,
                out var energy))
        {
            errors[$"{keyPrefix}energy"] =
            [
                "Energy must be Low, Medium, or High."
            ];
        }

        if (!TryParseEffort(
                effortValue,
                out var effort))
        {
            errors[$"{keyPrefix}effort"] =
            [
                "Effort must be Short, Medium, or Long."
            ];
        }

        if (!TryParsePriority(priorityValue, out var priority))
        {
            errors[$"{keyPrefix}priority"] =
            [
                "Priority must be Low, Normal, or High."
            ];
        }

        return new NormalizedTodo(
            task,
            energy,
            effort,
            priority);
    }

    public static bool TryParseEnergy(
        string? value,
        out EnergyLevel energy)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            energy = EnergyLevel.Medium;
            return true;
        }

        return Enum.TryParse(
                   value,
                   ignoreCase: true,
                   out energy) &&
               Enum.IsDefined(energy);
    }

    public static bool TryParseEffort(
        string? value,
        out EffortLevel effort)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            effort = EffortLevel.Medium;
            return true;
        }

        return Enum.TryParse(
                   value,
                   ignoreCase: true,
                   out effort) &&
               Enum.IsDefined(effort);
    }

    public static bool TryParseWorkItemKind(
        string? value,
        out WorkItemKind kind)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            kind = WorkItemKind.Project;
            return true;
        }

        return Enum.TryParse(
                   value,
                   ignoreCase: true,
                   out kind) &&
               Enum.IsDefined(kind);
    }

    public static bool TryParsePriority(
        string? value,
        out PriorityLevel priority)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            priority = PriorityLevel.Normal;
            return true;
        }

        return Enum.TryParse(
                   value,
                   ignoreCase: true,
                   out priority) &&
               Enum.IsDefined(priority);
    }

    private static void CompleteTrackedTodo(
        AppDbContext db,
        TodoItem todo,
        WorkItem workItem)
    {
        var workedAt = DateTimeOffset.UtcNow;

        todo.CompletedAt = workedAt;
        workItem.LastWorkedAt = workedAt;

        db.WorkHistoryEntries.Add(
            new WorkHistoryEntry
            {
                WorkItemId = workItem.Id,
                TodoItemId = todo.Id,
                TaskSnapshot = todo.Task,
                WorkedAt = workedAt
            });
    }
}

public sealed record CreateWorkItemInput(
    string? Name,
    string? Kind = null,
    string? Description = null,
    string? Url = null,
    string? Priority = null);

public sealed record CreateTodoInput(
    string? Task,
    string? Energy = null,
    string? Effort = null,
    string? Priority = null);

public sealed record NormalizedWorkItem(
    string Name,
    WorkItemKind Kind,
    string? Description,
    string? Url,
    PriorityLevel Priority);

public sealed record NormalizedTodo(
    string Task,
    EnergyLevel Energy,
    EffortLevel Effort,
    PriorityLevel Priority);

public sealed record CreatedTodo(
    TodoItem Todo,
    WorkItem WorkItem);

public enum PlanningMutationFailure
{
    Validation,
    NotFound,
    Conflict
}

public sealed class PlanningMutationResult<T>
    where T : class
{
    private PlanningMutationResult(
        T? value,
        bool changed,
        PlanningMutationFailure? failure,
        IReadOnlyDictionary<string, string[]>? validationErrors,
        string? error)
    {
        Value = value;
        Changed = changed;
        Failure = failure;
        ValidationErrors = validationErrors;
        Error = error;
    }

    public T? Value { get; }

    public bool Changed { get; }

    public PlanningMutationFailure? Failure { get; }

    public IReadOnlyDictionary<string, string[]>?
        ValidationErrors { get; }

    public string? Error { get; }

    public bool Succeeded => Failure is null;

    public static PlanningMutationResult<T> Success(
        T value,
        bool changed = true)
    {
        return new PlanningMutationResult<T>(
            value,
            changed,
            null,
            null,
            null);
    }

    public static PlanningMutationResult<T>
        ValidationFailure(
            IReadOnlyDictionary<string, string[]> errors)
    {
        return new PlanningMutationResult<T>(
            null,
            false,
            PlanningMutationFailure.Validation,
            errors,
            null);
    }

    public static PlanningMutationResult<T> NotFound(
        string error)
    {
        return new PlanningMutationResult<T>(
            null,
            false,
            PlanningMutationFailure.NotFound,
            null,
            error);
    }

    public static PlanningMutationResult<T> Conflict(
        string error)
    {
        return new PlanningMutationResult<T>(
            null,
            false,
            PlanningMutationFailure.Conflict,
            null,
            error);
    }
}

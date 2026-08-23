using WhatShouldIWorkOnToday.Services;

namespace WhatShouldIWorkOnToday.Api;

public static partial class ApiEndpoints
{
    private static async Task<IResult> GetRandomPickAsync(
        WorkChooser workChooser,
        CancellationToken cancellationToken,
        bool favorPriority = false)
    {
        var todo = await workChooser.ChooseRandomAsync(
            favorPriority,
            cancellationToken);

        if (todo is null)
        {
            return Results.NoContent();
        }

        return Results.Ok(
            new RandomPickDto(
                new TodoItemDto(
                    todo.Id,
                    todo.WorkItemId,
                    todo.WorkItem.Name,
                    todo.Task,
                    todo.Energy.ToString(),
                    todo.Effort.ToString(),
                    todo.Priority.ToString(),
                    todo.CreatedAt,
                    todo.CompletedAt),
                favorPriority));
    }
}

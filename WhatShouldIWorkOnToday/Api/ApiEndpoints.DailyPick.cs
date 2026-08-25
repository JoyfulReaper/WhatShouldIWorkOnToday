using WhatShouldIWorkOnToday.Services;

namespace WhatShouldIWorkOnToday.Api;

public static partial class ApiEndpoints
{
    private static async Task<IResult> GetDailyPickAsync(
        WorkChooser workChooser,
        PlanningClock planningClock,
        CancellationToken cancellationToken)
    {
        var date = planningClock.Today();
        var todo = await workChooser.ChooseDailyAsync(date, cancellationToken: cancellationToken);

        if (todo is null)
        {
            return Results.NoContent();
        }

        var response =
            new DailyPickDto(
                date,
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
                todo.WorkItem.LastWorkedAt);

        return Results.Ok(response);
    }
}

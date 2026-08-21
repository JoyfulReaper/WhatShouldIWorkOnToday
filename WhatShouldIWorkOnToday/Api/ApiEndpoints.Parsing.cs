using WhatShouldIWorkOnToday.Models;
using WhatShouldIWorkOnToday.Services;

namespace WhatShouldIWorkOnToday.Api;

public static partial class ApiEndpoints
{
    private static NormalizedWorkItem
        ValidateAndNormalizeWorkItem(
            string? nameValue,
            string? kindValue,
            string? descriptionValue,
            string? urlValue,
            string? priorityValue,
            string keyPrefix,
            Dictionary<string, string[]> errors)
    {
        return PlanningMutationService
            .ValidateAndNormalizeWorkItem(
                nameValue,
                kindValue,
                descriptionValue,
                urlValue,
                priorityValue,
                keyPrefix,
                errors);
    }

    private static NormalizedTodo ValidateAndNormalizeTodo(
        string? taskValue,
        string? energyValue,
        string? effortValue,
        string? priorityValue,
        string keyPrefix,
        Dictionary<string, string[]> errors)
    {
        return PlanningMutationService.ValidateAndNormalizeTodo(
            taskValue,
            energyValue,
            effortValue,
            priorityValue,
            keyPrefix,
            errors);
    }

}

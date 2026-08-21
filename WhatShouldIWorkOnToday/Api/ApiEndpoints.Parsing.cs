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
            string keyPrefix,
            Dictionary<string, string[]> errors)
    {
        return PlanningMutationService
            .ValidateAndNormalizeWorkItem(
                nameValue,
                kindValue,
                descriptionValue,
                urlValue,
                keyPrefix,
                errors);
    }

    private static bool TryParseEnergy(
        string? value,
        out EnergyLevel energy)
    {
        return PlanningMutationService
            .TryParseEnergy(
                value,
                out energy);
    }

    private static bool TryParseEffort(
        string? value,
        out EffortLevel effort)
    {
        return PlanningMutationService
            .TryParseEffort(
                value,
                out effort);
    }
}

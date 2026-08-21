using WhatShouldIWorkOnToday.Models;

namespace WhatShouldIWorkOnToday.Api;

public static partial class ApiEndpoints
{
    private static bool TryParseEnergy(
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

    private static bool TryParseEffort(
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

    private static bool TryParseWorkItemKind(
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
}
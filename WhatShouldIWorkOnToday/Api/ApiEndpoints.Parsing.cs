using WhatShouldIWorkOnToday.Models;

namespace WhatShouldIWorkOnToday.Api;

public static partial class ApiEndpoints
{
    private static (
        string Name,
        WorkItemKind Kind,
        string? Description,
        string? Url) ValidateAndNormalizeWorkItem(
            string? nameValue,
            string? kindValue,
            string? descriptionValue,
            string? urlValue,
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
            string.IsNullOrWhiteSpace(
                descriptionValue)
                ? null
                : descriptionValue.Trim();

        if (description?.Length > 2000)
        {
            errors[$"{keyPrefix}description"] =
            [
                "Description cannot exceed 2000 characters."
            ];
        }

        var url =
            string.IsNullOrWhiteSpace(urlValue)
                ? null
                : urlValue.Trim();

        if (url?.Length > 2048)
        {
            errors[$"{keyPrefix}url"] =
            [
                "URL cannot exceed 2048 characters."
            ];
        }

        return (
            name,
            kind,
            description,
            url
        );
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

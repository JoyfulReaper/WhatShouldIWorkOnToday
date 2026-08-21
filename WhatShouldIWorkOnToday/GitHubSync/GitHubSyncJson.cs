using System.Text.Json;
using System.Text.Json.Serialization;

namespace WhatShouldIWorkOnToday.GitHubSync;

public static class GitHubSyncJson
{
    public static readonly JsonSerializerOptions Compact =
        Create(writeIndented: false);

    public static readonly JsonSerializerOptions Indented =
        Create(writeIndented: true);

    private static JsonSerializerOptions Create(
        bool writeIndented)
    {
        return new JsonSerializerOptions(
            JsonSerializerDefaults.Web)
        {
            WriteIndented = writeIndented,
            DefaultIgnoreCondition =
                JsonIgnoreCondition.WhenWritingNull
        };
    }
}

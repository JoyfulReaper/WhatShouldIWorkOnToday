namespace WhatShouldIWorkOnToday.Data;

public static class DatabasePath
{
    public const string DefaultRelativePath =
        "App_Data/WhatShouldIWorkOnToday.db";

    public static string Resolve(
        string path,
        string basePath)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new InvalidOperationException("Database path cannot be empty.");
        }

        var resolvedPath = Path.IsPathRooted(path)
            ? path
            : Path.GetFullPath(Path.Combine(basePath, path));

        var directory = Path.GetDirectoryName(resolvedPath);

        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        return resolvedPath;
    }
}
namespace WhatShouldIWorkOnToday.GitHubSync;

public interface IGitHubSyncClient
{
    Task<GitHubSyncFile?> GetFileAsync(
        string path,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<GitHubSyncFileEntry>>
        ListFilesAsync(
            string path,
            CancellationToken cancellationToken = default);

    Task WriteFileAsync(
        string path,
        byte[] content,
        string commitMessage,
        string? existingSha,
        CancellationToken cancellationToken = default);

    Task DeleteFileAsync(
        string path,
        string sha,
        string commitMessage,
        CancellationToken cancellationToken = default);
}

public sealed record GitHubSyncFile(
    string Path,
    string Sha,
    byte[] Content);

public sealed record GitHubSyncFileEntry(
    string Name,
    string Path,
    string Sha);

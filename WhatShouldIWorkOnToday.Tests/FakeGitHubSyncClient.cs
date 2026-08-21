using WhatShouldIWorkOnToday.GitHubSync;

namespace WhatShouldIWorkOnToday.Tests;

internal sealed class FakeGitHubSyncClient
    : IGitHubSyncClient
{
    private readonly Dictionary<string, GitHubSyncFile>
        _files = new(StringComparer.Ordinal);

    private int _shaSequence;

    public List<string> Operations { get; } = [];

    public Dictionary<string, int> WriteFailuresRemaining { get; } =
        new(StringComparer.Ordinal);

    public Dictionary<string, int> DeleteFailuresRemaining { get; } =
        new(StringComparer.Ordinal);

    public IReadOnlyDictionary<string, GitHubSyncFile> Files =>
        _files;

    public void SetFile(
        string path,
        byte[] content)
    {
        _files[path] = new GitHubSyncFile(
            path,
            NextSha(),
            content.ToArray());
    }

    public Task<GitHubSyncFile?> GetFileAsync(
        string path,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Operations.Add($"get:{path}");

        return Task.FromResult(
            _files.TryGetValue(path, out var file)
                ? file with
                {
                    Content = file.Content.ToArray()
                }
                : null);
    }

    public Task<IReadOnlyList<GitHubSyncFileEntry>>
        ListFilesAsync(
            string path,
            CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Operations.Add($"list:{path}");

        var prefix = path.TrimEnd('/') + "/";

        IReadOnlyList<GitHubSyncFileEntry> entries =
            _files.Values
                .Where(file =>
                    file.Path.StartsWith(
                        prefix,
                        StringComparison.Ordinal) &&
                    !file.Path[prefix.Length..]
                        .Contains('/'))
                .Select(file =>
                    new GitHubSyncFileEntry(
                        Path.GetFileName(file.Path),
                        file.Path,
                        file.Sha))
                .ToList();

        return Task.FromResult(entries);
    }

    public Task WriteFileAsync(
        string path,
        byte[] content,
        string commitMessage,
        string? existingSha,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Operations.Add($"write:{path}");

        if (WriteFailuresRemaining.TryGetValue(
                path,
                out var failures) &&
            failures > 0)
        {
            WriteFailuresRemaining[path] = failures - 1;

            throw new GitHubSyncException(
                "Simulated transient GitHub failure.");
        }

        _files[path] = new GitHubSyncFile(
            path,
            NextSha(),
            content.ToArray());

        return Task.CompletedTask;
    }

    public Task DeleteFileAsync(
        string path,
        string sha,
        string commitMessage,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Operations.Add($"delete:{path}");

        if (DeleteFailuresRemaining.TryGetValue(
                path,
                out var failures) &&
            failures > 0)
        {
            DeleteFailuresRemaining[path] = failures - 1;

            throw new GitHubSyncException(
                "Simulated transient GitHub delete failure.");
        }

        _files.Remove(path);

        return Task.CompletedTask;
    }

    private string NextSha()
    {
        _shaSequence++;
        return $"sha-{_shaSequence}";
    }
}

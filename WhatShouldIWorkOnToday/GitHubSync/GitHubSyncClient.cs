using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;

namespace WhatShouldIWorkOnToday.GitHubSync;

public sealed class GitHubSyncClient(
    HttpClient httpClient,
    IOptions<GitHubSyncOptions> options)
    : IGitHubSyncClient
{
    private readonly GitHubSyncOptions _options =
        options.Value;

    public async Task<GitHubSyncFile?> GetFileAsync(
        string path,
        CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest(
            HttpMethod.Get,
            BuildContentsUri(
                path,
                includeBranchQuery: true));

        using var response = await httpClient.SendAsync(
            request,
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await EnsureSuccessAsync(
            response,
            path,
            cancellationToken);

        var content = await response.Content
            .ReadFromJsonAsync<GitHubFileResponse>(
                GitHubSyncJson.Compact,
                cancellationToken);

        if (content is null ||
            string.IsNullOrWhiteSpace(content.Sha) ||
            string.IsNullOrWhiteSpace(content.Content))
        {
            throw new GitHubSyncException(
                $"GitHub returned an invalid file response for '{path}'.");
        }

        try
        {
            var bytes = Convert.FromBase64String(
                content.Content.Replace(
                    "\n",
                    string.Empty,
                    StringComparison.Ordinal));

            return new GitHubSyncFile(
                content.Path ?? path,
                content.Sha,
                bytes);
        }
        catch (FormatException exception)
        {
            throw new GitHubSyncException(
                $"GitHub returned invalid base64 content for '{path}'.",
                exception);
        }
    }

    public async Task<IReadOnlyList<GitHubSyncFileEntry>>
        ListFilesAsync(
            string path,
            CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest(
            HttpMethod.Get,
            BuildContentsUri(
                path,
                includeBranchQuery: true));

        using var response = await httpClient.SendAsync(
            request,
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return [];
        }

        await EnsureSuccessAsync(
            response,
            path,
            cancellationToken);

        var entries = await response.Content
            .ReadFromJsonAsync<List<GitHubDirectoryEntryResponse>>(
                GitHubSyncJson.Compact,
                cancellationToken)
            ?? [];

        return entries
            .Where(entry =>
                string.Equals(
                    entry.Type,
                    "file",
                    StringComparison.Ordinal) &&
                !string.IsNullOrWhiteSpace(entry.Name) &&
                !string.IsNullOrWhiteSpace(entry.Path) &&
                !string.IsNullOrWhiteSpace(entry.Sha))
            .Select(entry =>
                new GitHubSyncFileEntry(
                    entry.Name!,
                    entry.Path!,
                    entry.Sha!))
            .ToList();
    }

    public async Task WriteFileAsync(
        string path,
        byte[] content,
        string commitMessage,
        string? existingSha,
        CancellationToken cancellationToken = default)
    {
        var body = new GitHubWriteFileRequest(
            commitMessage,
            Convert.ToBase64String(content),
            _options.Branch,
            existingSha);

        using var request = CreateRequest(
            HttpMethod.Put,
            BuildContentsUri(
                path,
                includeBranchQuery: false));

        request.Content = JsonContent.Create(
            body,
            options: GitHubSyncJson.Compact);

        using var response = await httpClient.SendAsync(
            request,
            cancellationToken);

        await EnsureSuccessAsync(
            response,
            path,
            cancellationToken);
    }

    public async Task DeleteFileAsync(
        string path,
        string sha,
        string commitMessage,
        CancellationToken cancellationToken = default)
    {
        var body = new GitHubDeleteFileRequest(
            commitMessage,
            sha,
            _options.Branch);

        using var request = CreateRequest(
            HttpMethod.Delete,
            BuildContentsUri(
                path,
                includeBranchQuery: false));

        request.Content = JsonContent.Create(
            body,
            options: GitHubSyncJson.Compact);

        using var response = await httpClient.SendAsync(
            request,
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return;
        }

        await EnsureSuccessAsync(
            response,
            path,
            cancellationToken);
    }

    private HttpRequestMessage CreateRequest(
        HttpMethod method,
        string requestUri)
    {
        var request = new HttpRequestMessage(
            method,
            requestUri);

        request.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                _options.Token);

        return request;
    }

    private string BuildContentsUri(
        string path,
        bool includeBranchQuery)
    {
        var escapedPath = string.Join(
            '/',
            path.Split(
                    '/',
                    StringSplitOptions.RemoveEmptyEntries)
                .Select(Uri.EscapeDataString));

        var uri =
            $"repos/{Uri.EscapeDataString(_options.Owner)}/" +
            $"{Uri.EscapeDataString(_options.Repository)}/" +
            $"contents/{escapedPath}";

        return includeBranchQuery
            ? $"{uri}?ref={Uri.EscapeDataString(_options.Branch)}"
            : uri;
    }

    private static async Task EnsureSuccessAsync(
        HttpResponseMessage response,
        string path,
        CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var rateLimitRemaining = response.Headers
            .TryGetValues(
                "X-RateLimit-Remaining",
                out var values)
            ? values.FirstOrDefault()
            : null;

        await response.Content
            .ReadAsByteArrayAsync(cancellationToken);

        var rateLimitDetail =
            rateLimitRemaining is null
                ? string.Empty
                : $" GitHub rate-limit remaining: {rateLimitRemaining}.";

        throw new GitHubSyncException(
            $"GitHub request for '{path}' failed with HTTP " +
            $"{(int)response.StatusCode} ({response.ReasonPhrase})." +
            rateLimitDetail);
    }

    private sealed record GitHubFileResponse(
        string? Path,
        string? Sha,
        string? Content);

    private sealed record GitHubDirectoryEntryResponse(
        string? Name,
        string? Path,
        string? Sha,
        string? Type);

    private sealed record GitHubWriteFileRequest(
        string Message,
        string Content,
        string Branch,
        [property: JsonPropertyName("sha")]
        string? Sha);

    private sealed record GitHubDeleteFileRequest(
        string Message,
        string Sha,
        string Branch);
}

public sealed class GitHubSyncException
    : Exception
{
    public GitHubSyncException(string message)
        : base(message)
    {
    }

    public GitHubSyncException(
        string message,
        Exception innerException)
        : base(message, innerException)
    {
    }
}

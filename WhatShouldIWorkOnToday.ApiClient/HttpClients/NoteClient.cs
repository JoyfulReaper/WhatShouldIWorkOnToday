using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using WhatShouldIWorkOnToday.ApiClient.Common;
using WhatShouldIWorkOnToday.ApiClient.Common.Options;
using WhatShouldIWorkOnToday.ApiClient.Contracts;

namespace WhatShouldIWorkOnToday.ApiClient.HttpClients;
public class NoteClient : INoteClient
{
    private readonly HttpClient _client;
    private readonly WsiwotClientOptions _options;

    public NoteClient(HttpClient client, IOptions<WsiwotClientOptions> options)
    {
        _client = client;
        _options = options.Value;

        ApiClientConfigurationHelper.ConfigureHttpClient(client, _options);
    }

    public async Task<Note> CreateAsync(Note note, CancellationToken cancellationToken = default)
    {
        using var response = await _client.PostAsJsonAsync(_client.BaseAddress + "Note/", note, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<Note>(cancellationToken: cancellationToken);

        return result;
    }

    public async Task DeleteAsync(int noteId, CancellationToken cancellationToken = default)
    {
        using var response = await _client.DeleteAsync(_client.BaseAddress + $"Note/{noteId}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<Note?> GetAsync(int noteId, CancellationToken cancellationToken = default)
    {
        var response = await _client.GetFromJsonAsync<Note>(_client.BaseAddress + $"Note/{noteId}", cancellationToken);
        return response;
    }

    public async Task<List<Note>> GetByWorkItemAsync(int workItemId, CancellationToken cancellationToken = default)
    {
        var response = await _client.GetFromJsonAsync<List<Note>>(_client.BaseAddress + $"Note/WorkItem/{workItemId}", cancellationToken);
        return response;
    }
}

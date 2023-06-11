using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
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
        var response = await _client.PostAsJsonAsync(_client.BaseAddress + "/Note", note);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<Note>();

        return result;
    }

    public Task DeleteAsync(int noteId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Note?> GetAsync(int noteId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<Note>> GetByWorkItemAsync(int workItemId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using WhatShouldIWorkOnToday.ApiClient.Common;
using WhatShouldIWorkOnToday.ApiClient.Common.Options;
using WhatShouldIWorkOnToday.ApiClient.Contracts;

namespace WhatShouldIWorkOnToday.ApiClient.HttpClients;
public class ToDoItemClient : ITodoItemClient
{
    private readonly HttpClient _httpClient;
    private readonly WsiwotClientOptions _options;

    public ToDoItemClient(HttpClient httpClient, IOptions<WsiwotClientOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;

        ApiClientConfigurationHelper.ConfigureHttpClient(httpClient, _options);
    }

    public async Task CompleteAsync(int todoItemId, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PutAsync(_httpClient.BaseAddress + $"ToDo/complete/{todoItemId}", null, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<ToDoItem> CreateAsync(ToDoItemRequest todoItemrequest, CancellationToken cancellationToken = default)
    {
        using var resposne = await _httpClient.PostAsJsonAsync(_httpClient.BaseAddress + "ToDo", todoItemrequest, cancellationToken);
        resposne.EnsureSuccessStatusCode();
        var output = await resposne.Content.ReadFromJsonAsync<ToDoItem>();
        return output;
    }

    public async Task DeleteAsync(int todoItemId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync(_httpClient.BaseAddress + $"ToDo/{todoItemId}", cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<ToDoItem?> GetAsync(int todoItemId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetFromJsonAsync<ToDoItem>(_httpClient.BaseAddress + $"ToDo/{todoItemId}");
        return response;
    }

    public async Task<List<ToDoItem>> GetByWorkItemAsync(int workItemId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetFromJsonAsync<List<ToDoItem>>(_httpClient.BaseAddress + $"ToDo/WorkItem/${workItemId}");
        return response ?? new List<ToDoItem>();
    }
}

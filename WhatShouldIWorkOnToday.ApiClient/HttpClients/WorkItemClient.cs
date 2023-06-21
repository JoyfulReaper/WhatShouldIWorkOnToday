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
public class WorkItemClient : IWorkItemClient
{
    private readonly HttpClient _client;
    private readonly WsiwotClientOptions _options;

    public WorkItemClient(HttpClient client, IOptions<WsiwotClientOptions> options)
    {
        _client = client;
        _options = options.Value;

        ApiClientConfigurationHelper.ConfigureHttpClient(_client, _options);
    }

    public async Task<WorkItem?> CreateAsync(WorkItem workItem, CancellationToken cancellationToken)
    {
        using var response = await _client.PostAsJsonAsync(_client.BaseAddress + "WorkItem/", workItem, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<WorkItem>();
        return result;
    }

    public async Task DeleteAsync(int workItemId, CancellationToken cancellationToken)
    {
        using var response = await _client.DeleteAsync(_client.BaseAddress + $"WorkItem/{workItemId}", cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<WorkItem>> GetAllAsync(CancellationToken cancellationToken)
    {
        var response = await _client.GetFromJsonAsync<List<WorkItem>>(_client.BaseAddress + "WorkItem/", cancellationToken);
        return response ?? new List<WorkItem>();
    }

    public async Task<WorkItem?> GetAsync(int workItemId, CancellationToken cancellationToken)
    {
        var response = await _client.GetFromJsonAsync<WorkItem>(_client.BaseAddress + $"WorkItem/{workItemId}", cancellationToken);
        return response;
    }

    public async Task<List<WorkItem>> GetBySequenceNumberAsync(int sequenceNumber, CancellationToken cancellationToken)
    {
        var response = await _client.GetFromJsonAsync<List<WorkItem>>(_client.BaseAddress + $"WorkItem/SequenceNumber/{sequenceNumber}", cancellationToken);
        return response ?? new List<WorkItem>();
    }

    public async Task<List<WorkItem>> GetCompletedAsync(CancellationToken cancellationToken)
    {
        var response = await _client.GetFromJsonAsync<List<WorkItem>>(_client.BaseAddress + "WorkItem/Completed", cancellationToken);
        return response ?? new List<WorkItem>();
    }

    public async Task<WorkItem> UpdateAsync(WorkItem workItem, CancellationToken cancellationToken)
    {
        using var response = await _client.PutAsJsonAsync(_client.BaseAddress + "WorkItem/", workItem, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<WorkItem>();
        return result;
    }

    public async Task MarkAsWorked(int workItemId, CancellationToken cancellationToken = default)
    {
        var response = await _client.PutAsync(_client.BaseAddress + $"WorkItem/MarkWorked/{workItemId}", null, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<WorkItem>> SearchAsync(string searchTerm, CancellationToken cancellationToken)
    {
        var response = await _client.GetAsync(_client.BaseAddress + $"WorkItem/Search/{searchTerm}", cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<List<WorkItem>>();
        return result ?? new List<WorkItem>();
    }

    public async Task<List<WorkItemHistory>> GetHistoryAsync(int workItemId)
    {
        var response = await _client.GetFromJsonAsync<List<WorkItemHistory>>(_client.BaseAddress + $"WorkItem/History/{workItemId}");
        return response ?? new List<WorkItemHistory>();
    }
}

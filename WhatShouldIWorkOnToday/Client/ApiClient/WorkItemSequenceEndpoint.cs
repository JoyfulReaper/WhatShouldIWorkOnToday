using System.Net.Http.Json;
using WhatShouldIWorkOnToday.Client.Models;

namespace WhatShouldIWorkOnToday.Client.ApiClient;

public class WorkItemSequenceEndpoint : Endpoint, IWorkItemSequenceEndpoint
{
    private readonly HttpClient _httpClient;

    public WorkItemSequenceEndpoint(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<WorkItemSequence>> GetAllAsync()
    {
        var seqItems = await _httpClient.GetFromJsonAsync<List<WorkItemSequence>>("api/WorkItemSequence");
        if (seqItems is null)
        {
            throw new Exception("Failed to de-serialize work sequence list.");
        }

        return seqItems;
    }

    public async Task<WorkItemSequence> PostAsync(WorkItemSequence workItemSequence)
    {
        using var response = await _httpClient.PostAsJsonAsync("api/WorkItemSequence", workItemSequence);
        CheckResponse(response);

        var workItemResp = await response.Content.ReadFromJsonAsync<WorkItemSequence>();
        if (workItemResp is null)
        {
            throw new Exception("Failed to de-serialize work item.");
        }

        return workItemResp;
    }
}

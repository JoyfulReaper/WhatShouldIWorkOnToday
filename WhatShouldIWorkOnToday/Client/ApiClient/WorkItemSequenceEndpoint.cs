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

    public async Task<WorkSequenceNumber> PostAsync(WorkSequenceNumber workSequenceNumber)
    {
        using var response = await _httpClient.PostAsJsonAsync("api/WorkItemSequence", workSequenceNumber);
        CheckResponse(response);

        var workItemResp = await response.Content.ReadFromJsonAsync<WorkSequenceNumber>();
        if (workItemResp is null)
        {
            throw new Exception("Failed to de-serialize work item.");
        }

        return workItemResp;
    }

    public async Task PutAsync(WorkSequenceNumber workSequenceNumber)
    {
        using var response = await _httpClient.PutAsJsonAsync($"api/WorkItemSequence/{workSequenceNumber.WorkItemId}", workSequenceNumber);
        CheckResponse(response);
    }
}

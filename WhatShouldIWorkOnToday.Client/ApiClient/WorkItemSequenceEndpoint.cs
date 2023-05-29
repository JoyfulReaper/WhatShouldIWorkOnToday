using System.Net.Http.Json;
using WhatShouldIWorkOnToday.Client.ApiClient.Interfaces;
using WhatShouldIWorkOnToday.Client.Models;

namespace WhatShouldIWorkOnToday.Client.ApiClient;

public class WorkItemSequenceEndpoint : Endpoint, IWorkItemSequenceEndpoint
{
    private readonly HttpClient _httpClient;

    public WorkItemSequenceEndpoint(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<WorkItemSequence> GetWorkItemSequenceAsync(int workSequenceNumberId)
    {
        var wis = await _httpClient.GetFromJsonAsync<WorkItemSequence>($"api/WorkItemSequence/{workSequenceNumberId}");
        ThrowIfNull(wis);

        return wis!;
    }

    public async Task<List<WorkItemSequence>> GetAllWorkItemSequenceAsync()
    {
        var seqItems = await _httpClient.GetFromJsonAsync<List<WorkItemSequence>>("api/WorkItemSequence");
        ThrowIfNull(seqItems);

        return seqItems!;
    }

    public async Task<WorkSequenceNumber> PostAsync(WorkSequenceNumber workSequenceNumber)
    {
        using var response = await _httpClient.PostAsJsonAsync("api/WorkItemSequence", workSequenceNumber);
        CheckResponse(response);

        var workItemResp = await response.Content.ReadFromJsonAsync<WorkSequenceNumber>();
        ThrowIfNull(workItemResp);

        return workItemResp!;
    }

    public async Task PutAsync(WorkSequenceNumber workSequenceNumber)
    {
        using var response = await _httpClient.PutAsJsonAsync($"api/WorkItemSequence/{workSequenceNumber.WorkItemId}", workSequenceNumber);
        CheckResponse(response);
    }
}

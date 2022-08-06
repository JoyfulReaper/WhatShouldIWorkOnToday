using System.Net.Http.Json;
using WhatShouldIWorkOnToday.Client.Models;

namespace WhatShouldIWorkOnToday.Client.ApiClient;

public class WorkItemEndpoint : Endpoint, IWorkItemEndpoint
{
    private readonly HttpClient _httpClient;

    public WorkItemEndpoint(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<WorkItem>> GetAllAsync()
    {
        var workItems = await _httpClient.GetFromJsonAsync<List<WorkItem>>("api/WorkItem");
        if (workItems is null)
        {
            throw new Exception("Failed to de-serialize list of work items.");
        }

        return workItems;
    }

    public async Task<WorkItem> GetAsync(int id)
    {
        var workItem = await _httpClient.GetFromJsonAsync<WorkItem>($"api/WorkItem/{id}");
        if (workItem is null)
        {
            throw new Exception("Failed to de-serialize work item.");
        }

        return workItem;
    }

    public async Task<WorkItem> PostAsync(WorkItem workItem)
    {
        using var response = await _httpClient.PostAsJsonAsync("api/WorkItem", workItem);
        CheckResponse(response);

        var workItemResp = await response.Content.ReadFromJsonAsync<WorkItem>();
        if (workItemResp is null)
        {
            throw new Exception("Failed to de-serialize work item.");
        }

        return workItemResp;
    }

    public async Task PutAsync(WorkItem workItem)
    {
        using var response = await _httpClient.PutAsJsonAsync($"api/WorkItem/{workItem.WorkItemId}", workItem);
        CheckResponse(response);
    }

    public async Task DeleteAsync(int Id)
    {
        using var response = await _httpClient.DeleteAsync($"/api/WorkItem/{Id}");
        CheckResponse(response);
    }
}

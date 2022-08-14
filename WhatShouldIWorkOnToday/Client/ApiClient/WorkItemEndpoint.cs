using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Web;
using WhatShouldIWorkOnToday.Client.ApiClient.Interfaces;
using WhatShouldIWorkOnToday.Client.Models;

namespace WhatShouldIWorkOnToday.Client.ApiClient;

public class WorkItemEndpoint : Endpoint, IWorkItemEndpoint
{
    private readonly HttpClient _httpClient;

    public WorkItemEndpoint(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<WorkItem> UpdateWorkedOn(int workItemId)
    {
        var response = await _httpClient.PostAsync("api/WorkItem/UpdateWorkedOn", 
            new StringContent(JsonSerializer.Serialize(workItemId), Encoding.UTF8, "application/json"));
        CheckResponse(response);
        var workItem = await response.Content.ReadFromJsonAsync<WorkItem>();
        ThrowIfNull(workItem);

        return workItem!;
    }

    public async Task<List<WorkItem>> GetCurrent()
    {
        var workItems = await _httpClient.GetFromJsonAsync<List<WorkItem>>("api/WorkItem/Current");
        ThrowIfNull(workItems);

        return workItems!;
    }

    public async Task<List<WorkItem>> GetAllAsync()
    {
        var workItems = await _httpClient.GetFromJsonAsync<List<WorkItem>>("api/WorkItem");
        ThrowIfNull(workItems);

        return workItems!;
    }

    public async Task<WorkItem> GetAsync(int id)
    {
        var workItem = await _httpClient.GetFromJsonAsync<WorkItem>($"api/WorkItem/{id}");
        ThrowIfNull(workItem);

        return workItem!;
    }

    public async Task<WorkItem> PostAsync(WorkItem workItem)
    {
        using var response = await _httpClient.PostAsJsonAsync("api/WorkItem", workItem);
        CheckResponse(response);

        var workItemResp = await response.Content.ReadFromJsonAsync<WorkItem>();
        ThrowIfNull(workItemResp);

        return workItemResp!;
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

    // Search work item names
    public async Task<List<WorkItem>> Search(string term)
    {
        var encodedTerm = HttpUtility.UrlEncode(term);
        var workItems = await _httpClient.GetFromJsonAsync<List<WorkItem>>($"api/WorkItem/Search?term={encodedTerm}");
        ThrowIfNull(workItems);

        return workItems!;
    }
}

using WhatShouldIWorkOnToday.Client.Models;

namespace WhatShouldIWorkOnToday.Client.ApiClient;
public interface IWorkItemEndpoint
{
    Task DeleteAsync(int Id);
    Task<List<WorkItem>> GetAllAsync();
    Task<WorkItem> GetAsync(int id);
    Task<WorkItem> PostAsync(WorkItem workItem);
    Task PutAsync(WorkItem workItem);
}
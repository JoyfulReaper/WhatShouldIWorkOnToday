using WhatShouldIWorkOnToday.Client.Models;

namespace WhatShouldIWorkOnToday.Client.ApiClient;
public interface IWorkItemEndpoint
{
    Task DeleteAsync(int Id);
    Task<List<WorkItem>> GetAllAsync();
    Task<WorkItem> GetAsync(int id);
    Task<List<WorkItem>> GetCurrent();
    Task<WorkItem> PostAsync(WorkItem workItem);
    Task PutAsync(WorkItem workItem);
    Task<List<WorkItem>> Search(string term);
    Task<WorkItem> UpdateWorkedOn(int workItemId);
}
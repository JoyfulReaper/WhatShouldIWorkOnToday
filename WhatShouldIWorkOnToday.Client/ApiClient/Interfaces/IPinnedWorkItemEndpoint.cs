using WhatShouldIWorkOnToday.Client.Models;

namespace WhatShouldIWorkOnToday.Client.ApiClient.Interfaces;
public interface IPinnedWorkItemEndpoint
{
    Task<List<PinnedWorkItem>> GetAllAsync();
    Task<List<WorkItem>> GetAllPinnedWorkItems();
    Task PinWorkItem(int workItemId);
    Task UnpinWorkItem(int workItemId);
}
using WhatShouldIWorkOnToday.ApiClient.Contracts;

namespace WhatShouldIWorkOnToday.ApiClient.HttpClients;
public interface IWorkItemClient
{
    Task<List<WorkItem>> GetCompletedAsync(CancellationToken cancellationToken);
    Task<List<WorkItem>> GetAllAsync(CancellationToken cancellationToken);
    Task<WorkItem?> GetAsync(int workItemId, CancellationToken cancellationToken);
    Task<List<WorkItem>> GetBySequenceNumberAsync(int sequenceNumber, CancellationToken cancellationToken);

    Task<WorkItem?> CreateAsync(WorkItem workItem, CancellationToken cancellationToken);
    Task<WorkItem> UpdateAsync(WorkItem workItem, CancellationToken cancellationToken);
    Task DeleteAsync(int workItemId, CancellationToken cancellationToken);
}

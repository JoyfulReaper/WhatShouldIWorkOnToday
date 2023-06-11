using WhatShouldIWorkOnToday.ApiClient.Contracts;

namespace WhatShouldIWorkOnToday.ApiClient.HttpClients;
public interface IWorkItemClient
{
    Task<List<WorkItem>> GetCompletedAsync(CancellationToken cancellationToken = default);
    Task<List<WorkItem>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<WorkItem?> GetAsync(int workItemId, CancellationToken cancellationToken = default);
    Task<List<WorkItem>> GetBySequenceNumberAsync(int sequenceNumber, CancellationToken cancellationToken = default);

    Task<WorkItem?> CreateAsync(WorkItem workItem, CancellationToken cancellationToken = default);
    Task<WorkItem> UpdateAsync(WorkItem workItem, CancellationToken cancellationToken = default);
    Task DeleteAsync(int workItemId, CancellationToken cancellationToken = default);
}

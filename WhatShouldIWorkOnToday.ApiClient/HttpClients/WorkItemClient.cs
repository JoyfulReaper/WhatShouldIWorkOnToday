using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatShouldIWorkOnToday.ApiClient.Contracts;

namespace WhatShouldIWorkOnToday.ApiClient.HttpClients;
public class WorkItemClient : IWorkItemClient
{
    public Task<WorkItem?> CreateAsync(WorkItem workItem, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(int workItemId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<List<WorkItem>> GetAllAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<WorkItem?> GetAsync(int workItemId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<List<WorkItem>> GetBySequenceNumberAsync(int sequenceNumber, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<List<WorkItem>> GetCompletedAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<WorkItem> UpdateAsync(WorkItem workItem, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

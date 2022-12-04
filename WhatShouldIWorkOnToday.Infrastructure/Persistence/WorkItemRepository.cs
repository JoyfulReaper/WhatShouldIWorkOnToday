
using WhatShouldIWorkOnToday.Application.Common.Interfaces.Persistence;
using WhatShouldIWorkOnToday.Domain.WorkItem;

namespace WhatShouldIWorkOnToday.Infrastructure.Persistence;

public class WorkItemRepository : IWorkItemRepository
{
    private static readonly List<WorkItem> _workItems = new();

    public void Add(WorkItem workItem)
    {
        _workItems.Add(workItem);
    }
}

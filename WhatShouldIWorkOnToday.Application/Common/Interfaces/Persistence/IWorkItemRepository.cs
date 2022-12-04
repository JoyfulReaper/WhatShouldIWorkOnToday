
using WhatShouldIWorkOnToday.Domain.WorkItem;

namespace WhatShouldIWorkOnToday.Application.Common.Interfaces.Persistence;

public interface IWorkItemRepository
{
    void Add(WorkItem workItem);
}

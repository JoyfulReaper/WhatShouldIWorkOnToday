using WhatShouldIWorkOnToday.Shared.Models;

namespace WhatShouldIWorkOnToday.Server.DataAccess;
public interface IWorkItemData
{
    Task<List<WorkItem>> GetAll();
    Task Save(WorkItem workItem);
}
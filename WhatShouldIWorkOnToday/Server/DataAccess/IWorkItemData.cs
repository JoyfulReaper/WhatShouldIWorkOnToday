using WhatShouldIWorkOnToday.Shared.Models;

namespace WhatShouldIWorkOnToday.Server.DataAccess;
public interface IWorkItemData
{
    Task<WorkItem?> Get(int id);
    Task<List<WorkItem>> GetAll();
    Task Save(WorkItem workItem);
}
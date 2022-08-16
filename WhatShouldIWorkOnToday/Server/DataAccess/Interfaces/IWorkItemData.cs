using WhatShouldIWorkOnToday.Server.Models;

namespace WhatShouldIWorkOnToday.Server.DataAccess.Interfaces;
public interface IWorkItemData
{
    Task<WorkItem?> GetAsync(int id);
    Task<List<WorkItem>> GetAllAsync();
    Task SaveAsync(WorkItem workItem);
    Task<List<WorkItem>> GetCompleteAsync();
    Task<List<WorkItem>> GetIncompleteAsync();
    Task<List<WorkItem>> GetBySequeunceNumber(int sequenceNumber);
}
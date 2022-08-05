using WhatShouldIWorkOnToday.Server.Models;

namespace WhatShouldIWorkOnToday.Server.DataAccess;
public interface IWorkItemSequenceData
{
    Task<WorkItemSequence?> GetAsync(int id);
    Task<List<WorkItemSequence>> GetAllAsync();
    Task SaveAsync(WorkItemSequence workItemSequence);
}
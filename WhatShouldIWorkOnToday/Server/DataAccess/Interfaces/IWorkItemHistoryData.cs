using WhatShouldIWorkOnToday.Server.Models;

namespace WhatShouldIWorkOnToday.Server.DataAccess.Interfaces;
public interface IWorkItemHistoryData
{
    Task<IEnumerable<WorkItemHistory>> Get(int workItemId);
    Task Save(int workItemId);
}
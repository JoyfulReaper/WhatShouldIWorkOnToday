using WhatShouldIWorkOnToday.Server.Models;

namespace WhatShouldIWorkOnToday.Server.DataAccess.Interfaces;
public interface IPinnedWorkItemData
{
    Task<PinnedWorkItem?> Get(int pinnedWorkItemId);
    Task<List<PinnedWorkItem>> GetAllAsync();
    Task Pin(int workItemId);
    Task Unpin(int workItemId);
}
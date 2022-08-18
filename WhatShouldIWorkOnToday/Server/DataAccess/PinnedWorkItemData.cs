using WhatShouldIWorkOnToday.Server.DataAccess.Interfaces;
using WhatShouldIWorkOnToday.Server.Models;

namespace WhatShouldIWorkOnToday.Server.DataAccess;

public class PinnedWorkItemData : IPinnedWorkItemData
{
    private IDataAccess _dataAccess;

    public PinnedWorkItemData(IDataAccess dataAccess)
    {
        _dataAccess = dataAccess;
    }

    public async Task<PinnedWorkItem?> Get(int pinnedWorkItemId)
    {
        return (await _dataAccess.LoadDataAsync<PinnedWorkItem, dynamic>("spPinnedWorkItem_Get", new { }, "WSIWOT"))
            .SingleOrDefault();
    }

    public async Task<List<PinnedWorkItem>> GetAllAsync()
    {
        return await _dataAccess.LoadDataAsync<PinnedWorkItem, dynamic>("spPinnedWorkItem_GetAll", new { }, "WSIWOT");
    }

    public async Task<List<WorkItem>> GetPinnedWorkItems()
    {
        return await _dataAccess.LoadDataAsync<WorkItem, dynamic>("spGetPinnedWorkItems", new { }, "WSIWOT");
    }

    public async Task Pin(int workItemId)
    {
        await _dataAccess.SaveDataAsync("spPinnedWorkItem_Pin", new
        {
            workItemId
        }, "WSIWOT");
    }

    public async Task Unpin(int workItemId)
    {
        await _dataAccess.SaveDataAsync("spPinnedWorkItem_Unpin", new
        {
            workItemId
        }, "WSIWOT");
    }
}

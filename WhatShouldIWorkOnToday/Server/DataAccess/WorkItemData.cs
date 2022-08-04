using WhatShouldIWorkOnToday.Shared.Models;

namespace WhatShouldIWorkOnToday.Server.DataAccess;

public class WorkItemData : IWorkItemData
{
	private readonly IDataAccess _dataAccess;

	public WorkItemData(IDataAccess dataAccess)
	{
		_dataAccess = dataAccess;
	}

	public async Task Save(WorkItem workItem)
	{
		var id = await _dataAccess.SaveDataAndGetIdAsync("spWorkItem_Upsert", new
		{
			WorkItemId = workItem.WorkItemId,
			Name = workItem.Name,
			Description = workItem.Description,
			Url = workItem.Url,
			DateDeleted = workItem.DateDeleted,
			DateCompleted = workItem.DateCompleted
		},
		"WSIWOT");

		workItem.WorkItemId = id;
	}

	public Task<List<WorkItem>> GetAll()
	{
		return _dataAccess.LoadDataAsync<WorkItem, dynamic>("spWorkItem_GetAll", new { }, "WSIWOT");
	}

	public async Task<WorkItem?> Get(int id)
	{
		return (await _dataAccess.LoadDataAsync<WorkItem, dynamic>("spWorkItem_Get", new { WorkItemId = id }, "WSIWOT")).SingleOrDefault();
	}
}

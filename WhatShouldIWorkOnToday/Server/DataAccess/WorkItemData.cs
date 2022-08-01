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
		},
		"WSIWOT");
	}

	public Task<List<WorkItem>> GetAll()
	{
		return _dataAccess.LoadDataAsync<WorkItem, dynamic>("spWorkItem_GetAll", new { }, "WSIWOT");
	}
}

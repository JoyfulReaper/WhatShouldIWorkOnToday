using WhatShouldIWorkOnToday.Server.DataAccess.Interfaces;
using WhatShouldIWorkOnToday.Server.Models;

namespace WhatShouldIWorkOnToday.Server.DataAccess;

public class WorkItemHistoryData : IWorkItemHistoryData
{
	private readonly IDataAccess _dataAccess;

	public WorkItemHistoryData(IDataAccess dataAccess)
	{
		_dataAccess = dataAccess;
	}

	public async Task Save(int workItemId)
	{
		await _dataAccess.SaveDataAsync("spWorkItemHistory_Insert", new { workItemId }, "WSIWOT");
	}

	public async Task<IEnumerable<WorkItemHistory>> Get(int workItemId)
	{
		var history = await _dataAccess.LoadDataAsync<WorkItemHistory, dynamic>("spWorkItemHistory_GetAll", new { workItemId }, "WSIWOT");
		return history;
	}
}

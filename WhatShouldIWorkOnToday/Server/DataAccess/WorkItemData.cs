using WhatShouldIWorkOnToday.Server.Models;

namespace WhatShouldIWorkOnToday.Server.DataAccess;

public class WorkItemData : IWorkItemData
{
	private readonly IDataAccess _dataAccess;

	public WorkItemData(IDataAccess dataAccess)
	{
		_dataAccess = dataAccess;
	}

	public async Task SaveAsync(WorkItem workItem)
	{
		var id = await _dataAccess.SaveDataAndGetIdAsync("spWorkItem_Upsert", new
		{
			WorkItemId = workItem.WorkItemId,
			Name = workItem.Name,
			Description = workItem.Description,
			Url = workItem.Url,
			DateDeleted = workItem.DateDeleted,
			DateCompleted = workItem.DateCompleted,
			DateWorkedOn = workItem.DateWorkedOn
		},
		"WSIWOT");

		workItem.WorkItemId = id;
	}

	public Task<List<WorkItem>> GetAllAsync()
	{
		return _dataAccess.LoadDataAsync<WorkItem, dynamic>("spWorkItem_GetAll", new { }, "WSIWOT");
	}

    public Task<List<WorkItem>> GetCompleteAsync()
	{
		return _dataAccess.LoadDataAsync<WorkItem, dynamic>("spWorkItem_GetComplete", new { }, "WSIWOT");
	}

    public Task<List<WorkItem>> GetBySequeunceNumber(int sequenceNumber)
	{
		return _dataAccess.LoadDataAsync<WorkItem, dynamic>("spWorkItem_GetBySequenceNumber", new
		{
            SequenceNumber = sequenceNumber
		}, "WSIWOT");
	}

    public Task<List<WorkItem>> GetIncompleteAsync()
	{
		return _dataAccess.LoadDataAsync<WorkItem, dynamic>("spWorkItem_GetIncomplete", new { }, "WSIWOT");
	}


    public async Task<WorkItem?> GetAsync(int id)
	{
		return (await _dataAccess.LoadDataAsync<WorkItem, dynamic>("spWorkItem_Get", new { WorkItemId = id }, "WSIWOT")).SingleOrDefault();
	}
}

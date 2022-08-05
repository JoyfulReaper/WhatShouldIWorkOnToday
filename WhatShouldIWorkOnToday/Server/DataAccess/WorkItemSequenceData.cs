using WhatShouldIWorkOnToday.Server.Models;

namespace WhatShouldIWorkOnToday.Server.DataAccess;

public class WorkItemSequenceData : IWorkItemSequenceData
{
    private readonly IDataAccess _dataAccess;

    public WorkItemSequenceData(IDataAccess dataAccess)
    {
        _dataAccess = dataAccess;
    }

    public async Task SaveAsync(WorkItemSequence workItemSequence)
    {
        var id = await _dataAccess.SaveDataAndGetIdAsync("spWorkItemSequence_Upsert", new
        {
            WorkSequenceNumberId = workItemSequence.WorkSequenceNumberId,
            WorkItemId = workItemSequence.WorkItemId,
            SequenceNumber = workItemSequence.SequenceNumber
        }, "WSIWOT");

        workItemSequence.WorkSequenceNumberId = id;
    }

    public async Task<List<WorkItemSequence>> GetAllAsync()
    {
        return await _dataAccess.LoadDataAsync<WorkItemSequence, dynamic>("spWorkItemSequence_GetAll", new { }, "WSIWOT");
    }

    public async Task<WorkItemSequence?> GetAsync(int id)
    {
        return (await _dataAccess.LoadDataAsync<WorkItemSequence, dynamic>("spWorkItemSequence_Get", new
        {
            WorkSequenceNumberId = id
        }, "WSIWOT")).SingleOrDefault();
    }
}

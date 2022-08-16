using WhatShouldIWorkOnToday.Server.DataAccess.Interfaces;
using WhatShouldIWorkOnToday.Server.DTOs;
using WhatShouldIWorkOnToday.Server.Models;

namespace WhatShouldIWorkOnToday.Server.DataAccess;

public class WorkSequenceNumberData : IWorkSequenceNumberData
{
    private readonly IDataAccess _dataAccess;

    public WorkSequenceNumberData(IDataAccess dataAccess)
    {
        _dataAccess = dataAccess;
    }
    
    public async Task SaveAsync(WorkSequenceNumber wsn)
    {
        var id = await _dataAccess.SaveDataAndGetIdAsync("spWorkSequenceNumber_Upsert", wsn, "WSIWOT");
        wsn.WorkSequenceNumberId = id;
    }

    public async Task<List<WorkItemSequenceDto>> GetAllWorkItemSequenceAsync()
    {
        return await _dataAccess.LoadDataAsync<WorkItemSequenceDto, dynamic>("spWorkItemSequence_GetAll", new { }, "WSIWOT");
    }

    public async Task<WorkItemSequenceDto?> GetWorkItemSequenceAsync(int workSequenceNumberId)
    {
        return (await _dataAccess.LoadDataAsync<WorkItemSequenceDto, dynamic>("spWorkItemSequence_Get", new
        {
            WorkSequenceNumberId = workSequenceNumberId
        }, "WSIWOT")).SingleOrDefault();
    }
}

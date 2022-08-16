using WhatShouldIWorkOnToday.Server.DataAccess.Interfaces;
using WhatShouldIWorkOnToday.Server.Models;

namespace WhatShouldIWorkOnToday.Server.DataAccess;

public class CurrentSequenceNumberData : ICurrentSequenceNumberData
{
    private readonly IDataAccess _dataAccess;

    public CurrentSequenceNumberData(IDataAccess dataAccess)
    {
        _dataAccess = dataAccess;
    }

    public async Task UpdateAsync(CurrentSequenceNumber currentSequenceNumber)
    {
        await _dataAccess.SaveDataAsync("spCurrentSequenceNumber_Update", new
        {
            CurrentSequence = currentSequenceNumber.CurrentSequence,
            DateSet = currentSequenceNumber.DateSet
        }, "WSIWOT");
    }

    public async Task<CurrentSequenceNumber> GetAsync()
    {
        var currentSeq = (await _dataAccess.LoadDataAsync<CurrentSequenceNumber, dynamic>("spCurrentSequenceNumber_Get", new { }, "WSIWOT")).SingleOrDefault();
        if (currentSeq == null)
        {
            currentSeq = new CurrentSequenceNumber()
            {
                CurrentSequence = 1,
                DateSet = DateTime.UtcNow
            };
            await UpdateAsync(currentSeq);
        }

        return currentSeq;
    }

    public async Task<int> GetMaxSequenceNumber()
    {
        return (await _dataAccess.LoadDataAsync<int, dynamic>("spGetMaxSequenceNumber", new { }, "WSIWOT")).SingleOrDefault();
    }
}

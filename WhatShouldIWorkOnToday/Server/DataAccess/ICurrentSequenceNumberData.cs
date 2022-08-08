using WhatShouldIWorkOnToday.Server.Models;

namespace WhatShouldIWorkOnToday.Server.DataAccess;
public interface ICurrentSequenceNumberData
{
    Task<CurrentSequenceNumber> GetAsync();
    Task<int> GetMaxSequenceNumber();
    Task UpdateAsync(CurrentSequenceNumber currentSequenceNumber);
}
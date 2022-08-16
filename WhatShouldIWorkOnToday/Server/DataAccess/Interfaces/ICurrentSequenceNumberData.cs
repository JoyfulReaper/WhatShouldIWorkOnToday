using WhatShouldIWorkOnToday.Server.Models;

namespace WhatShouldIWorkOnToday.Server.DataAccess.Interfaces;
public interface ICurrentSequenceNumberData
{
    Task<CurrentSequenceNumber> GetAsync();
    Task<int> GetMaxSequenceNumber();
    Task UpdateAsync(CurrentSequenceNumber currentSequenceNumber);
}
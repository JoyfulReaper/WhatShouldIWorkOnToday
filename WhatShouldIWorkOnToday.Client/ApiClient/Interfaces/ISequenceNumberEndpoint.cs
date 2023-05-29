using WhatShouldIWorkOnToday.Client.Models;

namespace WhatShouldIWorkOnToday.Client.ApiClient.Interfaces;
public interface ISequenceNumberEndpoint
{
    Task<CurrentSequenceNumber> Get();
    Task<int> GetMaxSequenceNumber();
    Task Put(CurrentSequenceNumber currentSequenceNumber);
}
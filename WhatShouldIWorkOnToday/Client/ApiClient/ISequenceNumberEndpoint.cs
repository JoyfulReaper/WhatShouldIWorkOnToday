using WhatShouldIWorkOnToday.Client.Models;

namespace WhatShouldIWorkOnToday.Client.ApiClient;
public interface ISequenceNumberEndpoint
{
    Task<CurrentSequenceNumber> Get();
    Task<int> GetMaxSequenceNumber();
    Task Put(CurrentSequenceNumber currentSequenceNumber);
}
using WhatShouldIWorkOnToday.Client.Models;

namespace WhatShouldIWorkOnToday.Client.ApiClient;
public interface ISequenceNumberEndpoint
{
    Task<CurrentSequenceNumber> Get();
    Task Put(CurrentSequenceNumber currentSequenceNumber);
}
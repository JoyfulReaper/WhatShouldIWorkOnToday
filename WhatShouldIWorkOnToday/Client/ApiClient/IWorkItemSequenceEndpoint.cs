using WhatShouldIWorkOnToday.Client.Models;

namespace WhatShouldIWorkOnToday.Client.ApiClient;
public interface IWorkItemSequenceEndpoint
{
    Task<List<WorkItemSequence>> GetAllAsync();
    Task<WorkSequenceNumber> PostAsync(WorkSequenceNumber workSequence);
}
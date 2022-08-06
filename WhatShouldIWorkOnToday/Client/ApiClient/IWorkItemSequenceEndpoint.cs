using WhatShouldIWorkOnToday.Client.Models;

namespace WhatShouldIWorkOnToday.Client.ApiClient;
public interface IWorkItemSequenceEndpoint
{
    Task<List<WorkItemSequence>> GetAllWorkItemSequenceAsync();
    Task<WorkItemSequence> GetWorkItemSequenceAsync(int id);
    Task<WorkSequenceNumber> PostAsync(WorkSequenceNumber workSequence);
    Task PutAsync(WorkSequenceNumber workSequenceNumber);
}
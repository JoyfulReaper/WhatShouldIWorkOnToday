using WhatShouldIWorkOnToday.Server.DTOs;
using WhatShouldIWorkOnToday.Server.Models;

namespace WhatShouldIWorkOnToday.Server.DataAccess.Interfaces;
public interface IWorkSequenceNumberData
{
    Task<WorkItemSequenceDto?> GetWorkItemSequenceAsync(int id);
    Task<List<WorkItemSequenceDto>> GetAllWorkItemSequenceAsync();
    Task SaveAsync(WorkSequenceNumber workSequenceNumber);
}
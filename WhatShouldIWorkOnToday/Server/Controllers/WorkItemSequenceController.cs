using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WhatShouldIWorkOnToday.Server.DataAccess;
using WhatShouldIWorkOnToday.Server.DTOs;
using WhatShouldIWorkOnToday.Server.Models;

namespace WhatShouldIWorkOnToday.Server.Controllers;
[Route("api/[controller]")]
[ApiController]
public class WorkItemSequenceController : ControllerBase
{
	private readonly IWorkItemSequenceData _workItemSequenceData;

	public WorkItemSequenceController(IWorkItemSequenceData workItemSequenceData)
	{
		_workItemSequenceData = workItemSequenceData;
	}

	[HttpGet]
	public async Task<List<WorkItemSequence>> GetAll()
	{
		return await _workItemSequenceData.GetAllAsync();
	}

	[HttpPost]
	public async Task<ActionResult<WorkItemSequence>> Post([FromBody] WorkItemSequenceDto workItemSequenceDto)
	{
		var workItemSequence = new WorkItemSequence()
		{
			WorkSequenceNumberId = workItemSequenceDto.WorkSequenceNumberId,
			WorkItemId = workItemSequenceDto.WorkItemId,
			SequenceNumber = workItemSequenceDto.SequenceNumber
		};

		await _workItemSequenceData.SaveAsync(workItemSequence);

        var savedItem = await _workItemSequenceData.GetAsync(workItemSequence.WorkSequenceNumberId);
        if (savedItem is null)
        {
            return BadRequest();
        }

		return savedItem;
    }
}

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
	private readonly IWorkSequenceNumberData _workSequenceNumber;

	public WorkItemSequenceController(IWorkSequenceNumberData workItemSequenceData)
	{
		_workSequenceNumber = workItemSequenceData;
	}
    
    [HttpGet("{workSequenceNumberId}")]
    public async Task<ActionResult<WorkItemSequenceDto>> Get(int workSequenceNumberId)
    {
        var wis =  await _workSequenceNumber.GetWorkItemSequenceAsync(workSequenceNumberId);
        if(wis is null)
        {
            return NotFound();
        }
        else
        {
            return wis;
        }
    }

    [HttpGet]
	public async Task<List<WorkItemSequenceDto>> GetAll()
	{
		return await _workSequenceNumber.GetAllWorkItemSequenceAsync();
	}

	[HttpPost]
	public async Task<ActionResult<WorkSequenceNumber>> Post([FromBody] WorkSequenceNumber workSequenceNumber)
	{
		await _workSequenceNumber.SaveAsync(workSequenceNumber);
		return workSequenceNumber;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, WorkSequenceNumber workSequenceNumber)
    {
        if (id != workSequenceNumber.WorkItemId)
        {
            return BadRequest();
        }

        await _workSequenceNumber.SaveAsync(workSequenceNumber);

        return NoContent();
    }
}

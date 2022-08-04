using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WhatShouldIWorkOnToday.Client.Pages.WorkItems;
using WhatShouldIWorkOnToday.Server.DataAccess;
using WhatShouldIWorkOnToday.Shared.Models;

namespace WhatShouldIWorkOnToday.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WorkItemController : ControllerBase
{
	private readonly IWorkItemData _workItemData;

	public WorkItemController(IWorkItemData workItemData)
	{
		_workItemData = workItemData;
	}
    
    [HttpGet]
    public async Task<List<WorkItem>> Get()
    {
        return await _workItemData.GetAll();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WorkItem>> Get(int id)
    {
        var workItem = await _workItemData.Get(id);
        if(workItem is null)
        {
            return NotFound();
        }

        return workItem;
    }

    [HttpPost]
    public async Task<ActionResult<WorkItem>> Post([FromBody] WorkItem workItem)
    {
        await _workItemData.Save(workItem);
        var savedItem = await _workItemData.Get(workItem.WorkItemId);
        if(savedItem is null)
        {
            return BadRequest();
        }

        return savedItem;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, WorkItem workItem)
    {
        if (id != workItem.WorkItemId)
        {
            return BadRequest();
        }

        var workItemDb = await _workItemData.Get(id);
        if(workItemDb is null)
        {
            return NotFound();
        }

        workItemDb.Name = workItem.Name;
        workItemDb.Description = workItem.Description;
        workItemDb.Url = workItem.Url;
        workItemDb.DateWorkedOn = workItem.DateWorkedOn;
        workItemDb.DateDeleted = workItem.DateDeleted;
        workItemDb.DateCompleted = workItem.DateCompleted;

        await _workItemData.Save(workItemDb);

        return NoContent();
    }
}

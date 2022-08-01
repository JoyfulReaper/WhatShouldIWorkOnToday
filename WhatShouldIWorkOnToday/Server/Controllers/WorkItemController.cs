using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

    [HttpPost]
    public async Task<WorkItem> Post([FromBody] WorkItem workItem)
    {
        await _workItemData.Save(workItem);
        return workItem;
    }
}

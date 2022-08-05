using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WhatShouldIWorkOnToday.Server.Models;
using WhatShouldIWorkOnToday.Server.DataAccess;
using WhatShouldIWorkOnToday.Server.DTOs;

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
    public async Task<List<WorkItemDto>> GetAll()
    {
        return (await _workItemData.GetAllAsync())
            .Select(x => WorkItemToDto(x))
            .ToList();
    }

    [HttpGet("Complete")]
    public async Task<List<WorkItemDto>> GetAllCompleted()
    {
        return (await _workItemData.GetCompleteAsync())
            .Select(x => WorkItemToDto(x))
            .ToList();
    }

    [HttpGet("Incomplete")]
    public async Task<List<WorkItemDto>> GetAllIncomplete()
    {
        return (await _workItemData.GetIncompleteAsync())
            .Select(x => WorkItemToDto(x))
            .ToList();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WorkItemDto>> Get(int id)
    {
        var workItem = await _workItemData.GetAsync(id);
        if (workItem is null)
        {
            return NotFound();
        }

        return WorkItemToDto(workItem);
    }

    [HttpPost]
    public async Task<ActionResult<WorkItemDto>> Post([FromBody] WorkItemDto workItemDto)
    {
        var workItem = DtoToWorkItem(workItemDto);
        
        await _workItemData.SaveAsync(workItem);
        var savedItem = await _workItemData.GetAsync(workItem.WorkItemId);
        if (savedItem is null)
        {
            return BadRequest();
        }

        return WorkItemToDto(savedItem);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, WorkItemDto workItem)
    {
        if (id != workItem.WorkItemId)
        {
            return BadRequest();
        }

        var workItemDb = await _workItemData.GetAsync(id);
        if (workItemDb is null)
        {
            return NotFound();
        }

        workItemDb.Name = workItem.Name;
        workItemDb.Description = workItem.Description;
        workItemDb.Url = workItem.Url;
        workItemDb.DateWorkedOn = workItem.DateWorkedOn;
        workItemDb.DateCompleted = workItem.DateCompleted;

        await _workItemData.SaveAsync(workItemDb);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var workItem = await _workItemData.GetAsync(id);
        if (workItem is null)
        {
            return NotFound();
        }

        workItem.DateDeleted = DateTime.UtcNow;
        await _workItemData.SaveAsync(workItem);

        return NoContent();
    }

    private WorkItemDto WorkItemToDto(WorkItem workItem)
    {
        var dto = new WorkItemDto()
        {
            WorkItemId = workItem.WorkItemId,
            Name = workItem.Name,
            Description = workItem.Description,
            Url = workItem.Url,
            DateCreated = workItem.DateCreated,
            DateCompleted = workItem.DateCompleted
        };

        return dto;
    }

    private WorkItem DtoToWorkItem(WorkItemDto dto)
    {
        var workItem = new WorkItem()
        {
            WorkItemId = dto.WorkItemId,
            Name = dto.Name,
            Description = dto.Description,
            Url = dto.Url,
            DateCreated = dto.DateCreated,
            DateWorkedOn = dto.DateWorkedOn,
            DateCompleted = dto.DateCompleted
        };

        return workItem;
    }
}

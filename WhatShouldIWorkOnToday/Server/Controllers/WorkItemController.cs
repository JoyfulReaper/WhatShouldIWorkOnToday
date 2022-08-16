using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WhatShouldIWorkOnToday.Server.Models;
using WhatShouldIWorkOnToday.Server.DTOs;
using WhatShouldIWorkOnToday.Server.Authentication;
using System.Web;
using AutoMapper;
using WhatShouldIWorkOnToday.Server.DataAccess.Interfaces;

namespace WhatShouldIWorkOnToday.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
[BasicAuthorization]
public class WorkItemController : ControllerBase
{
    private readonly IWorkItemData _workItemData;
    private readonly ICurrentSequenceNumberData _currentSequenceNumberData;
    private readonly IMapper _mapper;

    public WorkItemController(IWorkItemData workItemData,
        ICurrentSequenceNumberData currentSequenceNumberData,
        IMapper mapper)
    {
        _workItemData = workItemData;
        _currentSequenceNumberData = currentSequenceNumberData;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<List<WorkItemDto>> GetAll()
    {
        return (await _workItemData.GetAllAsync())
            .Select(x => _mapper.Map<WorkItemDto>(x))
            .ToList();
    }

    [HttpGet("Complete")]
    public async Task<List<WorkItemDto>> GetAllCompleted()
    {
        return (await _workItemData.GetCompleteAsync())
            .Select(x => _mapper.Map<WorkItemDto>(x))
            .ToList();
    }

    [HttpGet("Current")]
    public async Task<List<WorkItemDto>> GetCurrent()
    {
        var seqNum = await _currentSequenceNumberData.GetAsync();

        return (await _workItemData.GetBySequeunceNumber(seqNum.CurrentSequence))
            .Select(x => _mapper.Map<WorkItemDto>(x))
            .ToList();
    }

    [HttpGet("Incomplete")]
    public async Task<List<WorkItemDto>> GetAllIncomplete()
    {
        return (await _workItemData.GetIncompleteAsync())
            .Select(x => _mapper.Map<WorkItemDto>(x))
            .ToList();
    }

    [HttpPost("UpdateWorkedOn")]
    public async Task<ActionResult<WorkItem>> UpdateWorkedOn([FromBody]int Id)
    {
        var workItem = await _workItemData.GetAsync(Id);
        if (workItem == null)
        {
            return NotFound();
        }

        workItem.DateWorkedOn = DateTime.UtcNow;
        await _workItemData.SaveAsync(workItem);

        return workItem;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WorkItemDto>> Get(int id)
    {
        var workItem = await _workItemData.GetAsync(id);
        if (workItem is null)
        {
            return NotFound();
        }

        return _mapper.Map<WorkItemDto>(workItem);
    }

    [HttpPost]
    public async Task<ActionResult<WorkItemDto>> Post([FromBody] WorkItemDto workItemDto)
    {
        var workItem = _mapper.Map<WorkItem>(workItemDto);
        workItem.DateWorkedOn = null;

        await _workItemData.SaveAsync(workItem);
        var savedItem = await _workItemData.GetAsync(workItem.WorkItemId);
        if (savedItem is null)
        {
            return BadRequest();
        }
        
        return CreatedAtAction(nameof(Get), new { id = savedItem.WorkItemId }, _mapper.Map<WorkItemDto>(savedItem));
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

    [HttpGet("Search")]
    public async Task <IEnumerable<WorkItemDto>> Search(string term)
    {
        var decodeTerm = HttpUtility.UrlDecode(term);
        var items = await _workItemData.GetAllAsync();
        var matchingItems = items
            .Where(i => i.Name
                .ToUpper()
                .Contains(term.ToUpper()) && i.DateDeleted is null)
            .ToList();

        return _mapper.Map<IEnumerable<WorkItemDto>>(matchingItems);
    }
}
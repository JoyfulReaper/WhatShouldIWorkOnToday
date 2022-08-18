using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WhatShouldIWorkOnToday.Server.DataAccess.Interfaces;
using WhatShouldIWorkOnToday.Server.DTOs;
using WhatShouldIWorkOnToday.Server.Models;

namespace WhatShouldIWorkOnToday.Server.Controllers;
[Route("api/[controller]")]
[ApiController]
public class PinnedWorkItemController : ControllerBase
{
    private readonly IPinnedWorkItemData _pinnedWorkItemData;
    private readonly IMapper _mapper;

    public PinnedWorkItemController(IPinnedWorkItemData pinnedWorkItemData,
        IMapper mapper)
    {
        _pinnedWorkItemData = pinnedWorkItemData;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<List<PinnedWorkItemDto>> GetAll()
    {
        var pinned = await _pinnedWorkItemData.GetAllAsync();
        return pinned.Select(p => _mapper.Map<PinnedWorkItemDto>(p))
            .ToList();
    }

    [HttpGet("Pinned")] 
    public async Task<List<WorkItem>> GetAllPinnedWorkItems()
    {
        return await _pinnedWorkItemData.GetPinnedWorkItems();
    }

    [HttpPost("PinWorkItem/{workItemId}")]
    public async Task<IActionResult> Pin(int workItemId)
    {
        await _pinnedWorkItemData.Pin(workItemId);
        return NoContent();
    }
    
    [HttpPost("UnpinWorkItem/{workItemId}")]
    public async Task<IActionResult> Unpin(int workItemId)
    {
        await _pinnedWorkItemData.Unpin(workItemId);
        return NoContent();
    }
}

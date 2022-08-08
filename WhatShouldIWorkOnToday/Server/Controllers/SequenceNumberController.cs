using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WhatShouldIWorkOnToday.Server.DataAccess;
using WhatShouldIWorkOnToday.Server.Models;

namespace WhatShouldIWorkOnToday.Server.Controllers;
[Route("api/[controller]")]
[ApiController]
public class SequenceNumberController : ControllerBase
{
	private readonly ICurrentSequenceNumberData _currentSequenceNumberData;

	public SequenceNumberController(ICurrentSequenceNumberData currentSequenceNumberData)
	{
		_currentSequenceNumberData = currentSequenceNumberData;
	}
    
	[HttpGet]
    public async Task<ActionResult<CurrentSequenceNumber>> Get()
	{
		var curSeq = await _currentSequenceNumberData.GetAsync();
        if (curSeq is null)
        {
            return NotFound();
        }
        else
        {
            return curSeq;
        }
    }

    [HttpGet("MaxSequence")]
    public async Task<int> GetMaxSequence()
    {
        return await _currentSequenceNumberData.GetMaxSequenceNumber();
    }

    [HttpPut]
    public async Task<IActionResult> Put([FromBody] CurrentSequenceNumber currentSequenceNumber)
    {
        await _currentSequenceNumberData.UpdateAsync(currentSequenceNumber);
        return NoContent();
    }
}

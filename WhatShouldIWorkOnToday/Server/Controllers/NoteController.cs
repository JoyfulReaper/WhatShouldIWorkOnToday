using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WhatShouldIWorkOnToday.Server.Authentication;
using WhatShouldIWorkOnToday.Server.DataAccess;
using WhatShouldIWorkOnToday.Server.Models;

namespace WhatShouldIWorkOnToday.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
[BasicAuthorization]
public class NoteController : ControllerBase
{
	private readonly INoteData _noteData;

	public NoteController(INoteData noteData)
	{
		_noteData = noteData;
	}

    [HttpGet("{workItemId}")]
    public async Task<List<Note>> Get(int workItemId)
    {
        return await _noteData.GetByWorkItemAsync(workItemId);
    }

    [HttpPost]
    public async Task<ActionResult<Note>> Post(Note note)
    {
        await _noteData.SaveAsync(note);
        var savedNote = await _noteData.GetAsync(note.NoteId);

        if(savedNote is null)
        {
            return BadRequest();
        }

        return CreatedAtAction(nameof(Get), new { workItemId = note.WorkItemId }, savedNote);
    }
}

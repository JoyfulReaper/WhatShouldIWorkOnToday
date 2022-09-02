using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WhatShouldIWorkOnToday.Server.Models;
using WhatShouldIWorkOnToday.Server.DataAccess.Interfaces;

namespace WhatShouldIWorkOnToday.Server.Controllers;
[Route("api/[controller]")]
[ApiController]
public class TodoItemController : ControllerBase
{
	private readonly ITodoItemData _todoItemData;

	public TodoItemController(ITodoItemData todoItemData)
	{
		_todoItemData = todoItemData;
	}

	[HttpGet("{workItemId}")]
	public async Task<IEnumerable<TodoItem>> Get(int workItemId)
	{
		return await _todoItemData.GetAll(workItemId);
	}

	[HttpPost]
	public async Task<ActionResult<TodoItem>> Post(TodoItem todoItem)
	{
		await _todoItemData.Save(todoItem);
		var savedItem = await _todoItemData.Get(todoItem.TodoItemId);
		if(savedItem == null)
		{
			return BadRequest();
		}

		return savedItem;
	}

	[HttpPost("Complete/{todoItemId}")]
	public async Task<IActionResult> Completed(int todoItemId)
	{
		var item = await _todoItemData.Get(todoItemId);
		if(item is null)
		{
			return NotFound();
		}
		item.DateCompleted = DateTime.Now;
		await _todoItemData.Save(item);

		return NoContent();
	}

    [HttpPost("UnComplete/{todoItemId}")]
    public async Task<IActionResult> UnComplete(int todoItemId)
    {
        var item = await _todoItemData.Get(todoItemId);
        if (item is null)
        {
            return NotFound();
        }
        item.DateCompleted = null;
        await _todoItemData.Save(item);

        return NoContent();
    }

}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WhatShouldIWorkOnToday.Server.Models;
using WhatShouldIWorkOnToday.Server.DataAccess.Interfaces;
using WhatShouldIWorkOnToday.Server.DTOs;

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

    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TodoItem>))]
    [HttpGet("{workItemId}")]
	public async Task<IEnumerable<TodoItem>> Get(int workItemId)
	{
		return await _todoItemData.GetAll(workItemId);
	}

	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status200OK, Type=typeof(TodoItem))]
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

    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
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

    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
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

	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[HttpPut("Edit/{todoItemId}")]
	public async Task<IActionResult> Edit(TodoItemEditRequest editRequest)
	{
		try
		{
			var todoItem = await _todoItemData.Get(editRequest.TodoItemId);
			if (todoItem is null)
			{
				return NotFound();
			}

			todoItem.Task = editRequest.Task;
			await _todoItemData.Save(todoItem);
			return NoContent();
		}
		catch
		{
			return StatusCode(StatusCodes.Status500InternalServerError);
		}
	}

    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HttpDelete("{todoItemId}")]
	public async Task<IActionResult> Delete(int todoItemId)
	{
		try
		{
			var item = await _todoItemData.Get(todoItemId);
			if(item is null)
			{
				return NotFound();
			}
			await _todoItemData.Delete(todoItemId);
			return NoContent();
		}
		catch (Exception ex)
		{

            return StatusCode(StatusCodes.Status500InternalServerError);
        }
	}
}

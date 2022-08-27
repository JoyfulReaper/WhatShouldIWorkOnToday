using WhatShouldIWorkOnToday.Server.DataAccess.Interfaces;
using WhatShouldIWorkOnToday.Server.Models;

namespace WhatShouldIWorkOnToday.Server.DataAccess;

public class TodoItemData : ITodoItemData
{
    private readonly IDataAccess _dataAccess;

    public TodoItemData(IDataAccess dataAccess)
    {
        _dataAccess = dataAccess;
    }

    public async Task Save(TodoItem todoItem)
    {
        var id = await _dataAccess.SaveDataAndGetIdAsync("spToDoItem_Upsert", new
        {
            TodoItemId = todoItem.TodoItemId,
            WorkItemId = todoItem.WorkItemId,
            Task = todoItem.Task,
            DateCompleted = todoItem.DateCompleted
        }, "WSIWOT");

        todoItem.TodoItemId = id;
    }

    public async Task<IEnumerable<TodoItem>> GetAll(int workItemId)
    {
        return await _dataAccess.LoadDataAsync<TodoItem, dynamic>("spTodoItems_GetAll", new { workItemId }, "WSIWOT");
    }

    public async Task<TodoItem?> Get(int todoItemId)
    {
        return (await _dataAccess.LoadDataAsync<TodoItem, dynamic>("spTodoItem_Get", new { todoItemId }, "WSIWOT"))
            .SingleOrDefault();
    }
}

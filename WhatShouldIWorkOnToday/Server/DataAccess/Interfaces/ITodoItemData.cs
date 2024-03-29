﻿using WhatShouldIWorkOnToday.Server.Models;

namespace WhatShouldIWorkOnToday.Server.DataAccess.Interfaces;
public interface ITodoItemData
{
    Task Delete(int todoItemId);
    Task<TodoItem?> Get(int todoItemId);
    Task<IEnumerable<TodoItem>> GetAll(int workItemId);
    Task Save(TodoItem todoItem);
}
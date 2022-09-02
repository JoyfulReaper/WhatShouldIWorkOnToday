using WhatShouldIWorkOnToday.Client.Models;

namespace WhatShouldIWorkOnToday.Client.ApiClient.Interfaces;
public interface ITodoItemEndpoint
{
    Task CompleteAsync(int todoItemId);
    Task<IEnumerable<TodoItem>> GetAsync(int workItemId);
    Task<TodoItem> PostAsync(TodoItem item);
    Task UnCompleteAsync(int todoItemId);
}
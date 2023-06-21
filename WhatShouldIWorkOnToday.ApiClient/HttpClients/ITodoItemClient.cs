using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatShouldIWorkOnToday.ApiClient.Contracts;

namespace WhatShouldIWorkOnToday.ApiClient.HttpClients;
public interface ITodoItemClient
{
    Task<ToDoItem?> GetAsync(int todoItemId, CancellationToken cancellationToken = default);
    Task<List<ToDoItem>> GetByWorkItemAsync(int workItemId, CancellationToken cancellationToken = default);
    Task<ToDoItem> CreateAsync(ToDoItemRequest todoItemrequest, CancellationToken cancellationToken = default);
    Task CompleteAsync(int todoItemId, CancellationToken cancellationToken = default);
    Task UncompleteAsync(int todoItemId, CancellationToken cancellationToken = default);
    Task DeleteAsync(int todoItemId, CancellationToken cancellationToken = default);
    Task<ToDoItem> UpdateAsync(ToDoItem todoItem, CancellationToken cancellationToken = default);
}

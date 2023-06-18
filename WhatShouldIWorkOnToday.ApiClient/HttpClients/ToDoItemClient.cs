using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatShouldIWorkOnToday.ApiClient.Contracts;

namespace WhatShouldIWorkOnToday.ApiClient.HttpClients;
public class ToDoItemClient : ITodoItemClient
{
    public Task CompleteAsync(int todoItemId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ToDoItem> CreateAsync(ToDoItemRequest todoItemrequest, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ToDoItem?> GetAsync(int todoItemId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<ToDoItem>> GetByWorkItemAsync(int workItemId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

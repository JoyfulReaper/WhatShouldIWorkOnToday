namespace WhatShouldIWorkOnToday.Api;

public sealed record BulkCreateTodosRequest(
    IReadOnlyList<BulkCreateTodoItemRequest>? Items);

public sealed record BulkCreateTodoItemRequest(
    int WorkItemId,
    string Task,
    string? Energy = null,
    string? Effort = null,
    string? Priority = null);

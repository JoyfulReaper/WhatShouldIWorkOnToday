namespace WhatShouldIWorkOnToday.Api;

public sealed record BulkCreateWorkItemsRequest(
    IReadOnlyList<BulkCreateWorkItemRequest>? Items);

public sealed record BulkCreateWorkItemRequest(
    string Name,
    string? Kind = null,
    string? Description = null,
    string? Url = null,
    IReadOnlyList<CreateTodoRequest>? Todos = null);
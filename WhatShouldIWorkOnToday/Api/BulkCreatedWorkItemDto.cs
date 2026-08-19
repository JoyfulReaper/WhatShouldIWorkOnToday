namespace WhatShouldIWorkOnToday.Api;

public sealed record BulkCreatedWorkItemDto(
    WorkItemDto WorkItem,
    IReadOnlyList<TodoItemDto> Todos);
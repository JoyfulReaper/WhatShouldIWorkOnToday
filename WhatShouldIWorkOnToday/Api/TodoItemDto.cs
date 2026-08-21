namespace WhatShouldIWorkOnToday.Api;

public sealed record TodoItemDto(
    int Id,
    int WorkItemId,
    string WorkItemName,
    string Task,
    string Energy,
    string Effort,
    string Priority,
    DateTimeOffset CreatedAt,
    DateTimeOffset? CompletedAt);

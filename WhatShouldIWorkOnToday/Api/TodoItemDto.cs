namespace WhatShouldIWorkOnToday.Api;

public sealed record TodoItemDto(
    int Id,
    int WorkItemId,
    string WorkItemName,
    string Task,
    string Energy,
    string Effort,
    DateTimeOffset CreatedAt,
    DateTimeOffset? CompletedAt);
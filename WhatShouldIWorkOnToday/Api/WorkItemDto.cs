namespace WhatShouldIWorkOnToday.Api;

public sealed record WorkItemDto(
    int Id,
    string Name,
    string? Description,
    string? Url,
    string Kind,
    DateTimeOffset CreatedAt,
    DateTimeOffset? LastWorkedAt,
    DateTimeOffset? CompletedAt,
    DateTimeOffset? ArchivedAt,
    int TodoCount,
    int ActiveTodoCount);
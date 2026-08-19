namespace WhatShouldIWorkOnToday.Api;

public sealed record DailyPickDto(
    DateOnly Date,
    TodoItemDto Todo,
    DateTimeOffset? LastWorkedAt);
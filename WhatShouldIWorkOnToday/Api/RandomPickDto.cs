namespace WhatShouldIWorkOnToday.Api;

public sealed record RandomPickDto(
    TodoItemDto Todo,
    bool FavorPriority);

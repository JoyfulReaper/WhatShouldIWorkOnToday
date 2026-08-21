namespace WhatShouldIWorkOnToday.Api;

public sealed record CreateWorkItemRequest(
    string Name,
    string? Kind = null,
    string? Description = null,
    string? Url = null);

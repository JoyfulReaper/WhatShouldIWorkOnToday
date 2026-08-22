namespace WhatShouldIWorkOnToday.Api;

public sealed record RenameWorkItemRequest(
    string? Name);

public sealed record RenameWorkItemResponse(
    int Id,
    string Name);
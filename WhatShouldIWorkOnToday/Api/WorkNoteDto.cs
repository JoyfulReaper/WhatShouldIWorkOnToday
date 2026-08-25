namespace WhatShouldIWorkOnToday.Api;

public sealed record WorkNoteDto(
    int Id,
    int WorkItemId,
    string WorkItemName,
    int? TodoItemId,
    string? TaskSnapshot,
    string? Note,
    DateTimeOffset WorkedAt);
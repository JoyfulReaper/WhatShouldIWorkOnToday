namespace WhatShouldIWorkOnToday.Api;

public sealed record CreateWorkNoteRequest(
    string? Note,
    int? TodoItemId = null);
namespace WhatShouldIWorkOnToday.Api;

public sealed record CreateTodoRequest(
    string Task,
    string? Energy = null,
    string? Effort = null);
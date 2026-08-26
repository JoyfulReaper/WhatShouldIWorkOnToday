namespace WhatShouldIWorkOnToday.Models;

public sealed class DailyPick
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public int TodoItemId { get; set; }
    public TodoItem TodoItem { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
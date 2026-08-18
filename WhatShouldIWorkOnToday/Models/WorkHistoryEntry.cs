using System.ComponentModel.DataAnnotations;

namespace WhatShouldIWorkOnToday.Models;

public sealed class WorkHistoryEntry
{
    public int Id { get; set; }

    public int WorkItemId { get; set; }

    public WorkItem WorkItem { get; set; } = null!;

    public int? TodoItemId { get; set; }

    [MaxLength(500)]
    public string? TaskSnapshot { get; set; }

    public DateTimeOffset WorkedAt { get; set; } = DateTimeOffset.UtcNow;
}
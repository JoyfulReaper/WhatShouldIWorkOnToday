using System.ComponentModel.DataAnnotations;

namespace WhatShouldIWorkOnToday.Models;

public sealed class TodoItem
{
    public int Id { get; set; }

    public int WorkItemId { get; set; }

    public WorkItem WorkItem { get; set; } = null!;

    [Required]
    [MaxLength(500)]
    public string Task { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? CompletedAt { get; set; }
}
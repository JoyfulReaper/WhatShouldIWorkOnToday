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

    public EnergyLevel Energy { get; set; } = EnergyLevel.Medium;

    public EffortLevel Effort { get; set; } = EffortLevel.Medium;

    public PriorityLevel Priority { get; set; } = PriorityLevel.Normal;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? CompletedAt { get; set; }
}

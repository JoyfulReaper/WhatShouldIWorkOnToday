using System.ComponentModel.DataAnnotations;

namespace WhatShouldIWorkOnToday.Models;

public sealed class WorkItem
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [MaxLength(2048)]
    public string? Url { get; set; }

    public WorkItemKind Kind { get; set; } = WorkItemKind.Project;

    public EnergyLevel Energy { get; set; } = EnergyLevel.Medium;

    public EffortLevel Effort { get; set; } = EffortLevel.Medium;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? LastWorkedAt { get; set; }

    public DateTimeOffset? CompletedAt { get; set; }

    public DateTimeOffset? ArchivedAt { get; set; }

    public List<TodoItem> Todos { get; set; } = [];
}
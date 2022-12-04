using WhatShouldIWorkOnToday.Domain.Common;

namespace WhatShouldIWorkOnToday.Domain.WorkItem.Entities;

public sealed class Note : BaseEntity
{
    public int WorkItemId { get; set; }
    public string Text { get; set; } = null!;

    private Note(
        int workItemId, 
        string text)
    {
        WorkItemId = workItemId;
        Text = text;
    }

    public static Note Create(
        int workItemId,
        string text)
    {
        return new Note(
            workItemId,
            text);
    }

    // Navigation
    public WorkItem WorkItem { get; set; } = null!;
}

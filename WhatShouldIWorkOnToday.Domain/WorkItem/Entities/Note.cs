using WhatShouldIWorkOnToday.Domain.Common;

namespace WhatShouldIWorkOnToday.Domain.WorkItem.Entities;

public class Note : BaseEntity
{
    public int WorkItemId { get; set; }
    public string Text { get; set; } = null!;

    public WorkItem WorkItem { get; set; } = null!;
}

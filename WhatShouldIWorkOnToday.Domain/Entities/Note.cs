using WhatShouldIWorkOnToday.Domain.Common;

namespace WhatShouldIWorkOnToday.Domain.Entities;

public class Note : BaseAuditableEntity
{
    public int WorkItemId { get; set; }
    public string Text { get; set; } = null!;

    public WorkItem WorkItem { get; set; } = null!;
}

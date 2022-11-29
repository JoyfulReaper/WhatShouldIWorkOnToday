
using WhatShouldIWorkOnToday.Domain.Common;

namespace WhatShouldIWorkOnToday.Domain.Entities;

public class ToDoItem : BaseAuditableEntity
{
    public int WorkItemId { get; set; }

    public string Task { get; set; } = null!;
    public DateTime? DateCompleted { get; set; }
    public DateTime? DateDeleted { get; set; }

    public WorkItem WorkItem { get; set; } = null!;
}

using WhatShouldIWorkOnToday.Domain.Common;

namespace WhatShouldIWorkOnToday.Domain.Entities;

public class WorkItem : BaseAuditableEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Url { get; set; }
    public bool Pinned { get; set; }
    public int SequenceNumber { get; set; }
    public DateTime? LastDateWorkedOn { get; set; }
    public DateTime? DateCompleted { get; set; }

    public IList<Note> Notes { get; private set;} = new List<Note>();
    public IList<ToDoItem> TodoItems { get; private set; } = new List<ToDoItem>();
    public IList<WorkItemHistory> WorkItemHistories { get; private set; } = new List<WorkItemHistory>();
}

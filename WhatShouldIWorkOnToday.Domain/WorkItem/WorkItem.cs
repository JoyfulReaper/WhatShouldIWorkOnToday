using WhatShouldIWorkOnToday.Domain.Common;
using WhatShouldIWorkOnToday.Domain.WorkItem.Entities;

namespace WhatShouldIWorkOnToday.Domain.WorkItem;

public class WorkItem : AggregateRoot
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Url { get; set; }
    public bool Pinned { get; set; }
    public int SequenceNumber { get; set; }
    public DateTime? LastDateWorkedOn { get; set; }
    public DateTime? DateCompleted { get; set; }

    public IList<Note> Notes { get; private set; } = new List<Note>();
    public IList<ToDoItem> TodoItems { get; private set; } = new List<ToDoItem>();
    public IList<WorkItemHistory> WorkItemHistories { get; private set; } = new List<WorkItemHistory>();
}

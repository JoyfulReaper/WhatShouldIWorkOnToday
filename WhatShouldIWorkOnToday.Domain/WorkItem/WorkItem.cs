using WhatShouldIWorkOnToday.Domain.Common;
using WhatShouldIWorkOnToday.Domain.WorkItem.Entities;

namespace WhatShouldIWorkOnToday.Domain.WorkItem;

public sealed class WorkItem : AggregateRoot
{

    private WorkItem(
        string name,
        string? description,
        string? url,
        bool pinned,
        int sequenceNumber,
        DateTime? lastDateWorkedOn,
        DateTime? dateCompleted,
        DateTime? dateDeleted
        )
    {
        Name = name;
        Description = description;
        Url = url;
        Pinned = pinned;
        SequenceNumber = sequenceNumber;
        LastDateWorkedOn = lastDateWorkedOn;
        DateCompleted = dateCompleted;
        DateDeleted = dateDeleted;
    }

    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Url { get; set; }
    public bool Pinned { get; set; }
    public int SequenceNumber { get; set; }
    public DateTime? LastDateWorkedOn { get; set; }
    public DateTime? DateCompleted { get; set; }
    public DateTime? DateDeleted = null;


    public static WorkItem Create(
        string name,
        string? description,
        string? url,
        bool pinned,
        int sequenceNumber,
        DateTime? lastDateWorkedOn,
        DateTime? dateCompleted,
        DateTime? dateDeleted
        )
    {
        return new WorkItem(
            name,
            description,
            url,
            pinned,
            sequenceNumber,
            lastDateWorkedOn,
            dateCompleted,
            dateDeleted
            );
    }

    // Navigation
    public IList<Note> Notes { get; private set; } = new List<Note>();
    public IList<ToDoItem> TodoItems { get; private set; } = new List<ToDoItem>();
    public IList<WorkItemHistory> WorkItemHistories { get; private set; } = new List<WorkItemHistory>();
}

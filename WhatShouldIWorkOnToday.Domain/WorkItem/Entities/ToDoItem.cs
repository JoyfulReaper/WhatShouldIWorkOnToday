using WhatShouldIWorkOnToday.Domain.Common;

namespace WhatShouldIWorkOnToday.Domain.WorkItem.Entities;

public sealed class ToDoItem : BaseAuditableEntity
{
    public int WorkItemId { get; set; }
    public string Task { get; set; } = null!;
    public DateTime? DateCompleted { get; set; }
    public DateTime? DateDeleted { get; set; }

    private ToDoItem(
        int workItemId,
        string task,
        DateTime? dateCompleted,
        DateTime? dateDeleted) 
    { 
        WorkItemId = workItemId;
        Task = task;
        DateCompleted = dateCompleted;
        DateDeleted = dateDeleted;
    }


    public static ToDoItem Create(
        int workItemId,
        string task,
        DateTime? dateCompleted,
        DateTime? dateDeleted)
    {
        return new ToDoItem(
            workItemId,
            task,
            dateCompleted,
            dateDeleted
            );
    }

    // Navigation
    public WorkItem WorkItem { get; set; } = null!;
}

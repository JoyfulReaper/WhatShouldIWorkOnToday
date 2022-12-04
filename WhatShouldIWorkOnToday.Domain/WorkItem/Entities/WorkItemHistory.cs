using WhatShouldIWorkOnToday.Domain.Common;

namespace WhatShouldIWorkOnToday.Domain.WorkItem.Entities;

public sealed class WorkItemHistory : BaseEntity
{
    public int WorkItemId { get; set; }
    public DateTime DateWorkedOn { get; set; }

    private WorkItemHistory(
        int workItemId,
        DateTime dateWorkedOn)
    {
        WorkItemId = workItemId;
        DateWorkedOn = dateWorkedOn;
    }

    public static WorkItemHistory Create(
        int workItemId,
        DateTime dateWorkedOn)
    {
        return new WorkItemHistory(workItemId, dateWorkedOn);
    }

    // Navigation
    WorkItem WorkItem { get; set; } = null!;
}

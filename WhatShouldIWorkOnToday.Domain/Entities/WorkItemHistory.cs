using WhatShouldIWorkOnToday.Domain.Common;

namespace WhatShouldIWorkOnToday.Domain.Entities;

public class WorkItemHistory : BaseEntity
{
    public int WorkItemId { get; set; }
    public DateTime DateWorkedOn { get; set; }
}

using System;
using System.Collections.Generic;

namespace WhatShouldIWorkOnToday.Repository.Sql.Entities.Entities;

public partial class WorkItemHistory
{
    public int WorkItemHistoryId { get; set; }

    public int WorkItemId { get; set; }

    public DateTime DateWorkedOn { get; set; }

    public virtual WorkItem WorkItem { get; set; } = null!;
}

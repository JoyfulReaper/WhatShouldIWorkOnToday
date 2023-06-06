using System;
using System.Collections.Generic;

namespace WhatShouldIWorkOnToday.Repository.Sql.Entities.Entities;

public partial class WorkSequenceNumber
{
    public int WorkSequenceNumberId { get; set; }

    public int WorkItemId { get; set; }

    public int SequenceNumber { get; set; }

    public virtual WorkItem WorkItem { get; set; } = null!;
}

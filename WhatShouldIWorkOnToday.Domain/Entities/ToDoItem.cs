using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatShouldIWorkOnToday.Domain.Common;

namespace WhatShouldIWorkOnToday.Domain.Entities;
public class ToDoItem : BaseAuditableEntity
{
    public int WorkItemId;

    public string Task { get; set; } = null!;
    public DateTime? DateCompleted { get; set; }
    public DateTime? DateDeleted { get; set; }

    public WorkItem WorkItem { get; set; } = null!;
}

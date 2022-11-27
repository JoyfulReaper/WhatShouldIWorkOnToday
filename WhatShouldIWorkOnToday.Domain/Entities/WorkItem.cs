using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    public IList<TodoItem> Notes { get; private set;} = new List<TodoItem>();
    public IList<ToDoItem> TodoItems { get; private set; } = new List<ToDoItem>();
}

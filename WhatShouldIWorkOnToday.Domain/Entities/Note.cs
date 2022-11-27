using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatShouldIWorkOnToday.Domain.Common;

namespace WhatShouldIWorkOnToday.Domain.Entities;
public class TodoItem : BaseAuditableEntity
{
    public int WorkItemId { get; set; }
    public string Text { get; set; } = null!;

    public WorkItem WorkItem { get; set; } = null!;
}

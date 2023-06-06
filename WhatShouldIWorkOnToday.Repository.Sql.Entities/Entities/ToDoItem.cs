using System;
using System.Collections.Generic;

namespace WhatShouldIWorkOnToday.Repository.Sql.Entities.Entities;

public partial class ToDoItem
{
    public int ToDoItemId { get; set; }

    public int WorkItemId { get; set; }

    public string Task { get; set; } = null!;

    public DateTime DateAdded { get; set; }

    public DateTime? DateCompleted { get; set; }

    public DateTime? DateDeleted { get; set; }

    public virtual WorkItem WorkItem { get; set; } = null!;
}

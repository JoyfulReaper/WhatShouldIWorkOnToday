using System;
using System.Collections.Generic;

namespace WhatShouldIWorkOnToday.Repository.Sql.Entities.Entities;

public partial class WorkItem
{
    public int WorkItemId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Url { get; set; }

    public bool Pinned { get; set; }

    public int? SequenceNumber { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime? DateWorkedOn { get; set; }

    public DateTime? DateDeleted { get; set; }

    public DateTime? DateCompleted { get; set; }

    public virtual ICollection<Note> Notes { get; set; } = new List<Note>();

    public virtual ICollection<ToDoItem> ToDoItems { get; set; } = new List<ToDoItem>();

    public virtual ICollection<WorkItemHistory> WorkItemHistories { get; set; } = new List<WorkItemHistory>();
}

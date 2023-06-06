using System;
using System.Collections.Generic;

namespace WhatShouldIWorkOnToday.Repository.Sql.Entities.Entities;

public partial class Note
{
    public int NoteId { get; set; }

    public int WorkItemId { get; set; }

    public string Text { get; set; } = null!;

    public DateTime DateCreated { get; set; }

    public DateTime? DateDeleted { get; set; }

    public virtual WorkItem WorkItem { get; set; } = null!;
}

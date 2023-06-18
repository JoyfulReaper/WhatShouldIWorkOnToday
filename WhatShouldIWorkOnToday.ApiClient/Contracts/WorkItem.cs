using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatShouldIWorkOnToday.ApiClient.Contracts;
public class WorkItem
{
    public int WorkItemId { get; set; }

    [Required]
    public string Name { get; set; } = default!;

    public string? Description { get; set; }
    public string? Url { get; set; }
    public bool Pinned { get; set; }
    public int? SequenceNumber { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime? DateCompleted { get; set; }
    public DateTime? DateWorkedOn { get; set; }
}

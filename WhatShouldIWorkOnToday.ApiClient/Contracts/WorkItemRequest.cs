using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatShouldIWorkOnToday.ApiClient.Contracts;
public class WorkItemRequest
{
    [Required]
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string? Url { get; set; }
    public bool Pinned { get; set; }
}

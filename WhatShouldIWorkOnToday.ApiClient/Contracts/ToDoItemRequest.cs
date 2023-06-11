using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatShouldIWorkOnToday.ApiClient.Contracts;
public class ToDoItemRequest
{
    public int WorkItemId { get; set; }

    [Required]
    public string Task { get; set; } = default!;
}

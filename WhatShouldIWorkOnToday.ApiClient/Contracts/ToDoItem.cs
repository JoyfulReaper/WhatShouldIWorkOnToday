using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatShouldIWorkOnToday.ApiClient.Contracts;
public class ToDoItem
{
    public int ToDoItemId { get; set; }

    public int WorkItemId { get; set; }

    [Required]
    public string Task { get; set; } = default!;

    public DateTime DateAdded { get; set; }
    public DateTime? DateCompelted { get; set; }
}

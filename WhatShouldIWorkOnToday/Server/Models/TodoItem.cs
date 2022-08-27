using System.ComponentModel.DataAnnotations;

namespace WhatShouldIWorkOnToday.Server.Models;

public class TodoItem
{
    public int TodoItemId { get; set; }
    public int WorkItemId { get; set; }

    [MaxLength(300)]
    public string Task { get; set; } = null!;

    public DateTime DateAdded { get; set; }
    public DateTime? DateCompleted { get; set; }
}

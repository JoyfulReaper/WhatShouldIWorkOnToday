using System.ComponentModel.DataAnnotations;

namespace WhatShouldIWorkOnToday.Client.Models;

public class TodoItem
{
    public int TodoItemId { get; set; }
    public int WorkItemId { get; set; }

    [MaxLength(300)]
    public string Task { get; set; } = null!;

    public DateTime DateAdded { get; set; }
    public DateTime? DateCompleted { get; set; }
}
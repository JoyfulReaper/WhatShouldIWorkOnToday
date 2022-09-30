using System.ComponentModel.DataAnnotations;

namespace WhatShouldIWorkOnToday.Server.DTOs;

public class TodoItemEditRequest
{
    public int TodoItemId { get; set; }

    [Required]
    public string Task { get; set; } = default!;
}

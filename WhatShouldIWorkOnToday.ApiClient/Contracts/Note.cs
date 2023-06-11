using System.ComponentModel.DataAnnotations;

namespace WhatShouldIWorkOnToday.ApiClient.Contracts;

public class Note
{
    public int NoteId { get; set; }

    public int WorkItemId { get; set; }

    [Required]
    public string Text { get; set; } = default!;

    public DateTime Created { get; set; }
}
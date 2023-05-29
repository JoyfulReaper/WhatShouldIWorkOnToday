using System.ComponentModel.DataAnnotations;

namespace WhatShouldIWorkOnToday.Client.Models;

public class Note
{
    public int NoteId { get; set; }
    public int WorkItemId { get; set; }
    public string Text { get; set; } = null!;
    public DateTime DateCreated { get; set; }
}

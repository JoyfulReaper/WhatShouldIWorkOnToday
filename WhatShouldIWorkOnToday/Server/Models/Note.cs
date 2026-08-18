using System.ComponentModel.DataAnnotations;
using WhatShouldIWorkOnToday.Client.Shared.Components;

namespace WhatShouldIWorkOnToday.Server.Models;

public class Note
{
    public int NoteId { get; set; }
    public int WorkItemId { get; set; }
    [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$",
         ErrorMessage = "Characters are not allowed.")]
    public string Text { get; set; } = null!;
    public DateTime DateCreated { get; set; }
}

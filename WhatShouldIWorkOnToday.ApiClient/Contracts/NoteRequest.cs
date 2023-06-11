using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatShouldIWorkOnToday.ApiClient.Contracts;
public class NoteRequest
{
    public int WorkItemId { get; set; }

    [Required]
    public string Text { get; set; } = default!;
}

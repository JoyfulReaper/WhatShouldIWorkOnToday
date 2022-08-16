namespace WhatShouldIWorkOnToday.Server.Models;

public class PinnedWorkItem
{
    public int PinnedWorkItemId { get; set; }
    public int WorkItemId { get; set; }
    public DateTime DatePinned { get; set; }
    public DateTime? DateUnpinned { get; set; }
}

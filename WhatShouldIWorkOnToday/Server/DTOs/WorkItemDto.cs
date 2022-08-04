namespace WhatShouldIWorkOnToday.Server.DTOs;
public class WorkItemDto
{
    public int WorkItemId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Url { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime? DateWorkedOn { get; set; }
    public DateTime? DateCompleted { get; set; }
}

﻿namespace WhatShouldIWorkOnToday.Client.Models;

public class WorkItemHistory
{
    public int WorkItemHistoryId { get; set; }
    public int WorkItemId { get; set; }
    public DateTime DateWorkedOn { get; set; }
}
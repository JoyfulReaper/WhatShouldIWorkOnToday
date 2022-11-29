using WhatShouldIWorkOnToday.Domain.Common;

namespace WhatShouldIWorkOnToday.Domain.Entities;

public class WsiwotSettings : BaseEntity
{
    public int CurrentSequenceNumber { get; set; }
    public DateTime DateSet { get; set; }
}

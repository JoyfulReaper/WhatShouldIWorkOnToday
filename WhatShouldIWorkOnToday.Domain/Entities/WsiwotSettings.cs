using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatShouldIWorkOnToday.Domain.Common;

namespace WhatShouldIWorkOnToday.Domain.Entities;
public class WsiwotSettings : BaseEntity
{
    public int CurrentSequenceNumber { get; set; }
    public DateTime DateSet { get; set; }
}

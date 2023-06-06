using System;
using System.Collections.Generic;

namespace WhatShouldIWorkOnToday.Repository.Sql.Entities.Entities;

public partial class Setting
{
    public int SettingId { get; set; }

    public int CurrentSequence { get; set; }

    public DateTime DateSet { get; set; }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatShouldIWorkOnToday.Application.Common.Models;

public record User(string Id,
    string FirstName,
    string LastName,
    string Email);

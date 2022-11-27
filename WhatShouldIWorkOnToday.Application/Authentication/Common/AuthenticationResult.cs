
using WhatShouldIWorkOnToday.Application.Common.Models;

namespace WhatShouldIWorkOnToday.Application.Authentication.Common;
public record AuthenticationResult(User User, string Token);

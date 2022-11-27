using WhatShouldIWorkOnToday.Application.Common.Models;

namespace WhatShouldIWorkOnToday.Application.Common.Interfaces.Authentication;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}

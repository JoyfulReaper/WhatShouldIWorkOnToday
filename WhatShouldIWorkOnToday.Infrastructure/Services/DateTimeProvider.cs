using WhatShouldIWorkOnToday.Application.Common.Interfaces.Services;

namespace WhatShouldIWorkOnToday.Infrastructure.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}

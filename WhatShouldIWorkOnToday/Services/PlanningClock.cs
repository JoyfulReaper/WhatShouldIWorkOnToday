using Microsoft.Extensions.Options;
using WhatShouldIWorkOnToday.Services;

public sealed class PlanningClock(
    IOptions<PlanningOptions> options,
    TimeProvider timeProvider)
{
    private readonly TimeZoneInfo _timeZone =
        TimeZoneInfo.FindSystemTimeZoneById(
            options.Value.TimeZone);

    public DateOnly Today()
    {
        var localNow = TimeZoneInfo.ConvertTime(
            timeProvider.GetUtcNow(),
            _timeZone);

        return DateOnly.FromDateTime(localNow.DateTime);
    }
}
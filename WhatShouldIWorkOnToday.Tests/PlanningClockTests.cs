using Microsoft.Extensions.Options;
using WhatShouldIWorkOnToday.Services;
using Xunit;

namespace WhatShouldIWorkOnToday.Tests;

public sealed class PlanningClockTests
{
    [Fact]
    public void UsesConfiguredTimeZone()
    {
        var options = Options.Create(new PlanningOptions
        {
            TimeZone = "America/New_York"
        });

        var clock = new PlanningClock(
            options,
            TimeProvider.System);

        var today = clock.Today();

        Assert.Equal(
            DateOnly.FromDateTime(
                TimeZoneInfo.ConvertTime(
                    DateTimeOffset.UtcNow,
                    TimeZoneInfo.FindSystemTimeZoneById(
                        "America/New_York"))
                .DateTime),
            today);
    }
}
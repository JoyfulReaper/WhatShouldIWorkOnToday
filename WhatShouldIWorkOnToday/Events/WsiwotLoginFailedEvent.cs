namespace WhatShouldIWorkOnToday.Events;

public sealed record WsiwotLoginFailedEvent(
    string Username,
    DateTimeOffset FailedAtUtc,
    string? Remote)
{
    public const string EventType = "wsiwot.user.login.failed";
}
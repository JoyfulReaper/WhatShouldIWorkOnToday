namespace WhatShouldIWorkOnToday.Events;

public sealed record WsiwotLoginSucceededEvent(
    string Username,
    DateTimeOffset AuthenticatedAtUtc,
    string? Remote)
{
    public const string EventType = "wsiwot.user.login.succeeded";
}
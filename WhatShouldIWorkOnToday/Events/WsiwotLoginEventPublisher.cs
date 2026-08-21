using JoyfulReaperLib.MissionControl;

namespace WhatShouldIWorkOnToday.Events;

public sealed class WsiwotLoginEventPublisher(
    IMissionControlClient missionControlClient,
    ILogger<WsiwotLoginEventPublisher> logger)
{
    public async Task TryPublishSucceededAsync(
        string username,
        string? remoteIpAddress,
        CancellationToken cancellationToken = default)
    {
        var occurredAt = DateTimeOffset.UtcNow;

        var payload =
            new WsiwotLoginSucceededEvent(
                Username: username,
                AuthenticatedAtUtc: occurredAt,
                Remote: remoteIpAddress);

        bool published =
            await missionControlClient.TryPublishAsync(
                WsiwotLoginSucceededEvent.EventType,
                payload,
                WsiwotEventJsonContext.Default
                    .WsiwotLoginSucceededEvent,
                occurredAt,
                cancellationToken: cancellationToken);

        if (!published)
        {
            logger.LogWarning(
                "Mission Control did not accept the WSIWOT login succeeded event.");
        }
    }

    public async Task TryPublishFailedAsync(
        string username,
        string? remoteIpAddress,
        CancellationToken cancellationToken = default)
    {
        var occurredAt =
            DateTimeOffset.UtcNow;

        var payload =
            new WsiwotLoginFailedEvent(
                Username: username,
                FailedAtUtc: occurredAt,
                Remote: remoteIpAddress);

        bool published =
            await missionControlClient.TryPublishAsync(
                WsiwotLoginFailedEvent.EventType,
                payload,
                WsiwotEventJsonContext.Default
                    .WsiwotLoginFailedEvent,
                occurredAt,
                cancellationToken: cancellationToken);

        if (!published)
        {
            logger.LogWarning("Mission Control did not accept the WSIWOT login failed event.");
        }
    }
}
using Microsoft.Extensions.Options;

namespace WhatShouldIWorkOnToday.GitHubSync;

public sealed class GitHubSyncBackgroundService(
    GitHubSyncCoordinator coordinator,
    IOptions<GitHubSyncOptions> options,
    ILogger<GitHubSyncBackgroundService> logger)
    : BackgroundService
{
    private readonly GitHubSyncOptions _options =
        options.Value;

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        if (!_options.Enabled)
        {
            return;
        }

        logger.LogInformation(
            "GitHub sync is enabled for {Owner}/{Repository} on branch {Branch}.",
            _options.Owner,
            _options.Repository,
            _options.Branch);

        var interval = TimeSpan.FromSeconds(
            _options.PollIntervalSeconds);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await coordinator.RunCycleAsync(
                    stoppingToken);
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogWarning(
                    exception,
                    "GitHub sync cycle failed. The next cycle will retry.");
            }

            try
            {
                await Task.Delay(
                    interval,
                    stoppingToken);
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }
    }
}

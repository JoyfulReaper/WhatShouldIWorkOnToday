namespace WhatShouldIWorkOnToday.GitHubSync;

public static class GitHubSyncServiceExtensions
{
    public static IServiceCollection AddGitHubSync(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<GitHubSyncOptions>()
            .Bind(
                configuration.GetSection(
                    GitHubSyncOptions.SectionName))
            .Validate(
                options =>
                    !options.Enabled ||
                    !string.IsNullOrWhiteSpace(
                        options.Owner),
                "GitHubSync:Owner is required when GitHub sync is enabled.")
            .Validate(
                options =>
                    !options.Enabled ||
                    !string.IsNullOrWhiteSpace(
                        options.Repository),
                "GitHubSync:Repository is required when GitHub sync is enabled.")
            .Validate(
                options =>
                    !options.Enabled ||
                    !string.IsNullOrWhiteSpace(
                        options.Branch),
                "GitHubSync:Branch is required when GitHub sync is enabled.")
            .Validate(
                options =>
                    !options.Enabled ||
                    !string.IsNullOrWhiteSpace(
                        options.Token),
                "GitHubSync:Token is required when GitHub sync is enabled.")
            .Validate(
                options =>
                    !options.Enabled ||
                    options.PollIntervalSeconds >= 30,
                "GitHubSync:PollIntervalSeconds must be at least 30 when GitHub sync is enabled.")
            .ValidateOnStart();

        services.AddHttpClient<
            IGitHubSyncClient,
            GitHubSyncClient>(client =>
        {
            client.BaseAddress =
                new Uri("https://api.github.com/");

            client.DefaultRequestHeaders.UserAgent.ParseAdd(
                "WhatShouldIWorkOnToday-GitHubSync/1.0");

            client.DefaultRequestHeaders.Accept.ParseAdd(
                "application/vnd.github+json");

            client.DefaultRequestHeaders.Add(
                "X-GitHub-Api-Version",
                "2022-11-28");
        });

        services.AddSingleton<SyncSnapshotBuilder>();
        services.AddSingleton<SyncSnapshotPublisher>();
        services.AddSingleton<SyncCommandParser>();
        services.AddSingleton<SyncCommandProcessor>();
        services.AddSingleton<GitHubSyncCoordinator>();
        services.AddHostedService<GitHubSyncBackgroundService>();

        return services;
    }
}

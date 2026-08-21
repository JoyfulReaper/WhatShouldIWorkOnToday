namespace WhatShouldIWorkOnToday.GitHubSync;

public sealed class GitHubSyncOptions
{
    public const string SectionName = "GitHubSync";

    public bool Enabled { get; set; }

    public string Owner { get; set; } = string.Empty;

    public string Repository { get; set; } = string.Empty;

    public string Branch { get; set; } = "main";

    public string Token { get; set; } = string.Empty;

    public int PollIntervalSeconds { get; set; } = 300;
}

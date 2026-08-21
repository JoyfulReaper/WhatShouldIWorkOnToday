namespace WhatShouldIWorkOnToday.GitHubSync;

public sealed record SyncSnapshot(
    int SchemaVersion,
    DateTimeOffset GeneratedAtUtc,
    string StateHash,
    IReadOnlyList<SyncWorkItemSnapshot> WorkItems);

public sealed record SyncWorkItemSnapshot(
    int Id,
    string Name,
    string Kind,
    string Priority,
    string? Description,
    string? Url,
    DateTimeOffset CreatedAt,
    DateTimeOffset? LastWorkedAt,
    DateTimeOffset? CompletedAt,
    DateTimeOffset? ArchivedAt,
    IReadOnlyList<SyncTodoSnapshot> Todos);

public sealed record SyncTodoSnapshot(
    int Id,
    string Task,
    string Energy,
    string Effort,
    string Priority,
    DateTimeOffset CreatedAt,
    DateTimeOffset? CompletedAt);



namespace WhatShouldIWorkOnToday.Contracts.WorkItems;

public record CreateWorkItemRequest (
    string Name,
    string? Description,
    string? Url,
    bool Pinned,
    int SequenceNumber,
    DateTime? LastDateWorkedOn,
    DateTime? DateCompleted
    );
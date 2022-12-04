namespace WhatShouldIWorkOnToday.Contracts.WorkItems;

public record WorkItemResponse(
    int Id,
    string Name,
    string? Description,
    string? Url,
    bool Pinned,
    int SequenceNumber,
    DateTime? LastDateWorkedOn,
    DateTime? DateCompleted,
    List<WorkItemHistoryResponse> WorkItemHistory
    );

public record NoteResponse(
    int Id,
    string Text
    );

public record ToDoItemResponse(
    int Id,
    string Task,
    DateTime? DateCompleted
    );

public record WorkItemHistoryResponse(
    int Id,
    DateTime DateWorkedOk
    );
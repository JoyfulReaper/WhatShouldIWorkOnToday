using ErrorOr;
using MediatR;
using WhatShouldIWorkOnToday.Domain.WorkItem;

namespace WhatShouldIWorkOnToday.Application.WorkItems.Commands.CreateWorkItem;

public record CreateWorkItemCommand(
    string Name,
    string? Description,
    string? Url,
    bool Pinned,
    int SequenceNumber
    ) : IRequest<ErrorOr<WorkItem>>;

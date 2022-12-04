using ErrorOr;
using MediatR;
using WhatShouldIWorkOnToday.Application.Common.Interfaces.Persistence;
using WhatShouldIWorkOnToday.Domain.WorkItem;

namespace WhatShouldIWorkOnToday.Application.WorkItems.Commands.CreateWorkItem;

public class CreateWorkItemCommandHandler : IRequestHandler<CreateWorkItemCommand, ErrorOr<WorkItem>>
{
    private readonly IWorkItemRepository _workItemRepository;

    public CreateWorkItemCommandHandler(IWorkItemRepository workItemRepository)
    {
        _workItemRepository = workItemRepository;
    }

    public async Task<ErrorOr<WorkItem>> Handle(CreateWorkItemCommand request, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;

        var workItem = WorkItem.Create(
            request.Name,
            request.Description,
            request.Url,
            request.Pinned,
            request.SequenceNumber,
            null,
            null,
            null
            );

        _workItemRepository.Add(workItem);

        return workItem;
    }
}

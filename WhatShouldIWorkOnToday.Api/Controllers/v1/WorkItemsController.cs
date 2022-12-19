using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WhatShouldIWorkOnToday.Application.WorkItems.Commands.CreateWorkItem;
using WhatShouldIWorkOnToday.Contracts.WorkItems;

namespace WhatShouldIWorkOnToday.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class WorkItemsController : ApiController
{
    private readonly IMapper _mapper;
    private readonly ISender _mediator;

    public WorkItemsController(IMapper mapper, ISender mediator)
    {
        _mapper = mapper;
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateWorkItem(CreateWorkItemRequest request)
    {
        var command = _mapper.Map<CreateWorkItemCommand>(request);
        var createWorkItemResult = await _mediator.Send(command);

        return createWorkItemResult.Match(
            workItem => Ok(_mapper.Map<WorkItemResponse>(workItem)),
            errors => Problem(errors)
            );
    }

    [HttpGet]
    public IActionResult ListWorkItems()
    {
        return Ok(Array.Empty<string>());
    }
}

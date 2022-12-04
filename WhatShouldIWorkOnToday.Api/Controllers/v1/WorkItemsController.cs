using Microsoft.AspNetCore.Mvc;

namespace WhatShouldIWorkOnToday.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class WorkItemsController : ApiController
{
    [HttpPost]
    public async Task<IActionResult> AddWorkItem()
    {

    }

    [HttpGet]
    public IActionResult ListWorkItems()
    {
        return Ok(Array.Empty<string>());
    }
}

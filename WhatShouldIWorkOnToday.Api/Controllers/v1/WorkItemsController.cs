using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WhatShouldIWorkOnToday.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class WorkItemsController : ApiController
{
    [HttpGet]
    public IActionResult ListWorkItems()
    {
        var test = HttpContext.User.Identity.IsAuthenticated;
        return Ok(Array.Empty<string>());
    }
}

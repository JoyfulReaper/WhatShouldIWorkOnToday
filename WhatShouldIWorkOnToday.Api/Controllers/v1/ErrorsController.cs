using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WhatShouldIWorkOnToday.Api.Controllers.v1;

[ApiVersion("1.0")]
public class ErrorsController : ControllerBase
{
    [Route("/error")]
    [HttpGet]
    public IActionResult Error()
    {
        var exception = HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;

        return Problem(title: exception?.Message, statusCode: 400);
    }
}

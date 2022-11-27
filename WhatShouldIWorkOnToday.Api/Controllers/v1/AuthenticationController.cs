using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhatShouldIWorkOnToday.Application.Authentication.Commands.Register;
using WhatShouldIWorkOnToday.Application.Authentication.Common;
using WhatShouldIWorkOnToday.Application.Authentication.Queries;
using WhatShouldIWorkOnToday.Contracts.Authentication;

namespace WhatShouldIWorkOnToday.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/auth")]
[ApiVersion("1.0")]
[AllowAnonymous]
public class AuthenticationController : ApiController
{
    private readonly ISender _mediator;

    public AuthenticationController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var command = new RegisterCommand(request.FirstName, request.LastName, request.Email, request.Password);
        var authResult = await _mediator.Send(command);

        return authResult.Match(
            authResult => Ok(MapAuthResponse(authResult)),
            errors => Problem(errors));
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var querry = new LoginQuery(request.Email, request.Password);
        var authResult = await _mediator.Send(querry);

        return authResult.Match(
            authResult => Ok(MapAuthResponse(authResult)),
            errors => Problem(errors));
    }

    private static AuthenticationResponse MapAuthResponse(AuthenticationResult result)
    {
        return new AuthenticationResponse(
            result.User.Id,
            result.User.FirstName,
            result.User.LastName,
            result.User.Email,
            result.Token);
    }
}

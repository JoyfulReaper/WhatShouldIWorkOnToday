using ErrorOr;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WhatShouldIWorkOnToday.Application.Common.Interfaces.Authentication;
using WhatShouldIWorkOnToday.Application.Common.Models;
using WhatShouldIWorkOnToday.Contracts.Authentication;
using WhatShouldIWorkOnToday.Domain.Common.Errors;

namespace WhatShouldIWorkOnToday.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/auth")]
[ApiVersion("1.0")]
[AllowAnonymous]
public class AuthenticationController : ApiController
{
    private readonly IIdentityService _identityService;

    public AuthenticationController(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var result = await _identityService.CreateUserAsync(request.Email, request.Password, request.Email);

        return result.Match(
            result => Ok(MapToAuthResponse(result)),
            errors => Problem(errors));
    }

    private async AuthenticationResponse MapToAuthResponse(string token)
    {
        var user = await _identityService.GetUser()
        return new AuthenticationResponse(user.Id, user.FirstName, user.LastName, user.Email, token);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _identityService.LoginUserAsync(request.Email, request.Password);

        return result.Match(
            authResult => NoContent(),
            errors => Problem(errors));
    }
}

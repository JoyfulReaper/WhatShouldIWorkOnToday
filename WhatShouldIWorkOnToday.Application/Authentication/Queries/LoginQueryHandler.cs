using ErrorOr;
using MediatR;
using WhatShouldIWorkOnToday.Application.Authentication.Common;
using WhatShouldIWorkOnToday.Application.Common.Interfaces.Authentication;

namespace WhatShouldIWorkOnToday.Application.Authentication.Queries;
public class LoginQueryHandler : IRequestHandler<LoginQuery, ErrorOr<AuthenticationResult>>
{
    private readonly IIdentityService _identityService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginQueryHandler(
        IIdentityService identityService,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _identityService = identityService;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<ErrorOr<AuthenticationResult>> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var result = await _identityService.LoginUserAsync(request.Email, request.Password);
        if (result.IsError)
        {
            return result.Errors;
        }

        var user = await _identityService.GetUser(result.Value);
        if (user.IsError)
        {
            return user.Errors;
        }

        var token = _jwtTokenGenerator.GenerateToken(user.Value);
        return new AuthenticationResult(user.Value, token);
    }
}

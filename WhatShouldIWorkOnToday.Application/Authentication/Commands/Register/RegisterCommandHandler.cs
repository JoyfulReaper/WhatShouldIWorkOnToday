using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatShouldIWorkOnToday.Application.Authentication.Common;
using WhatShouldIWorkOnToday.Application.Common.Interfaces.Authentication;

namespace WhatShouldIWorkOnToday.Application.Authentication.Commands.Register;
public class RegisterCommandHandler : 
    IRequestHandler<RegisterCommand, ErrorOr<AuthenticationResult>>
{
    private readonly IIdentityService _identityService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RegisterCommandHandler(IIdentityService identityService, IJwtTokenGenerator jwtTokenGenerator)
    {
        _identityService = identityService;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<ErrorOr<AuthenticationResult>> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        var result = await _identityService.CreateUserAsync(command.FirstName, command.LastName, command.Email, command.Password, command.Email);
        if(result.IsError)
        {
            return result.Errors;
        }

        var user = await _identityService.GetUser(result.Value);
        if(user.IsError)
        {
            return user.Errors;
        }

        var token = _jwtTokenGenerator.GenerateToken(user.Value);
        return new AuthenticationResult(user.Value, token);
    }
}

using ErrorOr;
using MediatR;
using WhatShouldIWorkOnToday.Application.Authentication.Common;

namespace WhatShouldIWorkOnToday.Application.Authentication.Commands.Register;

public record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password) : IRequest<ErrorOr<AuthenticationResult>>;

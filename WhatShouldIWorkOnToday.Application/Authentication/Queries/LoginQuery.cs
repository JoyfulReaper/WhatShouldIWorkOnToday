using ErrorOr;
using MediatR;
using WhatShouldIWorkOnToday.Application.Authentication.Common;

namespace WhatShouldIWorkOnToday.Application.Authentication.Queries;

public record LoginQuery(
    string Email,
    string Password) : IRequest<ErrorOr<AuthenticationResult>>;
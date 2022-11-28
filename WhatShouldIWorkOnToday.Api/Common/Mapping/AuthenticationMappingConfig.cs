using Mapster;
using WhatShouldIWorkOnToday.Application.Authentication.Commands.Register;
using WhatShouldIWorkOnToday.Application.Authentication.Common;
using WhatShouldIWorkOnToday.Application.Authentication.Queries;
using WhatShouldIWorkOnToday.Contracts.Authentication;

namespace WhatShouldIWorkOnToday.Api.Common.Mapping;

public class AuthenticationMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<RegisterRequest, RegisterCommand>();

        config.NewConfig<LoginRequest, LoginQuery>();

        config.NewConfig<AuthenticationResult, AuthenticationResponse>()
            .Map(dest => dest, src => src.User);
    }
}

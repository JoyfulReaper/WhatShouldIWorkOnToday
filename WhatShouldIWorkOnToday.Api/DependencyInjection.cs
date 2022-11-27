using WhatShouldIWorkOnToday.Api.Services;
using WhatShouldIWorkOnToday.Application.Common.Interfaces.Services;

namespace WhatShouldIWorkOnToday.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddSingleton<ICurrentUserService, CurrentUserService>();

        return services;
    }
}

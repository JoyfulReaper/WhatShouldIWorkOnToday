using Microsoft.Extensions.DependencyInjection;
using WhatShouldIWorkOnToday.Application.Services.Authentication;

namespace WhatShouldIWorkOnToday.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        return services;
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WhatShouldIWorkOnToday.Application.Common.Interfaces.Authentication;
using WhatShouldIWorkOnToday.Application.Common.Interfaces.Services;
using WhatShouldIWorkOnToday.Infrastructure.Authentication;
using WhatShouldIWorkOnToday.Infrastructure.Services;

namespace WhatShouldIWorkOnToday.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        return services;
    }

    public static IServiceCollection AddIdentity(
        this IServiceCollection services, 
        ConfigurationManager configuration)
    {
        // Identity
        services.AddDbContext<IdentityDbContext>(opts =>
        {
            opts.UseSqlServer(configuration["ConnectionStrings:TodoApiIdentity"],
                opts => opts.MigrationsAssembly("TodoApi")
                );
        });

        services.AddIdentity<ApiIdentityUser, IdentityRole>(opts =>
        {
            opts.SignIn.RequireConfirmedAccount = false;
        }).AddEntityFrameworkStores<IdentityContext>()
        .AddDefaultTokenProviders();
    }
}
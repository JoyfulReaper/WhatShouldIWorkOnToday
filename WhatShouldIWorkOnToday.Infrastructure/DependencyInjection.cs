using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WhatShouldIWorkOnToday.Application.Common.Interfaces;
using WhatShouldIWorkOnToday.Application.Common.Interfaces.Authentication;
using WhatShouldIWorkOnToday.Application.Common.Interfaces.Services;
using WhatShouldIWorkOnToday.Infrastructure.Authentication;
using WhatShouldIWorkOnToday.Infrastructure.Identity;
using WhatShouldIWorkOnToday.Infrastructure.Persistence;
using WhatShouldIWorkOnToday.Infrastructure.Persistence.Interceptors;
using WhatShouldIWorkOnToday.Infrastructure.Services;

namespace WhatShouldIWorkOnToday.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        ConfigurationManager configuration)
    {

        // Entity Framework
        services.AddScoped<AuditableEntitySaveChangesInterceptor>();
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseInMemoryDatabase("WsiwotDb");
        });
        services.AddDatabaseDeveloperPageExceptionFilter();
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        //services.AddDbContext<ApplicationDbContext>(options =>
        //        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
        //            builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        // Jwt Tokens
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
        services.AddIdentity<ApplicationUser, IdentityRole>(opts =>
        {
            opts.SignIn.RequireConfirmedAccount = false;
        }).AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        return services;
    }
}
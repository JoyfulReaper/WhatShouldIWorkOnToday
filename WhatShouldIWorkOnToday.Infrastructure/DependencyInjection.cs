using Microsoft.AspNetCore.Identity;
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
        services.AddEntityFrameworkCore();
        services.AddIdentity();

        services.AddTransient<IIdentityService, IdentityService>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        // Jwt Tokens
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        
        return services;
    }

    public static IServiceCollection AddEntityFrameworkCore(this IServiceCollection services)
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

        return services;
    }

    public static IServiceCollection AddIdentity(
        this IServiceCollection services)
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
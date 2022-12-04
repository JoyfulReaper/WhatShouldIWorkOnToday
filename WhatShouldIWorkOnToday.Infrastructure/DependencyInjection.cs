using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WhatShouldIWorkOnToday.Application.Common.Interfaces;
using WhatShouldIWorkOnToday.Application.Common.Interfaces.Authentication;
using WhatShouldIWorkOnToday.Application.Common.Interfaces.Persistence;
using WhatShouldIWorkOnToday.Application.Common.Interfaces.Services;
using WhatShouldIWorkOnToday.Infrastructure.Authentication;
using WhatShouldIWorkOnToday.Infrastructure.Identity;
using WhatShouldIWorkOnToday.Infrastructure.Persistence;
using WhatShouldIWorkOnToday.Infrastructure.Persistence.Interceptors;
using WhatShouldIWorkOnToday.Infrastructure.Services;

namespace WhatShouldIWorkOnToday.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.AddEntityFrameworkCore();
        services.AddIdentity();
        services.AddAuth(configuration);
        
        services.AddTransient<IIdentityService, IdentityService>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        services.AddPersistance();

        return services;
    }

    public static IServiceCollection AddPersistance(
        this IServiceCollection services)
    {
        services.AddScoped<IWorkItemRepository, WorkItemRepository>();

        return services;
    }

    public static IServiceCollection AddAuth(
        this IServiceCollection services,
        ConfigurationManager configuration)
    {
        // Jwt Tokens
        var jwtSettings = new JwtSettings();
        configuration.Bind(JwtSettings.SectionName, jwtSettings);

        services.AddSingleton(Options.Create(jwtSettings));
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.Secret)
                )
                };
            });

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
        services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

        return services;
    }
}
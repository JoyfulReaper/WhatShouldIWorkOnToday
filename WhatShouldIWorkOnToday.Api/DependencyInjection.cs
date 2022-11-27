using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.OpenApi.Models;
using System.Reflection;
using WhatShouldIWorkOnToday.Api.Common.Errors;
using WhatShouldIWorkOnToday.Api.Services;
using WhatShouldIWorkOnToday.Application.Common.Interfaces.Services;

namespace WhatShouldIWorkOnToday.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddSingleton<ProblemDetailsFactory, WsiwotProblemDetailsFactory>();
        services.AddSingleton<ICurrentUserService, CurrentUserService>();
        services.AddHttpContextAccessor();

        services.AddSwagger();

        return services;
    }

    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

        var securityScheme = new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Description = "JWT Authorization header info using bearer tokens",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        };

        var securityRequirement = new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "bearerAuth"
                    }
                },
                new string[] { }
            }
        };

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(opts =>
        {
            var title = "WhatShouldIWorkOnToday";
            var description = "API for telling you what to do!";
            var terms = new Uri("https://opensource.org/licenses/MIT");
            var license = new OpenApiLicense()
            {
                Name = "MIT",
                Url = new Uri("https://opensource.org/licenses/MIT"),
            };

            OpenApiContact contact = new()
            {
                Name = "Kyle Givler",
                Url = new Uri("https://github.com/JoyfulReaper")
            };

            opts.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = $"{title} v1",
                Description = description,
                TermsOfService = terms,
                License = license,
                Contact = contact
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            opts.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));

            opts.AddSecurityDefinition("bearerAuth", securityScheme);
            opts.AddSecurityRequirement(securityRequirement);
        });

        services.AddApiVersioning(opts =>
        {
            opts.AssumeDefaultVersionWhenUnspecified = true;
            opts.DefaultApiVersion = new(1, 0);
            opts.ReportApiVersions = true;
        });

        services.AddVersionedApiExplorer(opts =>
        {
            opts.GroupNameFormat = "'v'VVV";
            opts.SubstituteApiVersionInUrl = true;
        });

        return services;
    }
}

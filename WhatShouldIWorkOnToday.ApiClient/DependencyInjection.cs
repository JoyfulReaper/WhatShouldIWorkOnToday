using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WhatShouldIWorkOnToday.ApiClient.Common.Options;
using WhatShouldIWorkOnToday.ApiClient.HttpClients;

namespace WhatShouldIWorkOnToday.ApiClient;

public static class DependencyInjection
{
    public static IServiceCollection AddTaskClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<WsiwotClientOptions>(
            configuration.GetSection(nameof(WsiwotClientOptions)));

        services.AddHttpClient<ISettingsClient, SettingsClient>();

        return services;
    }
}

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatShouldIWorkOnToday.ApiClient;

namespace NaiveTests;
internal static class DependencyInjection
{
    internal static IHost SetupDi(string[] args)
    {
        IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) => {
                services.AddHostedService<NaiveTestHostedClient>();
                services.AddWsiwotClient(hostContext.Configuration);
            })
            .ConfigureLogging(logging => {
                logging.SetMinimumLevel(LogLevel.None);
            })
            .Build();

        return host;
    }
}
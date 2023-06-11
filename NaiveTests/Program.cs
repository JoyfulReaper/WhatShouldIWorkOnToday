using NaiveTests;

var host = DependencyInjection.SetupDi(args);
await host.StartAsync();

host.Dispose();
// TODO: Create an API Wrapper instead of just calling the API directly from the components

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WhatShouldIWorkOnToday.Client;
using WhatShouldIWorkOnToday.Client.ApiClient;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IWorkItemEndpoint, WorkItemEndpoint>();

await builder.Build().RunAsync();

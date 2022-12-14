using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WhatShouldIWorkOnToday.Client;
using WhatShouldIWorkOnToday.Client.ApiClient;
using WhatShouldIWorkOnToday.Client.ApiClient.Interfaces;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IWorkItemEndpoint, WorkItemEndpoint>();
builder.Services.AddScoped<IWorkItemSequenceEndpoint, WorkItemSequenceEndpoint>();
builder.Services.AddScoped<ISequenceNumberEndpoint, SequenceNumberEndpoint>();
builder.Services.AddScoped<INoteEndpoint, NoteEndpoint>();
builder.Services.AddScoped<IPinnedWorkItemEndpoint, PinnedWorkItemEndpoint>();
builder.Services.AddScoped<ITodoItemEndpoint, TodoItemEndpoint>();

await builder.Build().RunAsync();

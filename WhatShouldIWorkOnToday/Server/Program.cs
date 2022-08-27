using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.ResponseCompression;
using WhatShouldIWorkOnToday.Server.Authentication;
using WhatShouldIWorkOnToday.Server.DataAccess;
using WhatShouldIWorkOnToday.Server.DataAccess.Interfaces;
using WhatShouldIWorkOnToday.Server.DTOs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication()
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication",
    options => { });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("BasicAuthentication", new AuthorizationPolicyBuilder("BasicAuthentication")
        .RequireAuthenticatedUser()
        .Build());
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<IDataAccess, SqlDataAccess>();
builder.Services.AddScoped<IWorkItemData, WorkItemData>();
builder.Services.AddScoped<IWorkSequenceNumberData, WorkSequenceNumberData>();
builder.Services.AddScoped<ICurrentSequenceNumberData, CurrentSequenceNumberData>();
builder.Services.AddScoped<INoteData, NoteData>();
builder.Services.AddScoped<IPinnedWorkItemData, PinnedWorkItemData>();
builder.Services.AddScoped<IWorkItemData, WorkItemData>();
builder.Services.AddScoped<IWorkItemHistoryData, WorkItemHistoryData>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();

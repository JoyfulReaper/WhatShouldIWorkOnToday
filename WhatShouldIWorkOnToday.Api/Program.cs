using Microsoft.AspNetCore.Mvc.Infrastructure;
using WhatShouldIWorkOnToday.Api;
using WhatShouldIWorkOnToday.Api.Common.Errors;
using WhatShouldIWorkOnToday.Application;
using WhatShouldIWorkOnToday.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddControllers();
    builder.Services.AddSingleton<ProblemDetailsFactory, WsiwotProblemDetailsFactory>();

    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddPresentation();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler("/error");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

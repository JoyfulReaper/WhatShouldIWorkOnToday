open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.EntityFrameworkCore
open Giraffe
open WorkItemService
open SettingRepository
open WhatShouldIWorkOnToday.Repository
open WhatShouldIWorkOnToday.Repository.Sql.Entities
open Microsoft.Extensions.Configuration

let webApp =
    choose [
        route "/ping"   >=> text "pong"
        route "/"       >=> text "You got root..." 
        route "/hello"  >=> sayHelloWorld "fred" 
        subRoute "/WorkItem" 
            (choose [
                route "" >=> GET >=> warbler ( fun _ -> 
                    (getWorkItemHandler))
            ])]

let configureApp (app : IApplicationBuilder) =
    // Add Giraffe to the ASP.NET Core pipeline
    app.UseGiraffe webApp

let configureServices (services : IServiceCollection) =

    let configuration = services.BuildServiceProvider().GetService<IConfiguration>()

    // Add Giraffe dependencies
    services.AddGiraffe() |> ignore
    services.AddSingleton<ISettingRepository, SqlSettingRepository>() |> ignore

    services.AddDbContext<SqlDbContext>(fun builder ->
        builder.UseSqlServer(configuration.GetConnectionString("Default")) |> ignore
    ) |> ignore

[<EntryPoint>]
let main _ =
    Host.CreateDefaultBuilder()
        .ConfigureWebHostDefaults(
            fun webHostBuilder ->
                webHostBuilder
                    .Configure(configureApp)
                    .ConfigureServices(configureServices)
                    |> ignore)
        .Build()
        .Run()
    0

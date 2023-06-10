open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.EntityFrameworkCore
open Giraffe
open WhatShouldIWorkOnToday.Repository
open WhatShouldIWorkOnToday.Repository.Sql.Entities
open Microsoft.Extensions.Configuration
open SettingRepository
open NoteRepository
open ToDoItemRepository
open WorkItemRepository
open WorkItemHistoryRepository

let webApp =
    subRouteCi "/api" (choose [
        subRouteCi "/v1" (choose [
            subRouteCi "/WorkItem" (choose [
                GET >=>
                    choose [
                        routeCi "/" >=>
                            warbler (fun _ -> WorkItemService.getAllWorkItemsHandler)
                        routeCif "/%i" (fun workItemId ->
                            warbler (fun _ -> WorkItemService.getWorkItemHandler workItemId))

                    ]
                POST >=>
                    choose [
                        routeCi "/" >=>
                            warbler (fun _ -> WorkItemService.createWorkItemHandler)
                    ]
            ])
            subRouteCi "/SequenceNumber" (choose [
                GET >=>
                    choose [
                        routeCi "/" >=>
                            warbler (fun _ -> SettingService.getSeqeunceNumberHandler)
                        routeCi "/max" >=>
                            warbler (fun _ -> SettingService.getMaxSequenceNumberHandler)
                    ]
                POST >=>
                    choose [
                        routeCif "/set/%i" (fun seqNum -> warbler (fun _ -> SettingService.setSequenceNumberHandler seqNum))
                    ]
            ])
        ])
        subRouteCi "/v2" (choose [
            route "/foo" >=> text "Foo"
            route "/bar" >=> text "Bar"
        ])
    ])

let configureApp (app : IApplicationBuilder) =
    // Add Giraffe to the ASP.NET Core pipeline
    app.UseGiraffe webApp

let configureServices (services : IServiceCollection) =

    let configuration = services.BuildServiceProvider().GetService<IConfiguration>()

    // Add Giraffe dependencies
    services.AddGiraffe() |> ignore
    services.AddTransient<ISettingRepository, SqlSettingRepository>() |> ignore
    services.AddTransient<INoteRepository, SqlNoteRepository>() |> ignore
    services.AddTransient<IToDoItemRepository, SqlToDoRepository>() |> ignore
    services.AddTransient<IWorkItemRepository, SqlWorkItemRepository>() |> ignore
    services.AddTransient<IWorkItemHistoryRepository, SqlWorkItemHistoryRepository>() |> ignore

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

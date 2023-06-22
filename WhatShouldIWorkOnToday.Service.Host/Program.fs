open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.Authentication;
open Microsoft.AspNetCore.Authorization;
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
open WhatShouldIWorkOnToday.Server.Authentication

let webApp =
    subRouteCi "/api" (choose [
        subRouteCi "/v1" (choose [
            subRouteCi "/WorkItem" (choose [
                GET >=>
                    choose [
                        routeCi "/Completed" >=>
                            warbler (fun _ -> WorkItemService.getCompletedWorkItemsHandler)
                        routeCi "/" >=>
                            warbler (fun _ -> WorkItemService.getAllWorkItemsHandler)
                        routeCif "/%i" (fun workItemId ->
                            warbler (fun _ -> WorkItemService.getWorkItemHandler workItemId))
                        routeCif "/SequenceNumber/%i" (fun seqNum ->
                            warbler (fun _ -> WorkItemService.getWorkItemsBySeqeunceNumberHandler seqNum))
                        routeCif "/Search/%s" (fun search ->
                            warbler (fun _ -> WorkItemService.searchWorkItemsHandler search))
                        routeCif "/History/%i" (fun workItemId ->
                            warbler (fun _ -> WorkItemService.getWorkItemHistoryHandler workItemId))
                    ]
                POST >=>
                    choose [
                        routeCi "/" >=>
                            warbler (fun _ -> WorkItemService.createWorkItemHandler)
                    ]
                PUT >=>
                    choose [
                        routeCi "/" >=>
                            warbler (fun _ -> WorkItemService.updateWorkItemHandler)
                        routeCif "/MarkWorked/%i" (fun workItemId ->
                            warbler (fun _ -> WorkItemService.markAsWorkedHandler workItemId))
                    ]
                DELETE >=>
                    choose [
                        routeCif "/%i" (fun workItemId ->
                            warbler (fun _ -> WorkItemService.deleteWorkItemHandler workItemId))
                    ]
            ])
            subRouteCi "/ToDo" (choose [
                GET >=>
                    choose [
                        routeCif "/%i" (fun toDoItemId ->
                            warbler (fun _ -> ToDoItemService.getToDoItemHandler toDoItemId))
                        routeCif "/WorkItem/%i" (fun workItemId ->
                            warbler (fun _ -> ToDoItemService.getToDoItemsByWorkItemHandler workItemId))
                    ]
                POST >=>
                    choose [
                        routeCi "/" >=>
                            warbler (fun _ -> ToDoItemService.createToDoItemHandler)
                    ]
                PUT >=>
                    choose [
                        routeCif "/complete/%i" (fun toDoItemId ->
                            warbler (fun _ -> ToDoItemService.completeToDoItemHandler toDoItemId))
                        routeCi "/" >=>
                            warbler (fun _ -> ToDoItemService.updateToDoItemHandler)
                    ]
                DELETE >=>
                    choose [
                        routeCif "/%i" (fun toDoItemId ->
                            warbler (fun _ -> ToDoItemService.deleteToDoItemHandler toDoItemId))
                    ]
            ])
            subRouteCi "/Note" (choose [
                GET >=> 
                    choose [
                        routeCif "/%i" (fun noteId -> 
                            warbler (fun _ -> NoteService.getNoteHandler noteId))
                        routeCif "/WorkItem/%i" (fun seqNum ->
                            warbler (fun _ -> NoteService.gotNoteByWorkItemIdHandler seqNum))
                    ]
                POST >=>
                    choose [
                        routeCi "/" >=>
                            warbler (fun _ -> NoteService.saveNoteHandler)
                            ]
                DELETE >=>
                    choose [
                        routeCif "/%i" (fun noteId -> 
                            warbler (fun _ -> NoteService.deleteNoteHandler noteId))
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
                PUT >=>
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
    app.UseAuthentication() |> ignore
    app.UseAuthorization() |> ignore
    app.UseCors(fun cors ->
        cors.AllowAnyOrigin() // TODO: Don't allow all origins
            .AllowAnyMethod()
            .AllowAnyHeader() |> ignore) |> ignore
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
    services.AddCors() |> ignore

    services.AddAuthentication()
            .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication",
                fun options -> ()) |> ignore

    services.AddAuthorization(fun options ->
        options.AddPolicy("BasicAuthentication", AuthorizationPolicyBuilder("BasicAuthentication")
            .RequireAuthenticatedUser()
            .Build())
        |> ignore) |> ignore


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

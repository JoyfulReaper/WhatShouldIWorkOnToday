module WorkItemService
open Giraffe
open Microsoft.AspNetCore.Http
open WhatShouldIWorkOnToday.Repository
open WhatShouldIWorkOnToday.Models

let getWorkItemHandler workItemId : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let workItemRepo = ctx.GetService<IWorkItemRepository>()
            let! workItem = workItemRepo.Get workItemId
            match workItem with
                | None -> return! RequestErrors.NOT_FOUND { Message = sprintf "Work item: (id: %i) not found" workItemId } next ctx
                | Some workItem -> return! Successful.OK (workItem |> WorkItem.toDto) next ctx
        }

let getAllWorkItemsHandler : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let workItemRepo = ctx.GetService<IWorkItemRepository>()
            let! workItems = workItemRepo.GetAll()
            return! Successful.OK (workItems |> List.map WorkItem.toDto) next ctx
        }

let getWorkItemsBySeqeunceNumberHandler seqeunceNumber : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let workItemRepo = ctx.GetService<IWorkItemRepository>()
            let! workItems = workItemRepo.GetBySequenceNumber(seqeunceNumber)
            return! Successful.OK (workItems |> List.map WorkItem.toDto) next ctx
        }

let getCompletedWorkItemsHandler : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let workItemRepo = ctx.GetService<IWorkItemRepository>()
            let! workItems = workItemRepo.GetCompleted()
            return! Successful.OK (workItems |> List.map WorkItem.toDto) next ctx
        }

let createWorkItemHandler : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let workItemRepo = ctx.GetService<IWorkItemRepository>()
            let! workItem = ctx.BindJsonAsync<WorkItemRequest>()
            let! savedWorkItem = workItemRepo.Save(workItem |> WorkItem.fromWorkItemRequest)

            match savedWorkItem with
            | None -> 
                return! RequestErrors.BAD_REQUEST { Message = "Failed to save work item" } next ctx
            | Some savedWorkItem ->
                ctx.SetHttpHeader("Location", sprintf "/api/v1/WorkItem/%s" (savedWorkItem.WorkItemId.ToString()))
                return! Successful.CREATED (savedWorkItem |> WorkItem.toDto) next ctx
        }

let deleteWorkItemHandler workItemId : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let workItemRepo = ctx.GetService<IWorkItemRepository>()
            do! workItemRepo.Delete(workItemId)
            return! Successful.NO_CONTENT next ctx
        }

let updateWorkItemHandler : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let workItemRepo = ctx.GetService<IWorkItemRepository>()
            let! workItem = ctx.BindJsonAsync<WorkItemDto>()
            let! updatedWorkItem = workItemRepo.Update(workItem |> WorkItem.fromDto)
            return! Successful.OK (updatedWorkItem |> WorkItem.toDto) next ctx
        }
module ToDoItemService

open Giraffe
open Microsoft.AspNetCore.Http
open WhatShouldIWorkOnToday.Repository
open WhatShouldIWorkOnToday.Models

let getToDoItemHandler toDoItemId : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let toDoItemRepo = ctx.GetService<IToDoItemRepository>()
            let! toDoItem = toDoItemRepo.Get toDoItemId
            match toDoItem with
            | None -> return! RequestErrors.NOT_FOUND { Message = sprintf "To Do Item: (id: %i) not found" toDoItemId } next ctx
            | Some x -> return! Successful.OK (ToDoItem.toDto x) next ctx
        }

let getToDoItemsByWorkItemHandler workItemId : HttpHandler =
    fun (next : HttpFunc) (ctx: HttpContext) ->
        task {
            let toDoItemRepo = ctx.GetService<IToDoItemRepository>()
            let! toDoItems = toDoItemRepo.GetByWorkItemId workItemId
            return! Successful.OK (toDoItems |> List.map ToDoItem.toDto) next ctx
        }

let createToDoItemHandler : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let toDoItemRepo = ctx.GetService<IToDoItemRepository>()
            let! toDoItemRequest = ctx.BindJsonAsync<ToDoItemRequest>()
            let! savedToDoItem = toDoItemRepo.Save(ToDoItem.fromTodoItemRequest toDoItemRequest)

            match savedToDoItem with
            | Some toDoItem -> return! Successful.OK (ToDoItem.toDto toDoItem) next ctx
            | None -> return! RequestErrors.BAD_REQUEST { Message = "Failed to save to do item" } next ctx
        }

let completeToDoItemHandler toDoItemId : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
    task {
        let toDoItemRepo = ctx.GetService<IToDoItemRepository>()
        do! toDoItemRepo.Complete toDoItemId

        return! Successful.NO_CONTENT next ctx
    }

let deleteToDoItemHandler todoItemId : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let toDoItemRepo = ctx.GetService<IToDoItemRepository>()
            do! toDoItemRepo.Delete todoItemId

            return! Successful.NO_CONTENT next ctx
        }

let updateToDoItemHandler : HttpHandler =
    fun (next : HttpFunc) (ctx: HttpContext) ->
    task {
        let todoItemRepo = ctx.GetService<IToDoItemRepository>()
        let! todoItem = ctx.BindJsonAsync<ToDoItemDto>()
        let! updatedToDoItem = todoItemRepo.Update (todoItem |> ToDoItem.fromDto)

        return! Successful.OK (ToDoItem.toDto updatedToDoItem) next ctx
    }
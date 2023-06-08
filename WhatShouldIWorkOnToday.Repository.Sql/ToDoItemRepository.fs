﻿module ToDoItemRepository

open WhatShouldIWorkOnToday.Repository.Sql.Entities
open WhatShouldIWorkOnToday.Repository
open WhatShouldIWorkOnToday.Repository.Sql.Mapping
open Microsoft.EntityFrameworkCore
open System
open System.Linq

let getToDoItem (context : SqlDbContext) (id : int) =
    async {
        let! entity = context.ToDoItems.FindAsync(id).AsTask() |> Async.AwaitTask
        return entity  |> Option.ofObj |> Option.map ToDoItem.toModel
    }

let deleteToDoItem (context : SqlDbContext) (id: int) =
    async {
        let! model = getToDoItem context id 
        match model with
        | None -> ()
        | Some entity ->
            let entity = entity |> ToDoItem.toEntity
            entity.DateDeleted <- DateTime.Now
            context.SaveChangesAsync() |> Async.AwaitTask |> ignore
    }

let getToDoItemsByWorkItemId (context : SqlDbContext) (workItemId : int) =
    async {
        let! entities = context.ToDoItems.AsNoTracking()
                                         .Where(fun wi -> wi.WorkItemId = workItemId && wi.DateDeleted = Nullable())
                                         .ToListAsync() |> Async.AwaitTask
        return entities |> List.ofSeq |> List.map ToDoItem.toModel
    }

let saveNewToDoItem (context : SqlDbContext) (todoItem) =
    async {
        context.ToDoItems.Add(todoItem |> ToDoItem.toEntity) |> ignore
        context.SaveChangesAsync() |> Async.AwaitTask |> ignore
        return getToDoItem context todoItem.ToDoItemId |> Async.RunSynchronously
    }

type SqlToDoRepository(context: SqlDbContext) =
    interface IToDoItemRepository with
        member this.Delete(id: int): Async<unit> = 
            deleteToDoItem context id
        member this.Get(id: int): Async<WhatShouldIWorkOnToday.Models.ToDoItem.ToDoItem option> = 
            getToDoItem context id
        member this.GetByWorkItemId(workItemId: int): Async<WhatShouldIWorkOnToday.Models.ToDoItem.ToDoItem list> = 
            getToDoItemsByWorkItemId context workItemId
        member this.Save(toDoItem: WhatShouldIWorkOnToday.Models.ToDoItem.ToDoItem): Async<WhatShouldIWorkOnToday.Models.ToDoItem.ToDoItem option> = 
            saveNewToDoItem context toDoItem
        
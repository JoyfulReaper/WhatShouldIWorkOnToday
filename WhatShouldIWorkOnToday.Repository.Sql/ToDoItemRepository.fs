module ToDoItemRepository

open WhatShouldIWorkOnToday.Repository.Sql.Entities
open WhatShouldIWorkOnToday.Repository
open WhatShouldIWorkOnToday.Repository.Sql.Mapping
open WhatShouldIWorkOnToday.Models
open Microsoft.EntityFrameworkCore
open System
open System.Linq

let getToDoItem (context : SqlDbContext) (id : int) =
    async {
        let! entity = context.ToDoItems.AsNoTracking()
                                       .Where(fun tdi -> tdi.ToDoItemId = id && tdi.DateDeleted = Nullable())
                                       .SingleOrDefaultAsync() 
                                       |> Async.AwaitTask
        return entity |> Option.ofObj |> Option.map ToDoItem.toModel
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
                                         .Where(fun ti -> ti.WorkItemId = workItemId && ti.DateDeleted = Nullable())
                                         .ToListAsync() |> Async.AwaitTask
        return entities |> List.ofSeq |> List.map ToDoItem.toModel
    }

let saveNewToDoItem (context : SqlDbContext) (todoItem) =
    async {
        context.ToDoItems.Add(todoItem |> ToDoItem.toEntity) |> ignore
        context.SaveChangesAsync() |> Async.AwaitTask |> ignore
        return getToDoItem context todoItem.ToDoItemId |> Async.RunSynchronously
    }

let completeToDoItem (context: SqlDbContext) (toDoItemId: int) =
    async {
        let item = context.ToDoItems.SingleOrDefault(fun tdi -> tdi.ToDoItemId = toDoItemId && tdi.DateDeleted = Nullable())
        match item with
        | null -> raise (System.Exception("ToDoItem not found"))
        | _ ->
            item.DateCompleted <- DateTime.Now
            context.SaveChangesAsync() |> Async.AwaitTask |> ignore 
    }

let updateToDoItem (context: SqlDbContext) (todoItem: ToDoItem.ToDoItem) =
    async {
        let! entity = context.ToDoItems.SingleOrDefaultAsync(fun ti -> ti.ToDoItemId = todoItem.ToDoItemId) |> Async.AwaitTask
        
        entity.Task <- todoItem.Task
        entity.DateCompleted <- Option.toNullable todoItem.DateCompleted

        context.SaveChangesAsync() |> Async.AwaitTask |> ignore
        return entity |> ToDoItem.toModel
    }

type SqlToDoRepository(context: SqlDbContext) =
    interface IToDoItemRepository with
        member this.Update(todoItem: WhatShouldIWorkOnToday.Models.ToDoItem.ToDoItem): Async<WhatShouldIWorkOnToday.Models.ToDoItem.ToDoItem> = 
            raise (System.NotImplementedException())
        member this.Delete(id: int): Async<unit> = 
            deleteToDoItem context id
        member this.Get(id: int): Async<WhatShouldIWorkOnToday.Models.ToDoItem.ToDoItem option> = 
            getToDoItem context id
        member this.GetByWorkItemId(workItemId: int): Async<WhatShouldIWorkOnToday.Models.ToDoItem.ToDoItem list> = 
            getToDoItemsByWorkItemId context workItemId
        member this.Save(toDoItem: WhatShouldIWorkOnToday.Models.ToDoItem.ToDoItem): Async<WhatShouldIWorkOnToday.Models.ToDoItem.ToDoItem option> = 
            saveNewToDoItem context toDoItem
        member this.Complete(toDoItemId: int): Async<unit> = 
            completeToDoItem context toDoItemId
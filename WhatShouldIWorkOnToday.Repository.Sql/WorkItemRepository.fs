module WorkItemRepository

open WhatShouldIWorkOnToday.Repository.Sql.Entities
open WhatShouldIWorkOnToday.Repository
open WhatShouldIWorkOnToday.Repository.Sql.Mapping
open Microsoft.EntityFrameworkCore
open WhatShouldIWorkOnToday.Models
open System.Linq
open System

let getWorkItem(context : SqlDbContext) (id : int) =
    async {
        let! workItem = context.WorkItems.AsNoTracking()
                                         .SingleOrDefaultAsync(fun wi -> wi.WorkItemId = id && wi.DateDeleted = Nullable()) |> Async.AwaitTask
        return workItem |> Option.ofObj |> Option.map WorkItem.toModel
    }

let getAllWorkItems(context : SqlDbContext) =
    async {
        let query = context.WorkItems.AsNoTracking().AsQueryable()
        let! workItems = query.Where(fun wi -> wi.DateDeleted = Nullable()).ToListAsync() |> Async.AwaitTask
        return workItems |> List.ofSeq |> List.map WorkItem.toModel
    }

let getWorkItemsBySequenceNumber (context: SqlDbContext) (sequenceNumber: int) =
    async {
        let query = context.WorkItems.AsNoTracking().AsQueryable()
        let! filteredQuery = query.Where(fun wi -> wi.SequenceNumber = Nullable sequenceNumber && wi.DateDeleted = Nullable()).ToListAsync() |> Async.AwaitTask
        return filteredQuery |> List.ofSeq |> List.map WorkItem.toModel
    }

let getAllCompletedWorkItems (context: SqlDbContext) =
    async {
        let query = context.WorkItems.AsQueryable()
        let! filteredQuery = query.Where(fun wi -> wi.DateCompleted.HasValue && wi.DateDeleted = Nullable()).ToListAsync() |> Async.AwaitTask
        return filteredQuery |> List.ofSeq |> List.map WorkItem.toModel
    }

let saveNewWorkItem (context : SqlDbContext) (workItem : WorkItem.WorkItem) =
    async {
        context.WorkItems.Add(workItem |> WorkItem.toEntity) |> ignore
        context.SaveChangesAsync() |> Async.AwaitTask |> ignore
        return getWorkItem context workItem.WorkItemId |> Async.RunSynchronously
    }

let deleteWorkItem (context : SqlDbContext) (id: int) =
    async {
        let! workItem = context.WorkItems.SingleOrDefaultAsync(fun wi -> wi.WorkItemId = id) |> Async.AwaitTask
        workItem.DateDeleted <- DateTime.Now
        context.SaveChangesAsync() |> Async.AwaitTask |> ignore
    }

type SqlWorkItemRepository(context: SqlDbContext) =
    interface IWorkItemRepository with
        member this.GetAll(): Async<WhatShouldIWorkOnToday.Models.WorkItem.WorkItem list> = 
            getAllWorkItems context
        member this.GetBySequenceNumber(seqeunceNumber: int): Async<WhatShouldIWorkOnToday.Models.WorkItem.WorkItem list> = 
            getWorkItemsBySequenceNumber context seqeunceNumber
        member this.GetCompleted(): Async<WhatShouldIWorkOnToday.Models.WorkItem.WorkItem list> = 
            getAllCompletedWorkItems context
        member this.Save(workItem: WhatShouldIWorkOnToday.Models.WorkItem.WorkItem): Async<WhatShouldIWorkOnToday.Models.WorkItem.WorkItem option> = 
           saveNewWorkItem context workItem
        member __.Get(id) =
            getWorkItem context id
        member __.Delete(id) =
            deleteWorkItem context id
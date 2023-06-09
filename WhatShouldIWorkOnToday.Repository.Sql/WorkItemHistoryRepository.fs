module WorkItemHistoryRepository

open WhatShouldIWorkOnToday.Repository.Sql.Entities
open WhatShouldIWorkOnToday.Repository.Sql.Mapping
open WhatShouldIWorkOnToday.Repository
open Microsoft.EntityFrameworkCore
open System.Linq
open WhatShouldIWorkOnToday.Models


let getWorkItemHistory (context : SqlDbContext) (workItemHistoryId : int) =
    async {
        let! entity = context.WorkItemHistories.AsNoTracking()
                                               .Where(fun wih -> wih.WorkItemHistoryId = workItemHistoryId)
                                               .SingleOrDefaultAsync() |> Async.AwaitTask
                      
        return entity |> Option.ofObj |> Option.map WorkItemHistory.toModel
    }

let getWorkItemHistoryByWorkItemId (context : SqlDbContext) (workItemId : int) =
    async {
        let! entities = context.WorkItemHistories.AsNoTracking()
                                                 .Where(fun wih -> wih.WorkItemId = workItemId)
                                                 .ToListAsync() |> Async.AwaitTask

        return entities |> List.ofSeq |> List.map WorkItemHistory.toModel
    }

let saveWorkItemHistory (context : SqlDbContext) (workItemHistory : WorkItemHistory.WorkItemHistory) =
    async {
        context.WorkItemHistories.Add(workItemHistory |> WorkItemHistory.toEntity) |> ignore
        context.SaveChangesAsync() |> Async.AwaitTask |> ignore
        return getWorkItemHistory context workItemHistory.WorkItemHistoryId |> Async.RunSynchronously
    }

type SqlWorkItemHistoryRepository(context : SqlDbContext) =
    interface IWorkItemHistoryRepository with
        member this.Get(workItemHistoryId: int): Async<WhatShouldIWorkOnToday.Models.WorkItemHistory.WorkItemHistory option> = 
            getWorkItemHistory context workItemHistoryId
        member this.GetByWorkItemId(workItemId: int): Async<WhatShouldIWorkOnToday.Models.WorkItemHistory.WorkItemHistory list> = 
            getWorkItemHistoryByWorkItemId context workItemId
        member this.Save(workItemHistory: WhatShouldIWorkOnToday.Models.WorkItemHistory.WorkItemHistory): Async<WhatShouldIWorkOnToday.Models.WorkItemHistory.WorkItemHistory option> = 
            raise (System.NotImplementedException())
        
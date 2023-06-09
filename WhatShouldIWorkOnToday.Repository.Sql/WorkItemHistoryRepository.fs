module WorkItemHistoryRepository

open WhatShouldIWorkOnToday.Repository.Sql.Entities
open WhatShouldIWorkOnToday.Repository.Sql.Mapping
open WhatShouldIWorkOnToday.Repository
open Microsoft.EntityFrameworkCore
open System.Linq


let getWorkItemHistory (context : SqlDbContext) (workItemHistoryId: int) =
    async {
        let! entity = context.WorkItemHistories.AsNoTracking().Where(fun wih -> wih.WorkItemHistoryId = workItemHistoryId).SingleOrDefaultAsync() 
                      |> Async.AwaitTask
        return entity |> Option.ofObj |> Option.map WorkItemHistory.toModel
    }

type SqlWorkItemHistoryRepository(context : SqlDbContext) =
    interface IWorkItemHistoryRepository with
        member this.Get(workItemHistoryId: int): Async<WhatShouldIWorkOnToday.Models.WorkItemHistory.WorkItemHistory option> = 
            raise (System.NotImplementedException())
        member this.GetByWorkItemId(workItemId: int): Async<WhatShouldIWorkOnToday.Models.WorkItemHistory.WorkItemHistory list> = 
            raise (System.NotImplementedException())
        member this.Save(workItemHistory: WhatShouldIWorkOnToday.Models.WorkItemHistory.WorkItemHistory): Async<WhatShouldIWorkOnToday.Models.WorkItemHistory.WorkItemHistory option> = 
            raise (System.NotImplementedException())
        
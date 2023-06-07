module WorkItemRepository

open WhatShouldIWorkOnToday.Repository.Sql.Entities
open WhatShouldIWorkOnToday.Repository
open WhatShouldIWorkOnToday.Repository.Sql.Mapping
open Microsoft.EntityFrameworkCore

let getWorkItem(context : SqlDbContext) (id : int) =
    async {
        let! workItem = context.WorkItems.SingleOrDefaultAsync(fun wi -> wi.WorkItemId = id) |> Async.AwaitTask
        return workItem |> Option.ofObj |> Option.map WorkItem.toModel
    }

type SqlWorkItemRepository(context: SqlDbContext) =
    interface IWorkItemRepository with
        member this.GetAll(): Async<WhatShouldIWorkOnToday.Models.WorkItem.WorkItem list> = 
            raise (System.NotImplementedException())
        member this.GetBySequenceNumber(arg1: int): Async<WhatShouldIWorkOnToday.Models.WorkItem.WorkItem option> = 
            raise (System.NotImplementedException())
        member this.GetCompleted(): Async<WhatShouldIWorkOnToday.Models.WorkItem.WorkItem list> = 
            raise (System.NotImplementedException())
        member this.Save(arg1: WhatShouldIWorkOnToday.Models.WorkItem.WorkItem): Async<WhatShouldIWorkOnToday.Models.WorkItem.WorkItem> = 
            raise (System.NotImplementedException())
        member __.Get(id) =
            getWorkItem context id
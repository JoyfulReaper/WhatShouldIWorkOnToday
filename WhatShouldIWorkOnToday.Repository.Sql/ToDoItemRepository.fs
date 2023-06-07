module ToDoItemRepository

open WhatShouldIWorkOnToday.Repository.Sql.Entities
open WhatShouldIWorkOnToday.Repository

type SqlToDoRepository(context: SqlDbContext) =
    interface IToDoItemRepository with
        member this.Delete(arg1: int): Async<unit> = 
            raise (System.NotImplementedException())
        member this.Get(arg1: int): Async<WhatShouldIWorkOnToday.Models.ToDoItem.ToDoItem option> = 
            raise (System.NotImplementedException())
        member this.GetByWorkItemId(arg1: int): Async<WhatShouldIWorkOnToday.Models.ToDoItem.ToDoItem list> = 
            raise (System.NotImplementedException())
        member this.Save(arg1: WhatShouldIWorkOnToday.Models.ToDoItem.ToDoItem): Async<WhatShouldIWorkOnToday.Models.ToDoItem.ToDoItem> = 
            raise (System.NotImplementedException())
        
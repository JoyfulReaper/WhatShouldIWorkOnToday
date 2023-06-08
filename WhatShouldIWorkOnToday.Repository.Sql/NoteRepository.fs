module NoteRepository

open WhatShouldIWorkOnToday.Repository.Sql.Entities
open WhatShouldIWorkOnToday.Repository

let deleteNote (context : SqlDbContext) (id : int) =
    async {
        let! note = context.FindAsync(id).AsTask() |> Async.AwaitTask
        context.Notes.Remove(note) |> ignore
        context.SaveChangesAsync() |> Async.AwaitTask |> ignore
    }
    

type SqlNoteRepository(context : SqlDbContext) =
    interface INoteRepository with
        member this.Delete(id: int): Async<unit> = 
            deleteNote context id
        member this.Get(arg1: int): Async<WhatShouldIWorkOnToday.Models.Note.Note option> = 
            raise (System.NotImplementedException())
        member this.GetByWorkItemId(arg1: int): Async<WhatShouldIWorkOnToday.Models.Note.Note list> = 
            raise (System.NotImplementedException())
        member this.Save(arg1: WhatShouldIWorkOnToday.Models.Note.Note): Async<WhatShouldIWorkOnToday.Models.Note.Note option> = 
            raise (System.NotImplementedException())
        
module NoteRepository

open WhatShouldIWorkOnToday.Repository.Sql.Entities
open WhatShouldIWorkOnToday.Repository
open WhatShouldIWorkOnToday.Repository.Sql.Mapping
open WhatShouldIWorkOnToday.Models
open System.Linq
open Microsoft.EntityFrameworkCore
open System

let getNote (context : SqlDbContext) (noteId : int) =
    async {
        let! note = context.Notes.AsNoTracking()
                                 .SingleOrDefaultAsync(fun n -> n.NoteId = noteId && n.DateDeleted = Nullable()) |> Async.AwaitTask
        return note |> Option.ofObj |> Option.map Note.toModel
    }

let deleteNote (context : SqlDbContext) (id : int) =
    async {
        let! note = context.FindAsync(id).AsTask() |> Async.AwaitTask
        context.Notes.Remove(note) |> ignore
        context.SaveChangesAsync() |> Async.AwaitTask |> ignore
    }
    
let getNoteByWorkItem (context : SqlDbContext) (workItemId : int) =
    async {
        let! note = context.Notes.AsNoTracking()
                                 .Where(fun n -> n.WorkItemId = workItemId && n.DateDeleted = Nullable()).ToListAsync() |> Async.AwaitTask
        return note |> List.ofSeq |> List.map Note.toModel
    }

let saveNewNote (context : SqlDbContext) (note : Note.Note) =
    async {
        context.Notes.Add(note |> Note.toEntity) |> ignore
        context.SaveChangesAsync() |> Async.AwaitTask |> ignore
        return getNote context note.NoteId |> Async.RunSynchronously
    }

type SqlNoteRepository(context : SqlDbContext) =
    interface INoteRepository with
        member this.Delete(id: int): Async<unit> = 
            deleteNote context id
        member this.Get(noteId: int): Async<WhatShouldIWorkOnToday.Models.Note.Note option> = 
            getNote context noteId
        member this.GetByWorkItemId(workItemId: int): Async<WhatShouldIWorkOnToday.Models.Note.Note list> = 
            getNoteByWorkItem context workItemId
        member this.Save(note: WhatShouldIWorkOnToday.Models.Note.Note): Async<WhatShouldIWorkOnToday.Models.Note.Note option> = 
            saveNewNote context note
        
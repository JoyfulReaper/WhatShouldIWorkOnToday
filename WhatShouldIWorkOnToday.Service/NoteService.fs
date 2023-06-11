module NoteService
open Giraffe
open Microsoft.AspNetCore.Http
open WhatShouldIWorkOnToday.Repository
open WhatShouldIWorkOnToday.Models

let getNoteHandler noteId : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let noteRepo = ctx.GetService<INoteRepository>()
            let! note = noteRepo.Get noteId
            match note with
            | None -> return! RequestErrors.NOT_FOUND { Message = sprintf "Work item: (id: %i) not found" noteId } next ctx
            | Some x -> return! Successful.OK (Note.toDto x) next ctx
        }

let gotNoteByWorkItemIdHandler workitemId : HttpHandler =
    fun (next : HttpFunc) (ctx: HttpContext) ->
        task {
            let noteRepo = ctx.GetService<INoteRepository>()
            let! notes = noteRepo.GetByWorkItemId workitemId
            return! Successful.OK (notes |> List.map Note.toDto) next ctx
        }

let saveNoteHandler : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let noteRepo = ctx.GetService<INoteRepository>()
            let! note = ctx.BindJsonAsync<NoteRequest>()
            let! savedNote = noteRepo.Save(Note.fromNoteRequest note)

            match savedNote with
            | Some note -> return! Successful.OK (Note.toDto note) next ctx
            | None -> return! RequestErrors.BAD_REQUEST { Message = "Failed to save note" } next ctx
        }

let deleteNoteHandler noteId : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
    task {
        let noteRepo = ctx.GetService<INoteRepository>()
        do! noteRepo.Delete noteId

        return! Successful.NO_CONTENT next ctx
    }
module SettingService
open Giraffe
open Microsoft.AspNetCore.Http
open WhatShouldIWorkOnToday.Repository

let getSeqeunceNumberHandler : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let settingsRepo = ctx.GetService<ISettingRepository>()
            let! seqNum = settingsRepo.GetSequenceNumber()
            return! json seqNum next ctx
        }

let getMaxSequenceNumberHandler : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            // TODO: Verify that there actually is a max seq num
            let settingsRepo = ctx.GetService<ISettingRepository>()
            let! maxSeqNum = settingsRepo.GetMaxSequenceNumber()
            return! json maxSeqNum next ctx
        }

let setSequenceNumberHandler (seqNum : int) : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            // TODO: Verify the seq num being set exists
            let settingsRepo = ctx.GetService<ISettingRepository>()
            do! settingsRepo.SetSequenceNumber seqNum
            return! json seqNum next ctx
        }
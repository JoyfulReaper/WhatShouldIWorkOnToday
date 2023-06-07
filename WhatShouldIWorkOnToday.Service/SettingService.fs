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

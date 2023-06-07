module SettingRepository

open WhatShouldIWorkOnToday.Repository
open WhatShouldIWorkOnToday.Repository.Sql.Entities
open WhatShouldIWorkOnToday.Repository.Sql.Entities.Entities
open Microsoft.EntityFrameworkCore
open System

let getSequenceNumber(context : SqlDbContext) =
    async {
        let! settings = context.Settings.SingleOrDefaultAsync() |> Async.AwaitTask
        if settings = null then
            let newSettings = Setting(CurrentSequence = 1, DateSet = DateTime.Now)
            context.Settings.Add(newSettings) |> ignore
            context.SaveChangesAsync() |> Async.AwaitTask |> ignore
            return newSettings.CurrentSequence
        else
            return settings.CurrentSequence
    }

let getMaxSequenceNumber(context : SqlDbContext) =
    async {
        return! context.WorkSequenceNumbers.MaxAsync(fun ws -> ws.SequenceNumber) |> Async.AwaitTask
    }

let setSequenceNumber(context : SqlDbContext) (sequenceNumber : int) =
    async {
        let! settings = context.Settings.SingleOrDefaultAsync() |> Async.AwaitTask
        if settings = null then
            let newSettings = Setting(CurrentSequence = sequenceNumber, DateSet = DateTime.Now)
            context.Settings.Add(newSettings) |> ignore
        else
            settings.CurrentSequence <- sequenceNumber
        context.SaveChangesAsync() |> Async.AwaitTask |> ignore
    }

type SqlSettingRepository(context : SqlDbContext) =
    interface ISettingRepository with
        member __.GetSequenceNumber() =
            getSequenceNumber context

        member __.GetMaxSequenceNumber() =
            getMaxSequenceNumber context

        member __.SetSequenceNumber(sequenceNumber) =
            setSequenceNumber context sequenceNumber
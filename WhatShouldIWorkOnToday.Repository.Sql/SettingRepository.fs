module SettingRepository

open WhatShouldIWorkOnToday.Repository

let getSequenceNumber() =
    1

let getMaxSequenceNumber() =
    1

let setSequenceNumber(sequenceNumber : int) =
    ()

type SqlSettingRepository() =
    interface ISettingRepository with
        member __.GetSequenceNumber() =
            getSequenceNumber()

        member __.GetMaxSequenceNumber() =
            getMaxSequenceNumber()

        member __.SetSequenceNumber(sequenceNumber) =
            setSequenceNumber(sequenceNumber)
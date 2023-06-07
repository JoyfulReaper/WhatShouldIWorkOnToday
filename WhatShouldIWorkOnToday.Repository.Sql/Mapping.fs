namespace WhatShouldIWorkOnToday.Repository.Sql.Mapping
open WhatShouldIWorkOnToday.Repository.Sql.Entities.Entities
open WhatShouldIWorkOnToday.Models
open System

module WorkItem =
    let toEntity(workItem: WorkItem.WorkItem) : WorkItem =
        WorkItem(Name = workItem.Name,
                 Description = Option.toObj workItem.Description, // HOW TO DO IT WITHOUT A MATCH!! -FINDME-
                 Url = Option.toObj workItem.Url,
                 Pinned = workItem.Pinned,
                 SequenceNumber = Option.toNullable workItem.SequenceNumber,
                 DateCreated = workItem.DateCreated,
                 DateCompleted = Option.toNullable workItem.DateCompleted)

    let toModel(entity: WorkItem) : WorkItem.WorkItem =
        { WorkItemId = entity.WorkItemId
          Name = entity.Name
          Description = match entity.Description with
                        | null -> None
                        | x -> Some x
          Url = match entity.Url with
                | null -> None
                | x -> Some x
          Pinned = entity.Pinned
          SequenceNumber = match entity.SequenceNumber with
                           | sn when sn.HasValue -> Some (sn.Value)
                           | _ -> None
          DateCreated = entity.DateCreated
          DateCompleted = match entity.DateCompleted with
                          | dc when dc.HasValue -> Some (dc.Value)
                          | _ -> None
        }
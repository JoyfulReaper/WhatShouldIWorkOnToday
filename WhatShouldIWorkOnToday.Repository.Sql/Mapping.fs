namespace WhatShouldIWorkOnToday.Repository.Sql.Mapping
open WhatShouldIWorkOnToday.Repository.Sql.Entities.Entities
open WhatShouldIWorkOnToday.Models

module WorkItem =
    let toEntity(dto: WorkItemDto) : WorkItem =
        WorkItem(
            WorkItemId = dto.WorkItemId,
            Name = dto.Name,
            Description = dto.Description,
            Url = dto.Url,
            Pinned = dto.Pinned,
            SequenceNumber = dto.SequenceNumber,
            DateCreated = dto.DateCreated,
            DateCompleted = dto.DateCompleted)

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
          DateWorkedOn = match entity.DateWorkedOn with
                         | dwo when dwo.HasValue -> Some (dwo.Value)
                         | _ -> None
          DateCompleted = match entity.DateCompleted with
                          | dc when dc.HasValue -> Some (dc.Value)
                          | _ -> None
        }
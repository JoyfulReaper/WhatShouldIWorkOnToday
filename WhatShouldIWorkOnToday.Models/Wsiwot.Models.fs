namespace WhatShouldIWorkOnToday.Models

module Note =
    type Note =
        { NoteId: int
          WorkItemId: int
          Text: string
          DateCreated: System.DateTime }

    let toDto (note: Note) : NoteDto =
        { NoteId = note.NoteId
          WorkItemId = note.WorkItemId
          Text = note.Text
          DateCreated = note.DateCreated }

    let fromDto (noteDto: NoteDto) : Note =
        { NoteId = noteDto.NoteId
          WorkItemId = noteDto.WorkItemId
          Text = noteDto.Text
          DateCreated = noteDto.DateCreated }

    let fromNoteRequest (noteRequest : NoteRequest) : Note =
        { NoteId = 0
          WorkItemId = noteRequest.WorkItemId
          Text = noteRequest.Text
          DateCreated = System.DateTime.Now }
        

module ToDoItem =
    type ToDoItem =
        { ToDoItemId: int
          WorkItemId: int
          Task: string
          DateAdded: System.DateTime
          DateCompleted: System.DateTime option }

    let toDto (todoItem: ToDoItem) : ToDoItemDto =
        { ToDoItemId = todoItem.ToDoItemId
          WorkItemId = todoItem.WorkItemId
          Task = todoItem.Task
          DateAdded = todoItem.DateAdded
          DateCompleted = Option.toNullable todoItem.DateCompleted
        }

    let fromDto (todoItemDto: ToDoItemDto) : ToDoItem =
            { ToDoItemId = todoItemDto.ToDoItemId
              WorkItemId = todoItemDto.WorkItemId
              Task = todoItemDto.Task
              DateAdded = todoItemDto.DateAdded
              DateCompleted = Option.ofNullable todoItemDto.DateCompleted
            }

    let fromTodoItemRequest (request : ToDoItemRequest) : ToDoItem =
        { ToDoItemId = 0
          WorkItemId = request.WorkItemId
          Task = request.Task
          DateAdded = System.DateTime.Now
          DateCompleted = None }

module Setting =
    type Setting =
        { SettingId: int
          CurrentSequence: int
          DateSet: System.DateTime }

module WorkItem =
    type WorkItem =
        { WorkItemId: int
          Name: string
          Description: string option
          Url: string option
          Pinned: bool
          SequenceNumber: int option
          DateCreated: System.DateTime
          DateCompleted: System.DateTime option
          DateWorkedOn: System.DateTime option }

    let toDto (workItem: WorkItem) : WorkItemDto =
        { WorkItemId = workItem.WorkItemId
          Name = workItem.Name
          Description = match workItem.Description with
                        | None -> null
                        | Some x -> x
          Url = match workItem.Url with
                | None -> null
                | Some x -> x
          Pinned = workItem.Pinned
          SequenceNumber = Option.toNullable workItem.SequenceNumber
          DateCreated = workItem.DateCreated
          DateWorkedOn = Option.toNullable workItem.DateWorkedOn
          DateCompleted = match workItem.DateCompleted with
                          | None -> System.Nullable()
                          | Some x -> System.Nullable(x)
        }

    let fromDto (workItemDto: WorkItemDto) : WorkItem =
        { WorkItemId = workItemDto.WorkItemId
          Name = workItemDto.Name
          Description = match workItemDto.Description with
                        | null -> None
                        | x -> Some x
          Url = match workItemDto.Url with
                | null -> None
                | x -> Some x
          Pinned = workItemDto.Pinned
          SequenceNumber = match workItemDto.SequenceNumber with
                           | sn when sn.HasValue -> Some (sn.Value)
                           | _ -> None
          DateCreated = workItemDto.DateCreated
          DateWorkedOn = Option.ofNullable workItemDto.DateWorkedOn
          DateCompleted = match workItemDto.DateCompleted with
                          | dc when dc.HasValue -> Some (dc.Value)
                          | _ -> None
        }

    let fromWorkItemRequest (request : WorkItemRequest) : WorkItem =
        { WorkItemId = 0
          Name = request.Name
          Description = Some request.Description
          Url = Some request.Url
          Pinned = request.Pinned
          SequenceNumber = None
          DateCreated = System.DateTime.Now
          DateWorkedOn = None
          DateCompleted = None }

module WorkItemHistory =
    type WorkItemHistory =
        { WorkItemHistoryId: int
          WorkItemId: int
          DateWorkedOn: System.DateTime
        }

    let toDto (workItemHistory: WorkItemHistory) : WorkItemHistoryDto =
        { WorkItemHistoryId = workItemHistory.WorkItemHistoryId
          WorkItemId = workItemHistory.WorkItemId
          DateWorkedOn = workItemHistory.DateWorkedOn
        }
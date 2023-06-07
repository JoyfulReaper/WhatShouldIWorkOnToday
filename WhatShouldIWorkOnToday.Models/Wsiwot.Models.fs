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
        

module ToDoItem =
    type ToDoItem =
        { ToDoItemId: int
          WorkItemId: int
          Task: string
          DateAddded: System.DateTime
          DateCompleted: System.DateTime option }

    let toDto (todoItem: ToDoItem) : ToDoItemDto =
        { ToDoItemId = todoItem.ToDoItemId
          WorkItemId = todoItem.WorkItemId
          Task = todoItem.Task
          DateAddded = todoItem.DateAddded
          DateCompleted = match todoItem.DateCompleted with
                          | None -> System.Nullable() 
                          | Some x -> System.Nullable(x)
        }

    let fromDto (todoItemDto: ToDoItemDto) : ToDoItem =
            { ToDoItemId = todoItemDto.ToDoItemId
              WorkItemId = todoItemDto.WorkItemId
              Task = todoItemDto.Task
              DateAddded = todoItemDto.DateAddded
              DateCompleted = match todoItemDto.DateCompleted with
                              | dc  when dc.HasValue -> Some (dc.Value)
                              | _ -> None
            }

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
          DateWorkedOn: System.DateTime option
          DateCompleted: System.DateTime option }

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
          SequenceNumber = match workItem.SequenceNumber with
                           | None -> System.Nullable()
                           | Some x -> System.Nullable(x)
          DateCreated = workItem.DateCreated
          DateWorkedOn = match workItem.DateWorkedOn with
                         | None -> System.Nullable()
                         | Some x -> System.Nullable(x)
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
          DateWorkedOn = match workItemDto.DateWorkedOn with
                         | dc when dc.HasValue -> Some (dc.Value)
                         | _ -> None
          DateCompleted = match workItemDto.DateCompleted with
                          | dc when dc.HasValue -> Some (dc.Value)
                          | _ -> None
        }
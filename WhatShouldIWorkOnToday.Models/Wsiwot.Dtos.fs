namespace WhatShouldIWorkOnToday.Models

open System

[<CLIMutable>]
type NoteDto =
    { NoteId: int
      WorkItemId: int
      Text: string
      DateCreated: DateTime }

[<CLIMutable>]
 type NoteRequest =
    { WorkItemId: int
      Text: string }

[<CLIMutable>]
type ToDoItemDto =
    { ToDoItemId: int
      WorkItemId: int
      Task: string
      DateAddded: DateTime
      DateCompleted: Nullable<DateTime> }

[<CLIMutable>]
type ToDoItemRequest =
    { WorkItemId: int
      Task: string }

[<CLIMutable>]
type WorkItemDto =
   { WorkItemId: int
     Name: string
     Description: string
     Url: string
     Pinned: bool
     DateCreated: DateTime
     DateWorkedOn: Nullable<DateTime>
     DateCompleted: Nullable<DateTime> }

[<CLIMutable>]
type WorkItemRequest =
    { Name: string
      Description: string
      Url: string
      Pinned: bool }
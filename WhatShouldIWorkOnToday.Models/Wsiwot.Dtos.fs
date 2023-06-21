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
      DateAdded: DateTime
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
     SequenceNumber: Nullable<int>
     DateCreated: DateTime
     DateCompleted: Nullable<DateTime>
     DateWorkedOn: Nullable<DateTime> }

[<CLIMutable>]
type WorkItemRequest =
    { Name: string
      Description: string
      Url: string
      Pinned: bool }

[<CLIMutable>]
type ErrorResponse =
    { Message: string }

[<CLIMutable>]
type WorkItemHistoryDto =
    { WorkItemHistoryId: int
      WorkItemId: int
      DateWorkedOn: DateTime }
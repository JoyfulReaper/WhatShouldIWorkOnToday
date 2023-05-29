module Dtos

    [<CLIMutable>]
    type NoteDto =
        { NoteId: int
          WorkItemId: int
          Text: string
          DateCreated: System.DateTime }

    [<CLIMutable>]
    type NoteRequest =
        { WorkItemId: int
          Text: string }

    [<CLIMutable>]
    type ToDoItemDto =
        { ToDoItemId: int
          WorkItemId: int
          Task: string
          DateAddded: System.DateTime
          DateCompleted: System.DateTime option }

    [<CLIMutable>]
    type ToDoItemRequest =
        { WorkItemId: int
          Task: string }

    [<CLIMutable>]
    type WorkItemDto =
        { WorkItemId: int
          Name: string
          Description: string option
          Url: string option
          Pinned: bool
          DateCreated: System.DateTime
          DateWorkedOn: System.DateTime option
          DateCompleted: System.DateTime option }

    [<CLIMutable>]
    type WorkItemRequest =
        { Name: string
          Description: string option
          Url: string option
          Pinned: bool }
namespace WhatShouldIWorkOnToday.Domain

type Note =
    { NoteId: int
      WorkItemId: int
      Text: string
      DateCreated: System.DateTime }

type ToDoItem =
    { ToDoItemId: int
      WorkItemId: int
      Task: string
      DateAddded: System.DateTime
      DateCompleted: System.DateTime option }

type Setting =
    { SettingId: int
      CurrentSequence: int
      DateSet: System.DateTime }

type WorkItem =
    { WorkItemId: int
      Name: string
      Description: string option
      Url: string option
      Pinned: bool
      DateCreated: System.DateTime
      DateWorkedOn: System.DateTime option
      DateCompleted: System.DateTime option }
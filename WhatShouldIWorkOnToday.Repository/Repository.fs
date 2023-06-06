namespace WhatShouldIWorkOnToday.Repository
open WhatShouldIWorkOnToday.Models

type ISettingRepository =
    abstract member GetSequenceNumber : unit -> int
    abstract member GetMaxSequenceNumber : unit -> int
    abstract member SetSequenceNumber : int -> unit

type INoteRepository =
    abstract member Get : int -> NoteDto option
    abstract member GetByWorkItemId : int -> NoteDto list
    abstract member Save : NoteDto -> unit

type IToDoItemRepository =
    abstract member Get : int -> ToDoItemDto option
    abstract member GetByWorkItemId : int -> ToDoItemDto list
    abstract member Save : ToDoItemDto -> unit
    abstract member Delete : int -> unit

type IWorkItemRepository =
    abstract member Get : int -> WorkItemDto option
    abstract member GetAll : unit -> WorkItemDto list
    abstract member Save : WorkItemDto -> unit
    abstract member GetCompleted : unit -> WorkItemDto list
    abstract member GetIncomplete : unit -> WorkItemDto list
    abstract member GetBySequenceNumber : int -> WorkItemDto option
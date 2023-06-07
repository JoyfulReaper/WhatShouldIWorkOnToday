namespace WhatShouldIWorkOnToday.Repository
open WhatShouldIWorkOnToday.Models

type ISettingRepository =
    abstract member GetSequenceNumber : unit -> Async<int>
    abstract member GetMaxSequenceNumber : unit -> Async<int>
    abstract member SetSequenceNumber : int -> Async<unit>

type INoteRepository =
    abstract member Get : int -> Async<NoteDto option>
    abstract member GetByWorkItemId : int -> Async<NoteDto list> 
    abstract member Save : Note.Note -> Async<NoteDto>

type IToDoItemRepository =
    abstract member Get : int -> Async<ToDoItemDto option>
    abstract member GetByWorkItemId : int -> Async<ToDoItemDto list>
    abstract member Save : ToDoItem.ToDoItem -> Async<ToDoItemDto>
    abstract member Delete : int -> Async<unit>

type IWorkItemRepository =
    abstract member Get : int -> Async<WorkItemDto option>
    abstract member GetAll : unit -> Async<WorkItemDto list>
    abstract member Save : WorkItem.WorkItem -> Async<WorkItemDto>
    abstract member GetCompleted : unit -> Async<WorkItemDto list>
    abstract member GetIncomplete : unit -> Async<WorkItemDto list>
    abstract member GetBySequenceNumber : int -> Async<WorkItemDto option>
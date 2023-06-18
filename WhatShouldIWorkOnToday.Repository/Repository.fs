namespace WhatShouldIWorkOnToday.Repository
open WhatShouldIWorkOnToday.Models

type ISettingRepository =
    abstract member GetSequenceNumber : unit -> Async<int>
    abstract member GetMaxSequenceNumber : unit -> Async<int>
    abstract member SetSequenceNumber : int -> Async<unit>

type INoteRepository =
    abstract member Get : int -> Async<Note.Note option>
    abstract member GetByWorkItemId : int -> Async<Note.Note list> 
    abstract member Save : Note.Note -> Async<Note.Note option>
    abstract member Delete : int -> Async<unit>

type IToDoItemRepository =
    abstract member Get : int -> Async<ToDoItem.ToDoItem option>
    abstract member GetByWorkItemId : int -> Async<ToDoItem.ToDoItem list>
    abstract member Save : ToDoItem.ToDoItem -> Async<ToDoItem.ToDoItem option>
    abstract member Delete : int -> Async<unit>
    abstract member Complete : int -> Async<unit>

type IWorkItemRepository =
    abstract member Get : int -> Async<WorkItem.WorkItem option>
    abstract member GetAll : unit -> Async<WorkItem.WorkItem list>
    abstract member Save : WorkItem.WorkItem -> Async<WorkItem.WorkItem option>
    abstract member Update : WorkItem.WorkItem -> Async<WorkItem.WorkItem>
    abstract member GetCompleted : unit -> Async<WorkItem.WorkItem list>
    abstract member GetBySequenceNumber : int -> Async<WorkItem.WorkItem list>
    abstract member Delete : int -> Async<unit>
    abstract member Search : string -> Async<WorkItem.WorkItem list>

type IWorkItemHistoryRepository =
    abstract member Get : int -> Async<WorkItemHistory.WorkItemHistory option>
    abstract member GetByWorkItemId : int -> Async<WorkItemHistory.WorkItemHistory list>
    abstract member Save : WorkItemHistory.WorkItemHistory -> Async<WorkItemHistory.WorkItemHistory option>
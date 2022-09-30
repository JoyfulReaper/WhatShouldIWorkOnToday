CREATE TABLE [dbo].[ToDoItem]
(
	[ToDoItemId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [WorkItemId] INT NOT NULL, 
    [Task] VARCHAR(300) NOT NULL, 
    [DateAdded] DATETIME2 NOT NULL DEFAULT GETUTCDATE(), 
    [DateCompleted] DATETIME2 NULL, 
    [DateDeleted] DATETIME2 NULL, 
    CONSTRAINT [FK_ToDoItem_WorkItem] FOREIGN KEY ([WorkItemId]) REFERENCES [WorkItem]([WorkItemId])
)

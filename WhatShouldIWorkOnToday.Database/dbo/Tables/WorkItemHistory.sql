CREATE TABLE [dbo].[WorkItemHistory]
(
	[WorkItemHistoryId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [WorkItemId] INT NOT NULL, 
    [DateWorkedOn] DATETIME2 NOT NULL DEFAULT GETUTCDATE(), 
    CONSTRAINT [FK_WorkItemHistory_WorkItem] FOREIGN KEY ([WorkItemId]) REFERENCES [WorkItem]([WorkItemId])
)

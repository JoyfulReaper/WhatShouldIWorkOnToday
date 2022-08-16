CREATE TABLE [dbo].[PinnedWorkItem]
(
	[PinnedWorkItemId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [WorkItemId] INT NOT NULL, 
    [DatePinned] DATETIME2 NOT NULL DEFAULT GETUTCDATE(), 
    [DateUnpinned] DATETIME2 NULL, 
    CONSTRAINT [FK_PinnedWorkItem_WorkItem] FOREIGN KEY ([WorkItemId]) REFERENCES [WorkItem]([WorkItemId])
)

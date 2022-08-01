CREATE TABLE [dbo].[WorkSequenceNumber]
(
	[WorkSequenceNumberId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [WorkItemId] INT NOT NULL, 
    [SequenceNumber] INT NOT NULL UNIQUE, 
    CONSTRAINT [FK_WorkSequenceNumber_WorkItem] FOREIGN KEY ([WorkItemId]) REFERENCES [WorkItem]([WorkItemId])
)

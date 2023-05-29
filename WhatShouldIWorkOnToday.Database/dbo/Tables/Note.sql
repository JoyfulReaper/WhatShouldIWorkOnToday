﻿CREATE TABLE [dbo].[Note]
(
	[NoteId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [WorkItemId] INT NOT NULL, 
    [Text] NVARCHAR(2000) NOT NULL, 
    [DateCreated] DATETIME2 NOT NULL DEFAULT GETUTCDATE(), 
    [DateDeleted] DATETIME2 NULL, 
    CONSTRAINT [FK_Note_WorkItem] FOREIGN KEY ([WorkItemId]) REFERENCES [WorkItem]([WorkItemId])
)

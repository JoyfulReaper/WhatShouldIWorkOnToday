CREATE TABLE [dbo].[WorkItem]
(
	[WorkItemId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(300) NOT NULL, 
    [Description] NVARCHAR(500) NULL, 
    [Url] NVARCHAR(300) NULL,
    [Pinned] BIT NOT NULL DEFAULT 0,
    [SequenceNumber] INT NULL,
    [DateCreated] DATETIME2 NOT NULL DEFAULT GETUTCDATE(), 
    [DateWorkedOn] DATETIME2 NULL, 
    [DateDeleted] DATETIME2 NULL, 
    [DateCompleted] DATETIME2 NULL
)

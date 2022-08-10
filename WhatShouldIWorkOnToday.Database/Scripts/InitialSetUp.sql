CREATE TABLE [dbo].[WorkItem]
(
	[WorkItemId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(300) NOT NULL, 
    [Description] NVARCHAR(500) NULL, 
    [Url] NVARCHAR(300) NULL, 
    [DateCreated] DATETIME2 NOT NULL DEFAULT GETUTCDATE(), 
    [DateWorkedOn] DATETIME2 NULL, 
    [DateDeleted] DATETIME2 NULL, 
    [DateCompleted] DATETIME2 NULL
)
GO

CREATE TABLE [dbo].[WorkSequenceNumber]
(
	[WorkSequenceNumberId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [WorkItemId] INT NOT NULL, 
    [SequenceNumber] INT NOT NULL, 
    CONSTRAINT [FK_WorkSequenceNumber_WorkItem] FOREIGN KEY ([WorkItemId]) REFERENCES [WorkItem]([WorkItemId])
)
GO


CREATE TABLE [dbo].[CurrentSequenceNumber]
(
	[CurrentSequenceNumberId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CurrentSequence] INT NOT NULL, 
    [DateSet] DATE NOT NULL DEFAULT GETUTCDATE()
)
GO

CREATE PROCEDURE [dbo].[spWorkSequenceNumber_Upsert]
	@WorkSequenceNumberId INT,
	@WorkItemId INT,
	@SequenceNumber INT
AS
BEGIN
	BEGIN TRANSACTION;
 
	INSERT dbo.WorkSequenceNumber([WorkItemId], [SequenceNumber])
	  SELECT @WorkItemId, @SequenceNumber
	  WHERE NOT EXISTS
	  (
		SELECT 1 FROM dbo.WorkSequenceNumber WITH (UPDLOCK, SERIALIZABLE)
		  WHERE [WorkSequenceNumberId] = @WorkSequenceNumberId
	  );
 
	IF @@ROWCOUNT = 0
		BEGIN
		  UPDATE dbo.WorkSequenceNumber 
		  SET [WorkItemId] = @WorkItemId, [SequenceNumber] = @SequenceNumber
		  WHERE [WorkSequenceNumberId] = @WorkSequenceNumberId;
		END
	ELSE
		BEGIN
			SET @WorkSequenceNumberId = SCOPE_IDENTITY();
		END
 
	COMMIT TRANSACTION;

	SELECT @WorkSequenceNumberId;
END
GO

CREATE PROCEDURE [dbo].[spWorkItemSequence_GetAll]
AS
BEGIN
	SELECT
		[wsn].[WorkSequenceNumberId], 
		[wi].[WorkItemId],
		ISNULL([wsn].[SequenceNumber], -1) AS SequenceNumber,
		[wi].[Name], 
		[wi].[Description],
		[wi].[Url],
		[wi].[DateCreated], 
		[wi].[DateWorkedOn],
		[wi].[DateCompleted]
	FROM
		WorkItem wi LEFT JOIN
		WorkSequenceNumber wsn ON wsn.WorkItemId = wi.WorkItemId
	WHERE
		wi.DateDeleted IS NULL
	AND wi.DateCompleted IS NULL
	ORDER BY
		wsn.SequenceNumber;

END
GO

CREATE PROCEDURE [dbo].[spWorkItemSequence_Get]
	@WorkSequenceNumberId INT
AS
BEGIN
	SELECT
		[wsn].[WorkSequenceNumberId], 
		[wi].[WorkItemId],
		ISNULL([wsn].[SequenceNumber], -1) AS SequenceNumber,
		[wi].[Name], 
		[wi].[Description],
		[wi].[Url],
		[wi].[DateCreated], 
		[wi].[DateWorkedOn],
		[wi].[DateCompleted]
	FROM
		WorkItem wi LEFT JOIN
		WorkSequenceNumber wsn ON wsn.WorkItemId = wi.WorkItemId
	WHERE
		wi.DateDeleted IS NULL
	AND wi.DateCompleted IS NULL
	AND wsn.WorkSequenceNumberId = @WorkSequenceNumberId

END
GO

CREATE PROCEDURE [dbo].[spWorkItem_Upsert]
	@WorkItemId INT,
	@Name NVARCHAR(300),
	@Description NVARCHAR(500),
	@Url NVARCHAR(300),
	@DateDeleted DATETIME2(7),
	@DateCompleted DATETIME2(7),
	@DateWorkedOn DATETIME2(7)
AS
BEGIN

	BEGIN TRANSACTION;
 
	INSERT dbo.WorkItem
		([Name]
		,[Description]
		,[Url]
		,[DateDeleted]
		,[DateCompleted]
		,[DateWorkedOn])
	  SELECT 
		@Name
		,@Description
		,@Url
		,@DateDeleted
		,@DateCompleted
		,@DateWorkedOn
	  WHERE NOT EXISTS
	  (
		SELECT 1 FROM dbo.WorkItem WITH (UPDLOCK, SERIALIZABLE)
		  WHERE [WorkItemId] = @WorkItemId
	  );
 
	IF @@ROWCOUNT = 0
		BEGIN
		  UPDATE dbo.WorkItem 
		  SET 
			[Name] = @Name
			,[Description] = @Description
			,[Url] = @Url
			,[DateDeleted] = @DateDeleted
			,[DateCompleted] = @DateCompleted
			,[DateWorkedOn] = @DateWorkedOn
		  WHERE 
			[WorkItemId] = @WorkItemId;
		END
	ELSE
		BEGIN
			SET @WorkItemId = SCOPE_IDENTITY();
		END
 
	COMMIT TRANSACTION;

	SELECT @WorkItemId;

END
GO

CREATE PROCEDURE [dbo].[spWorkItem_GetIncomplete]
AS
BEGIN
	SELECT
		[WorkItemId], [Name], [Description], [Url], [DateCreated], [DateWorkedOn], [DateDeleted], [DateCompleted]
	FROM
		WorkItem
	WHERE
		DateCompleted IS NOT NULL
	AND DateDeleted IS NULL
END
GO

CREATE PROCEDURE [dbo].[spWorkItem_GetComplete]
AS
BEGIN
	SELECT
		[WorkItemId], [Name], [Description], [Url], [DateCreated], [DateWorkedOn], [DateDeleted], [DateCompleted]
	FROM
		WorkItem
	WHERE
		DateCompleted IS NULL
	AND DateDeleted IS NULL
END
GO

CREATE PROCEDURE [dbo].[spWorkItem_GetBySequenceNumber]
	@SequenceNumber INT
AS
BEGIN
	SELECT
		[wi].[WorkItemId], [wi].[Name], [wi].[Description], [wi].[Url], [wi].[DateCreated], [wi].[DateWorkedOn], [wi].[DateDeleted], [wi].[DateCompleted]
	FROM
		WorkSequenceNumber wsn INNER JOIN
		WorkItem wi on wsn.WorkItemId = wi.WorkItemId
	WHERE
		wsn.SequenceNumber = @SequenceNumber
	AND wi.DateCompleted IS NULL
	AND wi.DateDeleted IS NULL;
END
GO

CREATE PROCEDURE [dbo].[spWorkItem_GetAll]
AS
BEGIN

	SET NOCOUNT ON;

	SELECT
		[WorkItemId]
		,[Name]
		,[Description]
		,[Url]
		,[DateCreated]
		,[DateWorkedOn]
		,[DateCompleted]
		,[DateDeleted]
	FROM
		WorkItem
	WHERE
		DateDeleted IS NULL;

END
GO

CREATE PROCEDURE [dbo].[spWorkItem_Get]
	@WorkItemId INT
AS
BEGIN
	SELECT
		[WorkItemId]
		,[Name]
		,[Description]
		,[Url]
		,[DateCreated]
		,[DateWorkedOn]
		,[DateDeleted]
		,[DateCompleted]
	FROM
		WorkItem
	WHERE
		WorkItemId = @WorkItemId
	AND DateDeleted IS NULL;
END
GO

CREATE PROCEDURE [dbo].[spGetMaxSequenceNumber]
AS
BEGIN
	SET NOCOUNT ON;

    SELECT
		MAX(SequenceNumber)
	FROM
		WorkSequenceNumber
END
GO

CREATE PROCEDURE [dbo].[spCurrentSequenceNumber_Update]
	@CurrentSequence INT,
	@DateSet DateTime2(7)
AS
BEGIN
	BEGIN TRANSACTION;
	
		UPDATE dbo.CurrentSequenceNumber WITH (UPDLOCK, SERIALIZABLE)
		SET CurrentSequence = @CurrentSequence,
			DateSet = @DateSet;
			
		IF @@ROWCOUNT = 0
		BEGIN
			INSERT dbo.CurrentSequenceNumber
			(CurrentSequence, DateSet)
			VALUES (@CurrentSequence, @DateSet)
		END

	COMMIT TRANSACTION;
END
GO

CREATE PROCEDURE [dbo].[spCurrentSequenceNumber_Get]
AS
BEGIN
	SELECT
		[CurrentSequenceNumberId]
		,[CurrentSequence]
		,[DateSet]
	FROM
		CurrentSequenceNumber;
END
GO

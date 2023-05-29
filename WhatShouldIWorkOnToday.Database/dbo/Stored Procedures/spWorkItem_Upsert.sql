CREATE PROCEDURE [dbo].[spWorkItem_Upsert]
	@WorkItemId INT,
	@Name NVARCHAR(300),
	@Description NVARCHAR(500),
	@Url NVARCHAR(300),
	@Pinned BIT,
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
		,[Pinned]
		,[DateDeleted]
		,[DateCompleted]
		,[DateWorkedOn])
	  SELECT 
		@Name
		,@Description
		,@Url
		,@Pinned
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
			,[Pinned] = @Pinned
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
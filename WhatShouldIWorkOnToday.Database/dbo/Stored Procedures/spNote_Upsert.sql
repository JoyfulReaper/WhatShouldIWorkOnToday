CREATE PROCEDURE [dbo].[spNote_Upsert]
	@NoteId INT,
	@WorkItemId INT,
	@Text NVARCHAR(2000)
AS
BEGIN

	BEGIN TRANSACTION;
 
	INSERT dbo.Note
		([WorkItemId]
		,[Text])
	  SELECT 
		@WorkItemId
		,@Text
	  WHERE NOT EXISTS
	  (
		SELECT 1 FROM dbo.Note WITH (UPDLOCK, SERIALIZABLE)
		  WHERE [NoteId] = @NoteId
	  );
 
	IF @@ROWCOUNT = 0
		BEGIN
		  UPDATE dbo.Note 
		  SET 
			[WorkItemId] = @WorkItemId
			,[Text] = @Text
		  WHERE 
			[NoteId] = @NoteId;
		END
	ELSE
		BEGIN
			SET @NoteId = SCOPE_IDENTITY();
		END
 
	COMMIT TRANSACTION;

	SELECT @NoteId;

END
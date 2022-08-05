CREATE PROCEDURE [dbo].[spWorkItemSequence_Upsert]
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
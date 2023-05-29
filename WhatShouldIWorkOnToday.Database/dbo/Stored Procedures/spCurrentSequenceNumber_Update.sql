CREATE PROCEDURE [dbo].[spCurrentSequenceNumber_Update]
	@CurrentSequence INT,
	@DateSet DateTime2(7)
AS
BEGIN
	BEGIN TRANSACTION;
	
		UPDATE dbo.Settings WITH (UPDLOCK, SERIALIZABLE)
		SET CurrentSequence = @CurrentSequence,
			DateSet = @DateSet;
			
		IF @@ROWCOUNT = 0
		BEGIN
			INSERT dbo.Settings
			(CurrentSequence, DateSet)
			VALUES (@CurrentSequence, @DateSet)
		END

	COMMIT TRANSACTION;
END
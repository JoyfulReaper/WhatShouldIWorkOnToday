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
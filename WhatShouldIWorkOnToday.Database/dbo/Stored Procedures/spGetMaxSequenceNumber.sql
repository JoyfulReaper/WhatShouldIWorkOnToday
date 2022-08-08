CREATE PROCEDURE [dbo].[spGetMaxSequenceNumber]
AS
BEGIN
	SET NOCOUNT ON;

    SELECT
		MAX(SequenceNumber)
	FROM
		WorkSequenceNumber
END
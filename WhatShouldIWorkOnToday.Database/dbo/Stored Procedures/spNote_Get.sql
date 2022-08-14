CREATE PROCEDURE [dbo].[spNote_Get]
	@NoteId INT
AS
BEGIN
	SELECT
		[NoteId]
		,[WorkItemId]
		,[Text]
		,[DateCreated]
	FROM
		[Note]
	WHERE
		NoteId = @NoteId
END
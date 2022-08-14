CREATE PROCEDURE [dbo].[spNote_GetByWorkItem]
	@WorkItemId INT
AS
BEGIN
	SELECT
		[NoteId]
		,[WorkItemId]
		,[Text]
		,[DateCreated]
	FROM
		Note
	WHERE
		WorkItemId = @WorkItemId
	ORDER BY
		DateCreated DESC;
END
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
	AND DateDeleted IS NULL
	ORDER BY
		DateCreated DESC;
END
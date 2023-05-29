CREATE PROCEDURE [dbo].[spWorkItem_GetAll]
AS
BEGIN

	SET NOCOUNT ON;

	SELECT
		[WorkItemId]
		,[Name]
		,[Description]
		,[Url]
		,[Pinned]
		,[DateCreated]
		,[DateWorkedOn]
		,[DateCompleted]
		,[DateDeleted]
	FROM
		WorkItem
	WHERE
		DateDeleted IS NULL
	ORDER BY
		[Name];

END

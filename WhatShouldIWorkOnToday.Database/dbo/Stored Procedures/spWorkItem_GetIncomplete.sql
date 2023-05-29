CREATE PROCEDURE [dbo].[spWorkItem_GetIncomplete]
AS
BEGIN
	SELECT
		[WorkItemId], 
		[Name],
		[Description], 
		[Url],
		[Pinned],
		[DateCreated],
		[DateWorkedOn],
		[DateDeleted], 
		[DateCompleted]
	FROM
		WorkItem
	WHERE
		DateCompleted IS NOT NULL
	AND DateDeleted IS NULL
END
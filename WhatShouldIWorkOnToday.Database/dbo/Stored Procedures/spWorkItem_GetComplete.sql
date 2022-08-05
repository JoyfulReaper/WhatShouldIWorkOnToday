CREATE PROCEDURE [dbo].[spWorkItem_GetComplete]
AS
BEGIN
	SELECT
		[WorkItemId], [Name], [Description], [Url], [DateCreated], [DateWorkedOn], [DateDeleted], [DateCompleted]
	FROM
		WorkItem
	WHERE
		DateCompleted IS NULL
	AND DateDeleted IS NULL
END
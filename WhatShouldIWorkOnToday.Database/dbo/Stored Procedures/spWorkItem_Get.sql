CREATE PROCEDURE [dbo].[spWorkItem_Get]
	@WorkItemId INT
AS
BEGIN
	SELECT
		[WorkItemId]
		,[Name]
		,[Description]
		,[Url]
		,[Pinned]
		,[DateCreated]
		,[DateWorkedOn]
		,[DateDeleted]
		,[DateCompleted]
	FROM
		WorkItem
	WHERE
		WorkItemId = @WorkItemId
	AND DateDeleted IS NULL;
END
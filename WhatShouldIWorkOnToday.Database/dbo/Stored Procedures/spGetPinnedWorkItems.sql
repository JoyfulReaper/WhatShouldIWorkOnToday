CREATE PROCEDURE [dbo].[spGetPinnedWorkItems]
AS
BEGIN
	SELECT
		[wi].[WorkItemId]
		,[wi].[Name]
		,[wi].[Description]
		,[wi].[Url]
		,[wi].[DateCreated]
		,[wi].[DateWorkedOn]
		,[wi].[DateDeleted]
		,[wi].[DateCompleted]
		,[pwi].[PinnedWorkItemId]
		,[pwi].[WorkItemId]
		,[pwi].[DatePinned]
		,[pwi].[DateUnpinned]
	FROM
		WorkItem wi INNER JOIN
		PinnedWorkItem pwi ON wi.WorkItemId = pwi.WorkItemId;
END
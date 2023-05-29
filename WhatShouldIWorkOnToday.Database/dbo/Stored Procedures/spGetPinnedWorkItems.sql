CREATE PROCEDURE [dbo].[spGetPinnedWorkItems]
AS
BEGIN
	SELECT
		[wi].[WorkItemId],
		[wi].[Name], 
		[wi].[Description],
		[wi].[Url],
		[wi].[Pinned],
		[wi].[DateCreated],
		[wi].[DateWorkedOn], 
		[wi].[DateDeleted],
		[wi].[DateCompleted]
	FROM
		WorkItem [wi]
	WHERE
		wi.DateDeleted IS NULL
		AND [wi].Pinned = 1
END
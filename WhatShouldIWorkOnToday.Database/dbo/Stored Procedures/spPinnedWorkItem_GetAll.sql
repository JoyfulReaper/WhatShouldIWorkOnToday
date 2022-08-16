CREATE PROCEDURE [dbo].[spPinnedWorkItem_GetAll]
AS
BEGIN
	SELECT 
		[PinnedWorkItemId]
		,[WorkItemId]
		,[DatePinned]
		,[DateUnpinned] 
	FROM 
		[dbo].[PinnedWorkItem]
	WHERE
		DateUnpinned IS NULL;
END
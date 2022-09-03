CREATE PROCEDURE [dbo].[spPinnedWorkItem_GetAll]
AS
BEGIN
	SELECT 
		[PinnedWorkItemId]
		,[WorkItemId]
		,[DatePinned]
	FROM 
		[dbo].[PinnedWorkItem];
END
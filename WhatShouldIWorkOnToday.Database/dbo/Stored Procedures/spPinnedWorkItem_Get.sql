CREATE PROCEDURE [dbo].[spPinnedWorkItem_Get]
	@PinnedWorkItemId INT
AS
BEGIN
	SELECT 
		[PinnedWorkItemId]
		,[WorkItemId]
		,[DatePinned]
	FROM 
		[dbo].[PinnedWorkItem]
	WHERE
		PinnedWorkItemId = @PinnedWorkItemId;
END
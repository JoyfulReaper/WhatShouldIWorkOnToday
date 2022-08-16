CREATE PROCEDURE [dbo].[spPinnedWorkItem_Get]
	@PinnedWorkItemId INT
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
		PinnedWorkItemId = @PinnedWorkItemId;
END
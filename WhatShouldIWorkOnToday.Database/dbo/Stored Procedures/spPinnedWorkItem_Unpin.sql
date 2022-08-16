CREATE PROCEDURE [dbo].[spPinnedWorkItem_Unpin]
	@WorkItemId INT
AS
BEGIN
	UPDATE PinnedWorkItem
		SET DateUnpinned = GETUTCDATE()
	WHERE
		WorkItemId = @WorkItemId;
END
CREATE PROCEDURE [dbo].[spPinnedWorkItem_Unpin]
	@WorkItemId INT
AS
BEGIN
	DELETE FROM
		PinnedWorkItem
	WHERE
		WorkItemId = @WorkItemId;
END
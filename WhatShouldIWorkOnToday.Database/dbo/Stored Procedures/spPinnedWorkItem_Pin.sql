CREATE PROCEDURE [dbo].[spPinnedWorkItem_Pin]
	@WorkItemId INT
AS
BEGIN
	INSERT INTO PinnedWorkItem
		(WorkItemId)
	SELECT
		@WorkItemId
	WHERE NOT EXISTS
	(
		SELECT 1 FROM dbo.PinnedWorkItem
		WHERE [WorkItemId] = @WorkItemId
	);
END
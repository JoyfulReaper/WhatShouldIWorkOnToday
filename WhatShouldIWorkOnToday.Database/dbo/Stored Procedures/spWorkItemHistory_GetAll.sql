CREATE PROCEDURE [dbo].[spWorkItemHistory_GetAll]
	@WorkItemId INT
AS
BEGIN
	SELECT
		[WorkItemHistoryId]
		,[WorkItemId]
		,[DateWorkedOn]
	FROM
		WorkItemHistory
	WHERE
		WorkItemId = @WorkItemId;
END
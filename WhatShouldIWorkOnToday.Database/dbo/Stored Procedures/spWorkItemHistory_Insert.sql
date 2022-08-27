CREATE PROCEDURE [dbo].[spWorkItemHistory_Insert]
	@WorkItemId INT
AS
BEGIN
	INSERT INTO dbo.WorkItemHistory
		(WorkItemId)
	VALUES
		(@WorkItemId);
END
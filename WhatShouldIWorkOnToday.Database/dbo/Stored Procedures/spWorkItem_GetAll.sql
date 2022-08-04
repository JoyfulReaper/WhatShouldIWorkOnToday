CREATE PROCEDURE [dbo].[spWorkItem_GetAll]
AS
BEGIN

	SET NOCOUNT ON;

	SELECT
		[WorkItemId]
		,[Name]
		,[Description]
		,[Url]
		,[DateCreated]
		,[DateWorkedOn]
		,[DateCompleted]
		,[DateDeleted]
	FROM
		WorkItem
	WHERE
		DateDeleted IS NULL;

END

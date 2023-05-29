CREATE PROCEDURE [dbo].[spTodoItems_GetAll]
	@WorkItemId INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT
		[TodoItemId],
		[WorkItemId],
		[Task],
		[DateAdded],
		[DateCompleted]
	FROM
		TodoItem
	WHERE
		WorkItemId = @WorkItemId
	AND DateDeleted IS NULL;
END
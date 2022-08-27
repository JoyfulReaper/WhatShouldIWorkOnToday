CREATE PROCEDURE [dbo].[spTodoItems_GetAll]
	@WorkItemId INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT
		[ToDoItemId],
		[WorkItemId],
		[Task],
		[DateAdded],
		[DateCompleted]
	FROM
		TodoItem
	WHERE
		WorkItemId = @WorkItemId;
END
CREATE PROCEDURE [dbo].[spTodoItem_Get]
	@TodoItemId INT
AS
BEGIN
	SELECT
		[TodoItemId]
		,[WorkItemId]
		,[Task]
		,[DateAdded]
		,[DateCompleted]
	FROM
		TodoItem
	WHERE
		TodoItemId = @TodoItemId
	AND DateDeleted IS NULL;
END
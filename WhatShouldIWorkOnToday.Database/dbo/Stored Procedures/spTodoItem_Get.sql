CREATE PROCEDURE [dbo].[spTodoItem_Get]
	@TodoItemId INT
AS
BEGIN
	SELECT
		[ToDoItemId]
		,[WorkItemId]
		,[Task]
		,[DateAdded]
		,[DateCompleted]
	FROM
		TodoItem
	WHERE
		TodoItemId = @TodoItemId
END
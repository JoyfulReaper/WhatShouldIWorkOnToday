CREATE PROCEDURE [dbo].[spToDoItem_Delete]
	@TodoItemId INT
AS
BEGIN
	UPDATE dbo.[TodoItem]
	SET DateDeleted = GETUTCDATE()
	WHERE TodoItemId = @TodoItemId;
END
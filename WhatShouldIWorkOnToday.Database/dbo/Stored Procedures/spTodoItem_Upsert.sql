CREATE PROCEDURE [dbo].[spTodoItem_Upsert]
	@TodoItemId INT,
	@WorkItemId INT,
	@Task VARCHAR(300),
	@DateCompleted DATETIME2(7) = NULL
AS
BEGIN
	BEGIN TRANSACTION

		INSERT INTO dbo.TodoItem
			(WorkItemId,
			Task,
			DateCompleted)
		SELECT
			@WorkItemId,
			@Task,
			@DateCompleted
		WHERE NOT EXISTS
		(
			SELECT 1 FROM dbo.TodoItem WITH (UPDLOCK, SERIALIZABLE)
			WHERE [TodoItemId] = @TodoItemId
		);

		IF @@ROWCOUNT = 0
		BEGIN
			UPDATE dbo.TodoItem
			SET [WorkItemId] = @WorkItemId,
				[Task] = @Task,
				[DateCompleted] = @DateCompleted
			WHERE
				TodoItemId = @ToDoItemId;
		END
		ELSE
		BEGIN
			SET @TodoItemId = SCOPE_IDENTITY();
		END

	COMMIT TRANSACTION

	SELECT @TodoItemId;
END
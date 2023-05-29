CREATE PROCEDURE [dbo].[spWorkItem_GetBySequenceNumber]
	@SequenceNumber INT
AS
BEGIN
	SELECT
		[wi].[WorkItemId],
		[wi].[Name],
		[wi].[Description],
		[wi].[Url],
		[wi].[Pinned],
		[wi].[DateCreated],
		[wi].[DateWorkedOn], 
		[wi].[DateDeleted],
		[wi].[DateCompleted]
	FROM
		WorkSequenceNumber wsn INNER JOIN
		WorkItem wi on wsn.WorkItemId = wi.WorkItemId
	WHERE
		wsn.SequenceNumber = @SequenceNumber
	AND wi.DateCompleted IS NULL
	AND wi.DateDeleted IS NULL;
END
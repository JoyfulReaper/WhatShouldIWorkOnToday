CREATE PROCEDURE [dbo].[spWorkItemSequence_Get]
	@WorkSequenceNumberId INT
AS
BEGIN
	SELECT
		[wsn].[WorkSequenceNumberId], 
		[wi].[WorkItemId],
		ISNULL([wsn].[SequenceNumber], -1) AS SequenceNumber,
		[wi].[Name], 
		[wi].[Description],
		[wi].[Url],
		[wi].[DateCreated], 
		[wi].[DateWorkedOn],
		[wi].[DateCompleted]
	FROM
		WorkItem wi LEFT JOIN
		WorkSequenceNumber wsn ON wsn.WorkItemId = wi.WorkItemId
	WHERE
		wi.DateDeleted IS NULL
	AND wi.DateCompleted IS NULL
	AND wsn.WorkSequenceNumberId = @WorkSequenceNumberId

END
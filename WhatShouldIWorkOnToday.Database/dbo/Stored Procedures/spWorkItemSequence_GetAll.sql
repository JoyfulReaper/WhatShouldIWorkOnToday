CREATE PROCEDURE [dbo].[spWorkItemSequence_GetAll]
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
	ORDER BY
		wsn.SequenceNumber;

END
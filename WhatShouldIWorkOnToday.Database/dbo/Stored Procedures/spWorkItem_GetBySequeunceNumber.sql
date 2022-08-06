CREATE PROCEDURE [dbo].[spWorkItem_GetBySequenceNumber]
	@SequeunceNumber INT
AS
BEGIN
	SELECT
		[wi].[WorkItemId], [wi].[Name], [wi].[Description], [wi].[Url], [wi].[DateCreated], [wi].[DateWorkedOn], [wi].[DateDeleted], [wi].[DateCompleted]
	FROM
		WorkSequenceNumber wsn INNER JOIN
		WorkItem wi on wsn.WorkItemId = wi.WorkItemId
	WHERE
		wsn.SequenceNumber = 1
	AND wi.DateCompleted IS NULL
	AND wi.DateDeleted IS NULL;
END
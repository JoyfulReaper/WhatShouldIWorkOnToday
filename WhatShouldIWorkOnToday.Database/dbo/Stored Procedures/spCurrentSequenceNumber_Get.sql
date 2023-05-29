CREATE PROCEDURE [dbo].[spCurrentSequenceNumber_Get]
AS
BEGIN
	SELECT
		[SettingsId]
		,[CurrentSequence]
		,[DateSet]
	FROM
		Settings;
END
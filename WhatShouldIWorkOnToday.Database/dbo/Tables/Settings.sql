CREATE TABLE [dbo].[Settings]
(
	[SettingsId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CurrentSequence] INT NOT NULL, 
    [DateSet] DATE NOT NULL DEFAULT GETUTCDATE()
)

CREATE TABLE [dbo].[Setting]
(
	[SettingId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CurrentSequence] INT NOT NULL, 
    [DateSet] DATE NOT NULL DEFAULT GETUTCDATE()
)

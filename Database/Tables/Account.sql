CREATE TABLE [dbo].[Account]
(
	[AccountId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Username] VARCHAR(MAX) NOT NULL, 
    [PasswordHash] VARBINARY(MAX) NOT NULL, 
    [Email] VARCHAR(MAX) NOT NULL
)

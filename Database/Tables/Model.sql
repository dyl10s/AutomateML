CREATE TABLE [dbo].[Model]
(
	[ModelId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] VARCHAR(100) NOT NULL, 
    [CreatedBy] INT NOT NULL, 
    [CreatedOn] DATETIME NOT NULL, 
    [FileStoreId] INT NULL, 
    [Accuracy] FLOAT NULL, 
    [Rows] INT NULL, 
    [Description] VARCHAR(500) NULL, 
    CONSTRAINT [FK_Model_Account] FOREIGN KEY (CreatedBy) REFERENCES Account(AccountId), 
    CONSTRAINT [FK_Model_Files] FOREIGN KEY ([FileStoreId]) REFERENCES FileStore(FileStoreId)
)

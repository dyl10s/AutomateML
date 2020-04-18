CREATE TABLE [dbo].[ModelVote]
(
	[ModelVoteId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [AccountId] INT NOT NULL, 
    [ModelId] INT NOT NULL, 
    CONSTRAINT [FK_ModelVote_Account] FOREIGN KEY (AccountId) REFERENCES Account(AccountId), 
    CONSTRAINT [FK_ModelVote_Model] FOREIGN KEY (ModelId) REFERENCES Model(ModelId)
)

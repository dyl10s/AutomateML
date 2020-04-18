CREATE TABLE [dbo].[ModelField]
(
	[ModelFieldId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ModelId] INT NOT NULL, 
    [Name] VARCHAR(MAX) NOT NULL, 
    [DataTypeId] INT NOT NULL, 
    [IsOutput] BIT NOT NULL, 
    CONSTRAINT [FK_ModelField_Model] FOREIGN KEY (ModelId) REFERENCES Model(ModelId), 
    CONSTRAINT [FK_ModelField_DataType] FOREIGN KEY ([DataTypeId]) REFERENCES DataType(DataTypeId)
)

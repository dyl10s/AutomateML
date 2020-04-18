DELETE FROM DataType
SET IDENTITY_INSERT DataType ON
Insert into DataType (DataTypeId, Name)
VALUES
(9, 'Number'),
(11, 'Text'),
(12, 'True/False'),
(1, 'Ignore')
SET IDENTITY_INSERT DataType OFF

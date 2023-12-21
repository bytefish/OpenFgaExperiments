PRINT 'Inserting [Application].[Team] ...'

-----------------------------------------------
-- Global Parameters
-----------------------------------------------
DECLARE @ValidFrom datetime2(7) = '20130101'
DECLARE @ValidTo datetime2(7) =  '99991231 23:59:59.9999999'

-----------------------------------------------
-- [Application].[Team]
-----------------------------------------------
MERGE INTO [Application].[Team] AS [Target]
USING (VALUES 
      (1,	'Human Resources',        'Human Resource', 1, @ValidFrom, @ValidTo)
    , (2,	'Software Development',   'Software Development', 1, @ValidFrom, @ValidTo)
) AS [Source]([TeamID], [Name], [Description], [LastEditedBy], [ValidFrom], [ValidTo])
ON ([Target].[TeamID] = [Source].[TeamID])
WHEN NOT MATCHED BY TARGET THEN
	INSERT 
		([TeamID], [Name], [Description],  [LastEditedBy], [ValidFrom], [ValidTo])
	VALUES 
		([Source].[TeamID], [Source].[Name], [Source].[Description],  [Source].[LastEditedBy], [Source].[ValidFrom], [Source].[ValidTo]);
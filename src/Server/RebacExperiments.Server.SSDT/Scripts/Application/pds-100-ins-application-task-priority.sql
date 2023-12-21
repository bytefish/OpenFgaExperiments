PRINT 'Inserting [Application].[TaskItemPriority] ...'

-----------------------------------------------
-- Global Parameters
-----------------------------------------------
DECLARE @ValidFrom datetime2(7) = '20130101'
DECLARE @ValidTo datetime2(7) =  '99991231 23:59:59.9999999'

-----------------------------------------------
-- [Application].[TaskItemPriority]
-----------------------------------------------
MERGE INTO [Application].[TaskItemPriority] AS [Target]
USING (VALUES 
			  (1, 'Low', 1, @ValidFrom, @ValidTo)
			, (2, 'Normal', 1, @ValidFrom, @ValidTo)
			, (3, 'High', 1, @ValidFrom, @ValidTo)
		) AS [Source]([TaskItemPriorityID], [Name], [LastEditedBy], [ValidFrom], [ValidTo])
ON ([Target].[TaskItemPriorityID] = [Source].[TaskItemPriorityID])
WHEN NOT MATCHED BY TARGET THEN
	INSERT 
		([TaskItemPriorityID], [Name], [LastEditedBy], [ValidFrom], [ValidTo]) 
	VALUES 
		([Source].[TaskItemPriorityID], [Source].[Name], [Source].[LastEditedBy], [Source].[ValidFrom], [Source].[ValidTo]);
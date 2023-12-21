PRINT 'Inserting [Application].[TaskItemStatus] ...'

-----------------------------------------------
-- Global Parameters
-----------------------------------------------
DECLARE @ValidFrom datetime2(7) = '20130101'
DECLARE @ValidTo datetime2(7) =  '99991231 23:59:59.9999999'

-----------------------------------------------
-- [Application].[TaskItemStatus]
-----------------------------------------------
MERGE INTO [Application].[TaskItemStatus] AS [Target]
USING (VALUES 
			  (1, 'Not Started', 1, @ValidFrom, @ValidTo)
			, (2, 'In Progress', 1, @ValidFrom, @ValidTo)
			, (3, 'Completed', 1, @ValidFrom, @ValidTo)
			, (4, 'Waiting On Others', 1, @ValidFrom, @ValidTo)
			, (5, 'Deferred', 1, @ValidFrom, @ValidTo)
		) AS [Source]([TaskItemStatusID], [Name], [LastEditedBy], [ValidFrom], [ValidTo])
ON ([Target].[TaskItemStatusID] = [Source].[TaskItemStatusID])
WHEN NOT MATCHED BY TARGET THEN
	INSERT 
		([TaskItemStatusID], [Name], [LastEditedBy], [ValidFrom], [ValidTo])
	VALUES 
		([Source].[TaskItemStatusID], [Source].[Name], [Source].[LastEditedBy], [Source].[ValidFrom], [Source].[ValidTo]);
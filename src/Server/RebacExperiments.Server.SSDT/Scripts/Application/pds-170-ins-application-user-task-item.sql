PRINT 'Inserting [Application].[UserTaskItem] ...'

-----------------------------------------------
-- Global Parameters
-----------------------------------------------
DECLARE @ValidFrom datetime2(7) = '20130101'
DECLARE @ValidTo datetime2(7) =  '99991231 23:59:59.9999999'

-----------------------------------------------
-- [Application].[UserTaskItem]
-----------------------------------------------
MERGE INTO [Application].[UserTaskItem] AS [Target]
USING (VALUES 
      (1,	152,    2, 'owner', 1, @ValidFrom, @ValidTo)
    , (2,	323,    2, 'owner', 1, @ValidFrom, @ValidTo)
) AS [Source]([UserTaskItemID], [TaskItemID], [UserID], [Role], [LastEditedBy], [ValidFrom], [ValidTo])
ON ([Target].[UserTaskItemID] = [Source].[UserTaskItemID])
WHEN NOT MATCHED BY TARGET THEN
	INSERT 
		([UserTaskItemID], [TaskItemID], [UserID], [Role], [LastEditedBy], [ValidFrom], [ValidTo])
	VALUES 
		([Source].[UserTaskItemID], [Source].[TaskItemID], [Source].[UserID], [Source].[Role], [Source].[LastEditedBy], [Source].[ValidFrom], [Source].[ValidTo]);
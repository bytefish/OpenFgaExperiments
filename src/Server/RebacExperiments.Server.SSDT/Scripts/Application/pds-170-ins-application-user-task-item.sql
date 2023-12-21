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
      (1,	1,    2, 'owner', NULL, 1, @ValidFrom, @ValidTo)
    , (2,	2,    2, 'owner', NULL, 1, @ValidFrom, @ValidTo)
) AS [Source]([UserTaskItemID], [TaskItemID], [UserID], [Role], [RowVersion], [LastEditedBy], [ValidFrom], [ValidTo])
ON ([Target].[UserTaskItemID] = [Source].[UserTaskItemID])
WHEN NOT MATCHED BY TARGET THEN
	INSERT 
		([UserTaskItemID], [TaskItemID], [UserID], [Role], [RowVersion], [LastEditedBy], [ValidFrom], [ValidTo])
	VALUES 
		([Source].[UserTaskItemID], [Source].[TaskItemID], [Source].[UserID], [Source].[Role], [Source].[RowVersion], [Source].[LastEditedBy], [Source].[ValidFrom], [Source].[ValidTo]);
PRINT 'Inserting [Application].[TaskItemStatus] ...'

-----------------------------------------------
-- Global Parameters
-----------------------------------------------
DECLARE @ValidFrom datetime2(7) = '20130101'
DECLARE @ValidTo datetime2(7) =  '99991231 23:59:59.9999999'

-----------------------------------------------
-- [Application].[TaskItem]
-----------------------------------------------
MERGE INTO [Application].[TaskItem] AS [Target]
USING (VALUES 
      (152,	'Call Back',        'Call Back Philipp Wagner',    	NULL, NULL, NULL, NULL,	1,	1,	NULL, 1, @ValidFrom, @ValidTo)
    , (323,	'Sign Document',    'You need to Sign a Document',	NULL, NULL, NULL, NULL,	2,	2,	NULL, 1, @ValidFrom, @ValidTo)
) AS [Source]([TaskItemID], [Title], [Description], [DueDateTime] , [ReminderDateTime], [CompletedDateTime], [AssignedTo], [TaskItemPriorityID], [TaskItemStatusID], [RowVersion] , [LastEditedBy], [ValidFrom], [ValidTo])
ON ([Target].[TaskItemID] = [Source].[TaskItemID])
WHEN NOT MATCHED BY TARGET THEN
	INSERT 
		([TaskItemID], [Title], [Description], [DueDateTime] , [ReminderDateTime], [CompletedDateTime], [AssignedTo], [TaskItemPriorityID], [TaskItemStatusID], [RowVersion] , [LastEditedBy], [ValidFrom], [ValidTo])
	VALUES 
		([Source].[TaskItemID], [Source].[Title], [Source].[Description], [Source].[DueDateTime] , [Source].[ReminderDateTime], [Source].[CompletedDateTime], [Source].[AssignedTo], [Source].[TaskItemPriorityID], [Source].[TaskItemStatusID], [Source].[RowVersion] , [Source].[LastEditedBy], [Source].[ValidFrom], [Source].[ValidTo]);
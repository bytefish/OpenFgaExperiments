PRINT 'Inserting [Application].[TeamRole] ...'

-----------------------------------------------
-- Global Parameters
-----------------------------------------------
DECLARE @ValidFrom datetime2(7) = '20130101'
DECLARE @ValidTo datetime2(7) =  '99991231 23:59:59.9999999'

-----------------------------------------------
-- [Application].[TeamRole]
-----------------------------------------------
MERGE INTO [Application].[TeamRole] AS [Target]
USING (VALUES 
      (1,	1,    2, 'owner', NULL, 1, @ValidFrom, @ValidTo)
    , (2,	2,    2, 'owner', NULL, 1, @ValidFrom, @ValidTo)
) AS [Source]([TeamRoleID], [TeamID], [UserID], [Role], [RowVersion], [LastEditedBy], [ValidFrom], [ValidTo])
ON ([Target].[TeamRoleID] = [Source].[TeamRoleID])
WHEN NOT MATCHED BY TARGET THEN
	INSERT 
		([TeamRoleID], [TeamID], [UserID], [Role], [RowVersion], [LastEditedBy], [ValidFrom], [ValidTo])
	VALUES 
		([Source].[TeamRoleID], [Source].[TeamID], [Source].[UserID], [Source].[Role], [Source].[RowVersion], [Source].[LastEditedBy], [Source].[ValidFrom], [Source].[ValidTo]);
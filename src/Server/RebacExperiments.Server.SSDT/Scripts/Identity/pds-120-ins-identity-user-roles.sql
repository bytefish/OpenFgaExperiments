PRINT 'Inserting [Identity].[UserRole] ...'

-----------------------------------------------
-- Global Parameters
-----------------------------------------------
DECLARE @ValidFrom datetime2(7) = '20130101'
DECLARE @ValidTo datetime2(7) =  '99991231 23:59:59.9999999'

-----------------------------------------------
-- [Identity].[Role]
-----------------------------------------------
MERGE INTO [Identity].[UserRole] AS [Target]
USING (VALUES 
      (1, 2, 1, 1, @ValidFrom, @ValidTo)
    , (2, 2, 2, 1, @ValidFrom, @ValidTo)
) AS [Source]([UserRoleID], [UserID], [RoleID], [LastEditedBy], [ValidFrom], [ValidTo])
ON ([Target].[UserRoleID] = [Source].[UserRoleID])
WHEN NOT MATCHED BY TARGET THEN
	INSERT 
		([UserRoleID], [UserID], [RoleID],  [LastEditedBy], [ValidFrom], [ValidTo])
	VALUES 
		([Source].[UserRoleID], [Source].[UserID], [Source].[RoleID],  [Source].[LastEditedBy], [Source].[ValidFrom], [Source].[ValidTo]);
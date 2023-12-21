PRINT 'Inserting [Application].[OrganizationRole] ...'

-----------------------------------------------
-- Global Parameters
-----------------------------------------------
DECLARE @ValidFrom datetime2(7) = '20130101'
DECLARE @ValidTo datetime2(7) =  '99991231 23:59:59.9999999'

-----------------------------------------------
-- [Application].[OrganizationRole]
-----------------------------------------------
MERGE INTO [Application].[OrganizationRole] AS [Target]
USING (VALUES 
      (1,	1,    2, 'owner', NULL, 1, @ValidFrom, @ValidTo)
    , (2,	2,    2, 'owner', NULL, 1, @ValidFrom, @ValidTo)
) AS [Source]([OrganizationRoleID], [OrganizationID], [UserID], [RowVersion], [LastEditedBy], [ValidFrom], [ValidTo])
ON ([Target].[OrganizationRoleID] = [Source].[OrganizationRoleID])
WHEN NOT MATCHED BY TARGET THEN
	INSERT 
		([OrganizationRoleID], [OrganizationID], [UserID], [Role], [RowVersion], [LastEditedBy], [ValidFrom], [ValidTo])
	VALUES 
		([Source].[OrganizationRoleID], [Source].[OrganizationID], [Source].[UserID], [Source].[RowVersion], [Source].[LastEditedBy], [Source].[ValidFrom], [Source].[ValidTo]);
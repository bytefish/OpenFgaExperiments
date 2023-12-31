﻿CREATE PROCEDURE [Identity].[usp_TemporalTables_ReactivateTemporalTables]
AS BEGIN

	IF OBJECTPROPERTY(OBJECT_ID('[Identity].[User]'), 'TableTemporalType') = 0
	BEGIN
		PRINT 'Reactivate Temporal Table for [Identity].[User]'

		ALTER TABLE [Identity].[User] ADD PERIOD FOR SYSTEM_TIME([ValidFrom], [ValidTo]);
		ALTER TABLE [Identity].[User] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [Identity].[UserHistory], DATA_CONSISTENCY_CHECK = ON));
	END

	IF OBJECTPROPERTY(OBJECT_ID('[Identity].[Role]'), 'TableTemporalType') = 0
	BEGIN
		PRINT 'Reactivate Temporal Table for [Identity].[Role]'

		ALTER TABLE [Identity].[Role] ADD PERIOD FOR SYSTEM_TIME([ValidFrom], [ValidTo]);
		ALTER TABLE [Identity].[Role] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [Identity].[RoleHistory], DATA_CONSISTENCY_CHECK = ON));
	END

	IF OBJECTPROPERTY(OBJECT_ID('[Identity].[UserRole]'), 'TableTemporalType') = 0
	BEGIN
		PRINT 'Reactivate Temporal Table for [Identity].[UserRole]'

		ALTER TABLE [Identity].[UserRole] ADD PERIOD FOR SYSTEM_TIME([ValidFrom], [ValidTo]);
		ALTER TABLE [Identity].[UserRole] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [Identity].[UserRoleHistory], DATA_CONSISTENCY_CHECK = ON));
	END
    
END
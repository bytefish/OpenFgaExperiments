CREATE PROCEDURE [Identity].[usp_TemporalTables_DeactivateTemporalTables]
AS BEGIN
	IF OBJECTPROPERTY(OBJECT_ID('[Identity].[User]'), 'TableTemporalType') = 2
	BEGIN
		PRINT 'Deactivate Temporal Table for [Identity].[User]'

		ALTER TABLE [Identity].[User] SET (SYSTEM_VERSIONING = OFF);
		ALTER TABLE [Identity].[User] DROP PERIOD FOR SYSTEM_TIME;
	END

    IF OBJECTPROPERTY(OBJECT_ID('[Identity].[Role]'), 'TableTemporalType') = 2
	BEGIN
		PRINT 'Deactivate Temporal Table for [Identity].[Role]'

		ALTER TABLE [Identity].[Role] SET (SYSTEM_VERSIONING = OFF);
		ALTER TABLE [Identity].[Role] DROP PERIOD FOR SYSTEM_TIME;
	END

    IF OBJECTPROPERTY(OBJECT_ID('[Identity].[UserRole]'), 'TableTemporalType') = 2
	BEGIN
		PRINT 'Deactivate Temporal Table for [Identity].[UserRole]'

		ALTER TABLE [Identity].[UserRole] SET (SYSTEM_VERSIONING = OFF);
		ALTER TABLE [Identity].[UserRole] DROP PERIOD FOR SYSTEM_TIME;
	END

END
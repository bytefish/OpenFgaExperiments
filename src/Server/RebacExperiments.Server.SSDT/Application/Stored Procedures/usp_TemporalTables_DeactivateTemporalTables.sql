CREATE PROCEDURE [Application].[usp_TemporalTables_DeactivateTemporalTables]
AS BEGIN
	IF OBJECTPROPERTY(OBJECT_ID('[Application].[TaskItem]'), 'TableTemporalType') = 2
	BEGIN
		PRINT 'Deactivate Temporal Table for [Application].[TaskItem]'

		ALTER TABLE [Application].[TaskItem] SET (SYSTEM_VERSIONING = OFF);
		ALTER TABLE [Application].[TaskItem] DROP PERIOD FOR SYSTEM_TIME;
	END

	IF OBJECTPROPERTY(OBJECT_ID('[Application].[TaskItemPriority]'), 'TableTemporalType') = 2
	BEGIN
		PRINT 'Deactivate Temporal Table for [Application].[TaskItemPriority]'

		ALTER TABLE [Application].[TaskItemPriority] SET (SYSTEM_VERSIONING = OFF);
		ALTER TABLE [Application].[TaskItemPriority] DROP PERIOD FOR SYSTEM_TIME;
	END

	IF OBJECTPROPERTY(OBJECT_ID('[Application].[TaskItemStatus]'), 'TableTemporalType') = 2
	BEGIN
		PRINT 'Deactivate Temporal Table for [Application].[TaskItemStatus]'

		ALTER TABLE [Application].[TaskItemStatus] SET (SYSTEM_VERSIONING = OFF);
		ALTER TABLE [Application].[TaskItemStatus] DROP PERIOD FOR SYSTEM_TIME;
	END
    
	IF OBJECTPROPERTY(OBJECT_ID('[Application].[Organization]'), 'TableTemporalType') = 2
	BEGIN
		PRINT 'Deactivate Temporal Table for [Application].[Organization]'

		ALTER TABLE [Application].[Organization] SET (SYSTEM_VERSIONING = OFF);
		ALTER TABLE [Application].[Organization] DROP PERIOD FOR SYSTEM_TIME;
	END

	IF OBJECTPROPERTY(OBJECT_ID('[Application].[Team]'), 'TableTemporalType') = 2
	BEGIN
		PRINT 'Deactivate Temporal Table for [Application].[Team]'

		ALTER TABLE [Application].[Team] SET (SYSTEM_VERSIONING = OFF);
		ALTER TABLE [Application].[Team] DROP PERIOD FOR SYSTEM_TIME;
	END

        
	IF OBJECTPROPERTY(OBJECT_ID('[Application].[OrganizationRole]'), 'TableTemporalType') = 2
	BEGIN
		PRINT 'Deactivate Temporal Table for [Application].[OrganizationRole]'

		ALTER TABLE [Application].[OrganizationRole] SET (SYSTEM_VERSIONING = OFF);
		ALTER TABLE [Application].[OrganizationRole] DROP PERIOD FOR SYSTEM_TIME;
	END

	IF OBJECTPROPERTY(OBJECT_ID('[Application].[TeamRole]'), 'TableTemporalType') = 2
	BEGIN
		PRINT 'Deactivate Temporal Table for [Application].[TeamRole]'

		ALTER TABLE [Application].[TeamRole] SET (SYSTEM_VERSIONING = OFF);
		ALTER TABLE [Application].[TeamRole] DROP PERIOD FOR SYSTEM_TIME;
	END
    
	IF OBJECTPROPERTY(OBJECT_ID('[Application].[UserTaskItem]'), 'TableTemporalType') = 2
	BEGIN
		PRINT 'Deactivate Temporal Table for [Application].[UserTaskItem]'

		ALTER TABLE [Application].[UserTaskItem] SET (SYSTEM_VERSIONING = OFF);
		ALTER TABLE [Application].[UserTaskItem] DROP PERIOD FOR SYSTEM_TIME;
	END

END
CREATE PROCEDURE [Application].[usp_TemporalTables_ReactivateTemporalTables]
AS BEGIN

	IF OBJECTPROPERTY(OBJECT_ID('[Application].[TaskItem]'), 'TableTemporalType') = 0
	BEGIN
		PRINT 'Reactivate Temporal Table for [Application].[TaskItem]'

		ALTER TABLE [Application].[TaskItem] ADD PERIOD FOR SYSTEM_TIME([ValidFrom], [ValidTo]);
		ALTER TABLE [Application].[TaskItem] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [Application].[TaskItemHistory], DATA_CONSISTENCY_CHECK = ON));
	END

	IF OBJECTPROPERTY(OBJECT_ID('[Application].[TaskItemPriority]'), 'TableTemporalType') = 0
	BEGIN
		PRINT 'Reactivate Temporal Table for [Application].[TaskItemPriority]'

		ALTER TABLE [Application].[TaskItemPriority] ADD PERIOD FOR SYSTEM_TIME([ValidFrom], [ValidTo]);
		ALTER TABLE [Application].[TaskItemPriority] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [Application].[TaskItemPriorityHistory], DATA_CONSISTENCY_CHECK = ON));
	END

	IF OBJECTPROPERTY(OBJECT_ID('[Application].[TaskItemStatus]'), 'TableTemporalType') = 0
	BEGIN
		PRINT 'Reactivate Temporal Table for [Application].[TaskItemStatus]'

		ALTER TABLE [Application].[TaskItemStatus] ADD PERIOD FOR SYSTEM_TIME([ValidFrom], [ValidTo]);
		ALTER TABLE [Application].[TaskItemStatus] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [Application].[TaskItemStatusHistory], DATA_CONSISTENCY_CHECK = ON));
	END
    
	IF OBJECTPROPERTY(OBJECT_ID('[Application].[Organization]'), 'TableTemporalType') = 0
	BEGIN
		PRINT 'Reactivate Temporal Table for [Application].[Organization]'

		ALTER TABLE [Application].[Organization] ADD PERIOD FOR SYSTEM_TIME([ValidFrom], [ValidTo]);
		ALTER TABLE [Application].[Organization] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [Application].[OrganizationHistory], DATA_CONSISTENCY_CHECK = ON));
	END
    
	IF OBJECTPROPERTY(OBJECT_ID('[Application].[Team]'), 'TableTemporalType') = 0
	BEGIN
		PRINT 'Reactivate Temporal Table for [Application].[Team]'

		ALTER TABLE [Application].[Team] ADD PERIOD FOR SYSTEM_TIME([ValidFrom], [ValidTo]);
		ALTER TABLE [Application].[Team] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [Application].[TeamHistory], DATA_CONSISTENCY_CHECK = ON));
	END

	IF OBJECTPROPERTY(OBJECT_ID('[Application].[TeamRole]'), 'TableTemporalType') = 0
	BEGIN
		PRINT 'Reactivate Temporal Table for [Application].[TeamRole]'

		ALTER TABLE [Application].[TeamRole] ADD PERIOD FOR SYSTEM_TIME([ValidFrom], [ValidTo]);
		ALTER TABLE [Application].[TeamRole] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [Application].[TeamRoleHistory], DATA_CONSISTENCY_CHECK = ON));
	END

    
	IF OBJECTPROPERTY(OBJECT_ID('[Application].[OrganizationRole]'), 'TableTemporalType') = 0
	BEGIN
		PRINT 'Reactivate Temporal Table for [Application].[OrganizationRole]'

		ALTER TABLE [Application].[OrganizationRole] ADD PERIOD FOR SYSTEM_TIME([ValidFrom], [ValidTo]);
		ALTER TABLE [Application].[OrganizationRole] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [Application].[OrganizationHistory], DATA_CONSISTENCY_CHECK = ON));
	END
    
    IF OBJECTPROPERTY(OBJECT_ID('[Application].[UserTaskItem]'), 'TableTemporalType') = 0
	BEGIN
		PRINT 'Reactivate Temporal Table for [Application].[UserTaskItem]'

		ALTER TABLE [Application].[UserTaskItem] ADD PERIOD FOR SYSTEM_TIME([ValidFrom], [ValidTo]);
		ALTER TABLE [Application].[UserTaskItem] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [Application].[UserTaskItemHistory], DATA_CONSISTENCY_CHECK = ON));
	END

    
END
PRINT 'Inserting [Application].[LocalizationRecord] ...'

-----------------------------------------------
-- Global Parameters
-----------------------------------------------
DECLARE @ValidFrom datetime2(7) = '20130101';
DECLARE @ValidTo datetime2(7) =  '99991231 23:59:59.9999999';

DECLARE @GlobalCategory NVARCHAR(255) = 'Global';

DECLARE @Language_En INT = (SELECT [LanguageID] FROM [Application].[Language] WHERE [Name] = 'en'); 

-----------------------------------------------
-- [Application].[LocalizationRecord]
-----------------------------------------------
MERGE INTO [Application].[LocalizationRecord] AS [Target]
USING (VALUES 
     (1,    N'DataGrid_Button_Delete',                      @Language_En,   N'Delete',                          @GlobalCategory, 1, @ValidFrom, @ValidTo)
    ,(2,    N'DataGrid_Button_Details',                     @Language_En,   N'Details',                         @GlobalCategory, 1, @ValidFrom, @ValidTo)
    ,(3,    N'DataGrid_Button_Edit',                        @Language_En,   N'Edit',                            @GlobalCategory, 1, @ValidFrom, @ValidTo)
    -- Filters                                              @Language_En,   
    ,(4,    N'FilterOperatorEnum_After',                    @Language_En,   N'After',                           @GlobalCategory, 1, @ValidFrom, @ValidTo)
    ,(5,    N'FilterOperatorEnum_All',                      @Language_En,   N'All',                             @GlobalCategory, 1, @ValidFrom, @ValidTo)
    ,(6,    N'FilterOperatorEnum_Before',                   @Language_En,   N'Before',                          @GlobalCategory, 1, @ValidFrom, @ValidTo)
    ,(7,    N'FilterOperatorEnum_BetweenExclusive',         @Language_En,   N'Between (Exclusive)',             @GlobalCategory, 1, @ValidFrom, @ValidTo)
    ,(8,    N'FilterOperatorEnum_BetweenInclusive',         @Language_En,   N'Between (Inclusive)',             @GlobalCategory, 1, @ValidFrom, @ValidTo)
    ,(9,    N'FilterOperatorEnum_Contains',                 @Language_En,   N'Contains',                        @GlobalCategory, 1, @ValidFrom, @ValidTo)
    ,(10,   N'FilterOperatorEnum_EndsWith',                 @Language_En,   N'Ends with',                       @GlobalCategory, 1, @ValidFrom, @ValidTo)
    ,(11,   N'FilterOperatorEnum_IsEmpty',                  @Language_En,   N'Is Empty',                        @GlobalCategory, 1, @ValidFrom, @ValidTo)
    ,(12,   N'FilterOperatorEnum_IsEqualTo',                @Language_En,   N'Is Equal To',                     @GlobalCategory, 1, @ValidFrom, @ValidTo)
    ,(13,   N'FilterOperatorEnum_IsGreaterThan',            @Language_En,   N'Is Greater Than',                 @GlobalCategory, 1, @ValidFrom, @ValidTo)
    ,(14,   N'FilterOperatorEnum_IsGreaterThanOrEqualTo',   @Language_En,   N'Is Greater Than or Equal To',     @GlobalCategory, 1, @ValidFrom, @ValidTo)
    ,(15,   N'FilterOperatorEnum_IsLessThan',               @Language_En,   N'Is Less Than',                    @GlobalCategory, 1, @ValidFrom, @ValidTo)
    ,(16,   N'FilterOperatorEnum_IsLessThanOrEqualTo',      @Language_En,   N'Is Less Than or Equal To',        @GlobalCategory, 1, @ValidFrom, @ValidTo)
    ,(17,   N'FilterOperatorEnum_IsNotEmpty',               @Language_En,   N'Is Not Empty',                    @GlobalCategory, 1, @ValidFrom, @ValidTo)
    ,(18,   N'FilterOperatorEnum_IsNotEqualTo',             @Language_En,   N'Is Not Equal To',                 @GlobalCategory, 1, @ValidFrom, @ValidTo)
    ,(19,   N'FilterOperatorEnum_IsNotNull',                @Language_En,   N'Is Not Null',                     @GlobalCategory, 1, @ValidFrom, @ValidTo)
    ,(20,   N'FilterOperatorEnum_IsNull',                   @Language_En,   N'Is Null',                         @GlobalCategory, 1, @ValidFrom, @ValidTo)
    ,(21,   N'FilterOperatorEnum_No',                       @Language_En,   N'No',                              @GlobalCategory, 1, @ValidFrom, @ValidTo)
    ,(22,   N'FilterOperatorEnum_None',                     @Language_En,   N'-',                               @GlobalCategory, 1, @ValidFrom, @ValidTo)
    ,(23,   N'FilterOperatorEnum_NotContains',              @Language_En,   N'Not Contains',                    @GlobalCategory, 1, @ValidFrom, @ValidTo)
    ,(24,   N'FilterOperatorEnum_StartsWith',               @Language_En,   N'Starts with',                     @GlobalCategory, 1, @ValidFrom, @ValidTo)
    ,(25,   N'FilterOperatorEnum_Yes',                      @Language_En,   N'Yes',                             @GlobalCategory, 1, @ValidFrom, @ValidTo)
    -- TaskItem                                             @Language_En,   
    ,(26,   N'TaskItem_Description',                        @Language_En,   N'Description',                     @GlobalCategory, 1, @ValidFrom, @ValidTo)
    ,(27,   N'TaskItem_TaskItemPriority',                   @Language_En,   N'Priority',                        @GlobalCategory, 1, @ValidFrom, @ValidTo)
    ,(28,   N'TaskItem_Title',                              @Language_En,   N'Title',                           @GlobalCategory, 1, @ValidFrom, @ValidTo)
    ,(29,   N'TaskItemPriorityEnum_High',                   @Language_En,   N'High',                            @GlobalCategory, 1, @ValidFrom, @ValidTo)
    ,(30,   N'TaskItemPriorityEnum_Low',                    @Language_En,   N'Low',                             @GlobalCategory, 1, @ValidFrom, @ValidTo)
    ,(31,   N'TaskItemPriorityEnum_Normal',                 @Language_En,   N'Normal',                          @GlobalCategory, 1, @ValidFrom, @ValidTo)
    ,(32,   N'TaskItemPriorityEnum_Select',                 @Language_En,   N'Select a Task Priority ...',      @GlobalCategory, 1, @ValidFrom, @ValidTo)
    ,(33,   N'Validation_IsRequired',                       @Language_En,   N'{0} is required',                 @GlobalCategory, 1, @ValidFrom, @ValidTo)
) AS [Source]([LocalizationRecordID], [Name], [LanguageID], [Value], [Category], [LastEditedBy], [ValidFrom], [ValidTo])
ON ([Target].[Name] = [Source].[Name] AND [Target].[LanguageID] = [Source].[LanguageID])
WHEN NOT MATCHED BY TARGET THEN
	INSERT 
		([LocalizationRecordID], [Name], [LanguageID], [Value], [Category], [LastEditedBy], [ValidFrom], [ValidTo])
	VALUES 
		([Source].[LocalizationRecordID], [Source].[Name], [Source].[LanguageID], [Source].[Category], [Source].[Value], [Source].[LastEditedBy], [Source].[ValidFrom], [Source].[ValidTo]);
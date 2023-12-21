CREATE TABLE [Application].[TaskItemPriority](
    [TaskItemPriorityID]    INT                                         NOT NULL,
    [Name]                  NVARCHAR(50)                                NOT NULL,
    [RowVersion]            ROWVERSION                                  NULL,
    [LastEditedBy]          INT                                         NOT NULL,
    [ValidFrom]             DATETIME2 (7) GENERATED ALWAYS AS ROW START NOT NULL,
    [ValidTo]               DATETIME2 (7) GENERATED ALWAYS AS ROW END   NOT NULL,
    CONSTRAINT [PK_TaskItemPriority] PRIMARY KEY ([TaskItemPriorityID]),
    CONSTRAINT [FK_TaskItemPriority_User_LastEditedBy] FOREIGN KEY ([LastEditedBy]) REFERENCES [Identity].[User] ([UserID]),
    PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [Application].[TaskItemPriorityHistory]));
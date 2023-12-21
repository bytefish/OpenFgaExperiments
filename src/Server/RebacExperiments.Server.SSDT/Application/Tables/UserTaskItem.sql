CREATE TABLE [Application].[UserTaskItem](
    [UserTaskItemID]        INT                                         CONSTRAINT [DF_Application_UserTaskItem_UserTaskItemID] DEFAULT (NEXT VALUE FOR [Application].[sq_UserTaskItem]) NOT NULL,
    [TaskItemID]            INT                                         NOT NULL,
    [UserID]                INT                                         NOT NULL,
    [Role]                  NVARCHAR(255)                               NOT NULL,
    [RowVersion]            ROWVERSION                                  NULL,
    [LastEditedBy]          INT                                         NOT NULL,
    [ValidFrom]             DATETIME2 (7) GENERATED ALWAYS AS ROW START NOT NULL,
    [ValidTo]               DATETIME2 (7) GENERATED ALWAYS AS ROW END   NOT NULL,
    CONSTRAINT [PK_UserTaskItem] PRIMARY KEY ([UserTaskItemID]),
    CONSTRAINT [FK_UserTaskItem_TaskItem_TaskItemID] FOREIGN KEY ([TaskItemID]) REFERENCES [Application].[TaskItem] ([TaskItemID]),
    CONSTRAINT [FK_UserTaskItem_User_UserID] FOREIGN KEY ([UserID]) REFERENCES [Identity].[User] ([UserID]),
    CONSTRAINT [FK_UserTaskItem_User_LastEditedBy] FOREIGN KEY ([LastEditedBy]) REFERENCES [Identity].[User] ([UserID]),
    PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [Application].[UserTaskItemHistory]));
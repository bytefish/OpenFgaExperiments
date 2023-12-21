CREATE TABLE [Application].[TeamRole](
    [TeamRoleID]            INT                                         CONSTRAINT [DF_Application_TeamRole_TeamRoleID] DEFAULT (NEXT VALUE FOR [Application].[sq_TeamRole]) NOT NULL,
    [TeamID]                INT                                         NOT NULL,
    [UserID]                INT                                         NOT NULL,
    [Role]                  NVARCHAR(255)                               NOT NULL,
    [RowVersion]            ROWVERSION                                  NULL,
    [LastEditedBy]          INT                                         NOT NULL,
    [ValidFrom]             DATETIME2 (7) GENERATED ALWAYS AS ROW START NOT NULL,
    [ValidTo]               DATETIME2 (7) GENERATED ALWAYS AS ROW END   NOT NULL,
    CONSTRAINT [PK_TeamRole] PRIMARY KEY ([TeamRoleID]),
    CONSTRAINT [FK_TeamRole_Team_TeamID] FOREIGN KEY ([TeamID]) REFERENCES [Application].[Team] ([TeamID]),
    CONSTRAINT [FK_TeamRole_User_UserID] FOREIGN KEY ([UserID]) REFERENCES [Identity].[User] ([UserID]),
    CONSTRAINT [FK_TeamRole_User_LastEditedBy] FOREIGN KEY ([LastEditedBy]) REFERENCES [Identity].[User] ([UserID]),
    PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [Application].[TeamRoleHistory]));
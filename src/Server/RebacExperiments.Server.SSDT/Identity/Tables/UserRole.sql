CREATE TABLE [Identity].[UserRole](
    [UserRoleID]            INT                                     CONSTRAINT [DF_Identity_UserRole_UserRoleID] DEFAULT (NEXT VALUE FOR [Identity].[sq_UserRole]) NOT NULL,
    [UserID]                INT                                         NOT NULL,
    [RoleID]                INT                                         NOT NULL,
    [RowVersion]            ROWVERSION                                  NULL,
    [LastEditedBy]          INT                                         NOT NULL,
    [ValidFrom]             DATETIME2 (7) GENERATED ALWAYS AS ROW START NOT NULL,
    [ValidTo]               DATETIME2 (7) GENERATED ALWAYS AS ROW END   NOT NULL,
    CONSTRAINT [PK_UserRole] PRIMARY KEY ([UserRoleID]),
    CONSTRAINT [FK_UserRole_UserID_User_UserID] FOREIGN KEY ([UserID]) REFERENCES [Identity].[User] ([UserID]),
    CONSTRAINT [FK_UserRole_RoleID_Role_RoleID] FOREIGN KEY ([RoleID]) REFERENCES [Identity].[Role] ([RoleID]),
    CONSTRAINT [FK_UserRole_LastEditedBy_User_UserID] FOREIGN KEY ([LastEditedBy]) REFERENCES [Identity].[User] ([UserID]),
    PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [Identity].[UserRoleHistory]));
CREATE TABLE [Application].[OrganizationRole](
    [OrganizationRoleID]    INT                                         CONSTRAINT [DF_Application_OrganizationRole_OrganizationRoleID] DEFAULT (NEXT VALUE FOR [Application].[sq_OrganizationRole]) NOT NULL,
    [OrganizationID]        INT                                         NOT NULL,
    [UserID]                INT                                         NOT NULL,
    [Role]                  NVARCHAR(255)                               NOT NULL,
    [RowVersion]            ROWVERSION                                  NULL,
    [LastEditedBy]          INT                                         NOT NULL,
    [ValidFrom]             DATETIME2 (7) GENERATED ALWAYS AS ROW START NOT NULL,
    [ValidTo]               DATETIME2 (7) GENERATED ALWAYS AS ROW END   NOT NULL,
    CONSTRAINT [PK_OrganizationRole] PRIMARY KEY ([OrganizationRoleID]),
    CONSTRAINT [FK_OrganizationRole_Organization_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [Application].[Organization] ([OrganizationID]),
    CONSTRAINT [FK_OrganizationRole_User_UserID] FOREIGN KEY ([UserID]) REFERENCES [Identity].[User] ([UserID]),
    CONSTRAINT [FK_OrganizationRole_User_LastEditedBy] FOREIGN KEY ([LastEditedBy]) REFERENCES [Identity].[User] ([UserID]),
    PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [Application].[OrganizationRoleHistory]));
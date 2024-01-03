CREATE TABLE [Application].[LocalizationRecord](
    [LocalizationRecordID]         INT                                         CONSTRAINT [DF_Application_LocalizationRecord_LocalizationRecordID] DEFAULT (NEXT VALUE FOR [Application].[sq_LocalizationRecord]) NOT NULL,
    [Name]                         NVARCHAR(255)                               NOT NULL,
    [Category]                     NVARCHAR(255)                               NOT NULL,
    [Value]                        NVARCHAR(2000)                              NOT NULL,
    [LanguageID]                   INT                                         NOT NULL,
    [RowVersion]                   ROWVERSION                                  NULL,
    [LastEditedBy]                 INT                                         NOT NULL,
    [ValidFrom]                    DATETIME2 (7) GENERATED ALWAYS AS ROW START NOT NULL,
    [ValidTo]                      DATETIME2 (7) GENERATED ALWAYS AS ROW END   NOT NULL,
    CONSTRAINT [PK_LocalizationRecord] PRIMARY KEY ([LocalizationRecordID]),
    CONSTRAINT [FK_LocalizationRecord_Language_LanguageID] FOREIGN KEY ([LanguageID]) REFERENCES [Application].[Language] ([LanguageID]),
    CONSTRAINT [FK_LocalizationRecord_User_LastEditedBy] FOREIGN KEY ([LastEditedBy]) REFERENCES [Identity].[User] ([UserID]),
    PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [Application].[LocalizationRecordHistory]));
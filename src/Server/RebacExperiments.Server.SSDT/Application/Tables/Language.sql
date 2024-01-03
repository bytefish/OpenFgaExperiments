CREATE TABLE [Application].[Language](
    [LanguageID]                        INT                                         CONSTRAINT [DF_Application_Language_LanguageID] DEFAULT (NEXT VALUE FOR [Application].[sq_Language]) NOT NULL,
    [Name]                              NVARCHAR(255)                               NOT NULL,
    [DisplayName]                       NVARCHAR(255)                               NOT NULL,
    [EnglishName]                       NVARCHAR(255)                               NOT NULL,
    [TwoLetterISOLanguageName]          NCHAR(2)                                    NOT NULL,
    [ThreeLetterISOLanguageName]        NCHAR(3)                                    NOT NULL,
    [ThreeLetterWindowsLanguageName]    NCHAR(3)                                    NOT NULL,
    [RowVersion]                        ROWVERSION                                  NULL,
    [LastEditedBy]                      INT                                         NOT NULL,
    [ValidFrom]                         DATETIME2 (7) GENERATED ALWAYS AS ROW START NOT NULL,
    [ValidTo]                           DATETIME2 (7) GENERATED ALWAYS AS ROW END   NOT NULL,
    CONSTRAINT [PK_Language] PRIMARY KEY ([LanguageID]),
    CONSTRAINT [FK_Language_User_LastEditedBy] FOREIGN KEY ([LastEditedBy]) REFERENCES [Identity].[User] ([UserID]),
    PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [Application].[LanguageHistory]));
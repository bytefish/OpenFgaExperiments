PRINT 'Inserting [Application].[Language] ...'

-----------------------------------------------
-- Global Parameters
-----------------------------------------------
DECLARE @ValidFrom datetime2(7) = '20130101'
DECLARE @ValidTo datetime2(7) =  '99991231 23:59:59.9999999'

-----------------------------------------------
-- [Application].[Language]
-----------------------------------------------
MERGE INTO [Application].[Language] AS [Target]
USING (VALUES 
       (1,	'en', 'English', 'English', 'en', 'eng', 'ENU', 1, @ValidFrom, @ValidTo)
      ,(2,	'de', 'German', 'German', 'de', 'deu', 'DEU', 1, @ValidFrom, @ValidTo)
) AS [Source]([LanguageID], [Name], [DisplayName], [EnglishName], [TwoLetterISOLanguageName], [ThreeLetterISOLanguageName], [ThreeLetterWindowsLanguageName], [LastEditedBy], [ValidFrom], [ValidTo])
ON ([Target].[LanguageID] = [Source].[LanguageID])
WHEN NOT MATCHED BY TARGET THEN
	INSERT 
		([LanguageID], [Name], [DisplayName], [EnglishName], [TwoLetterISOLanguageName], [ThreeLetterISOLanguageName], [ThreeLetterWindowsLanguageName], [LastEditedBy], [ValidFrom], [ValidTo])
	VALUES 
		([Source].[LanguageID], [Source].[Name], [Source].[DisplayName], [Source].[EnglishName], [Source].[TwoLetterISOLanguageName], [Source].[ThreeLetterISOLanguageName], [Source].[ThreeLetterWindowsLanguageName], [Source].[LastEditedBy], [Source].[ValidFrom], [Source].[ValidTo]);
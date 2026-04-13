CREATE OR ALTER FUNCTION UFN_Setting(
	@ModuleId	INT
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT	MAX(CASE WHEN SettingName = 'SettingName'	THEN SettingValue ELSE '' END) EntityName,
			MAX(CASE WHEN SettingName = 'TableName'		THEN SettingValue ELSE '' END) TableName
	  FROM Setting A (NOLOCK)
	WHERE EntityId = @ModuleId
)
GO
SELECT * FROM dbo.UFN_Setting(37) A

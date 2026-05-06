SELECT TOP 1 [IsCreate], [IsUpdate], [IsDelete], [IsExport], [IsByOn]
  FROM [dbo].[Eazly.ConfigContents] (NOLOCK)
WHERE TenantId = @TenantId AND SiteId = @SiteId AND ModuleId = @ModuleId AND QueryID = '@QueryId'
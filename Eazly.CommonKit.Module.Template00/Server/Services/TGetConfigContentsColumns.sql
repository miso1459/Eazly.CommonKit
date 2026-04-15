SELECT [ColumnName], [ColumnCaption], [IsPrimary], [IsEditable], [IsRequired], [IsVisible], [DefaultValue], [DataFormat]
  FROM [dbo].[Eazly.ConfigContentsColumns] (NOLOCK)
WHERE TenantId = @TenantId AND SiteId = @SiteId AND ModuleId = @ModuleId AND QueryID = '@QueryId'
SELECT	A.[ColumnName], A.[ColumnCaption], A.[IsPrimary], A.[IsEditable], A.[IsRequired], A.[IsVisible], A.[DefaultValue], A.[DataFormat], A.[Width], 
		ISNULL(A.[ControlType], '') ControlType,
		CASE WHEN A.[ControlType] = 'DropDown' THEN 
				CASE	WHEN C.[FCode] IS NOT NULL THEN 'SELECT TOP 1000 [SEQ], [FCODE], [FDESC] FROM [dbo].[Eazly.CommonCodeList] (NOLOCK) WHERE [MCode] = '''+C.[FCode]+''' ORDER BY SEQ' 
						WHEN ISNULL(B.[FSQL], '') = '' THEN 'SELECT '''' [FCODE], '''' [FDESC]' 
					ELSE ISNULL(B.[FSQL], '') 
				END
			ELSE ''
		END DropDownSQL, 
		ISNULL(A.[DropDownWhere], '') DropDownWhere
  FROM [dbo].[Eazly.ConfigContentsColumns] A (NOLOCK)
LEFT JOIN [dbo].[Eazly.ConfigDropdown] B (NOLOCK) ON (A.[DropDownType] = B.[FCode] AND A.TenantId = B.TenantId AND A.SiteId = B.SiteId)
LEFT JOIN [dbo].[Eazly.CommonCode] C (NOLOCK) ON (A.[DropDownType] = C.[FCode] AND A.TenantId = C.TenantId AND A.SiteId = C.SiteId)
WHERE A.TenantId = @TenantId AND A.SiteId = @SiteId AND A.ModuleId = @ModuleId AND A.QueryID = '@QueryId'
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Eazly.ConfigContents]') AND type in (N'U'))
CREATE TABLE [dbo].[Eazly.ConfigContents](
	[ModuleId] [int] NOT NULL,
	[QueryID] [nvarchar](255) NOT NULL,
	[IsCreate] [nchar](1) NOT NULL,
    [IsUpdate] [nchar](1) NOT NULL,
    [IsDelete] [nchar](1) NOT NULL,
    [IsExport] [nchar](1) NOT NULL,
	[TenantId] [int] NOT NULL,
	[SiteId] [int] NOT NULL,
	[CreatedBy] [nvarchar](256) NOT NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[ModifiedBy] [nvarchar](256) NOT NULL,
	[ModifiedOn] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Eazly.ConfigContents] PRIMARY KEY CLUSTERED 
(
	[ModuleId] ASC,
	[QueryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT INTO [dbo].[Eazly.ConfigContents]
SELECT '@ModuleId', '@QueryId', 'Y', 'Y', 'Y', 'Y', @TenantId, @SiteId, '@UserId', GETDATE(), '@UserId', GETDATE()
  FROM (SELECT '@ModuleId' ModuleId, '@QueryId' QueryId) A
LEFT JOIN [dbo].[Eazly.ConfigContents] B ON (A.ModuleId = B.ModuleId AND A.queryId = B.queryId)
WHERE B.ModuleId IS NULL

GO

IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Eazly.ConfigContentsColumn]') AND type in (N'U'))
CREATE TABLE [dbo].[Eazly.ConfigContentsColumn](
	[ModuleId] [int] NOT NULL,
	[QueryID] [nvarchar](255) NOT NULL,
	[ColumnName] [nvarchar](255) NOT NULL,
	[ColumnTitle] [nvarchar](255) NOT NULL,
	[DataType] [nvarchar](255) NOT NULL,
	[IsPrimary] [nchar](1) NOT NULL,
	[IsRequired] [nchar](1) NOT NULL,
	[IsEditable] [nchar](1) NOT NULL,
	[IsVisible] [nchar](1) NOT NULL,
	[DefaultValue] [nvarchar](255) NOT NULL,
	[TenantId] [int] NOT NULL,
	[SiteId] [int] NOT NULL,
	[CreatedBy] [nvarchar](256) NOT NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[ModifiedBy] [nvarchar](256) NOT NULL,
	[ModifiedOn] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Eazly.ConfigContentsColumn] PRIMARY KEY CLUSTERED 
(
	[ModuleId] ASC,
	[QueryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
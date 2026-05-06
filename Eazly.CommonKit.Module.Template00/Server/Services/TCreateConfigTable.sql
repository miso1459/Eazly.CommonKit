IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Eazly.ConfigContents]') AND type in (N'U'))
CREATE TABLE [dbo].[Eazly.ConfigContents](
	[TId] [int] IDENTITY(1,1) NOT NULL,
	[ModuleId] [int] NOT NULL,
	[QueryID] [nvarchar](255) NOT NULL,
	[IsCreate] [bit] NOT NULL,
    [IsUpdate] [bit] NOT NULL,
    [IsDelete] [bit] NOT NULL,
    [IsExport] [bit] NOT NULL,
	[IsByOn] [bit] NOT NULL,
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
SELECT '@ModuleId', '@QueryId', 1, 1, 1, 1, 1, @TenantId, @SiteId, '@UserId', GETDATE(), '@UserId', GETDATE()
  FROM (SELECT '@ModuleId' ModuleId, '@QueryId' QueryId) A
LEFT JOIN [dbo].[Eazly.ConfigContents] B ON (A.ModuleId = B.ModuleId AND A.queryId = B.queryId)
WHERE B.ModuleId IS NULL

GO

IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Eazly.ConfigContentsColumns]') AND type in (N'U'))
CREATE TABLE [dbo].[Eazly.ConfigContentsColumns](
	[TId] [int] IDENTITY(1,1) NOT NULL,
	[ModuleId] [int] NOT NULL,
	[QueryID] [nvarchar](255) NOT NULL,
	[ColumnName] [nvarchar](255) NOT NULL,
	[ColumnCaption] [nvarchar](255) NOT NULL,
	[IsPrimary] [bit] NOT NULL,
	[IsEditable] [bit] NOT NULL,
	[IsRequired] [bit] NOT NULL,
	[IsVisible] [bit] NOT NULL,
	[DefaultValue] [nvarchar](255) NOT NULL,
	[DataFormat] [nvarchar](255) NOT NULL,
	[Width] [int] NOT NULL,
	[TenantId] [int] NOT NULL,
	[SiteId] [int] NOT NULL,
	[CreatedBy] [nvarchar](256) NOT NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[ModifiedBy] [nvarchar](256) NOT NULL,
	[ModifiedOn] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Eazly.ConfigContentsColumns] PRIMARY KEY CLUSTERED 
(
	[ModuleId] ASC,
	[QueryID] ASC,
	[ColumnName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Eazly.ConfigContentsColumns] ADD  CONSTRAINT [DF_Eazly.ConfigContentsColumns_Width]  DEFAULT ((0)) FOR [Width]
GO

IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[@TableName]') AND type in (N'U'))
CREATE TABLE [dbo].[@TableName](
	[TId] [int] IDENTITY(1,1) NOT NULL,
	
	[DOCUMENT_DT] [datetime2](7) NOT NULL,
	[FCode] [nvarchar](100) NOT NULL,
	[FDesc] [nvarchar](255) NULL,

	[TenantId] [int] NOT NULL,
	[SiteId] [int] NOT NULL,
	[CreatedBy] [nvarchar](256) NOT NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[ModifiedBy] [nvarchar](256) NOT NULL,
	[ModifiedOn] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_@TableName] PRIMARY KEY CLUSTERED 
(
	[TId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE [name] = N'FK_@TableName_Tenant')
ALTER TABLE [dbo].[@TableName]  WITH CHECK ADD CONSTRAINT [FK_@TableName_Tenant] FOREIGN KEY([TenantId])
REFERENCES [dbo].[Tenant] ([TenantId])
ON DELETE CASCADE
GO



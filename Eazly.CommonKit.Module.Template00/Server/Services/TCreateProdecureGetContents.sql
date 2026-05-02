IF OBJECT_ID('USP_@EntityName_@QueryId', 'P') IS NULL
	EXEC(N'
CREATE PROCEDURE USP_@EntityName_@QueryId(
	@TenantId	INT,
	@SiteId		INT,
	@UserId		NVARCHAR(256),
	@JsonParam	NVARCHAR(4000)
)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @dateFrom		DATETIME2(7),
			@dateTo			DATETIME2(7),
			@searchValue	NVARCHAR(256);

	SELECT	@dateFrom		= MAX(CASE WHEN [KEY] = ''DateFrom''	THEN [value] ELSE NULL END),
			@dateTo			= MAX(CASE WHEN [KEY] = ''DateTo''		THEN [value] ELSE NULL END),
			@searchValue	= MAX(CASE WHEN [KEY] = ''SearchValue''	THEN [value] ELSE NULL END)
	  FROM OPENJSON(@JsonParam)

	--	SELECT @dateFrom AS dateFrom, @dateTo AS dateTo, @searchValue AS searchValue;

	-- TODO: 구현 

	-- result
	SELECT * FROM @TableName (NOLOCK)
	WHERE TenantId = @TenantId AND SiteId = @SiteId
	--  AND DOCUMENT_DT BETWEEN @dateFrom AND @dateTo
	  --AND (ISNULL([Code], '''') LIKE ''%'' + @searchValue + ''%'' OR ISNULL([Desc], '''') LIKE ''%'' + @searchValue + ''%'')
END
')
GO
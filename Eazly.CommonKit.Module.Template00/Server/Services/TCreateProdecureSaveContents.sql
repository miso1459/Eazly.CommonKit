IF OBJECT_ID('USP_@EntityName_Save_Pre', 'P') IS NULL
	EXEC(N'
CREATE PROCEDURE USP_@EntityName_Save_Pre(
	@TenantId	INT,
	@SiteId		INT,
	@UserId		NVARCHAR(256),
	@JsonParam	NVARCHAR(4000)
)
AS
BEGIN
	SET NOCOUNT ON;

    -- TODO: 구현

    -- SELECT * FROM OPENJSON(@JsonParam)

	-- 본 저장 프로시저 실행 필요 없으면 RERUN 9
	-- RETURN 9

    RETURN 1;
END
')
GO

IF OBJECT_ID('USP_@EntityName_Save_Post', 'P') IS NULL
	EXEC(N'
CREATE PROCEDURE USP_@EntityName_Save_Post(
	@TenantId	INT,
	@SiteId		INT,
	@UserId		NVARCHAR(256),
	@JsonParam	NVARCHAR(4000)
)
AS
BEGIN
	SET NOCOUNT ON;

    -- TODO: 구현

    -- SELECT * FROM OPENJSON(@JsonParam)

    RETURN 1;
END
')
GO

	EXEC(N'
CREATE OR ALTER PROCEDURE USP_@EntityName_Save(
	@TenantId	INT,
	@SiteId		INT,
	@UserId		NVARCHAR(256),
	@JsonParam	NVARCHAR(4000)
)
AS
BEGIN
	SET NOCOUNT ON;

	/* 아주아주 특별한 경우가 아니면 수정하지 말것, Pre, Post를 수정해서 처리*/

	DECLARE	@RTN_VALUE		INTEGER;

	-- 전처리
	EXEC @RTN_VALUE = USP_@EntityName_Save_Pre @TenantId, @SiteId, @UserId, @JsonParam

	-- 전처리 후 RERUN 9 이면 본 저장 프로시저 실행 필요 없으므로 종료
	IF @RTN_VALUE = 9
		RETURN 1	-- 정상종료

    -- TODO: 구현

    SELECT * FROM OPENJSON(@JsonParam)

	-- @CRUD

	-- 후처리
	EXEC USP_@EntityName_Save_Post @TenantId, @SiteId, @UserId, @JsonParam

    RETURN 1;
END
')
GO
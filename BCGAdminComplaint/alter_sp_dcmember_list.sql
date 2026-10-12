ALTER PROCEDURE [dbo].[SP_DCMember_List]
    @Id BIGINT,
    @SearchValue NVARCHAR(250),
    @FromDate NVARCHAR(50),
    @ToDate NVARCHAR(50),
    @RecordStatus NVARCHAR(50),
    @PageNo INT,
    @PageSize INT,
    @LoginUserId BIGINT,
    @SubMenuId BIGINT,
    @SortBy NVARCHAR(250),
    @ActionMode NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNo - 1) * @PageSize;
    DECLARE @TotalRecord INT;

    -- ========== SELECT LIST ==========
    IF @ActionMode = 'SelectList'
    BEGIN
        SELECT @TotalRecord = COUNT(m.Id)
        FROM tbl_Member m
        LEFT JOIN tbl_Enrollment_Mst en ON en.EId = m.EnrollmentId
        WHERE (@SearchValue = '' OR ISNULL(en.EnroNumber,'') LIKE '%' + @SearchValue + '%' OR ISNULL(en.FullName,'') LIKE '%' + @SearchValue + '%' OR ISNULL(en.ContactNumber,'') LIKE '%' + @SearchValue + '%')
          AND (@RecordStatus = '' OR m.Flag = @RecordStatus)
          AND (@FromDate = '' OR CAST(m.CreateDate AS DATE) >= CAST(@FromDate AS DATE))
          AND (@ToDate = '' OR CAST(m.CreateDate AS DATE) <= CAST(@ToDate AS DATE))

        SELECT 
        ROW_NUMBER() OVER (ORDER BY CASE WHEN @SortBy = 'ByNewest' THEN m.Id END DESC,CASE WHEN @SortBy = 'ByTitle' THEN en.FullName END ASC,CASE WHEN @SortBy = 'ByCreateDate' THEN m.CreateDate END DESC) AS SrNo,
        CAST(@TotalRecord AS NVARCHAR(50)) AS TotalRecord,m.Id,m.EnrollmentId,ISNULL(en.EnroNumber, '') AS EnroNumber,ISNULL(en.FullName, '') AS FullName,ISNULL(en.ContactNumber, '') AS ContactNumber,ISNULL(m.EmailAddress, '') AS EmailAddress,
        ISNULL(m.[Password], '') AS [Password],ISNULL(m.Flag, 'A') AS Flag,'A' AS EditRights,'A' AS DeleteRights,'Admin' AS CreateBy,'Admin' AS LastModifyBy,
        FORMAT(m.CreateDate, 'dd-MM-yyyy') AS SCreateDate,FORMAT(m.CreateDate, 'hh:mm tt') AS SCreateTime,FORMAT(m.UpdateDate, 'dd-MM-yyyy') AS LastModifyDate,FORMAT(m.UpdateDate, 'hh:mm tt') AS LastModifyTime
        FROM tbl_Member m
		LEFT JOIN tbl_Enrollment_Mst as en on en.EId=m.EnrollmentId
        WHERE (@SearchValue = '' OR ISNULL(en.EnroNumber,'') LIKE '%' + @SearchValue + '%' OR ISNULL(en.FullName,'') LIKE '%' + @SearchValue + '%' OR ISNULL(en.ContactNumber,'') LIKE '%' + @SearchValue + '%')
          AND (@RecordStatus = '' OR m.Flag = @RecordStatus)
          AND (@FromDate = '' OR CAST(m.CreateDate AS DATE) >= CAST(@FromDate AS DATE))
          AND (@ToDate = '' OR CAST(m.CreateDate AS DATE) <= CAST(@ToDate AS DATE))
        ORDER BY 
            CASE WHEN @SortBy = 'ByNewest' THEN m.Id END DESC,
            CASE WHEN @SortBy = 'ByTitle' THEN en.EnroNumber END ASC,
            CASE WHEN @SortBy = 'ByCreateDate' THEN m.CreateDate END DESC
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
    END
    -- ========== EDIT RECORD ==========
    ELSE IF @ActionMode = 'EditRecord'
    BEGIN
        SELECT 
            m.Id,
            m.EnrollmentId,
            ISNULL(en.EnroNumber, '') AS EnroNumber,
            ISNULL(m.EmailAddress, '') AS EmailAddress,
            ISNULL(m.[Password], '') AS [Password],
            ISNULL(en.FullName, '') AS Name,
            ISNULL(en.ContactNumber, '') AS ContactNo,
            ISNULL(b.Description, '') AS BarAssociation
        FROM tbl_Member m
        LEFT JOIN tbl_Enrollment_Mst en ON m.EnrollmentId = en.EId
        LEFT JOIN tbl_bar_association_mst b ON en.BarAssoId = b.Id
        WHERE m.Id = @Id
    END
END

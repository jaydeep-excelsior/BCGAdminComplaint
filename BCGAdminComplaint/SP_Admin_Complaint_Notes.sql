IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SP_Admin_Complaint_Notes]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SP_Admin_Complaint_Notes]
GO

CREATE PROCEDURE [dbo].[SP_Admin_Complaint_Notes]
    @Mode NVARCHAR(50) = NULL,
    @Id BIGINT = 0,
    @NoteId BIGINT = 0,
    @FileId BIGINT = 0,
    @LoginUserId BIGINT = 0,
    @NoteType NVARCHAR(50) = NULL,
    @NoteTitle NVARCHAR(MAX) = NULL,
    @NoteReferanceId BIGINT = NULL,
    @NoteDescription NVARCHAR(MAX) = NULL,
    @FileType NVARCHAR(MAX) = NULL,
    @FileName NVARCHAR(MAX) = NULL,
    @FilePath NVARCHAR(MAX) = NULL,
    @SearchValue NVARCHAR(MAX) = NULL,
    @FromDate NVARCHAR(50) = NULL,
    @ToDate NVARCHAR(50) = NULL,
    @PageNo INT = 1,
    @PageSize INT = 10,
    @OutputId BIGINT = 0 OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. SELECT ALL (LOGIN USER WISE WITH PAGINATION & FILTER)
    IF @Mode = 'SelectAll'
    BEGIN
        SET @SearchValue = LTRIM(RTRIM(ISNULL(@SearchValue, '')));
        SET @NoteType = LTRIM(RTRIM(ISNULL(@NoteType, '')));
        IF @PageNo <= 0 SET @PageNo = 1;
        IF @PageSize <= 0 SET @PageSize = 10;

        DECLARE @FromDT DATETIME = NULL;
        DECLARE @ToDT DATETIME = NULL;

        IF ISNULL(@FromDate, '') <> ''
        BEGIN
            BEGIN TRY
                SET @FromDT = CONVERT(DATETIME, @FromDate, 105);
            END TRY
            BEGIN CATCH
                SET @FromDT = NULL;
            END CATCH
        END

        IF ISNULL(@ToDate, '') <> ''
        BEGIN
            BEGIN TRY
                SET @ToDT = DATEADD(SECOND, 86399, CONVERT(DATETIME, @ToDate, 105));
            END TRY
            BEGIN CATCH
                SET @ToDT = NULL;
            END CATCH
        END

        -- Query matching records for this user
        ;WITH FilteredNotes AS (
            SELECT 
                n.Id,
                n.NoteType,
                n.NoteTitle,
                n.NoteReferanceId,
                n.NoteDescription,
                n.Flag,
                n.CreateDate,
                FORMAT(n.CreateDate, 'dd-MM-yyyy hh:mm tt') AS SCreateDate,
                n.CreateUser,
                n.UpdateDate,
                FORMAT(n.UpdateDate, 'dd-MM-yyyy hh:mm tt') AS SUpdateDate,
                n.UpdateUser,
                cm.ComplaintNumber,
                cm.FullName AS ComplainantName,
                ISNULL(u.FirstName + ' ' + u.LastName, u.UserName) AS CreateUserName,
                (SELECT COUNT(1) FROM tbl_Admin_Complaint_Notes_Files f WHERE f.NoteId = n.Id AND ISNULL(f.Flag, 'A') = 'A') AS FileCount
            FROM tbl_Admin_Complaint_Notes n
            LEFT JOIN tbl_Complaints_Manage cm ON cm.Id = n.NoteReferanceId
            LEFT JOIN tbl_register_mst u ON u.RegisterId = n.CreateUser
            WHERE ISNULL(n.Flag, 'A') = 'A'
              AND (@LoginUserId = 0 OR n.CreateUser = @LoginUserId)
              AND (@NoteType = '' OR @NoteType = 'All' 
                   OR n.NoteType = @NoteType
                   OR (@NoteType = 'Complaint Note' AND n.NoteType IN ('Complaint Note', 'Complaint'))
                   OR (@NoteType = 'General Note' AND n.NoteType IN ('General Note', 'General'))
                   OR (@NoteType = 'Complaint' AND n.NoteType IN ('Complaint Note', 'Complaint'))
                   OR (@NoteType = 'General' AND n.NoteType IN ('General Note', 'General'))
                  )
              AND (@SearchValue = '' OR (n.NoteTitle LIKE '%' + @SearchValue + '%' 
                                     OR n.NoteDescription LIKE '%' + @SearchValue + '%'
                                     OR cm.ComplaintNumber LIKE '%' + @SearchValue + '%'
                                     OR cm.FullName LIKE '%' + @SearchValue + '%'))
              AND (@FromDT IS NULL OR n.CreateDate >= @FromDT)
              AND (@ToDT IS NULL OR n.CreateDate <= @ToDT)
        ),
        CountedNotes AS (
            SELECT *, COUNT(*) OVER() AS TotalRecord,
                   ROW_NUMBER() OVER(ORDER BY Id DESC) AS RowNum
            FROM FilteredNotes
        )
        SELECT *
        FROM CountedNotes
        WHERE RowNum BETWEEN ((@PageNo - 1) * @PageSize + 1) AND (@PageNo * @PageSize)
        ORDER BY RowNum ASC;
    END

    -- 2. SELECT BY ID
    IF @Mode = 'SelectById'
    BEGIN
        SELECT 
            n.Id,
            n.NoteType,
            n.NoteTitle,
            n.NoteReferanceId,
            n.NoteDescription,
            n.Flag,
            n.CreateDate,
            FORMAT(n.CreateDate, 'dd-MM-yyyy hh:mm tt') AS SCreateDate,
            n.CreateUser,
            n.UpdateDate,
            FORMAT(n.UpdateDate, 'dd-MM-yyyy hh:mm tt') AS SUpdateDate,
            n.UpdateUser,
            cm.ComplaintNumber,
            cm.FullName AS ComplainantName,
            ISNULL(u.FirstName + ' ' + u.LastName, u.UserName) AS CreateUserName,
            (SELECT COUNT(1) FROM tbl_Admin_Complaint_Notes_Files f WHERE f.NoteId = n.Id AND ISNULL(f.Flag, 'A') = 'A') AS FileCount
        FROM tbl_Admin_Complaint_Notes n
        LEFT JOIN tbl_Complaints_Manage cm ON cm.Id = n.NoteReferanceId
        LEFT JOIN tbl_register_mst u ON u.RegisterId = n.CreateUser
        WHERE n.Id = @Id;

        -- Return files as second result set
        SELECT 
            f.Id,
            f.NoteId,
            f.FileType,
            f.FileName,
            f.FilePath,
            f.Flag,
            f.CreateDate,
            FORMAT(f.CreateDate, 'dd-MM-yyyy hh:mm tt') AS SCreateDate,
            f.CreateUser
        FROM tbl_Admin_Complaint_Notes_Files f
        WHERE f.NoteId = @Id AND ISNULL(f.Flag, 'A') = 'A'
        ORDER BY f.Id ASC;
    END

    -- 3. GET FILES BY NOTE ID
    IF @Mode = 'GetFilesByNoteId'
    BEGIN
        SELECT 
            f.Id,
            f.NoteId,
            f.FileType,
            f.FileName,
            f.FilePath,
            f.Flag,
            f.CreateDate,
            FORMAT(f.CreateDate, 'dd-MM-yyyy hh:mm tt') AS SCreateDate,
            f.CreateUser
        FROM tbl_Admin_Complaint_Notes_Files f
        WHERE f.NoteId = @NoteId AND ISNULL(f.Flag, 'A') = 'A'
        ORDER BY f.Id ASC;
    END

    -- 4. INSERT / UPDATE NOTE
    IF @Mode = 'InsertUpdate'
    BEGIN
        IF @Id = 0
        BEGIN
            INSERT INTO tbl_Admin_Complaint_Notes
                (NoteType, NoteTitle, NoteReferanceId, NoteDescription, Flag, CreateDate, CreateUser)
            VALUES
                (@NoteType, @NoteTitle, @NoteReferanceId, @NoteDescription, 'A', 
                 CONVERT(DATETIME, SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30')), @LoginUserId);

            SET @OutputId = SCOPE_IDENTITY();
        END
        ELSE
        BEGIN
            UPDATE tbl_Admin_Complaint_Notes
            SET NoteType = @NoteType,
                NoteTitle = @NoteTitle,
                NoteReferanceId = @NoteReferanceId,
                NoteDescription = @NoteDescription,
                UpdateDate = CONVERT(DATETIME, SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30')),
                UpdateUser = @LoginUserId
            WHERE Id = @Id;

            SET @OutputId = @Id;
        END

        SELECT @OutputId AS NoteId;
    END

    -- 5. DELETE NOTE
    IF @Mode = 'Delete'
    BEGIN
        UPDATE tbl_Admin_Complaint_Notes
        SET Flag = 'D',
            UpdateDate = CONVERT(DATETIME, SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30')),
            UpdateUser = @LoginUserId
        WHERE Id = @Id;

        UPDATE tbl_Admin_Complaint_Notes_Files
        SET Flag = 'D',
            UpdateDate = CONVERT(DATETIME, SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30')),
            UpdateUser = @LoginUserId
        WHERE NoteId = @Id;

        SELECT 1 AS Result;
    END

    -- 6. INSERT FILE
    IF @Mode = 'InsertFile'
    BEGIN
        INSERT INTO tbl_Admin_Complaint_Notes_Files
            (NoteId, FileType, FileName, FilePath, Flag, CreateDate, CreateUser)
        VALUES
            (@NoteId, @FileType, @FileName, @FilePath, 'A', 
             CONVERT(DATETIME, SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30')), @LoginUserId);

        SET @OutputId = SCOPE_IDENTITY();
        SELECT @OutputId AS FileId;
    END

    -- 7. DELETE FILE
    IF @Mode = 'DeleteFile'
    BEGIN
        UPDATE tbl_Admin_Complaint_Notes_Files
        SET Flag = 'D',
            UpdateDate = CONVERT(DATETIME, SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30')),
            UpdateUser = @LoginUserId
        WHERE Id = @FileId;

        SELECT 1 AS Result;
    END

    -- 8. SUMMARY STATS FOR LOGIN USER
    IF @Mode = 'Summary'
    BEGIN
        SELECT
            COUNT(1) AS TotalCount,
            ISNULL(SUM(CASE WHEN n.NoteType IN ('Complaint Note', 'Complaint') THEN 1 ELSE 0 END), 0) AS ComplaintCount,
            ISNULL(SUM(CASE WHEN n.NoteType IN ('General Note', 'General') THEN 1 ELSE 0 END), 0) AS GeneralCount,
            (SELECT COUNT(1) 
             FROM tbl_Admin_Complaint_Notes_Files f 
             INNER JOIN tbl_Admin_Complaint_Notes n2 ON n2.Id = f.NoteId 
             WHERE ISNULL(f.Flag, 'A') = 'A' AND ISNULL(n2.Flag, 'A') = 'A' 
               AND (@LoginUserId = 0 OR n2.CreateUser = @LoginUserId)) AS AttachedFilesCount,
            0 AS MeetingCount,
            0 AS ConfidentialCount,
            0 AS OtherCount
        FROM tbl_Admin_Complaint_Notes n
        WHERE ISNULL(n.Flag, 'A') = 'A'
          AND (@LoginUserId = 0 OR n.CreateUser = @LoginUserId);
    END

    -- 9. SEARCH COMPLAINTS LOOKUP / GET ALL COMPLAINTS
    IF @Mode = 'SearchComplaints' OR @Mode = 'GetAllComplaints'
    BEGIN
        SET @SearchValue = LTRIM(RTRIM(ISNULL(@SearchValue, '')));

        SELECT 
            Id, 
            ComplaintNumber, 
            FullName, 
            Status
        FROM tbl_Complaints_Manage
        WHERE ISNULL(Flag, 'A') = 'A'
          AND (@SearchValue = '' OR ComplaintNumber LIKE '%' + @SearchValue + '%' OR FullName LIKE '%' + @SearchValue + '%')
        ORDER BY Id DESC;
    END
END
GO

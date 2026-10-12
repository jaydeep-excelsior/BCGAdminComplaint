-- =============================================
-- Stored Procedure: SP_Search_Advocate_List
-- Description: Search advocates by name, enrollment number, or mobile number with pagination
-- Run this script on the Barcouncil database
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SP_Search_Advocate_List')
    DROP PROCEDURE SP_Search_Advocate_List
GO

CREATE PROCEDURE [dbo].[SP_Search_Advocate_List]
    @SearchPara NVARCHAR(200) = '',
    @PageNo INT = 1,
    @PageSize INT = 10,
    @Mode NVARCHAR(50) = 'SearchAdvocate'
AS
BEGIN
    SET NOCOUNT ON;

    IF @Mode = 'SearchAdvocate'
    BEGIN
        SELECT 
            EId,
            EnroNumber,
            FullName,
            ISNULL(ContactNumber, '') AS ContactNumber,
            COUNT(*) OVER() AS TotalRecord
        FROM tbl_Enrollment_Mst
        WHERE 
            (
                FullName LIKE '%' + @SearchPara + '%'
                OR EnroNumber LIKE '%' + @SearchPara + '%'
                OR ContactNumber LIKE '%' + @SearchPara + '%'
            )
        ORDER BY FullName
        OFFSET ((@PageNo - 1) * @PageSize) ROWS
        FETCH NEXT @PageSize ROWS ONLY
    END
END
GO

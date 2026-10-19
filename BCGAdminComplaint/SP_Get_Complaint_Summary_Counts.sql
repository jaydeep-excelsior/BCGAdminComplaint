CREATE PROCEDURE [dbo].[SP_Get_Complaint_Summary_Counts]
	@FromDate nvarchar(max) = null,
	@ToDate nvarchar(max) = null,
	@Year nvarchar(max) = null
AS
BEGIN
	SET NOCOUNT ON;

	;WITH CTE AS (
		SELECT 
			CM.Id,
			CM.Status,
			CM.ComplaintNumber,
			CM.CreateDate,
			(SELECT COUNT(*) FROM tbl_Complaint_Status_Log csl WHERE csl.ComplaintId = CM.Id AND csl.Status = 'Old') AS HasOldLog,
			(SELECT COUNT(*) FROM tbl_Complaint_Status_Log csl WHERE csl.ComplaintId = CM.Id AND csl.Status = 'New') AS HasNewLog
		FROM tbl_Complaints_Manage CM
		WHERE (ISNULL(@FromDate,'') = '' AND ISNULL(@ToDate,'') = '' OR
			(ISNULL(@FromDate,'') <> '' AND ISNULL(@ToDate,'') <> '' AND CONVERT(date, CM.CreateDate) BETWEEN CONVERT(date, @FromDate) AND CONVERT(date, @ToDate) OR
			 ISNULL(@FromDate,'') <> '' AND ISNULL(@ToDate,'') = '' AND CONVERT(date, CM.CreateDate) = CONVERT(date, @FromDate) OR
			 ISNULL(@FromDate,'') = '' AND ISNULL(@ToDate,'') <> '' AND CONVERT(date, CM.CreateDate) = CONVERT(date, @ToDate)))
		AND (ISNULL(@Year,'') = '' OR dbo.fn_Get_Advocate_Enrollment_Year(CM.ComplaintNumber) = @Year)
	)
	SELECT 
		CAST(COUNT(*) AS INT) AS TotalComplaint,
		CAST(ISNULL(SUM(CASE WHEN HasOldLog > 0 OR UPPER(Status) = 'OLD' THEN 1 ELSE 0 END), 0) AS INT) AS [Old],
		CAST(ISNULL(SUM(CASE WHEN HasNewLog > 0 OR UPPER(Status) = 'NEW' THEN 1 ELSE 0 END), 0) AS INT) AS [New],
		CAST(ISNULL(SUM(CASE WHEN UPPER(Status) LIKE '%ACCEPTED%' OR UPPER(Status) = 'ACCEPT' THEN 1 ELSE 0 END), 0) AS INT) AS Accepted,
		CAST(ISNULL(SUM(CASE WHEN UPPER(Status) LIKE '%MORE DETAIL%' OR UPPER(Status) LIKE '%DETAIL%' THEN 1 ELSE 0 END), 0) AS INT) AS MoreDetail,
		CAST(ISNULL(SUM(CASE WHEN (UPPER(Status) LIKE '%REJECTED%' OR UPPER(Status) = 'REJECT') AND UPPER(Status) NOT LIKE '%BOARD%' THEN 1 ELSE 0 END), 0) AS INT) AS Rejected,
		CAST(ISNULL(SUM(CASE WHEN UPPER(Status) LIKE '%REFER TO BOARD%' OR UPPER(Status) LIKE '%GENERAL BOARD%' THEN 1 ELSE 0 END), 0) AS INT) AS ReferToBoard,
		CAST(ISNULL(SUM(CASE WHEN UPPER(Status) LIKE '%BOARD ACCEPT%' AND UPPER(Status) NOT LIKE '%APPROVED%' THEN 1 ELSE 0 END), 0) AS INT) AS BoardAccept,
		CAST(ISNULL(SUM(CASE WHEN UPPER(Status) LIKE '%BOARD REJECT%' AND UPPER(Status) NOT LIKE '%HAS REJECTED%' THEN 1 ELSE 0 END), 0) AS INT) AS BoardReject,
		CAST(ISNULL(SUM(CASE WHEN UPPER(Status) LIKE '%BOARD HAS APPROVED%' OR UPPER(Status) LIKE '%APPROVED%' THEN 1 ELSE 0 END), 0) AS INT) AS BoardHasApproved,
		CAST(ISNULL(SUM(CASE WHEN UPPER(Status) LIKE '%BOARD HAS REJECTED%' THEN 1 ELSE 0 END), 0) AS INT) AS BoardHasRejected
	FROM CTE;
END
GO

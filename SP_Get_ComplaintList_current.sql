CREATE PROCEDURE [dbo].[SP_Get_ComplaintList]
	@SearchValue nvarchar(max) = null,
	@Status nvarchar(max) = null,
	@PaymentStatus nvarchar(max) = null,
	@FromDate nvarchar(max) = null,
	@ToDate nvarchar(max) = null,
	@Year nvarchar(max) = null,
	@FromNo nvarchar(max) = null,
	@ToNo nvarchar(max) = null,
	@PageNo int = 0,
	@PageSize int = 0,
	@SortColumn int = 0,
	@SortOrder varchar(50) = null,
	@Mode varchar(50) = '',
	@Id bigint = 0,
	@ComplaintIds bigint =0,
	@CreateUser bigint = 0
AS
BEGIN
	If @Mode = 'SelectAll'
	Begin
		IF(isnull(@PageSize,0)>0)
		Begin
			Select ROW_NUMBER() OVER(ORDER BY convert(bigint,dbo.fn_Get_Advocate_Enrollment_Year(ComplaintNumber)) DESC,convert(bigint,dbo.fn_Get_Advocate_Enrollment_No(ComplaintNumber)) DESC) AS SrNo,
			COUNT(*) OVER() AS TotalRecord,CM.*,
			isnull(cm.TotalAmount,0) as TotalAmountVal,
			isnull((case when isnull(cm.PaidAmount,0)<0 then 0 else isnull(cm.PaidAmount,0) end),0) as PaidAmount,
			(isnull(cm.TotalAmount,0)-(case when isnull(cm.PaidAmount,0)<0 then 0 else isnull(cm.PaidAmount,0) end)) as DueAmount,
			format(cm.CreateDate,'dd-MM-yyyy hh:mm tt') as ComplaintsCreateDate,
			(
				SELECT STUFF((
					SELECT ', ' + 
						CASE 
							WHEN (ISNULL(ca.IsKnown,'') = 'Y' OR ISNULL(ca.EnrollmentId,0) > 0) AND ISNULL(em.EnroNumber,'') <> '' 
							THEN RTRIM(LTRIM(ISNULL(em.FullName, ca.AdvocateName))) + ' (' + RTRIM(LTRIM(em.EnroNumber)) + ')'
							ELSE RTRIM(LTRIM(ISNULL(ca.AdvocateName, ISNULL(em.FullName,''))))
						END
					FROM tbl_Complaint_Against ca
					LEFT JOIN tbl_Enrollment_Mst em ON em.EId = ca.EnrollmentId
					WHERE ca.ComplaintId = CM.Id
					FOR XML PATH(''), TYPE
				).value('.', 'NVARCHAR(MAX)'), 1, 2, '')
			) AS ComplainedAgainst,
			(
				SELECT STUFF((
					SELECT ', ' + cac.FileName
					FROM tbl_Complaint_Affidavit_Copy cac
					WHERE cac.ComplaintId = cm.Id AND cac.IsOld = 'N' AND ISNULL(cac.FilePath,'') <> ''
					FOR XML PATH(''), TYPE
				).value('.', 'NVARCHAR(MAX)'), 1, 2, '')
			) AS AffidavitFilesText,
			(
				SELECT STUFF((
					SELECT ', ' + cf.FileName
					FROM tbl_Complaint_FileUpload cf
					WHERE cf.ComplaintId = cm.Id AND ISNULL(cf.FilePath,'') <> ''
					FOR XML PATH(''), TYPE
				).value('.', 'NVARCHAR(MAX)'), 1, 2, '')
			) AS AdditionalFilesText,
			(
				SELECT STUFF((
					SELECT ', ' + 'Receipt: ' + CAST(cp.ReceiptNumber AS VARCHAR(50)) + ' (' + ISNULL(cp.PaymentMode,'') + ' - ' + FORMAT(cp.ReceiptDate,'dd-MM-yyyy') + ' - Rs.' + CAST(isnull(cp.PaidAmount,0) AS VARCHAR(50)) + ')'
					FROM tbl_Complaint_Payment_Log cp
					WHERE cp.ComplaintId = cm.Id
					FOR XML PATH(''), TYPE
				).value('.', 'NVARCHAR(MAX)'), 1, 2, '')
			) AS PaymentInfoText,
			(select isnull(cac.Id,0) as Id, isnull(cac.ComplaintId,0) as ComplaintId, isnull(cac.FileName,'') as FileName, isnull(cac.FilePath,'') as FilePath from tbl_Complaint_Affidavit_Copy as cac where cac.ComplaintId=cm.Id and cac.IsOld='N' and ISNULL(cac.FilePath,'')<>'' FOR JSON PATH) as ComplaintAffidavitCopyList,
			(select isnull(cf.Id,0) as Id, isnull(cf.ComplaintId,0) as ComplaintId, isnull(cf.FileTitle,'') as FileTitle, isnull(cf.FileName,'') as FileName, isnull(cf.FilePath,'') as FilePath, isnull(cf.Description,'') as Description from tbl_Complaint_FileUpload as cf where cf.ComplaintId=cm.Id and ISNULL(cf.FilePath,'')<>'' FOR JSON PATH) as ComplaintAdditionalAttachmentsList,
			(
				select isnull(cp.Id,0) as Id, isnull(cp.ComplaintId,0) as ComplaintId, isnull(cp.ReceiptNumber,0) as ReceiptNumber, isnull(format(cp.ReceiptDate,'dd-MM-yyyy'),'') as ReceiptDate, isnull(cp.PaidAmount,0) as PaidAmount, 
				isnull(cp.PaymentMode,'') as PaymentMode, isnull(cp.TransactionNumber,'') as TransactionNumber, isnull(format(cp.TransactionDate,'dd-MM-yyyy'),'') as TransactionDate, isnull(cp.BankId,0) as BankId, 
				isnull((select top 1 bm.BankName from tbl_Bank_Mst as bm where bm.Id=isnull(cp.BankId,0) order by bm.Id desc),'') as BankName 
				from tbl_Complaint_Payment_Log as cp 
				where cp.ComplaintId=cm.Id FOR JSON PATH
			) as ComplaintPaymentList,
			(select COUNT(*) from tbl_Complaint_Status_Log as csl where csl.ComplaintId=CM.Id and csl.Status='New') as NewStatusCount,
			(select COUNT(*) from tbl_Complaint_Status_Log as csl where csl.ComplaintId=CM.Id and csl.Status='Old') as OldStatusCount
			FROM tbl_Complaints_Manage as CM
			WHERE (isnull(@Status,'') = ''  
					OR UPPER(isnull(@Status,''))=UPPER('New') and (select COUNT(*) from tbl_Complaint_Status_Log as csl where csl.ComplaintId=CM.Id and csl.Status='New')>0
					OR UPPER(isnull(@Status,''))=UPPER('Old') and (select COUNT(*) from tbl_Complaint_Status_Log as csl where csl.ComplaintId=CM.Id and csl.Status='Old')>0
					OR UPPER(CM.Status) LIKE '%UPPER(' + @Status + ')%')
			AND (isnull(@PaymentStatus,'') = '' OR UPPER(CM.PaymentStatus) LIKE '%' + @PaymentStatus + '%')
			AND (isnull(@FromDate,'') = '' AND isnull(@ToDate,'') = '' OR
				(isnull(@FromDate,'') <> '' AND isnull(@ToDate,'') <> '' and convert(date,CM.CreateDate) between convert(date,@FromDate) and convert(date,@ToDate) OR
				 isnull(@FromDate,'') <> '' AND isnull(@ToDate,'') = '' and convert(date,CM.CreateDate) = convert(date,@FromDate) OR
				 isnull(@FromDate,'') <> '' AND isnull(@ToDate,'') = '' and convert(date,CM.CreateDate) = convert(date,@ToDate)))
			AND (isnull(@Year,'') = '' OR dbo.fn_Get_Advocate_Enrollment_Year(ComplaintNumber) = @Year)
			AND (isnull(@FromNo,'') = '' AND isnull(@ToNo,'') = '' OR
				(isnull(@FromNo,'') <> '' AND isnull(@ToNo,'') <> '' and convert(bigint, dbo.fn_Get_Advocate_Enrollment_No(ComplaintNumber)) between convert(bigint,@FromNo) and convert(bigint,@ToNo) OR
				 isnull(@FromNo,'') <> '' AND isnull(@ToNo,'') = '' and convert(bigint, dbo.fn_Get_Advocate_Enrollment_No(ComplaintNumber)) = convert(bigint,@FromNo) OR
				 isnull(@FromNo,'') = '' AND isnull(@ToNo,'') <> '' and convert(bigint, dbo.fn_Get_Advocate_Enrollment_No(ComplaintNumber)) = convert(bigint,@ToNo)))
			AND (isnull(@SearchValue,'') = '' OR (
				UPPER(CM.FullName) LIKE '%'+ @SearchValue + '%' 
				OR UPPER(CM.EmailAddress) LIKE '%'+ @SearchValue + '%' 
				OR CM.ContactNumber LIKE '%'+ @SearchValue + '%' 
				OR UPPER(CM.ComplaintNumber) LIKE '%'+ @SearchValue + '%'
				OR EXISTS (
					SELECT 1 FROM tbl_Complaint_Against ca2 
					LEFT JOIN tbl_Enrollment_Mst em2 ON em2.EId = ca2.EnrollmentId 
					WHERE ca2.ComplaintId = CM.Id 
					AND (UPPER(ca2.AdvocateName) LIKE '%'+ @SearchValue + '%' OR UPPER(em2.FullName) LIKE '%'+ @SearchValue + '%' OR UPPER(em2.EnroNumber) LIKE '%'+ @SearchValue + '%')
				)
			))
			ORDER BY convert(bigint,dbo.fn_Get_Advocate_Enrollment_Year(ComplaintNumber)) DESC,convert(bigint,dbo.fn_Get_Advocate_Enrollment_No(ComplaintNumber)) DESC
			OFFSET (@PageNo - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY
		End
		Else
		Begin
			Select ROW_NUMBER() OVER(ORDER BY convert(bigint,dbo.fn_Get_Advocate_Enrollment_Year(CM.ComplaintNumber)) asc,convert(bigint,dbo.fn_Get_Advocate_Enrollment_No(CM.ComplaintNumber)) asc) AS SrNo,
			COUNT(*) OVER() AS TotalRecord,CM.*,
			isnull(cm.TotalAmount,0) as TotalAmountVal,
			isnull((case when isnull(cm.PaidAmount,0)<0 then 0 else isnull(cm.PaidAmount,0) end),0) as PaidAmount,
			(isnull(cm.TotalAmount,0)-(case when isnull(cm.PaidAmount,0)<0 then 0 else isnull(cm.PaidAmount,0) end)) as DueAmount,
			format(cm.CreateDate,'dd-MM-yyyy hh:mm tt') as ComplaintsCreateDate,
			(
				SELECT STUFF((
					SELECT ', ' + 
						CASE 
							WHEN (ISNULL(ca.IsKnown,'') = 'Y' OR ISNULL(ca.EnrollmentId,0) > 0) AND ISNULL(em.EnroNumber,'') <> '' 
							THEN RTRIM(LTRIM(ISNULL(em.FullName, ca.AdvocateName))) + ' (' + RTRIM(LTRIM(em.EnroNumber)) + ')'
							ELSE RTRIM(LTRIM(ISNULL(ca.AdvocateName, ISNULL(em.FullName,''))))
						END
					FROM tbl_Complaint_Against ca
					LEFT JOIN tbl_Enrollment_Mst em ON em.EId = ca.EnrollmentId
					WHERE ca.ComplaintId = CM.Id
					FOR XML PATH(''), TYPE
				).value('.', 'NVARCHAR(MAX)'), 1, 2, '')
			) AS ComplainedAgainst,
			(
				SELECT STUFF((
					SELECT ', ' + cac.FileName
					FROM tbl_Complaint_Affidavit_Copy cac
					WHERE cac.ComplaintId = cm.Id AND cac.IsOld = 'N' AND ISNULL(cac.FilePath,'') <> ''
					FOR XML PATH(''), TYPE
				).value('.', 'NVARCHAR(MAX)'), 1, 2, '')
			) AS AffidavitFilesText,
			(
				SELECT STUFF((
					SELECT ', ' + cf.FileName
					FROM tbl_Complaint_FileUpload cf
					WHERE cf.ComplaintId = cm.Id AND ISNULL(cf.FilePath,'') <> ''
					FOR XML PATH(''), TYPE
				).value('.', 'NVARCHAR(MAX)'), 1, 2, '')
			) AS AdditionalFilesText,
			(
				SELECT STUFF((
					SELECT ', ' + 'Receipt: ' + CAST(cp.ReceiptNumber AS VARCHAR(50)) + ' (' + ISNULL(cp.PaymentMode,'') + ' - ' + FORMAT(cp.ReceiptDate,'dd-MM-yyyy') + ' - Rs.' + CAST(isnull(cp.PaidAmount,0) AS VARCHAR(50)) + ')'
					FROM tbl_Complaint_Payment_Log cp
					WHERE cp.ComplaintId = cm.Id
					FOR XML PATH(''), TYPE
				).value('.', 'NVARCHAR(MAX)'), 1, 2, '')
			) AS PaymentInfoText,
			(select isnull(cac.Id,0) as Id, isnull(cac.ComplaintId,0) as ComplaintId, isnull(cac.FileName,'') as FileName, isnull(cac.FilePath,'') as FilePath from tbl_Complaint_Affidavit_Copy as cac where cac.ComplaintId=cm.Id and cac.IsOld='N' and ISNULL(cac.FilePath,'')<>'' FOR JSON PATH) as ComplaintAffidavitCopyList,
			(select isnull(cf.Id,0) as Id, isnull(cf.ComplaintId,0) as ComplaintId, isnull(cf.FileTitle,'') as FileTitle, isnull(cf.FileName,'') as FileName, isnull(cf.FilePath,'') as FilePath, isnull(cf.Description,'') as Description from tbl_Complaint_FileUpload as cf where cf.ComplaintId=cm.Id and ISNULL(cf.FilePath,'')<>'' FOR JSON PATH) as ComplaintAdditionalAttachmentsList,
			(select isnull(cp.Id,0) as Id, isnull(cp.ComplaintId,0) as ComplaintId, isnull(cp.ReceiptNumber,0) as ReceiptNumber, isnull(format(cp.ReceiptDate,'dd-MM-yyyy'),'') as ReceiptDate, isnull(cp.PaidAmount,0) as PaidAmount, isnull(cp.PaymentMode,'') as PaymentMode, isnull(cp.TransactionNumber,'') as TransactionNumber, isnull(format(cp.TransactionDate,'dd-MM-yyyy'),'') as TransactionDate, isnull(cp.BankId,0) as BankId, isnull((select top 1 bm.BankName from tbl_Bank_Mst as bm where bm.Id=isnull(cp.BankId,0) order by bm.Id desc),'') as BankName from tbl_Complaint_Payment_Log as cp where cp.ComplaintId=cm.Id FOR JSON PATH) as ComplaintPaymentList,
			(select COUNT(*) from tbl_Complaint_Status_Log as csl where csl.ComplaintId=CM.Id and csl.Status='New') as NewStatusCount,
			(select COUNT(*) from tbl_Complaint_Status_Log as csl where csl.ComplaintId=CM.Id and csl.Status='Old') as OldStatusCount
			FROM tbl_Complaints_Manage as CM
			WHERE (isnull(@Status,'') = ''  
					OR UPPER(isnull(@Status,''))=UPPER('New') and (select COUNT(*) from tbl_Complaint_Status_Log as csl where csl.ComplaintId=CM.Id and csl.Status='New')>0
					OR UPPER(isnull(@Status,''))=UPPER('Old') and (select COUNT(*) from tbl_Complaint_Status_Log as csl where csl.ComplaintId=CM.Id and csl.Status='Old')>0
					OR UPPER(CM.Status) LIKE '%UPPER(' + @Status + ')%')
			AND (isnull(@PaymentStatus,'') = '' OR UPPER(CM.PaymentStatus) LIKE '%' + @PaymentStatus + '%')
			AND (isnull(@FromDate,'') = '' AND isnull(@ToDate,'') = '' OR
				(isnull(@FromDate,'') <> '' AND isnull(@ToDate,'') <> '' and convert(date,CM.CreateDate) between convert(date,@FromDate) and convert(date,@ToDate) OR
				 isnull(@FromDate,'') <> '' AND isnull(@ToDate,'') = '' and convert(date,CM.CreateDate) = convert(date,@FromDate) OR
				 isnull(@FromDate,'') <> '' AND isnull(@ToDate,'') = '' and convert(date,CM.CreateDate) = convert(date,@ToDate)))
			AND (isnull(@Year,'') = '' OR dbo.fn_Get_Advocate_Enrollment_Year(ComplaintNumber) = @Year)
			AND (isnull(@FromNo,'') = '' AND isnull(@ToNo,'') = '' OR
				(isnull(@FromNo,'') <> '' AND isnull(@ToNo,'') <> '' and convert(bigint, dbo.fn_Get_Advocate_Enrollment_No(ComplaintNumber)) between convert(bigint,@FromNo) and convert(bigint,@ToNo) OR
				 isnull(@FromNo,'') <> '' AND isnull(@ToNo,'') = '' and convert(bigint, dbo.fn_Get_Advocate_Enrollment_No(ComplaintNumber)) = convert(bigint,@FromNo) OR
				 isnull(@FromNo,'') = '' AND isnull(@ToNo,'') <> '' and convert(bigint, dbo.fn_Get_Advocate_Enrollment_No(ComplaintNumber)) = convert(bigint,@ToNo)))
			AND (isnull(@SearchValue,'') = '' OR (
				UPPER(CM.FullName) LIKE '%'+ @SearchValue + '%' 
				OR UPPER(CM.EmailAddress) LIKE '%'+ @SearchValue + '%' 
				OR CM.ContactNumber LIKE '%'+ @SearchValue + '%' 
				OR UPPER(CM.ComplaintNumber) LIKE '%'+ @SearchValue + '%'
				OR EXISTS (
					SELECT 1 FROM tbl_Complaint_Against ca2 
					LEFT JOIN tbl_Enrollment_Mst em2 ON em2.EId = ca2.EnrollmentId 
					WHERE ca2.ComplaintId = CM.Id 
					AND (UPPER(ca2.AdvocateName) LIKE '%'+ @SearchValue + '%' OR UPPER(em2.FullName) LIKE '%'+ @SearchValue + '%' OR UPPER(em2.EnroNumber) LIKE '%'+ @SearchValue + '%')
				)
			))
			ORDER BY convert(bigint,dbo.fn_Get_Advocate_Enrollment_Year(ComplaintNumber)) asc,convert(bigint,dbo.fn_Get_Advocate_Enrollment_No(ComplaintNumber)) asc
		End
	End

	If @Mode = 'ReportExcePDFSelectAll'
	Begin
		IF(isnull(@PageSize,0)>0)
		Begin
			Select ROW_NUMBER() OVER(ORDER BY convert(bigint,dbo.fn_Get_Advocate_Enrollment_Year(ComplaintNumber)) asc,convert(bigint,dbo.fn_Get_Advocate_Enrollment_No(ComplaintNumber)) asc) AS SrNo,
			COUNT(*) OVER() AS TotalRecord,CM.*,
			isnull(cm.TotalAmount,0) as TotalAmountVal,
			isnull((case when isnull(cm.PaidAmount,0)<0 then 0 else isnull(cm.PaidAmount,0) end),0) as PaidAmount,
			(isnull(cm.TotalAmount,0)-(case when isnull(cm.PaidAmount,0)<0 then 0 else isnull(cm.PaidAmount,0) end)) as DueAmount,
			format(cm.CreateDate,'dd-MM-yyyy hh:mm tt') as ComplaintsCreateDate,
			(
				SELECT STUFF((
					SELECT ', ' + 
						CASE 
							WHEN (ISNULL(ca.IsKnown,'') = 'Y' OR ISNULL(ca.EnrollmentId,0) > 0) AND ISNULL(em.EnroNumber,'') <> '' 
							THEN RTRIM(LTRIM(ISNULL(em.FullName, ca.AdvocateName))) + ' (' + RTRIM(LTRIM(em.EnroNumber)) + ')'
							ELSE RTRIM(LTRIM(ISNULL(ca.AdvocateName, ISNULL(em.FullName,''))))
						END
					FROM tbl_Complaint_Against ca
					LEFT JOIN tbl_Enrollment_Mst em ON em.EId = ca.EnrollmentId
					WHERE ca.ComplaintId = CM.Id
					FOR XML PATH(''), TYPE
				).value('.', 'NVARCHAR(MAX)'), 1, 2, '')
			) AS ComplainedAgainst,
			(
				SELECT STUFF((
					SELECT ', ' + cac.FileName
					FROM tbl_Complaint_Affidavit_Copy cac
					WHERE cac.ComplaintId = cm.Id AND cac.IsOld = 'N' AND ISNULL(cac.FilePath,'') <> ''
					FOR XML PATH(''), TYPE
				).value('.', 'NVARCHAR(MAX)'), 1, 2, '')
			) AS AffidavitFilesText,
			(
				SELECT STUFF((
					SELECT ', ' + cf.FileName
					FROM tbl_Complaint_FileUpload cf
					WHERE cf.ComplaintId = cm.Id AND ISNULL(cf.FilePath,'') <> ''
					FOR XML PATH(''), TYPE
				).value('.', 'NVARCHAR(MAX)'), 1, 2, '')
			) AS AdditionalFilesText,
			(
				SELECT STUFF((
					SELECT ', ' + 'Receipt: ' + CAST(cp.ReceiptNumber AS VARCHAR(50)) + ' (' + ISNULL(cp.PaymentMode,'') + ' - ' + FORMAT(cp.ReceiptDate,'dd-MM-yyyy') + ' - Rs.' + CAST(isnull(cp.PaidAmount,0) AS VARCHAR(50)) + ')'
					FROM tbl_Complaint_Payment_Log cp
					WHERE cp.ComplaintId = cm.Id
					FOR XML PATH(''), TYPE
				).value('.', 'NVARCHAR(MAX)'), 1, 2, '')
			) AS PaymentInfoText,
			(select isnull(cac.Id,0) as Id, isnull(cac.ComplaintId,0) as ComplaintId, isnull(cac.FileName,'') as FileName, isnull(cac.FilePath,'') as FilePath from tbl_Complaint_Affidavit_Copy as cac where cac.ComplaintId=cm.Id and cac.IsOld='N' and ISNULL(cac.FilePath,'')<>'' FOR JSON PATH) as ComplaintAffidavitCopyList,
			(select isnull(cf.Id,0) as Id, isnull(cf.ComplaintId,0) as ComplaintId, isnull(cf.FileTitle,'') as FileTitle, isnull(cf.FileName,'') as FileName, isnull(cf.FilePath,'') as FilePath, isnull(cf.Description,'') as Description from tbl_Complaint_FileUpload as cf where cf.ComplaintId=cm.Id and ISNULL(cf.FilePath,'')<>'' FOR JSON PATH) as ComplaintAdditionalAttachmentsList,
			(select isnull(cp.Id,0) as Id, isnull(cp.ComplaintId,0) as ComplaintId, isnull(cp.ReceiptNumber,0) as ReceiptNumber, isnull(format(cp.ReceiptDate,'dd-MM-yyyy'),'') as ReceiptDate, isnull(cp.PaidAmount,0) as PaidAmount, isnull(cp.PaymentMode,'') as PaymentMode, isnull(cp.TransactionNumber,'') as TransactionNumber, isnull(format(cp.TransactionDate,'dd-MM-yyyy'),'') as TransactionDate, isnull(cp.BankId,0) as BankId, isnull((select top 1 bm.BankName from tbl_Bank_Mst as bm where bm.Id=isnull(cp.BankId,0) order by bm.Id desc),'') as BankName from tbl_Complaint_Payment_Log as cp where cp.ComplaintId=cm.Id FOR JSON PATH) as ComplaintPaymentList
			FROM tbl_Complaints_Manage as CM
			WHERE (isnull(@Status,'') = ''  
					OR UPPER(isnull(@Status,''))=UPPER('New') and (select COUNT(*) from tbl_Complaint_Status_Log as csl where csl.ComplaintId=CM.Id and csl.Status='New')>0
					OR UPPER(isnull(@Status,''))=UPPER('Old') and (select COUNT(*) from tbl_Complaint_Status_Log as csl where csl.ComplaintId=CM.Id and csl.Status='Old')>0
					OR UPPER(CM.Status) LIKE '%UPPER(' + @Status + ')%')
			AND (isnull(@PaymentStatus,'') = '' OR UPPER(CM.PaymentStatus) LIKE '%' + @PaymentStatus + '%')
			AND (isnull(@FromDate,'') = '' AND isnull(@ToDate,'') = '' OR
				(isnull(@FromDate,'') <> '' AND isnull(@ToDate,'') <> '' and convert(date,CM.CreateDate) between convert(date,@FromDate) and convert(date,@ToDate) OR
				 isnull(@FromDate,'') <> '' AND isnull(@ToDate,'') = '' and convert(date,CM.CreateDate) = convert(date,@FromDate) OR
				 isnull(@FromDate,'') <> '' AND isnull(@ToDate,'') = '' and convert(date,CM.CreateDate) = convert(date,@ToDate)))
			AND (isnull(@Year,'') = '' OR dbo.fn_Get_Advocate_Enrollment_Year(ComplaintNumber) = @Year)
			AND (isnull(@FromNo,'') = '' AND isnull(@ToNo,'') = '' OR
				(isnull(@FromNo,'') <> '' AND isnull(@ToNo,'') <> '' and convert(bigint, dbo.fn_Get_Advocate_Enrollment_No(ComplaintNumber)) between convert(bigint,@FromNo) and convert(bigint,@ToNo) OR
				 isnull(@FromNo,'') <> '' AND isnull(@ToNo,'') = '' and convert(bigint, dbo.fn_Get_Advocate_Enrollment_No(ComplaintNumber)) = convert(bigint,@FromNo) OR
				 isnull(@FromNo,'') = '' AND isnull(@ToNo,'') <> '' and convert(bigint, dbo.fn_Get_Advocate_Enrollment_No(ComplaintNumber)) = convert(bigint,@ToNo)))
			AND (isnull(@SearchValue,'') = '' OR (
				UPPER(CM.FullName) LIKE '%'+ @SearchValue + '%' 
				OR UPPER(CM.EmailAddress) LIKE '%'+ @SearchValue + '%' 
				OR CM.ContactNumber LIKE '%'+ @SearchValue + '%' 
				OR UPPER(CM.ComplaintNumber) LIKE '%'+ @SearchValue + '%'
				OR EXISTS (
					SELECT 1 FROM tbl_Complaint_Against ca2 
					LEFT JOIN tbl_Enrollment_Mst em2 ON em2.EId = ca2.EnrollmentId 
					WHERE ca2.ComplaintId = CM.Id 
					AND (UPPER(ca2.AdvocateName) LIKE '%'+ @SearchValue + '%' OR UPPER(em2.FullName) LIKE '%'+ @SearchValue + '%' OR UPPER(em2.EnroNumber) LIKE '%'+ @SearchValue + '%')
				)
			))
			ORDER BY convert(bigint,dbo.fn_Get_Advocate_Enrollment_Year(ComplaintNumber)) asc,convert(bigint,dbo.fn_Get_Advocate_Enrollment_No(ComplaintNumber)) asc
			OFFSET (@PageNo - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY
		End
		Else
		Begin
			Select ROW_NUMBER() OVER(ORDER BY convert(bigint,dbo.fn_Get_Advocate_Enrollment_Year(CM.ComplaintNumber)) asc,convert(bigint,dbo.fn_Get_Advocate_Enrollment_No(CM.ComplaintNumber)) asc) AS SrNo,
			COUNT(*) OVER() AS TotalRecord,CM.*,
			isnull(cm.TotalAmount,0) as TotalAmountVal,
			isnull((case when isnull(cm.PaidAmount,0)<0 then 0 else isnull(cm.PaidAmount,0) end),0) as PaidAmount,
			(isnull(cm.TotalAmount,0)-(case when isnull(cm.PaidAmount,0)<0 then 0 else isnull(cm.PaidAmount,0) end)) as DueAmount,
			format(cm.CreateDate,'dd-MM-yyyy hh:mm tt') as ComplaintsCreateDate,
			(
				SELECT STUFF((
					SELECT ', ' + 
						CASE 
							WHEN (ISNULL(ca.IsKnown,'') = 'Y' OR ISNULL(ca.EnrollmentId,0) > 0) AND ISNULL(em.EnroNumber,'') <> '' 
							THEN RTRIM(LTRIM(ISNULL(em.FullName, ca.AdvocateName))) + ' (' + RTRIM(LTRIM(em.EnroNumber)) + ')'
							ELSE RTRIM(LTRIM(ISNULL(ca.AdvocateName, ISNULL(em.FullName,''))))
						END
					FROM tbl_Complaint_Against ca
					LEFT JOIN tbl_Enrollment_Mst em ON em.EId = ca.EnrollmentId
					WHERE ca.ComplaintId = CM.Id
					FOR XML PATH(''), TYPE
				).value('.', 'NVARCHAR(MAX)'), 1, 2, '')
			) AS ComplainedAgainst,
			(
				SELECT STUFF((
					SELECT ', ' + cac.FileName
					FROM tbl_Complaint_Affidavit_Copy cac
					WHERE cac.ComplaintId = cm.Id AND cac.IsOld = 'N' AND ISNULL(cac.FilePath,'') <> ''
					FOR XML PATH(''), TYPE
				).value('.', 'NVARCHAR(MAX)'), 1, 2, '')
			) AS AffidavitFilesText,
			(
				SELECT STUFF((
					SELECT ', ' + cf.FileName
					FROM tbl_Complaint_FileUpload cf
					WHERE cf.ComplaintId = cm.Id AND ISNULL(cf.FilePath,'') <> ''
					FOR XML PATH(''), TYPE
				).value('.', 'NVARCHAR(MAX)'), 1, 2, '')
			) AS AdditionalFilesText,
			(
				SELECT STUFF((
					SELECT ', ' + 'Receipt: ' + CAST(cp.ReceiptNumber AS VARCHAR(50)) + ' (' + ISNULL(cp.PaymentMode,'') + ' - ' + FORMAT(cp.ReceiptDate,'dd-MM-yyyy') + ' - Rs.' + CAST(isnull(cp.PaidAmount,0) AS VARCHAR(50)) + ')'
					FROM tbl_Complaint_Payment_Log cp
					WHERE cp.ComplaintId = cm.Id
					FOR XML PATH(''), TYPE
				).value('.', 'NVARCHAR(MAX)'), 1, 2, '')
			) AS PaymentInfoText,
			(select isnull(cac.Id,0) as Id, isnull(cac.ComplaintId,0) as ComplaintId, isnull(cac.FileName,'') as FileName, isnull(cac.FilePath,'') as FilePath from tbl_Complaint_Affidavit_Copy as cac where cac.ComplaintId=cm.Id and cac.IsOld='N' and ISNULL(cac.FilePath,'')<>'' FOR JSON PATH) as ComplaintAffidavitCopyList,
			(select isnull(cf.Id,0) as Id, isnull(cf.ComplaintId,0) as ComplaintId, isnull(cf.FileTitle,'') as FileTitle, isnull(cf.FileName,'') as FileName, isnull(cf.FilePath,'') as FilePath, isnull(cf.Description,'') as Description from tbl_Complaint_FileUpload as cf where cf.ComplaintId=cm.Id and ISNULL(cf.FilePath,'')<>'' FOR JSON PATH) as ComplaintAdditionalAttachmentsList,
			(select isnull(cp.Id,0) as Id, isnull(cp.ComplaintId,0) as ComplaintId, isnull(cp.ReceiptNumber,0) as ReceiptNumber, isnull(format(cp.ReceiptDate,'dd-MM-yyyy'),'') as ReceiptDate, isnull(cp.PaidAmount,0) as PaidAmount, isnull(cp.PaymentMode,'') as PaymentMode, isnull(cp.TransactionNumber,'') as TransactionNumber, isnull(format(cp.TransactionDate,'dd-MM-yyyy'),'') as TransactionDate, isnull(cp.BankId,0) as BankId, isnull((select top 1 bm.BankName from tbl_Bank_Mst as bm where bm.Id=isnull(cp.BankId,0) order by bm.Id desc),'') as BankName from tbl_Complaint_Payment_Log as cp where cp.ComplaintId=cm.Id FOR JSON PATH) as ComplaintPaymentList
			FROM tbl_Complaints_Manage as CM
			WHERE (isnull(@Status,'') = ''  
					OR UPPER(isnull(@Status,''))=UPPER('New') and (select COUNT(*) from tbl_Complaint_Status_Log as csl where csl.ComplaintId=CM.Id and csl.Status='New')>0
					OR UPPER(isnull(@Status,''))=UPPER('Old') and (select COUNT(*) from tbl_Complaint_Status_Log as csl where csl.ComplaintId=CM.Id and csl.Status='Old')>0
					OR UPPER(CM.Status) LIKE '%UPPER(' + @Status + ')%')
			AND (isnull(@PaymentStatus,'') = '' OR UPPER(CM.PaymentStatus) LIKE '%' + @PaymentStatus + '%')
			AND (isnull(@FromDate,'') = '' AND isnull(@ToDate,'') = '' OR
				(isnull(@FromDate,'') <> '' AND isnull(@ToDate,'') <> '' and convert(date,CM.CreateDate) between convert(date,@FromDate) and convert(date,@ToDate) OR
				 isnull(@FromDate,'') <> '' AND isnull(@ToDate,'') = '' and convert(date,CM.CreateDate) = convert(date,@FromDate) OR
				 isnull(@FromDate,'') <> '' AND isnull(@ToDate,'') = '' and convert(date,CM.CreateDate) = convert(date,@ToDate)))
			AND (isnull(@Year,'') = '' OR dbo.fn_Get_Advocate_Enrollment_Year(ComplaintNumber) = @Year)
			AND (isnull(@FromNo,'') = '' AND isnull(@ToNo,'') = '' OR
				(isnull(@FromNo,'') <> '' AND isnull(@ToNo,'') <> '' and convert(bigint, dbo.fn_Get_Advocate_Enrollment_No(ComplaintNumber)) between convert(bigint,@FromNo) and convert(bigint,@ToNo) OR
				 isnull(@FromNo,'') <> '' AND isnull(@ToNo,'') = '' and convert(bigint, dbo.fn_Get_Advocate_Enrollment_No(ComplaintNumber)) = convert(bigint,@FromNo) OR
				 isnull(@FromNo,'') = '' AND isnull(@ToNo,'') <> '' and convert(bigint, dbo.fn_Get_Advocate_Enrollment_No(ComplaintNumber)) = convert(bigint,@ToNo)))
			AND (isnull(@SearchValue,'') = '' OR (
				UPPER(CM.FullName) LIKE '%'+ @SearchValue + '%' 
				OR UPPER(CM.EmailAddress) LIKE '%'+ @SearchValue + '%' 
				OR CM.ContactNumber LIKE '%'+ @SearchValue + '%' 
				OR UPPER(CM.ComplaintNumber) LIKE '%'+ @SearchValue + '%'
				OR EXISTS (
					SELECT 1 FROM tbl_Complaint_Against ca2 
					LEFT JOIN tbl_Enrollment_Mst em2 ON em2.EId = ca2.EnrollmentId 
					WHERE ca2.ComplaintId = CM.Id 
					AND (UPPER(ca2.AdvocateName) LIKE '%'+ @SearchValue + '%' OR UPPER(em2.FullName) LIKE '%'+ @SearchValue + '%' OR UPPER(em2.EnroNumber) LIKE '%'+ @SearchValue + '%')
				)
			))
			ORDER BY convert(bigint,dbo.fn_Get_Advocate_Enrollment_Year(ComplaintNumber)) asc,convert(bigint,dbo.fn_Get_Advocate_Enrollment_No(ComplaintNumber)) asc
		End
	End
	
	If @Mode = 'GetDocument'
	Begin
		select T1.SrNo,T1.Id,T1.ComplaintId,T1.FileTitle,T1.FilePath,T1.FileName,T1.Description,T1.DocumetType from 
		(
			select ROW_NUMBER() OVER(ORDER BY Id desc) as SrNo,Id,CreateDate,ComplaintId,FileTitle,FilePath,FileName,Description,'Additional' as DocumetType,1 as ShortOrder 
			from tbl_Complaint_FileUpload 
			where complaintid = @Id and ISNULL(FilePath,'')<>'' and Flag='A'
			union
			select ROW_NUMBER() OVER(ORDER BY Id desc) as SrNo,Id,CreateDate,ComplaintId,'' as FileTitle,FilePath,FileName,'' as Description,'Affidavit' as DocumetType,2 as ShortOrder 
			from tbl_Complaint_Affidavit_Copy 
			where ComplaintId = @Id  and ISNULL(FilePath,'')<>'' and Flag='A' and IsOld='N'
		) as T1
		order by T1.ShortOrder,T1.CreateDate desc
	End

	If @Mode = 'GetAdjuntDetails'
	Begin
		select ROW_NUMBER() OVER(ORDER BY CM.Id desc) as SrNo,CA.Amount,CA.Id,CA.ComplaintId,
		CASE 
			WHEN (ISNULL(CA.IsKnown,'') = 'Y' OR ISNULL(CA.EnrollmentId,0) > 0) AND ISNULL(EM.EnroNumber,'') <> '' 
			THEN RTRIM(LTRIM(ISNULL(EM.FullName, CA.AdvocateName)))
			ELSE RTRIM(LTRIM(ISNULL(CA.AdvocateName, ISNULL(EM.FullName,''))))
		END AS Name,
		CASE 
			WHEN (ISNULL(CA.IsKnown,'') = 'Y' OR ISNULL(CA.EnrollmentId,0) > 0) AND ISNULL(EM.EnroNumber,'') <> '' 
			THEN RTRIM(LTRIM(EM.EnroNumber))
			ELSE ''
		END AS EnrollmentNumber,
		CA.EnrollmentId
		from tbl_Complaint_Against as CA
		Left Join tbl_Complaints_Manage as CM on CM.Id=CA.ComplaintId
		Left Join tbl_Enrollment_Mst as EM on EM.EId=CA.EnrollmentId
		where (CA.ComplaintId=@ComplaintIds OR CA.ComplaintId=@Id)
		order by CA.Id desc
	End

	If @Mode = 'GetComplaintDetails'
	Begin
		Select *,FORMAT(CreateDate,'dd-MM-yyyy hh:mm tt') as SCreateDate from tbl_Complaints_Manage where Id = @Id
	End	

	IF @Mode = 'MemberListById'
	Begin
		declare @ComplaintId bigint=0;
		set @ComplaintId = ( Select ComplaintId from tbl_Complaint_Against where EnrollmentId = @Id)

		Select 
			ROW_NUMBER() OVER(ORDER BY
			CASE WHEN(@SortColumn = 1 AND UPPER(@SortOrder) = 'ASC') THEN CM.Id END ASC,
			CASE WHEN(@SortColumn = 1 AND UPPER(@SortOrder) = 'DESC') THEN CM.Id END DESC) AS SrNo,
			COUNT(*) OVER() AS TotalRecord,EM.FullName,Em.EmailAddress,Em.ContactNumber,
			CM.Summary,CM.DetailComplaint,CM.Status,CM.ComplaintNumber,CM.Id as ComplaintId
			FROM tbl_Enrollment_Mst as EM				
			inner join tbl_Complaints_Manage as CM on CM.Id = @ComplaintId
			WHERE Em.EId = @Id
			
			ORDER BY 
			CASE WHEN(@SortColumn = 1 AND UPPER(@SortOrder) = 'ASC') THEN CM.Id END ASC,
			CASE WHEN(@SortColumn = 1 AND UPPER(@SortOrder) = 'DESC') THEN CM.Id END DESC
			OFFSET (@PageNo - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY
	End

	If @Mode = 'GetComplaintCount'
	Begin
		select(Select IsNUll(Count(*),0) from tbl_Complaints_Manage) as TotalCount,
		(Select IsNUll(Count(*),0) as newcount from tbl_Complaints_Manage where Status = 'New') as NewStatusCount,
		(Select IsNUll(Count(*),0) as hearingcount from tbl_Complaints_Manage where Status = 'Hearing') as HearingStatusCount,
		(Select IsNUll(Count(*),0) as adjuntcount from tbl_Complaints_Manage where Status = 'Adjunt') as AdjuntStatusCount,
		(Select IsNUll(Count(*),0) as approvedcount from tbl_Complaints_Manage where Status = 'Board Has Approved') as ApprovedStatusCount,
		(Select IsNUll(Count(*),0) as rejectedcount from tbl_Complaints_Manage where Status = 'Board Has Rejected') as RejectedStatusCount,
		(Select IsNUll(Count(*),0) as acceptedcount from tbl_Complaints_Manage where Status = 'Accepted') as AcceptedStatusCount,
		(Select IsNUll(Count(*),0) as orderPassedcount from tbl_Complaints_Manage where Status = 'Order Passed') as OrderPassedStatusCount
	End

	If @Mode = 'GetMemberComplaintList'
	Begin
		Select ROW_NUMBER() OVER(ORDER BY convert(bigint,dbo.fn_Get_Advocate_Enrollment_Year(CM.ComplaintNumber)) asc,convert(bigint,dbo.fn_Get_Advocate_Enrollment_No(CM.ComplaintNumber)) asc) AS SrNo,
		COUNT(*) OVER() AS TotalRecord,CML.Id as MemberLogId,CML.MemberId,CM.Id,CM.ComplaintNumber,CM.FullName,CM.EmailAddress,CM.ContactNumber,CM.Address,CM.Summary,CM.DetailComplaint,isnull(CM.TotalAmount,0) as TotalAmount,CM.PaymentStatus,CM.Status,format(cm.CreateDate,'dd-MM-yyyy hh:mm tt') as ComplaintsCreateDate
		FROM tbl_Complaints_Manage as CM
		Left join tbl_Complaint_Member_Log as CML on CML.ComplaintId=CM.Id
		WHERE (isnull(@Status,'') = '' OR UPPER(CM.Status) LIKE '%UPPER(' + @Status + ')%')
		AND (isnull(@PaymentStatus,'') = '' OR UPPER(CM.PaymentStatus) LIKE '%' + @PaymentStatus + '%')
		AND (isnull(@FromDate,'') = '' AND isnull(@ToDate,'') = '' OR
			(isnull(@FromDate,'') <> '' AND isnull(@ToDate,'') <> '' and convert(date,CM.CreateDate) between convert(date,@FromDate) and convert(date,@ToDate) OR
				isnull(@FromDate,'') <> '' AND isnull(@ToDate,'') = '' and convert(date,CM.CreateDate) = convert(date,@FromDate) OR
				isnull(@FromDate,'') <> '' AND isnull(@ToDate,'') = '' and convert(date,CM.CreateDate) = convert(date,@ToDate)))
		AND (isnull(@Year,'') = '' OR dbo.fn_Get_Advocate_Enrollment_Year(ComplaintNumber) = @Year)
		AND (isnull(@FromNo,'') = '' AND isnull(@ToNo,'') = '' OR
			(isnull(@FromNo,'') <> '' AND isnull(@ToNo,'') <> '' and convert(bigint, dbo.fn_Get_Advocate_Enrollment_No(ComplaintNumber)) between convert(bigint,@FromNo) and convert(bigint,@ToNo) OR
				isnull(@FromNo,'') <> '' AND isnull(@ToNo,'') = '' and convert(bigint, dbo.fn_Get_Advocate_Enrollment_No(ComplaintNumber)) = convert(bigint,@FromNo) OR
				isnull(@FromNo,'') = '' AND isnull(@ToNo,'') <> '' and convert(bigint, dbo.fn_Get_Advocate_Enrollment_No(ComplaintNumber)) = convert(bigint,@ToNo)))
		AND (isnull(@SearchValue,'') = '' OR (UPPER(CM.FullName) LIKE '%'+ @SearchValue + '%' OR UPPER(CM.EmailAddress) LIKE '%'+ @SearchValue + '%' OR CM.ContactNumber LIKE '%'+ @SearchValue + '%' OR UPPER(CM.ComplaintNumber) LIKE '%'+ @SearchValue + '%'
		OR EXISTS (
			SELECT 1 FROM tbl_Complaint_Against ca2 
			LEFT JOIN tbl_Enrollment_Mst em2 ON em2.EId = ca2.EnrollmentId 
			WHERE ca2.ComplaintId = CM.Id 
			AND (UPPER(ca2.AdvocateName) LIKE '%'+ @SearchValue + '%' OR UPPER(em2.FullName) LIKE '%'+ @SearchValue + '%' OR UPPER(em2.EnroNumber) LIKE '%'+ @SearchValue + '%')
		)))
		AND CML.MemberId = @CreateUser
		ORDER BY convert(bigint,dbo.fn_Get_Advocate_Enrollment_Year(ComplaintNumber)) asc,convert(bigint,dbo.fn_Get_Advocate_Enrollment_No(ComplaintNumber)) asc
		OFFSET (@PageNo - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY
	End

	If @Mode = 'GetMemberComplaintListDetails'
	Begin
		Select ROW_NUMBER() OVER(ORDER BY CM.Id asc) AS SrNo,
		COUNT(*) OVER() AS TotalRecord,CML.Id as MemberLogId,CML.MemberId,CM.Id,CM.ComplaintNumber,CM.FullName,CM.EmailAddress,CM.ContactNumber,CM.Address,CM.Summary,CM.DetailComplaint,CM.TotalAmount,CM.PaymentStatus,CM.Status,format(cm.CreateDate,'dd-MM-yyyy hh:mm tt') as ComplaintsCreateDate
		FROM tbl_Complaints_Manage as CM
		Left join tbl_Complaint_Member_Log as CML on CML.ComplaintId=CM.Id
		where CM.Id = @Id
		ORDER BY CM.Id asc
	End
END

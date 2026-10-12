
ALTER PROCEDURE [dbo].[SP_Get_ComplaintList]
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
			Select ROW_NUMBER() OVER(ORDER BY 
				CASE WHEN UPPER(@SortOrder) = 'ASC' THEN convert(bigint,dbo.fn_Get_Advocate_Enrollment_Year(ComplaintNumber)) END ASC,
				CASE WHEN UPPER(@SortOrder) = 'ASC' THEN convert(bigint,dbo.fn_Get_Advocate_Enrollment_No(ComplaintNumber)) END ASC,
				CASE WHEN UPPER(@SortOrder) = 'DESC' OR ISNULL(@SortOrder, '') = '' THEN convert(bigint,dbo.fn_Get_Advocate_Enrollment_Year(ComplaintNumber)) END DESC,
				CASE WHEN UPPER(@SortOrder) = 'DESC' OR ISNULL(@SortOrder, '') = '' THEN convert(bigint,dbo.fn_Get_Advocate_Enrollment_No(ComplaintNumber)) END DESC
			) AS SrNo,
			COUNT(*) OVER() AS TotalRecord,CM.*,format(cm.CreateDate,'dd-MM-yyyy hh:mm tt') as ComplaintsCreateDate
			FROM tbl_Complaints_Manage as CM
			WHERE (isnull(@Status,'') = '' OR UPPER(CM.Status) LIKE '%' + @Status + '%')
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
			AND (isnull(@SearchValue,'') = '' OR (UPPER(CM.FullName) LIKE '%'+ @SearchValue + '%' OR UPPER(CM.EmailAddress) LIKE '%'+ @SearchValue + '%' OR CM.ContactNumber LIKE '%'+ @SearchValue + '%' OR UPPER(CM.ComplaintNumber) LIKE '%'+ @SearchValue + '%'))
			ORDER BY 
				CASE WHEN UPPER(@SortOrder) = 'ASC' THEN convert(bigint,dbo.fn_Get_Advocate_Enrollment_Year(ComplaintNumber)) END ASC,
				CASE WHEN UPPER(@SortOrder) = 'ASC' THEN convert(bigint,dbo.fn_Get_Advocate_Enrollment_No(ComplaintNumber)) END ASC,
				CASE WHEN UPPER(@SortOrder) = 'DESC' OR ISNULL(@SortOrder, '') = '' THEN convert(bigint,dbo.fn_Get_Advocate_Enrollment_Year(ComplaintNumber)) END DESC,
				CASE WHEN UPPER(@SortOrder) = 'DESC' OR ISNULL(@SortOrder, '') = '' THEN convert(bigint,dbo.fn_Get_Advocate_Enrollment_No(ComplaintNumber)) END DESC
			OFFSET (@PageNo - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY
		End
		Else
		Begin
			Select ROW_NUMBER() OVER(ORDER BY 
				CASE WHEN UPPER(@SortOrder) = 'ASC' THEN convert(bigint,dbo.fn_Get_Advocate_Enrollment_Year(CM.ComplaintNumber)) END ASC,
				CASE WHEN UPPER(@SortOrder) = 'ASC' THEN convert(bigint,dbo.fn_Get_Advocate_Enrollment_No(CM.ComplaintNumber)) END ASC,
				CASE WHEN UPPER(@SortOrder) = 'DESC' OR ISNULL(@SortOrder, '') = '' THEN convert(bigint,dbo.fn_Get_Advocate_Enrollment_Year(CM.ComplaintNumber)) END DESC,
				CASE WHEN UPPER(@SortOrder) = 'DESC' OR ISNULL(@SortOrder, '') = '' THEN convert(bigint,dbo.fn_Get_Advocate_Enrollment_No(CM.ComplaintNumber)) END DESC
			) AS SrNo,
			COUNT(*) OVER() AS TotalRecord,CM.*,format(cm.CreateDate,'dd-MM-yyyy hh:mm tt') as ComplaintsCreateDate
			FROM tbl_Complaints_Manage as CM
			WHERE (isnull(@Status,'') = '' OR UPPER(CM.Status) LIKE '%' + @Status + '%')
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
			AND (isnull(@SearchValue,'') = '' OR (UPPER(CM.FullName) LIKE '%'+ @SearchValue + '%' OR UPPER(CM.EmailAddress) LIKE '%'+ @SearchValue + '%' OR CM.ContactNumber LIKE '%'+ @SearchValue + '%' OR UPPER(CM.ComplaintNumber) LIKE '%'+ @SearchValue + '%'))
			ORDER BY 
				CASE WHEN UPPER(@SortOrder) = 'ASC' THEN convert(bigint,dbo.fn_Get_Advocate_Enrollment_Year(ComplaintNumber)) END ASC,
				CASE WHEN UPPER(@SortOrder) = 'ASC' THEN convert(bigint,dbo.fn_Get_Advocate_Enrollment_No(ComplaintNumber)) END ASC,
				CASE WHEN UPPER(@SortOrder) = 'DESC' OR ISNULL(@SortOrder, '') = '' THEN convert(bigint,dbo.fn_Get_Advocate_Enrollment_Year(ComplaintNumber)) END DESC,
				CASE WHEN UPPER(@SortOrder) = 'DESC' OR ISNULL(@SortOrder, '') = '' THEN convert(bigint,dbo.fn_Get_Advocate_Enrollment_No(ComplaintNumber)) END DESC
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
			select ROW_NUMBER() OVER(ORDER BY CM.Id desc) as SrNo,CA.Amount,CA.Id,CA.ComplaintId,EM.FullName AS Name, EM.EnroNumber As EnrollmentNumber,CA.EnrollmentId
			from tbl_Complaint_Against as CA
			Inner Join tbl_Enrollment_Mst as EM on EM.EId = CA.EnrollmentId
			left join tbl_Complaints_Manage as CM on CM.Id = CA.ComplaintId
			where CA.complaintid = @Id --and CM.Status = 'Adjunt' 
			order by CM.id desc
		End	

	If @Mode = 'GetComplaintDetails'
		Begin
			Select *,FORMAT(CreateDate,'dd-MM-yyyy hh:mm tt') as SCreateDate from tbl_Complaints_Manage where Id = @Id
		End	

	IF @Mode = 'MemberListById'
		Begin
			declare @ComplaintId bigint=0;
			--declare @ContactNumber nvarchar(max);
			set @ComplaintId = ( Select ComplaintId from tbl_Complaint_Against where EnrollmentId = @Id)
			--set @ContactNumber = ( Select ContactNumber from tbl_Enrollment_Mst where EId = @Id)

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

	--If @Mode = 'GetReplyDetails'
	--	Begin
	--		Select Id,ComplaintId,ReceiverId,CreateUser,convert(char(5), CreateDate, 108)as Time,ReplyMessage,SenderType,ReceiverType,isnull(FileName,'') as FileName,isnull(FilePath,'') as FilePath,
	--		--(case when SenderType = 'A' then (1) else (2) end) as SenderName1,
	--		(case when SenderType = 'A' then (select CONCAT(FirstName,' ',Lastname) from tbl_register_mst where RegisterId = CreateUser) else (select  FullName from tbl_Enrollment_Mst where EId = CreateUser) end) as SenderName,
	--		(case when ReceiverType = 'A' then (select CONCAT(FirstName,' ',Lastname) from tbl_register_mst where RegisterId = ReceiverId) else (select  FullName from tbl_Enrollment_Mst where EId = ReceiverId) end) as ReceiverName
	--		FROM tbl_Complaint_Reply_Mst 
	--		where ComplaintId = @ComplaintIds and (CreateUser = @CreateUser or CreateUser = @Id) and (ReceiverId = @Id Or ReceiverId = @CreateUser) 
	--		and SenderType <> 'U' and ReceiverType <> 'U'
	--		--select top 1 CreateUser, ReceiverId,ComplaintId,ReplyMessage,(case when (select count(*) from tbl_register_mst where RegisterId = @CreateUser)>0 then (select CONCAT(FirstName,' ',Lastname) from tbl_register_mst where RegisterId = @CreateUser) else (select  FullName from tbl_Enrollment_Mst where EId = @CreateUser) end) as Name,
	--		--convert(char(5), CreateDate, 108)as Time,Id from tbl_Complaint_Reply_Mst 
	--		--where ComplaintId = @ComplaintIds and CreateUser = @CreateUser and ReceiverId = @Id			
	--	End

	--If @Mode = 'GetReplyDetailsForCount'
	--	Begin
	--		select top 1 CreateUser, ReceiverId,ComplaintId,ReplyMessage,(case when (select count(*) from tbl_register_mst where RegisterId = 1)>0 then (select CONCAT(FirstName,' ',Lastname) from tbl_register_mst where RegisterId = CreateUser) else (select  FullName from tbl_Enrollment_Mst where EId = 1) end) as SenderName,
	--		convert(char(5), CreateDate, 108)as Time,Id from tbl_Complaint_Reply_Mst 
	--		where ComplaintId = @ComplaintIds  and ReceiverId = @Id			
	--	End
END

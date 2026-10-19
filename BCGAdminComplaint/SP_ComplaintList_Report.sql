CREATE PROCEDURE [dbo].[SP_ComplaintList_Report] 
	@ComplaintId bigint = 0,
	@Mode nvarchar(max)=null
AS
BEGIN
	If @Mode = 'ComplaintReport'
		Begin
			Select ROW_NUMBER() OVER(ORDER BY Id desc) as SrNo,FullName,EmailAddress,ContactNumber,Address,Isnull(Summary,'') as Summary,isnull(DetailComplaint,'') as DetailComplaint,
			isnull(PaymentStatus,'') as PaymentStatus,Status,ComplaintNumber,TotalAmount,CreateDate,FORMAT(CreateDate,'dd-MM-yyyy hh:mm tt') as SCreateDate,Id From tbl_Complaints_Manage where Id = @ComplaintId
		End

	If @Mode = 'ComplaintAgainstReport'
		Begin
			Select ca.*,em.FullName,em.EnroNumber as EnrollmentNo,em.ContactNumber
			FROM tbl_Complaint_Against as ca
			left join tbl_Enrollment_Mst as em on em.EId = ca.EnrollmentId
			Where ComplaintId = @ComplaintId
		End

	If @Mode = 'ComplaintFilesReport'
		Begin
			Select ROW_NUMBER() OVER(ORDER BY cf.Id desc) as SrNo,cf.ComplaintId,cf.FileTitle,cf.FileName,cf.FilePath,ISNULL(Description,'') as Description,
			CASE
			WHEN (select count(*) from tbl_register_mst where RegisterId = cf.CreateUser and UserName = 'superadmin' ) >  0 THEN (select isnull(Concat(FirstName,' ',LastName),'') from tbl_register_mst where RegisterId = cf.CreateUser	)    
			ELSE (Select ISNULL(FullName,'') from tbl_Enrollment_Mst where EId = cf.CreateUser)
				END as Name
			 From tbl_Complaint_FileUpload as cf
			 Where cf.ComplaintId = @ComplaintId
		End

	If @Mode = 'ComplaintLogReport'
		Begin
			Select ROW_NUMBER() OVER(ORDER BY csl.Id desc) as SrNo,csl.Status,csl.NextDate,isnull(CONVERT(VARCHAR(10),csl.NextDate,105),'') as SNextDate,isnull(csl.OrderFileName,'') as OrderFileName,
			isnull(csl.OrderFilePath,'') as OrderFilePath,isnull(csl.Summary,'')as Summary,isnull(csl.CommitteTitle,'') as CommitteTitle,isnull(csl.OldStatus,'')OldStatus,csl.CreateDate
		
			From tbl_Complaint_Status_Log as csl	
				
			Where csl.ComplaintId = @ComplaintId
			order by csl.id desc
		End
END

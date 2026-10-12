
CREATE PROCEDURE [dbo].[SP_Complaint_Status_Log]
	@Id bigint =0,
	@ComplaintId bigint=0,
	@Status nvarchar(max)= null,
	@NextDate datetime = null,
	@OrderFileName nvarchar(max)=null,
	@OrderFilePath nvarchar(max)=null,
	@Summary nvarchar(max)=null,
	@Mode nvarchar(max),
	@CreateUser bigint = 0
AS
BEGIN
	declare @oldStatus nvarchar(max)=null;
	If @Mode = 'Insert'
	Begin
		set @oldStatus = (Select top 1 OldStatus from tbl_Complaint_Status_Log where ComplaintId=@ComplaintId order by id desc)
		Insert Into tbl_Complaint_Status_Log(ComplaintId,Status,NextDate,OrderFileName,OrderFilePath,Summary,Flag,CreateDate,CreateUser,OldStatus)
		Values(@ComplaintId,@Status,@NextDate,@OrderFileName,@OrderFilePath,@Summary,'A',CONVERT(Datetime,SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30')),@CreateUser,@oldStatus)

		Update tbl_Complaints_Manage Set Status = @Status WHERE Id = @ComplaintId

		if(@Status='Payment Done')
		Begin
			Update tbl_Complaints_Manage Set PaymentStatus = 'Completed' WHERE Id = @ComplaintId 
		End
	End

	If @Mode = 'GetLogByStatus' 
		Begin
			Select top 1 * From tbl_Complaint_Status_Log Where ComplaintId = @ComplaintId and Status = @Status 
			order by CreateDate desc
		End

	If @Mode = 'LogList'
		Begin
			select csl.Id,csl.Complaintid,concat(rm.FirstName,' ',rm.MiddleName,' ',rm.LastName) AS Name,csl.Status,isnull(csl.NextDate,csl.CreateDate) as NextDate,csl.OrderFileName,
			csl.OrderFilePath,csl.Summary,csl.CommitteeId,isnull(csl.CommitteTitle,'') as CommitteTitle,csl.OldStatus
			from  tbl_Complaint_Status_Log as csl
			inner join tbl_register_mst as rm on rm.RegisterId = csl.CreateUser		
			where csl.Complaintid= @ComplaintId 
			Order by csl.Id desc --and Status = @Status
		End

	If @Mode = 'ComplaintMemberLog'
	Begin
		select cml.Id,cml.MemberId,mem.EnrollmentId,em.EnroNumber,em.FullName,em.ContactNumber,cml.ComplaintDetailEncryptCode,
               COALESCE(NULLIF(mem.EmailAddress, ''), NULLIF(em.EmailAddress, '')) as EmailAddress
		from tbl_Complaint_Member_Log  as cml
		left join tbl_Member as mem on mem.Id=cml.MemberId
		left join tbl_Enrollment_Mst as em on em.EId=mem.EnrollmentId
		where cml.ComplaintId=@ComplaintId
		order by cml.Id
	End

	If @Mode = 'LogListByMemberId'
		Begin
			select distinct csl.Id,csl.Complaintid,concat(rm.FirstName,' ',rm.MiddleName,' ',rm.LastName) AS Name,csl.Status,csl.NextDate,
		    isnull(csl.OrderFileName,'') as OrderFileName,
			isnull(csl.OrderFilePath,'')as OrderFilePath,isnull(csl.Summary,'')as Summary,csl.CommitteeId,isnull(csl.CommitteTitle,'') as CommitteTitle,isnull(csl.OldStatus,'') as OldStatus
			from  tbl_Complaint_Status_Log as csl
			inner join tbl_register_mst as rm on rm.RegisterId = csl.CreateUser		
			left join tbl_Complaint_Member_Log as cml on cml.ComplaintId =  csl.Complaintid		
			where csl.ComplaintId= @ComplaintId --and cml.MemberId = @Id
			Order by csl.Id desc
		End

	If @Mode = 'UpdateStatus'
		Begin		

			set @oldStatus = (Select top 1 Status from tbl_Complaint_Status_Log where ComplaintId=@ComplaintId order by id desc)
			
			Update tbl_Complaints_Manage Set Status = @Status where Id = @ComplaintId

			if(@Status='Payment Done')
			Begin
				Update tbl_Complaints_Manage Set PaymentStatus = 'Completed' WHERE Id = @ComplaintId 
			End

			Insert into tbl_Complaint_Status_Log (ComplaintId,Status,Flag,CreateDate,CreateUser,OldStatus)
			values(@ComplaintId,@Status,'A',CONVERT(Datetime,SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30')),1,@oldStatus)
		End	
END

USE [Barcouncil_09102026]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[SP_Online_Complaint_Status_Update]
	@OnlineComplaintId bigint = 0,
	@Remark nvarchar(max)=null,
	@CreateUser bigint = 0,
	@Mode nvarchar(max)=null
AS
BEGIN
	If @Mode = 'Accepted'
	Begin
		declare @ComplaintId bigint = 0;
		declare @LastId bigint = 0;
		declare @ComplaintNumberCode bigint = 0;
		declare @ComplaintNumber nvarchar(max) = '';

		set @LastId = Isnull((select top 1 Isnull(ComplaintNumberCode,0) from tbl_Complaints_Manage where dbo.fn_Get_Complaint_Year(ComplaintNumber)=YEAR(GETDATE()) order by Id desc),0);

		set @ComplaintNumberCode = @LastId + 1;

		set @ComplaintNumber = CONCAT( 'BCG/CMP/' , @ComplaintNumberCode,'/' , DATEPART(yyyy, GETDATE()) );

		Insert into tbl_Complaints_Manage(ComplaintNumber,  ComplaintNumberCode, DCComplaintNumber, DCComplaintNumberCode, FullName, EmailAddress, ContactNumber, Address, Summary, DetailComplaint, TotalAmount, PaidAmount, DueAmount, PaymentStatus, Status, IsShowAdvocate, Flag, CreateDate, CreateUser)
								  select @ComplaintNumber, @ComplaintNumberCode,                '',                    '', FullName, EmailAddress, ContactNumber, Address, Summary, DetailComplaint, TotalAmount, PaidAmount, DueAmount, PaymentStatus,  'New', IsShowAdvocate,  'A',  GETDATE(), @CreateUser 
								  from tbl_Online_Complaints_Manage 
								  where Id=@OnlineComplaintId
			                      
		set @ComplaintId=@@IDENTITY;

		insert into tbl_Complaint_Against (ComplaintId, EnrollmentId, AdvocateName, City, MobileNo, Amount, IsKnown, Flag, CreateDate, CreatedUser)
		                           select @ComplaintId, EnrollmentId, AdvocateName, City, MobileNo, Amount, IsKnown,  'A',  GETDATE(), @CreateUser  
								   from tbl_Online_Complaint_Against 
								   where ComplaintId=@OnlineComplaintId

		insert into tbl_Complaint_Affidavit_Copy (ComplaintId, FileName, FilePath, IsOld, FileExtension, Flag,  CreateUser, CreateDate)
		                                  select @ComplaintId, FileName, FilePath, IsOld, FileExtension,  'A', @CreateUser,  GETDATE()  
										  from tbl_Online_Complaint_Affidavit_Copy 
								          where ComplaintId=@OnlineComplaintId

		insert into tbl_Complaint_FileUpload (ComplaintId, FileTitle, FileName, FilePath, Description, FileExtension, Flag,  CreateUser, CreateDate)
		                              select @ComplaintId, FileTitle, FileName, FilePath, Description, FileExtension,  'A', @CreateUser,  GETDATE()  
									  from tbl_Online_Complaint_FileUpload 
								      where ComplaintId=@OnlineComplaintId

		update tbl_Online_Complaints_Manage set Status='Accepted' where Id=@OnlineComplaintId

		insert into tbl_Online_Complaints_Status_Log (ComplaintId, ComplaintStatus,  Remark, Flag, CreateDate, CreateUser)
									   values (@OnlineComplaintId,      'Accepted', @Remark,  'A',  GETDATE(), @CreateUser)
					
		Select 'I' AS ReturnStatus
	End

	If @Mode = 'Rejected'
	Begin
		update tbl_Online_Complaints_Manage set Status='Rejected' where Id=@OnlineComplaintId

		insert into tbl_Online_Complaints_Status_Log (ComplaintId, ComplaintStatus,  Remark, Flag, CreateDate, CreateUser)
									   values (@OnlineComplaintId,      'Rejected', @Remark,  'A',  GETDATE(), @CreateUser)
					
		Select 'I' AS ReturnStatus
	End
END

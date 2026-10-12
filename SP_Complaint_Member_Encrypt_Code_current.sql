

CREATE PROCEDURE [dbo].[SP_Complaint_Member_Encrypt_Code]
	@Id bigint =0,
	@Mode nvarchar(max)
AS
BEGIN

	If @Mode = 'ComplaintMemberLogDetail'
	Begin
		select Id, ComplaintId, Status, MemberId, IsChecked, CommitteeId, ComplaintDetailEncryptCode from tbl_Complaint_Member_Log where Id=@Id	
	End
END


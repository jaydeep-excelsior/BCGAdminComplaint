CREATE PROCEDURE [dbo].[SP_DcCommittee_Mst]
	@Id bigint = 0,
	@DcCommitteeTitle nvarchar(max) = null,
	@Note nvarchar(max) = null,
	@Mode nvarchar(50) = null,
	@ShowRecord	bigint =null,
	@SearchTitle	nvarchar(MAX)=null,
	@SNOStart bigint =null,
	@SNOEnd bigint =null,
	@RoleId	bigint	=NULL,
	@RightsId	bigint	=null,
	@Flag char(1)=null,
	@intOutput nvarchar(50)=null OUTPUT,
	@NumRowsChanged nvarchar(50)=null OUTPUT,
	@ErrorCode nvarchar(50)=null OUTPUT
AS
BEGIN
	IF @Mode = 'InsertUpdate'
	Begin
		If @Id = 0
		Begin
			declare  @LastId bigint = 0;
			declare @DcCommitteeNo bigint = 0;
			declare @tablecount bigint = 0;

			set @tablecount = (select COUNT(*) from tbl_DC_Committee_Mst);

			set @LastId = (select top 1 Isnull(DcCommitteeNo,0) from tbl_DC_Committee_Mst order by Id desc);

			If @tablecount <= 0
				set @DcCommitteeNo = 1;
			else
				set @DcCommitteeNo = @LastId + 1;

			Insert Into tbl_DC_Committee_Mst(DcCommitteeTitle, DcCommitteeNo, Note,Flag,                                                    CreateDate,CreateUser)
			                        values (@DcCommitteeTitle,@DcCommitteeNo,@Note, 'A', CONVERT(Datetime,SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30')),1)

			SET @intOutput = CONVERT(nvarchar(100),@@IDENTITY);
			SET @NumRowsChanged =  @@ROWCOUNT
			SET @ErrorCode=@@ERROR
		End
		else
		Begin
			Update tbl_DC_Committee_Mst Set DcCommitteeTitle=@DcCommitteeTitle, Note = @Note, UpdateDate = CONVERT(Datetime,SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30')),UpdateUser = 1	Where Id = @Id
			
			SET @intOutput = @Id;
			SET @NumRowsChanged =  @@ROWCOUNT
			SET @ErrorCode=@@ERROR
		End
	End

	If @Mode = 'SelectAll'
	Begin
		if(@SearchTitle='NA')
		Begin
			Select ROW_NUMBER() OVER (ORDER BY dm.Id desc )AS SNO,dm.Id,dm.DcCommitteeNo,dm.DcCommitteeTitle as Title,dm.Note,dm.Flag,dm.CreateDate,dm.UpdateDate,dm.CreateUser,dm.UpdateUser,
			(select count(*) from tbl_DC_Commitee_Details where DcCommitteeId = dm.Id) as TotalMember
			INTO #ResultsActiveAll1
			from tbl_DC_Committee_Mst as dm					
			order by dm.Id desc

			SELECT *,(case when Flag='A' then 'Active' else 'Inactive' end) as RStatus,(case when Flag='A' then 'Delete' else 'Active' end) as BRStatus,
			(case when (select count(Id) from tbl_rights_details_mst where RightsId=@RightsId and RoleId=@RoleId and StatusFlag='A' and Title='Update')=0 then 'D' else 'A' end) as UpdateStatus,
			(case when (select count(Id) from tbl_rights_details_mst where RightsId=@RightsId and RoleId=@RoleId and StatusFlag='A' and Title='Delete')=0 then 'D' else 'A' end) as DeleteStatus,
			(case when (select count(Id) from tbl_rights_details_mst where RightsId=@RightsId and RoleId=@RoleId and StatusFlag='A' and Title='ViewRights')=0 then 'D' else 'A' end) as ViewRightsStatus,
			(case when (select count(Id) from tbl_rights_details_mst where RightsId=@RightsId and RoleId=@RoleId and StatusFlag='A' and Title='ManageRights')=0 then 'D' else 'A' end) as ManageRightsStatus
			FROM #ResultsActiveAll1
			WHERE SNO>=@SNOStart and  SNO<=@SNOEnd order by SNO asc

			Select Count(dm.Id) as TotalRecord from tbl_DC_Committee_Mst as dm						

			Select Count(dm.Id)/@ShowRecord as RecPageCount from tbl_DC_Committee_Mst as dm			

			DROP TABLE #ResultsActiveAll1
		end
		else
		Begin
			Select ROW_NUMBER() OVER (ORDER BY dm.Id desc )AS SNO,dm.Id,dm.DcCommitteeNo,dm.DcCommitteeTitle as Title,dm.Note,dm.Flag,dm.CreateDate,dm.UpdateDate,dm.CreateUser,dm.UpdateUser,
			(select count(*) from tbl_DC_Commitee_Details where DcCommitteeId = dm.Id) as TotalMember
			INTO #ResultsActiveAll2
			from tbl_DC_Committee_Mst as dm					
			where (dm.DcCommitteeTitle like '%'+@SearchTitle+'%')					 
			order by dm.Id desc

			SELECT *,(case when Flag='A' then 'Active' else 'Inactive' end) as RStatus,(case when Flag='A' then 'Delete' else 'Active' end) as BRStatus,
			(case when (select count(Id) from tbl_rights_details_mst where RightsId=@RightsId and RoleId=@RoleId and StatusFlag='A' and Title='Update')=0 then 'D' else 'A' end) as UpdateStatus,
			(case when (select count(Id) from tbl_rights_details_mst where RightsId=@RightsId and RoleId=@RoleId and StatusFlag='A' and Title='Delete')=0 then 'D' else 'A' end) as DeleteStatus,
			(case when (select count(Id) from tbl_rights_details_mst where RightsId=@RightsId and RoleId=@RoleId and StatusFlag='A' and Title='ViewRights')=0 then 'D' else 'A' end) as ViewRightsStatus,
			(case when (select count(Id) from tbl_rights_details_mst where RightsId=@RightsId and RoleId=@RoleId and StatusFlag='A' and Title='ManageRights')=0 then 'D' else 'A' end) as ManageRightsStatus
			FROM #ResultsActiveAll2
			WHERE SNO>=@SNOStart and  SNO<=@SNOEnd order by SNO asc

			Select Count(dm.Id) as TotalRecord from tbl_DC_Committee_Mst as dm where (dm.DcCommitteeTitle like '%'+@SearchTitle+'%')					 

			Select Count(dm.Id)/@ShowRecord as RecPageCount from tbl_DC_Committee_Mst as dm where (dm.DcCommitteeTitle like '%'+@SearchTitle+'%')			 

			DROP TABLE #ResultsActiveAll2
		end
	end

	If @Mode = 'ActiveSelectAll'
	begin
		if(@Flag='A')
		begin
			if(@SearchTitle='NA')
			begin
				Select ROW_NUMBER() OVER (ORDER BY dm.Id desc )AS SNO,dm.Id,dm.DcCommitteeNo,dm.DcCommitteeTitle as Title,dm.Note,dm.Flag,dm.CreateDate,dm.UpdateDate,dm.CreateUser,dm.UpdateUser,
				(select count(*) from tbl_DC_Commitee_Details where DcCommitteeId = dm.Id) as TotalMember
				INTO #ResultsActiveA1
				from tbl_DC_Committee_Mst as dm	where dm.Flag = 'A'				
				order by dm.Id desc

				SELECT *,(case when Flag='A' then 'Active' else 'Inactive' end) as RStatus,(case when Flag='A' then 'Delete' else 'Active' end) as BRStatus,
				(case when (select count(Id) from tbl_rights_details_mst where RightsId=@RightsId and RoleId=@RoleId and StatusFlag='A' and Title='Update')=0 then 'D' else 'A' end) as UpdateStatus,
				(case when (select count(Id) from tbl_rights_details_mst where RightsId=@RightsId and RoleId=@RoleId and StatusFlag='A' and Title='Delete')=0 then 'D' else 'A' end) as DeleteStatus,
				(case when (select count(Id) from tbl_rights_details_mst where RightsId=@RightsId and RoleId=@RoleId and StatusFlag='A' and Title='ViewRights')=0 then 'D' else 'A' end) as ViewRightsStatus,
				(case when (select count(Id) from tbl_rights_details_mst where RightsId=@RightsId and RoleId=@RoleId and StatusFlag='A' and Title='ManageRights')=0 then 'D' else 'A' end) as ManageRightsStatus
				FROM #ResultsActiveA1
				WHERE SNO>=@SNOStart and  SNO<=@SNOEnd order by SNO asc

				Select Count(dm.Id) as TotalRecord from tbl_DC_Committee_Mst as dm where dm.Flag = 'A'					

				Select Count(dm.Id)/@ShowRecord as RecPageCount from tbl_DC_Committee_Mst as dm	where dm.Flag = 'A'				

				DROP TABLE #ResultsActiveA1
			end
			else
			Begin
				Select ROW_NUMBER() OVER (ORDER BY dm.Id desc )AS SNO,dm.Id,dm.DcCommitteeNo,dm.DcCommitteeTitle as Title,dm.Note,dm.Flag,dm.CreateDate,dm.UpdateDate,dm.CreateUser,dm.UpdateUser,
				(select count(*) from tbl_DC_Commitee_Details where DcCommitteeId = dm.Id) as TotalMember
				INTO #ResultsActiveA2
				from tbl_DC_Committee_Mst as dm					
				where dm.Flag= 'A' and (dm.DcCommitteeTitle like '%'+@SearchTitle+'%')					 
				order by dm.Id desc

				SELECT *,(case when Flag='A' then 'Active' else 'Inactive' end) as RStatus,(case when Flag='A' then 'Delete' else 'Active' end) as BRStatus,
				(case when (select count(Id) from tbl_rights_details_mst where RightsId=@RightsId and RoleId=@RoleId and StatusFlag='A' and Title='Update')=0 then 'D' else 'A' end) as UpdateStatus,
				(case when (select count(Id) from tbl_rights_details_mst where RightsId=@RightsId and RoleId=@RoleId and StatusFlag='A' and Title='Delete')=0 then 'D' else 'A' end) as DeleteStatus,
				(case when (select count(Id) from tbl_rights_details_mst where RightsId=@RightsId and RoleId=@RoleId and StatusFlag='A' and Title='ViewRights')=0 then 'D' else 'A' end) as ViewRightsStatus,
				(case when (select count(Id) from tbl_rights_details_mst where RightsId=@RightsId and RoleId=@RoleId and StatusFlag='A' and Title='ManageRights')=0 then 'D' else 'A' end) as ManageRightsStatus
				FROM #ResultsActiveA2
				WHERE SNO>=@SNOStart and  SNO<=@SNOEnd order by SNO asc

				Select Count(dm.Id) as TotalRecord from tbl_DC_Committee_Mst as dm where dm.Flag= 'A' and (dm.DcCommitteeTitle like '%'+@SearchTitle+'%')					 

				Select Count(dm.Id)/@ShowRecord as RecPageCount from tbl_DC_Committee_Mst as dm where dm.Flag= 'A' and (dm.DcCommitteeTitle like '%'+@SearchTitle+'%')					 

				DROP TABLE #ResultsActiveA2
			end
		end
	end

	If @Mode = 'DeactiveSelectAll'
	begin
		if(@Flag='D')
		begin
			if(@SearchTitle='NA')
			begin
				Select ROW_NUMBER() OVER (ORDER BY dm.Id desc )AS SNO,dm.Id,dm.DcCommitteeNo,dm.DcCommitteeTitle as Title,dm.Note,dm.Flag,dm.CreateDate,dm.UpdateDate,dm.CreateUser,dm.UpdateUser,
				(select count(*) from tbl_DC_Commitee_Details where DcCommitteeId = dm.Id) as TotalMember
				INTO #ResultsActiveD1
				from tbl_DC_Committee_Mst as dm	where dm.Flag = 'D'				
				order by dm.Id desc

				SELECT *,(case when Flag='A' then 'Active' else 'Inactive' end) as RStatus,(case when Flag='A' then 'Delete' else 'Active' end) as BRStatus,
				(case when (select count(Id) from tbl_rights_details_mst where RightsId=@RightsId and RoleId=@RoleId and StatusFlag='A' and Title='Update')=0 then 'D' else 'A' end) as UpdateStatus,
				(case when (select count(Id) from tbl_rights_details_mst where RightsId=@RightsId and RoleId=@RoleId and StatusFlag='A' and Title='Delete')=0 then 'D' else 'A' end) as DeleteStatus,
				(case when (select count(Id) from tbl_rights_details_mst where RightsId=@RightsId and RoleId=@RoleId and StatusFlag='A' and Title='ViewRights')=0 then 'D' else 'A' end) as ViewRightsStatus,
				(case when (select count(Id) from tbl_rights_details_mst where RightsId=@RightsId and RoleId=@RoleId and StatusFlag='A' and Title='ManageRights')=0 then 'D' else 'A' end) as ManageRightsStatus
				FROM #ResultsActiveD1
				WHERE SNO>=@SNOStart and  SNO<=@SNOEnd order by SNO asc

				Select Count(dm.Id) as TotalRecord from tbl_DC_Committee_Mst as dm	where dm.Flag = 'D'						

				Select Count(dm.Id)/@ShowRecord as RecPageCount from tbl_DC_Committee_Mst as dm	where dm.Flag = 'D'			

				DROP TABLE #ResultsActiveD1
			end
			else
			Begin
				Select ROW_NUMBER() OVER (ORDER BY dm.Id desc )AS SNO,dm.Id,dm.DcCommitteeNo,dm.DcCommitteeTitle as Title,dm.Note,dm.Flag,dm.CreateDate,dm.UpdateDate,dm.CreateUser,dm.UpdateUser,
				(select count(*) from tbl_DC_Commitee_Details where DcCommitteeId = dm.Id) as TotalMember
				INTO #ResultsActiveD2
				from tbl_DC_Committee_Mst as dm					
				where dm.Flag= 'D' and (dm.DcCommitteeTitle like '%'+@SearchTitle+'%')					 
				order by dm.Id desc

				SELECT *,(case when Flag='A' then 'Active' else 'Inactive' end) as RStatus,(case when Flag='A' then 'Delete' else 'Active' end) as BRStatus,
				(case when (select count(Id) from tbl_rights_details_mst where RightsId=@RightsId and RoleId=@RoleId and StatusFlag='A' and Title='Update')=0 then 'D' else 'A' end) as UpdateStatus,
				(case when (select count(Id) from tbl_rights_details_mst where RightsId=@RightsId and RoleId=@RoleId and StatusFlag='A' and Title='Delete')=0 then 'D' else 'A' end) as DeleteStatus,
				(case when (select count(Id) from tbl_rights_details_mst where RightsId=@RightsId and RoleId=@RoleId and StatusFlag='A' and Title='ViewRights')=0 then 'D' else 'A' end) as ViewRightsStatus,
				(case when (select count(Id) from tbl_rights_details_mst where RightsId=@RightsId and RoleId=@RoleId and StatusFlag='A' and Title='ManageRights')=0 then 'D' else 'A' end) as ManageRightsStatus
				FROM #ResultsActiveD2
				WHERE SNO>=@SNOStart and  SNO<=@SNOEnd order by SNO asc

				Select Count(dm.Id) as TotalRecord from tbl_DC_Committee_Mst as dm where dm.Flag= 'D' and (dm.DcCommitteeTitle like '%'+@SearchTitle+'%')					 

				Select Count(dm.Id)/@ShowRecord as RecPageCount from tbl_DC_Committee_Mst as dm where dm.Flag= 'D' and (dm.DcCommitteeTitle like '%'+@SearchTitle+'%')					 

				DROP TABLE #ResultsActiveD2
			end
		end
	end

	If @Mode = 'GetDetails'
	Begin
		Select cd.memberid, em.Fullname, em.EnroNumber, em.ContactNumber, em.EmailAddress from tbl_DC_Commitee_Details as cd
		left join tbl_Member as mem on mem.Id = cd.MemberId
		left join tbl_Enrollment_Mst as em on em.EId = mem.EnrollmentId
		where cd.DcCommitteeId = @Id
		order by convert(bigint,isnull(dbo.fn_Get_Advocate_Enrollment_Year(em.EnroNumber),'0')),convert(bigint,isnull(dbo.fn_Get_Advocate_Enrollment_No(em.EnroNumber),'0'))
	End

	If @Mode = 'Delete'
	begin
		update tbl_DC_Committee_Mst set UpdateUser=1,Flag=@Flag,UpdateDate=CONVERT(Datetime,SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30')) Where Id=@Id
		SET @intOutput = 'D';
		SET @NumRowsChanged =  @@ROWCOUNT
		SET @ErrorCode=@@ERROR 
	end

	If @Mode = 'SelectById'
	Begin
		Select cd.MemberId as MemberId,em.FullName as FullName,em.EId as EId
		from tbl_DC_Commitee_Details as cd 
		inner join tbl_Member as mem on mem.Id = cd.MemberId
		left join tbl_Enrollment_Mst as em on em.EId = mem.EnrollmentId
		where cd.DcCommitteeId = @Id
	End
END
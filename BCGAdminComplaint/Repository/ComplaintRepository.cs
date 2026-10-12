using BCGAdminComplaint.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCGAdminComplaint.Repository
{
    public class ComplaintRepository
    {
        public List<ComplaintListModel> GetComplaintList(BaseFilter baseFilter, String Status, String PaymentStatus, String FromDate, String ToDate, String Year, String FromNo, String ToNo, String ActionMode)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Get_ComplaintList");

            unit.AddParameter("Status", SqlDbType.NVarChar, ParameterDirection.Input, string.IsNullOrEmpty(Status) ? string.Empty : Status);
            unit.AddParameter("PaymentStatus", SqlDbType.NVarChar, ParameterDirection.Input, string.IsNullOrEmpty(PaymentStatus) ? string.Empty : PaymentStatus);
            unit.AddParameter("FromDate", SqlDbType.NVarChar, ParameterDirection.Input, string.IsNullOrEmpty(FromDate) ? string.Empty : FromDate);
            unit.AddParameter("ToDate", SqlDbType.NVarChar, ParameterDirection.Input, string.IsNullOrEmpty(ToDate) ? string.Empty : ToDate);
            unit.AddParameter("Year", SqlDbType.NVarChar, ParameterDirection.Input, string.IsNullOrEmpty(Year) ? string.Empty : Year);
            unit.AddParameter("FromNo", SqlDbType.NVarChar, ParameterDirection.Input, string.IsNullOrEmpty(FromNo) ? string.Empty : FromNo);
            unit.AddParameter("ToNo", SqlDbType.NVarChar, ParameterDirection.Input, string.IsNullOrEmpty(ToNo) ? string.Empty : ToNo);
            unit.AddParameter("SearchValue", SqlDbType.VarChar, ParameterDirection.Input, string.IsNullOrEmpty(baseFilter.searchValue) ? string.Empty : baseFilter.searchValue);
            unit.AddParameter("PageNo", SqlDbType.Int, ParameterDirection.Input, baseFilter.pageNo);
            unit.AddParameter("PageSize", SqlDbType.Int, ParameterDirection.Input, baseFilter.pageSize);
            unit.AddParameter("SortColumn", SqlDbType.Int, ParameterDirection.Input, baseFilter.sortColumn);
            unit.AddParameter("SortOrder", SqlDbType.VarChar, ParameterDirection.Input, baseFilter.sortOrder);
            unit.AddParameter("Mode", SqlDbType.VarChar, ParameterDirection.Input, ActionMode);
            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<ComplaintListModel> List = new List<ComplaintListModel>();
                while (reader.Read())
                {
                    ComplaintListModel list = new ComplaintListModel();
                    list.SrNo = DatabaseAccessEngine.GetNullableInt64(reader, "SrNo");
                    list.Id = DatabaseAccessEngine.GetInt64(reader, "ID");
                    list.TotalRecord = DatabaseAccessEngine.GetNullableInt(reader, "TotalRecord");
                    list.FullName = DatabaseAccessEngine.GetString(reader, "FullName");
                    list.EmailAddress = DatabaseAccessEngine.GetString(reader, "EmailAddress");
                    list.ContactNumber = DatabaseAccessEngine.GetString(reader, "ContactNumber");
                    list.Summary = DatabaseAccessEngine.GetString(reader, "Summary");
                    list.DetailComplaint = DatabaseAccessEngine.GetString(reader, "DetailComplaint");
                    list.Status = DatabaseAccessEngine.GetString(reader, "Status");
                    list.PaymentStatus = DatabaseAccessEngine.GetString(reader, "PaymentStatus");
                    list.ComplaintNumber = DatabaseAccessEngine.GetString(reader, "ComplaintNumber");
                    list.TotalAmount = DatabaseAccessEngine.GetDecimal(reader, "TotalAmount");
                    list.PaidAmount = DatabaseAccessEngine.GetDecimal(reader, "PaidAmount");
                    list.DueAmount = DatabaseAccessEngine.GetDecimal(reader, "DueAmount");
                    list.ComplaintsCreateDate = DatabaseAccessEngine.GetString(reader, "ComplaintsCreateDate");
                    list.ComplainedAgainst = DatabaseAccessEngine.GetString(reader, "ComplainedAgainst");
                    list.ComplaintAffidavitCopyList = DatabaseAccessEngine.GetString(reader, "ComplaintAffidavitCopyList");
                    list.ComplaintAdditionalAttachmentsList = DatabaseAccessEngine.GetString(reader, "ComplaintAdditionalAttachmentsList");
                    list.ComplaintPaymentList = DatabaseAccessEngine.GetString(reader, "ComplaintPaymentList");
                    list.AffidavitFilesText = DatabaseAccessEngine.GetString(reader, "AffidavitFilesText");
                    list.AdditionalFilesText = DatabaseAccessEngine.GetString(reader, "AdditionalFilesText");
                    list.PaymentInfoText = DatabaseAccessEngine.GetString(reader, "PaymentInfoText");

                    if (ActionMode == "SelectAll")
                    {
                        list.NewStatusCount = DatabaseAccessEngine.GetInt(reader, "NewStatusCount");
                        list.OldStatusCount = DatabaseAccessEngine.GetInt(reader, "OldStatusCount");
                    }

                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public List<ComplaintListModel> GetOnlineComplaintList(BaseFilter baseFilter, String Status, String PaymentStatus, String FromDate, String ToDate, String Year, String FromNo, String ToNo,Int64 Id,Int64 ComplaintIds, String ActionMode)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Get_Online_Complaint_List");
            unit.AddParameter("SearchValue", SqlDbType.VarChar, ParameterDirection.Input, string.IsNullOrEmpty(baseFilter.searchValue) ? string.Empty : baseFilter.searchValue);
            unit.AddParameter("Status", SqlDbType.NVarChar, ParameterDirection.Input, string.IsNullOrEmpty(Status) ? string.Empty : Status);
            unit.AddParameter("FromDate", SqlDbType.NVarChar, ParameterDirection.Input, string.IsNullOrEmpty(FromDate) ? string.Empty : FromDate);
            unit.AddParameter("ToDate", SqlDbType.NVarChar, ParameterDirection.Input, string.IsNullOrEmpty(ToDate) ? string.Empty : ToDate);
            unit.AddParameter("Year", SqlDbType.NVarChar, ParameterDirection.Input, string.IsNullOrEmpty(Year) ? string.Empty : Year);
            unit.AddParameter("FromNo", SqlDbType.NVarChar, ParameterDirection.Input, string.IsNullOrEmpty(FromNo) ? string.Empty : FromNo);
            unit.AddParameter("ToNo", SqlDbType.NVarChar, ParameterDirection.Input, string.IsNullOrEmpty(ToNo) ? string.Empty : ToNo);
            
            unit.AddParameter("PageNo", SqlDbType.Int, ParameterDirection.Input, baseFilter.pageNo);
            unit.AddParameter("PageSize", SqlDbType.Int, ParameterDirection.Input, baseFilter.pageSize);
            unit.AddParameter("SortColumn", SqlDbType.Int, ParameterDirection.Input, baseFilter.sortColumn);
            unit.AddParameter("SortOrder", SqlDbType.VarChar, ParameterDirection.Input, baseFilter.sortOrder);
            unit.AddParameter("Id", SqlDbType.Int, ParameterDirection.Input, Id);
            unit.AddParameter("ComplaintIds", SqlDbType.Int, ParameterDirection.Input, ComplaintIds);
            unit.AddParameter("Mode", SqlDbType.VarChar, ParameterDirection.Input, ActionMode);
            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<ComplaintListModel> List = new List<ComplaintListModel>();
                while (reader.Read())
                {
                    ComplaintListModel list = new ComplaintListModel();
                    list.SrNo = DatabaseAccessEngine.GetNullableInt64(reader, "SrNo");
                    list.Id = DatabaseAccessEngine.GetInt64(reader, "ID");
                    list.TotalRecord = DatabaseAccessEngine.GetNullableInt(reader, "TotalRecord");
                    list.FullName = DatabaseAccessEngine.GetString(reader, "FullName");
                    list.EmailAddress = DatabaseAccessEngine.GetString(reader, "EmailAddress");
                    list.ContactNumber = DatabaseAccessEngine.GetString(reader, "ContactNumber");
                    list.Summary = DatabaseAccessEngine.GetString(reader, "Summary");
                    list.DetailComplaint = DatabaseAccessEngine.GetString(reader, "DetailComplaint");
                    list.Status = DatabaseAccessEngine.GetString(reader, "Status");
                    list.PaymentStatus = DatabaseAccessEngine.GetString(reader, "PaymentStatus");
                    list.ComplaintNumber = DatabaseAccessEngine.GetString(reader, "ComplaintNumber");
                    list.TotalAmount = DatabaseAccessEngine.GetDecimal(reader, "TotalAmount");
                    list.PaidAmount = DatabaseAccessEngine.GetDecimal(reader, "PaidAmount");
                    list.DueAmount = DatabaseAccessEngine.GetDecimal(reader, "DueAmount");
                    list.ComplaintsCreateDate = DatabaseAccessEngine.GetString(reader, "ComplaintsCreateDate");
                    list.ComplainedAgainst = DatabaseAccessEngine.GetString(reader, "ComplainedAgainst");
                    list.ComplaintAffidavitCopyList = DatabaseAccessEngine.GetString(reader, "ComplaintAffidavitCopyList");
                    list.ComplaintAdditionalAttachmentsList = DatabaseAccessEngine.GetString(reader, "ComplaintAdditionalAttachmentsList");
                    list.ComplaintPaymentList = DatabaseAccessEngine.GetString(reader, "ComplaintPaymentList");
                    list.AffidavitFilesText = DatabaseAccessEngine.GetString(reader, "AffidavitFilesText");
                    list.AdditionalFilesText = DatabaseAccessEngine.GetString(reader, "AdditionalFilesText");
                    list.PaymentInfoText = DatabaseAccessEngine.GetString(reader, "PaymentInfoText");

                    if (ActionMode == "SelectAll")
                    {
                        list.NewStatusCount = DatabaseAccessEngine.GetInt(reader, "NewStatusCount");
                        list.OldStatusCount = DatabaseAccessEngine.GetInt(reader, "OldStatusCount");
                    }

                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public List<ComplaintDocumentList> GetDocumentList(Int64 ComplaintId)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Get_ComplaintList");

            unit.AddParameter("Id", SqlDbType.BigInt, ParameterDirection.Input, ComplaintId);
            unit.AddParameter("Mode", SqlDbType.VarChar, ParameterDirection.Input, "GetDocument");

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<ComplaintDocumentList> List = new List<ComplaintDocumentList>();
                while (reader.Read())
                {
                    ComplaintDocumentList list = new ComplaintDocumentList();
                    list.Id = DatabaseAccessEngine.GetInt64(reader, "Id");
                    list.SrNo = DatabaseAccessEngine.GetNullableInt64(reader, "SrNo");
                    list.ComplaintId = DatabaseAccessEngine.GetInt64(reader, "ComplaintId");
                    list.FileTitle = DatabaseAccessEngine.GetString(reader, "FileTitle");
                    list.FilePath = DatabaseAccessEngine.GetString(reader, "FilePath");
                    list.FileName = DatabaseAccessEngine.GetString(reader, "FileName");
                    list.Description = DatabaseAccessEngine.GetString(reader, "Description");
                    list.DocumetType = DatabaseAccessEngine.GetString(reader, "DocumetType");
                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public List<ComplaintDocumentList> GetOnlineDocumentList(Int64 ComplaintId)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Get_Online_Complaint_List");

            unit.AddParameter("Id", SqlDbType.BigInt, ParameterDirection.Input, ComplaintId);
            unit.AddParameter("Mode", SqlDbType.VarChar, ParameterDirection.Input, "GetDocument");

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<ComplaintDocumentList> List = new List<ComplaintDocumentList>();
                while (reader.Read())
                {
                    ComplaintDocumentList list = new ComplaintDocumentList();
                    list.Id = DatabaseAccessEngine.GetInt64(reader, "Id");
                    list.SrNo = DatabaseAccessEngine.GetNullableInt64(reader, "SrNo");
                    list.ComplaintId = DatabaseAccessEngine.GetInt64(reader, "ComplaintId");
                    list.FileTitle = DatabaseAccessEngine.GetString(reader, "FileTitle");
                    list.FilePath = DatabaseAccessEngine.GetString(reader, "FilePath");
                    list.FileName = DatabaseAccessEngine.GetString(reader, "FileName");
                    list.Description = DatabaseAccessEngine.GetString(reader, "Description");
                    list.DocumetType = DatabaseAccessEngine.GetString(reader, "DocumetType");
                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public List<ComplaintAgainstList> GetAdjuntList(Int64 ComplaintId)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Complaint_Against");

            unit.AddParameter("ComplaintId", SqlDbType.BigInt, ParameterDirection.Input, ComplaintId);
            unit.AddParameter("Mode", SqlDbType.VarChar, ParameterDirection.Input, "GetDetailsById");

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<ComplaintAgainstList> List = new List<ComplaintAgainstList>();
                while (reader.Read())
                {
                    ComplaintAgainstList list = new ComplaintAgainstList();
                    list.Id = DatabaseAccessEngine.GetInt64(reader, "Id");
                    list.ComplaintId = DatabaseAccessEngine.GetInt64(reader, "ComplaintId");
                    list.AdvocateId = DatabaseAccessEngine.GetInt64(reader, "EnrollmentId");
                    list.AdvocateName = DatabaseAccessEngine.GetString(reader, "AdvocateName");
                    list.City = DatabaseAccessEngine.GetString(reader, "City");
                    list.MobileNo = DatabaseAccessEngine.GetString(reader, "MobileNo");
                    list.Amount = DatabaseAccessEngine.GetDecimal(reader, "Amount");
                    list.IsKnown = DatabaseAccessEngine.GetString(reader, "IsKnown");
                    list.EnrollmentNo = DatabaseAccessEngine.GetString(reader, "EnrollmentNo");
                    list.FullName = DatabaseAccessEngine.GetString(reader, "FullName");
                    list.ContactNumber = DatabaseAccessEngine.GetString(reader, "ContactNumber");

                    list.Name = DatabaseAccessEngine.GetString(reader, "Name");
                    list.FinalContactNumber = DatabaseAccessEngine.GetString(reader, "FinalContactNumber");

                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public List<ComplaintAgainstList> GetOnlineAdjuntList(Int64 ComplaintId)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Online_Complaint_Against");

            unit.AddParameter("ComplaintId", SqlDbType.BigInt, ParameterDirection.Input, ComplaintId);
            unit.AddParameter("Mode", SqlDbType.VarChar, ParameterDirection.Input, "GetDetailsById");

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<ComplaintAgainstList> List = new List<ComplaintAgainstList>();
                while (reader.Read())
                {
                    ComplaintAgainstList list = new ComplaintAgainstList();
                    list.Id = DatabaseAccessEngine.GetInt64(reader, "Id");
                    list.ComplaintId = DatabaseAccessEngine.GetInt64(reader, "ComplaintId");
                    list.AdvocateId = DatabaseAccessEngine.GetInt64(reader, "EnrollmentId");
                    list.AdvocateName = DatabaseAccessEngine.GetString(reader, "AdvocateName");
                    list.City = DatabaseAccessEngine.GetString(reader, "City");
                    list.MobileNo = DatabaseAccessEngine.GetString(reader, "MobileNo");
                    list.Amount = DatabaseAccessEngine.GetDecimal(reader, "Amount");
                    list.IsKnown = DatabaseAccessEngine.GetString(reader, "IsKnown");
                    list.EnrollmentNo = DatabaseAccessEngine.GetString(reader, "EnrollmentNo");
                    list.FullName = DatabaseAccessEngine.GetString(reader, "FullName");
                    list.ContactNumber = DatabaseAccessEngine.GetString(reader, "ContactNumber");

                    list.Name = DatabaseAccessEngine.GetString(reader, "Name");
                    list.FinalContactNumber = DatabaseAccessEngine.GetString(reader, "FinalContactNumber");

                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public List<ComplaintCurrentStatus> GetComplaintCurrentStatus(Int64 ComplaintId)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Complaint_Current_Status");

            unit.AddParameter("ComplaintId", SqlDbType.BigInt, ParameterDirection.Input, ComplaintId);
            unit.AddParameter("Mode", SqlDbType.VarChar, ParameterDirection.Input, "ComplaintCurrentStatus");

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<ComplaintCurrentStatus> List = new List<ComplaintCurrentStatus>();
                while (reader.Read())
                {
                    ComplaintCurrentStatus list = new ComplaintCurrentStatus();
                    list.Id = DatabaseAccessEngine.GetInt64(reader, "Id");
                    list.Status = DatabaseAccessEngine.GetString(reader, "Status");
                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public List<ComplaintListModel> GetComplaintDetails(Int64 ComplaintId)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Get_ComplaintList");

            unit.AddParameter("Id", SqlDbType.BigInt, ParameterDirection.Input, ComplaintId);
            unit.AddParameter("Mode", SqlDbType.VarChar, ParameterDirection.Input, "GetComplaintDetails");

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<ComplaintListModel> List = new List<ComplaintListModel>();
                while (reader.Read())
                {
                    ComplaintListModel list = new ComplaintListModel();
                    list.Id = DatabaseAccessEngine.GetInt64(reader, "ID");
                    list.FullName = DatabaseAccessEngine.GetString(reader, "FullName");
                    list.EmailAddress = DatabaseAccessEngine.GetString(reader, "EmailAddress");
                    list.ContactNumber = DatabaseAccessEngine.GetString(reader, "ContactNumber");
                    list.Summary = DatabaseAccessEngine.GetString(reader, "Summary");
                    list.DetailComplaint = DatabaseAccessEngine.GetString(reader, "DetailComplaint");
                    list.Status = DatabaseAccessEngine.GetString(reader, "Status");
                    list.PaymentStatus = DatabaseAccessEngine.GetString(reader, "PaymentStatus");
                    list.ComplaintNumber = DatabaseAccessEngine.GetString(reader, "ComplaintNumber");
                    list.SCreateDate = DatabaseAccessEngine.GetString(reader, "SCreateDate");
                    list.TotalAmount = DatabaseAccessEngine.GetDecimal(reader, "TotalAmount");
                    list.Address = DatabaseAccessEngine.GetString(reader, "Address");
                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public List<ComplaintMemberLog> GetDcMemberList(Int64 ComplaintId)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Complaint_Member_Log");

            unit.AddParameter("Id", SqlDbType.BigInt, ParameterDirection.Input, 0);
            unit.AddParameter("ComplaintId", SqlDbType.BigInt, ParameterDirection.Input, ComplaintId);
            unit.AddParameter("Status", SqlDbType.VarChar, ParameterDirection.Input, "");
            unit.AddParameter("MemberId ", SqlDbType.BigInt, ParameterDirection.Input, 0);
            unit.AddParameter("IsChecked ", SqlDbType.VarChar, ParameterDirection.Input, "");
            unit.AddParameter("CommitteeId ", SqlDbType.BigInt, ParameterDirection.Input, 0);
            unit.AddParameter("DCReferenceNumber ", SqlDbType.VarChar, ParameterDirection.Input, "");
            unit.AddParameter("ComplaintDetailEncryptCode ", SqlDbType.VarChar, ParameterDirection.Input, "");
            unit.AddParameter("Mode", SqlDbType.VarChar, ParameterDirection.Input, "GetByComplaintId");

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<ComplaintMemberLog> List = new List<ComplaintMemberLog>();
                while (reader.Read())
                {
                    ComplaintMemberLog list = new ComplaintMemberLog();
                    list.SrNo = DatabaseAccessEngine.GetNullableInt64(reader, "SrNo");
                    list.ComplaintId = DatabaseAccessEngine.GetInt64(reader, "ComplaintId");
                    list.MemberId = DatabaseAccessEngine.GetInt64(reader, "MemberId");
                    list.FullName = DatabaseAccessEngine.GetString(reader, "FullName");
                    list.EnroNumber = DatabaseAccessEngine.GetString(reader, "EnroNumber");
                    list.ContactNumber = DatabaseAccessEngine.GetString(reader, "ContactNumber");
                    list.IsChecked = DatabaseAccessEngine.GetString(reader, "IsChecked");
                    list.CommitteeId = DatabaseAccessEngine.GetInt64(reader, "CommitteeId");
                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public List<ComplaintListModel> GetMemberComplaintList(BaseFilter baseFilter, Int64 MemberId)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Get_ComplaintList");

            unit.AddParameter("SearchValue", SqlDbType.VarChar, ParameterDirection.Input, string.IsNullOrEmpty(baseFilter.searchValue) ? string.Empty : baseFilter.searchValue);
            unit.AddParameter("PageSize", SqlDbType.Int, ParameterDirection.Input, baseFilter.pageSize);
            unit.AddParameter("PageNo", SqlDbType.Int, ParameterDirection.Input, baseFilter.pageNo);
            unit.AddParameter("SortColumn", SqlDbType.Int, ParameterDirection.Input, baseFilter.sortColumn);
            unit.AddParameter("SortOrder", SqlDbType.VarChar, ParameterDirection.Input, baseFilter.sortOrder);
            unit.AddParameter("Id", SqlDbType.BigInt, ParameterDirection.Input, MemberId);

            unit.AddParameter("Mode", SqlDbType.VarChar, ParameterDirection.Input, "MemberListById");
            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<ComplaintListModel> List = new List<ComplaintListModel>();
                while (reader.Read())
                {
                    ComplaintListModel list = new ComplaintListModel();
                    list.SrNo = DatabaseAccessEngine.GetNullableInt64(reader, "SrNo");
                    list.Id = DatabaseAccessEngine.GetInt64(reader, "ID");
                    list.TotalRecord = DatabaseAccessEngine.GetNullableInt(reader, "TotalRecord");
                    list.FullName = DatabaseAccessEngine.GetString(reader, "FullName");
                    list.EmailAddress = DatabaseAccessEngine.GetString(reader, "EmailAddress");
                    list.ContactNumber = DatabaseAccessEngine.GetString(reader, "ContactNumber");
                    list.Summary = DatabaseAccessEngine.GetString(reader, "Summary");
                    list.DetailComplaint = DatabaseAccessEngine.GetString(reader, "DetailComplaint");
                    list.Status = DatabaseAccessEngine.GetString(reader, "Status");
                    list.PaymentStatus = DatabaseAccessEngine.GetString(reader, "PaymentStatus");
                    list.ComplaintNumber = DatabaseAccessEngine.GetString(reader, "ComplaintNumber");
                    list.TotalAmount = DatabaseAccessEngine.GetDecimal(reader, "TotalAmount");
                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public dynamic InsertMemberNote(Int64 MemberId, String NoteType, String Note, Int64 CreateUser, Int64 ComplaintId)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_DcMember_Notes");
            unit.AddParameter("Id", SqlDbType.BigInt, ParameterDirection.Input, 0);
            unit.AddParameter("DcComplaintId", SqlDbType.BigInt, ParameterDirection.Input, ComplaintId);
            unit.AddParameter("MemberId", SqlDbType.BigInt, ParameterDirection.Input, MemberId);
            unit.AddParameter("NoteType", SqlDbType.NVarChar, ParameterDirection.Input, NoteType);
            unit.AddParameter("Note", SqlDbType.NVarChar, ParameterDirection.Input, Note);
            unit.AddParameter("CreateUser", SqlDbType.BigInt, ParameterDirection.Input, CreateUser);
            unit.AddParameter("Mode", SqlDbType.NVarChar, ParameterDirection.Input, "Insert");

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<ComplaintUserLoginReturn> List = new List<ComplaintUserLoginReturn>();
                while (reader.Read())
                {
                    ComplaintUserLoginReturn list = new ComplaintUserLoginReturn();
                    list.ReturnStatus = DatabaseAccessEngine.GetString(reader, "ReturnStatus");
                    //list.RegisterId = DatabaseAccessEngine.GetString(reader, "RegisterId");
                    //list.UserName = DatabaseAccessEngine.GetString(reader, "UserName");
                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public List<MemberNoteList> GetMemberNoteList(Int64 MemberId, Int64 ComplaintId)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_DcMember_Notes");

            unit.AddParameter("MemberId", SqlDbType.BigInt, ParameterDirection.Input, MemberId);
            unit.AddParameter("DcComplaintId", SqlDbType.BigInt, ParameterDirection.Input, ComplaintId);
            unit.AddParameter("Mode", SqlDbType.VarChar, ParameterDirection.Input, "GetListByMemberId");

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<MemberNoteList> List = new List<MemberNoteList>();
                while (reader.Read())
                {
                    MemberNoteList list = new MemberNoteList();
                    list.SrNo = DatabaseAccessEngine.GetNullableInt64(reader, "SrNo");
                    list.Id = DatabaseAccessEngine.GetInt64(reader, "Id");
                    list.MemberId = DatabaseAccessEngine.GetInt64(reader, "MemberId");
                    list.MemberName = DatabaseAccessEngine.GetString(reader, "MemberName");
                    list.NoteType = DatabaseAccessEngine.GetString(reader, "NoteType");
                    list.Note = DatabaseAccessEngine.GetString(reader, "Note");
                    list.CreateDate = DatabaseAccessEngine.GetDateTime(reader, "CreateDate");
                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public List<ComplaintPaymentDetail> GetComplaintPaymentDetail(Int64 ComplaintId)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Complaint_Payment_Detail");

            unit.AddParameter("ComplaintId", SqlDbType.BigInt, ParameterDirection.Input, ComplaintId);
            unit.AddParameter("Mode", SqlDbType.VarChar, ParameterDirection.Input, "ComplaintPaymentDetail");

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<ComplaintPaymentDetail> List = new List<ComplaintPaymentDetail>();
                while (reader.Read())
                {
                    ComplaintPaymentDetail list = new ComplaintPaymentDetail();
                    list.TotalAmount = DatabaseAccessEngine.GetDecimal(reader, "TotalAmount");
                    list.PaidAmount = DatabaseAccessEngine.GetDecimal(reader, "PaidAmount");
                    list.DueAmount = DatabaseAccessEngine.GetDecimal(reader, "DueAmount");
                    list.PaymentStatus = DatabaseAccessEngine.GetString(reader, "PaymentStatus");
                    list.Status = DatabaseAccessEngine.GetString(reader, "Status");
                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public List<ComplaintPaymentReceived> GetComplaintPaymentReceived(Int64 ComplaintId, Int64 ReceiptNumber, DateTime ReceiptDate, Decimal PaidAmount, Int64 PaidById, String PaidOn, String PaymentMode, String TransactionNumber, DateTime TransactionDate, Int64 BankId, Int64 CreateUser)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Complaint_Payment_Received_By_Admin");

            unit.AddParameter("ComplaintId", SqlDbType.BigInt, ParameterDirection.Input, ComplaintId);
            unit.AddParameter("PaidAmount", SqlDbType.Decimal, ParameterDirection.Input, PaidAmount);
            unit.AddParameter("PaidById", SqlDbType.BigInt, ParameterDirection.Input, PaidById);
            unit.AddParameter("PaidOn", SqlDbType.VarChar, ParameterDirection.Input, PaidOn);
            unit.AddParameter("PaymentMode", SqlDbType.VarChar, ParameterDirection.Input, PaymentMode);
            unit.AddParameter("TransactionNumber", SqlDbType.VarChar, ParameterDirection.Input, TransactionNumber);
            unit.AddParameter("TransactionDate", SqlDbType.DateTime, ParameterDirection.Input, TransactionDate);
            unit.AddParameter("BankId", SqlDbType.BigInt, ParameterDirection.Input, BankId);
            unit.AddParameter("ReceiptNumber", SqlDbType.BigInt, ParameterDirection.Input, ReceiptNumber);
            unit.AddParameter("ReceiptDate", SqlDbType.DateTime, ParameterDirection.Input, ReceiptDate);
            unit.AddParameter("CreateUser", SqlDbType.BigInt, ParameterDirection.Input, CreateUser);
            unit.AddParameter("Mode", SqlDbType.VarChar, ParameterDirection.Input, "ComplaintPaymentReceived");

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<ComplaintPaymentReceived> List = new List<ComplaintPaymentReceived>();
                while (reader.Read())
                {
                    ComplaintPaymentReceived list = new ComplaintPaymentReceived();
                    list.ComplaintId = DatabaseAccessEngine.GetInt64(reader, "ComplaintId");
                    list.PaidAmount = DatabaseAccessEngine.GetDecimal(reader, "PaidAmount");
                    list.PaidBy = DatabaseAccessEngine.GetString(reader, "PaidBy");
                    list.PaymentStatus = DatabaseAccessEngine.GetString(reader, "PaymentStatus");
                    list.ComplaintStatus = DatabaseAccessEngine.GetString(reader, "ComplaintStatus");
                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public List<ComplaintBankList> GetComplaintBankList()
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Complaint_Bank_List");

            unit.AddParameter("Mode", SqlDbType.VarChar, ParameterDirection.Input, "ComplaintBankList");

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<ComplaintBankList> List = new List<ComplaintBankList>();
                while (reader.Read())
                {
                    ComplaintBankList list = new ComplaintBankList();
                    list.Id = DatabaseAccessEngine.GetInt64(reader, "Id");
                    list.BankName = DatabaseAccessEngine.GetString(reader, "BankName");
                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public List<ComplaintDCReferredListModel> GetComplaintDCReferredList(BaseFilter baseFilter, String Status, String FromDate, String ToDate, String DCCommittee = "")
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Get_DC_ReferredComplaint_List");

            unit.AddParameter("SearchValue", SqlDbType.VarChar, ParameterDirection.Input, string.IsNullOrEmpty(baseFilter.searchValue) ? string.Empty : baseFilter.searchValue);
            unit.AddParameter("Status", SqlDbType.NVarChar, ParameterDirection.Input, string.IsNullOrEmpty(Status) ? string.Empty : Status);
            unit.AddParameter("FromDate", SqlDbType.NVarChar, ParameterDirection.Input, string.IsNullOrEmpty(FromDate) ? string.Empty : FromDate);
            unit.AddParameter("ToDate", SqlDbType.NVarChar, ParameterDirection.Input, string.IsNullOrEmpty(ToDate) ? string.Empty : ToDate);
            unit.AddParameter("DCCommittee", SqlDbType.NVarChar, ParameterDirection.Input, string.IsNullOrEmpty(DCCommittee) ? string.Empty : DCCommittee);
            unit.AddParameter("PageNo", SqlDbType.Int, ParameterDirection.Input, baseFilter.pageNo);
            unit.AddParameter("PageSize", SqlDbType.Int, ParameterDirection.Input, baseFilter.pageSize);
            unit.AddParameter("SortColumn", SqlDbType.Int, ParameterDirection.Input, baseFilter.sortColumn);
            unit.AddParameter("SortOrder", SqlDbType.VarChar, ParameterDirection.Input, baseFilter.sortOrder);
            unit.AddParameter("Mode", SqlDbType.VarChar, ParameterDirection.Input, "SelectAll");
            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<ComplaintDCReferredListModel> List = new List<ComplaintDCReferredListModel>();
                while (reader.Read())
                {
                    ComplaintDCReferredListModel list = new ComplaintDCReferredListModel();
                    list.SrNo = DatabaseAccessEngine.GetNullableInt64(reader, "SrNo");
                    list.Id = DatabaseAccessEngine.GetInt64(reader, "ID");
                    list.TotalRecord = DatabaseAccessEngine.GetNullableInt(reader, "TotalRecord");
                    list.FullName = DatabaseAccessEngine.GetString(reader, "FullName");
                    list.EmailAddress = DatabaseAccessEngine.GetString(reader, "EmailAddress");
                    list.ContactNumber = DatabaseAccessEngine.GetString(reader, "ContactNumber");
                    list.Summary = DatabaseAccessEngine.GetString(reader, "Summary");
                    list.DetailComplaint = DatabaseAccessEngine.GetString(reader, "DetailComplaint");
                    list.Status = DatabaseAccessEngine.GetString(reader, "Status");
                    list.PaymentStatus = DatabaseAccessEngine.GetString(reader, "PaymentStatus");
                    list.ComplaintNumber = DatabaseAccessEngine.GetString(reader, "ComplaintNumber");
                    list.TotalAmount = DatabaseAccessEngine.GetDecimal(reader, "TotalAmount");
                    list.DCComplaintNumber = DatabaseAccessEngine.GetString(reader, "DCComplaintNumber");
                    list.DCComplaintNumberCode = DatabaseAccessEngine.GetString(reader, "DCComplaintNumberCode");
                    list.CommitteTitle = DatabaseAccessEngine.GetString(reader, "CommitteTitle");
                    list.AssignCommitteeDate = DatabaseAccessEngine.GetString(reader, "AssignCommitteeDate");
                    list.RespondentDetails = DatabaseAccessEngine.GetString(reader, "RespondentDetails");
                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public List<ComplaintDCReferredListReportModel> GetComplaintDCReferredListReport(String SearchValue, String FromDate, String ToDate, String SelectComplaintIds)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Get_DC_ReferredComplaint_Report_List");

            unit.AddParameter("SearchValue", SqlDbType.VarChar, ParameterDirection.Input, SearchValue);
            unit.AddParameter("FromDate", SqlDbType.NVarChar, ParameterDirection.Input, FromDate);
            unit.AddParameter("ToDate", SqlDbType.NVarChar, ParameterDirection.Input, ToDate);
            unit.AddParameter("SelectComplaintIds", SqlDbType.NVarChar, ParameterDirection.Input, SelectComplaintIds);
            unit.AddParameter("Mode", SqlDbType.VarChar, ParameterDirection.Input, "SelectAll");
            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<ComplaintDCReferredListReportModel> List = new List<ComplaintDCReferredListReportModel>();
                while (reader.Read())
                {
                    ComplaintDCReferredListReportModel list = new ComplaintDCReferredListReportModel();
                    list.SrNo = DatabaseAccessEngine.GetInt64(reader, "SrNo");
                    list.FullName = DatabaseAccessEngine.GetString(reader, "FullName");
                    list.EmailAddress = DatabaseAccessEngine.GetString(reader, "EmailAddress");
                    list.ContactNumber = DatabaseAccessEngine.GetString(reader, "ContactNumber");
                    list.ComplaintNumber = DatabaseAccessEngine.GetString(reader, "ComplaintNumber");
                    list.DCComplaintNumber = DatabaseAccessEngine.GetString(reader, "DCComplaintNumber");
                    list.RespondentDetails = DatabaseAccessEngine.GetString(reader, "RespondentDetails");
                    list.RespondentDetailsExcel = DatabaseAccessEngine.GetString(reader, "RespondentDetailsExcel");
                    list.MeetingDated = DatabaseAccessEngine.GetString(reader, "MeetingDated");
                    list.Status = DatabaseAccessEngine.GetString(reader, "Status");
                    list.CommitteTitle = DatabaseAccessEngine.GetString(reader, "CommitteTitle");
                    list.AssignCommitteeDate = DatabaseAccessEngine.GetString(reader, "AssignCommitteeDate");
                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public List<ComplaintPaymentReceivedLog> GetComplaintPaymentReceivedLog(Int64 ComplaintId)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Complaint_Payment_Received_Log");

            unit.AddParameter("ComplaintId", SqlDbType.BigInt, ParameterDirection.Input, ComplaintId);
            unit.AddParameter("Mode", SqlDbType.VarChar, ParameterDirection.Input, "ComplaintPaymentReceivedLog");

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<ComplaintPaymentReceivedLog> List = new List<ComplaintPaymentReceivedLog>();
                while (reader.Read())
                {
                    ComplaintPaymentReceivedLog list = new ComplaintPaymentReceivedLog();
                    list.ReceiptNumber = DatabaseAccessEngine.GetInt64(reader, "ReceiptNumber");
                    list.ReceiptDate = DatabaseAccessEngine.GetString(reader, "ReceiptDate");
                    list.PaidAmount = DatabaseAccessEngine.GetDecimal(reader, "PaidAmount");
                    list.PaymentMode = DatabaseAccessEngine.GetString(reader, "PaymentMode");
                    list.TransactionNumber = DatabaseAccessEngine.GetString(reader, "TransactionNumber");
                    list.TransactionDate = DatabaseAccessEngine.GetString(reader, "TransactionDate");
                    list.BankName = DatabaseAccessEngine.GetString(reader, "BankName");
                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public List<ComplaintNumberUpdate> ComplaintNumberUpdate(Int64 ComplaintId, Int64 ComplaintNumberCode, String ComplaintNumber, Int64 LastUpdateUser, String Mode)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Complaint_Number_Update");

            unit.AddParameter("Id", SqlDbType.BigInt, ParameterDirection.Input, ComplaintId);
            unit.AddParameter("ComplaintNumberCode", SqlDbType.BigInt, ParameterDirection.Input, ComplaintNumberCode);
            unit.AddParameter("ComplaintNumber", SqlDbType.VarChar, ParameterDirection.Input, ComplaintNumber);
            unit.AddParameter("LastUpdateUser", SqlDbType.BigInt, ParameterDirection.Input, LastUpdateUser);
            unit.AddParameter("Mode", SqlDbType.VarChar, ParameterDirection.Input, Mode);

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<ComplaintNumberUpdate> List = new List<ComplaintNumberUpdate>();
                while (reader.Read())
                {
                    ComplaintNumberUpdate list = new ComplaintNumberUpdate();
                    list.ReturnStatus = DatabaseAccessEngine.GetString(reader, "ReturnStatus");
                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public List<ComplaintNumberUpdate> ComplaintComplainantDetailsUpdate(Int64 Id, String FullName, String EmailAddress, String ContactNumber, String Address, Int64 UpdateUser, String Mode)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Complaint_Complainant_Details_Update");

            unit.AddParameter("Id", SqlDbType.BigInt, ParameterDirection.Input, Id);
            unit.AddParameter("FullName", SqlDbType.NVarChar, ParameterDirection.Input, string.IsNullOrEmpty(FullName) ? string.Empty : FullName);
            unit.AddParameter("EmailAddress", SqlDbType.NVarChar, ParameterDirection.Input, string.IsNullOrEmpty(EmailAddress) ? string.Empty : EmailAddress);
            unit.AddParameter("ContactNumber", SqlDbType.NVarChar, ParameterDirection.Input, string.IsNullOrEmpty(ContactNumber) ? string.Empty : ContactNumber);
            unit.AddParameter("Address", SqlDbType.NVarChar, ParameterDirection.Input, string.IsNullOrEmpty(Address) ? string.Empty : Address);
            unit.AddParameter("UpdateUser", SqlDbType.BigInt, ParameterDirection.Input, UpdateUser);
            unit.AddParameter("Mode", SqlDbType.NVarChar, ParameterDirection.Input, Mode);

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<ComplaintNumberUpdate> List = new List<ComplaintNumberUpdate>();
                while (reader.Read())
                {
                    ComplaintNumberUpdate list = new ComplaintNumberUpdate();
                    list.ReturnStatus = DatabaseAccessEngine.GetString(reader, "ReturnStatus");
                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public List<ComplaintNumberUpdate> ComplaintAffidavitCopyUpdate(Int64 ComplaintId, String FileName, String FilePath, String FileExtension, Int64 CreateUser, String Mode)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Complaint_Affidavit_Copy_Update");

            unit.AddParameter("ComplaintId", SqlDbType.BigInt, ParameterDirection.Input, ComplaintId);
            unit.AddParameter("FileName", SqlDbType.NVarChar, ParameterDirection.Input, string.IsNullOrEmpty(FileName) ? string.Empty : FileName);
            unit.AddParameter("FilePath", SqlDbType.NVarChar, ParameterDirection.Input, string.IsNullOrEmpty(FilePath) ? string.Empty : FilePath);
            unit.AddParameter("FileExtension", SqlDbType.NVarChar, ParameterDirection.Input, string.IsNullOrEmpty(FileExtension) ? string.Empty : FileExtension);
            unit.AddParameter("CreateUser", SqlDbType.BigInt, ParameterDirection.Input, CreateUser);
            unit.AddParameter("Mode", SqlDbType.NVarChar, ParameterDirection.Input, Mode);

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<ComplaintNumberUpdate> List = new List<ComplaintNumberUpdate>();
                while (reader.Read())
                {
                    ComplaintNumberUpdate list = new ComplaintNumberUpdate();
                    list.ReturnStatus = DatabaseAccessEngine.GetString(reader, "ReturnStatus");
                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public List<ComplaintNumberUpdate> ComplaintAdditionalAttachmentsUpdate(Int64 Id, Int64 ComplaintId, String FileTitle, String FileName, String FilePath, String Description, String FileExtension, Int64 CreateUser, String Mode)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Complaint_Additional_Attachments_Update");

            unit.AddParameter("Id", SqlDbType.BigInt, ParameterDirection.Input, Id);
            unit.AddParameter("ComplaintId", SqlDbType.BigInt, ParameterDirection.Input, ComplaintId);
            unit.AddParameter("FileTitle", SqlDbType.NVarChar, ParameterDirection.Input, string.IsNullOrEmpty(FileTitle) ? string.Empty : FileTitle);
            unit.AddParameter("FileName", SqlDbType.NVarChar, ParameterDirection.Input, string.IsNullOrEmpty(FileName) ? string.Empty : FileName);
            unit.AddParameter("FilePath", SqlDbType.NVarChar, ParameterDirection.Input, string.IsNullOrEmpty(FilePath) ? string.Empty : FilePath);
            unit.AddParameter("Description", SqlDbType.NVarChar, ParameterDirection.Input, string.IsNullOrEmpty(Description) ? string.Empty : Description);
            unit.AddParameter("FileExtension", SqlDbType.NVarChar, ParameterDirection.Input, string.IsNullOrEmpty(FileExtension) ? string.Empty : FileExtension);
            unit.AddParameter("CreateUser", SqlDbType.BigInt, ParameterDirection.Input, CreateUser);
            unit.AddParameter("Mode", SqlDbType.NVarChar, ParameterDirection.Input, Mode);

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<ComplaintNumberUpdate> List = new List<ComplaintNumberUpdate>();
                while (reader.Read())
                {
                    ComplaintNumberUpdate list = new ComplaintNumberUpdate();
                    list.ReturnStatus = DatabaseAccessEngine.GetString(reader, "ReturnStatus");
                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public List<ComplaintNumberUpdate> ComplaintComplaintReplySave(Int64 ComplaintId, Int64 SenderId, String SenderType, String ReceiverType, String Summery, String Comprehensive, String FileName, String FilePath, String Mode)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Complaint_Complaint_Reply_Save");

            unit.AddParameter("ComplaintId", SqlDbType.BigInt, ParameterDirection.Input, ComplaintId);
            unit.AddParameter("SenderId", SqlDbType.BigInt, ParameterDirection.Input, SenderId);
            unit.AddParameter("SenderType", SqlDbType.NVarChar, ParameterDirection.Input, string.IsNullOrEmpty(SenderType) ? string.Empty : SenderType);
            unit.AddParameter("ReceiverType", SqlDbType.NVarChar, ParameterDirection.Input, string.IsNullOrEmpty(ReceiverType) ? string.Empty : ReceiverType);
            unit.AddParameter("Summery", SqlDbType.NVarChar, ParameterDirection.Input, string.IsNullOrEmpty(Summery) ? string.Empty : Summery);
            unit.AddParameter("Comprehensive", SqlDbType.NVarChar, ParameterDirection.Input, string.IsNullOrEmpty(Comprehensive) ? string.Empty : Comprehensive);
            unit.AddParameter("FileName", SqlDbType.NVarChar, ParameterDirection.Input, string.IsNullOrEmpty(FileName) ? string.Empty : FileName);
            unit.AddParameter("FilePath", SqlDbType.NVarChar, ParameterDirection.Input, string.IsNullOrEmpty(FilePath) ? string.Empty : FilePath);
            unit.AddParameter("Mode", SqlDbType.NVarChar, ParameterDirection.Input, Mode);

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<ComplaintNumberUpdate> List = new List<ComplaintNumberUpdate>();
                while (reader.Read())
                {
                    ComplaintNumberUpdate list = new ComplaintNumberUpdate();
                    list.ReturnStatus = DatabaseAccessEngine.GetString(reader, "ReturnStatus");
                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public List<ComplaintModel> SaveComplaint(ComplaintModel complaint)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Complaints_Manage");
            unit.AddParameter("Id", SqlDbType.BigInt, ParameterDirection.Input, complaint.Id);
            unit.AddParameter("FullName", SqlDbType.NVarChar, ParameterDirection.Input, complaint.FullName);
            unit.AddParameter("EmailAddress", SqlDbType.NVarChar, ParameterDirection.Input, complaint.EmailAddress);
            unit.AddParameter("ContactNumber", SqlDbType.NVarChar, ParameterDirection.Input, complaint.ContactNumber);
            unit.AddParameter("Address", SqlDbType.NVarChar, ParameterDirection.Input, complaint.Address);
            unit.AddParameter("Summary", SqlDbType.NVarChar, ParameterDirection.Input, complaint.Summary);
            unit.AddParameter("DetailComplaint", SqlDbType.NVarChar, ParameterDirection.Input, complaint.DetailComplaint);
            unit.AddParameter("TotalAmount", SqlDbType.Decimal, ParameterDirection.Input, complaint.TotalAmount);
            unit.AddParameter("CreateUser", SqlDbType.BigInt, ParameterDirection.Input, complaint.CreateUser);
            unit.AddParameter("Status", SqlDbType.NVarChar, ParameterDirection.Input, complaint.Status);
            unit.AddParameter("Mode", SqlDbType.NVarChar, ParameterDirection.Input, "InsertMVC");

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<ComplaintModel> List = new List<ComplaintModel>();
                while (reader.Read())
                {
                    ComplaintModel list = new ComplaintModel();
                    list.ReturnStatus = DatabaseAccessEngine.GetString(reader, "ReturnStatus");
                    list.Id = DatabaseAccessEngine.GetInt64(reader, "Id");
                    List.Add(list);
                }

                return List;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                reader.Close();
            }
        }

        public dynamic SaveDefendantDetails(DefendantDetailsList den)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Complaint_Against");
            unit.AddParameter("Id", SqlDbType.BigInt, ParameterDirection.Input, den.Id);
            unit.AddParameter("EnrollmentId", SqlDbType.BigInt, ParameterDirection.Input, den.AdvocateId);
            unit.AddParameter("Amount", SqlDbType.Decimal, ParameterDirection.Input, den.Amount);
            unit.AddParameter("CreateUser", SqlDbType.BigInt, ParameterDirection.Input, den.CreateUser);
            unit.AddParameter("ComplaintId", SqlDbType.BigInt, ParameterDirection.Input, den.ComplaintId);
            unit.AddParameter("AdvocateName", SqlDbType.NVarChar, ParameterDirection.Input, den.AdvocateName);
            unit.AddParameter("City", SqlDbType.NVarChar, ParameterDirection.Input, den.City);
            unit.AddParameter("MobileNo", SqlDbType.NVarChar, ParameterDirection.Input, den.ContactNumber);
            unit.AddParameter("IsKnown", SqlDbType.NVarChar, ParameterDirection.Input, den.IsKnown);
            unit.AddParameter("Mode", SqlDbType.NVarChar, ParameterDirection.Input, "InsertForMVC");

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<DefendantDetailsList> List = new List<DefendantDetailsList>();
                while (reader.Read())
                {
                    DefendantDetailsList list = new DefendantDetailsList();
                    list.ReturnStatus = DatabaseAccessEngine.GetString(reader, "ReturnStatus");
                    List.Add(list);
                }

                return List;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                reader.Close();
            }
        }

        public dynamic SaveFileDetails(FileDetailsList file)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Complaint_FileUpload");
            unit.AddParameter("Id", SqlDbType.BigInt, ParameterDirection.Input, file.Id);
            unit.AddParameter("FileTitle", SqlDbType.NVarChar, ParameterDirection.Input, file.FileTitle);
            unit.AddParameter("FilePath", SqlDbType.NVarChar, ParameterDirection.Input, file.FilePath);
            unit.AddParameter("FileName", SqlDbType.NVarChar, ParameterDirection.Input, file.FileName);
            unit.AddParameter("Description", SqlDbType.NVarChar, ParameterDirection.Input, file.Description);
            unit.AddParameter("CreateUser", SqlDbType.BigInt, ParameterDirection.Input, file.CreateUser);
            unit.AddParameter("ComplaintId", SqlDbType.BigInt, ParameterDirection.Input, file.ComplaintId);
            unit.AddParameter("FileExtension", SqlDbType.VarChar, ParameterDirection.Input, file.FileExtension);

            unit.AddParameter("Mode", SqlDbType.NVarChar, ParameterDirection.Input, "InsertMVC");

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<FileDetailsList> List = new List<FileDetailsList>();
                while (reader.Read())
                {
                    FileDetailsList list = new FileDetailsList();
                    list.ReturnStatus = DatabaseAccessEngine.GetString(reader, "ReturnStatus");
                    List.Add(list);
                }

                return List;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                reader.Close();
            }
        }

        public dynamic SaveAffidavitCopy(AffidavitCopy affidavit)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Complaint_Affidavit_Copy");
            unit.AddParameter("Id", SqlDbType.BigInt, ParameterDirection.Input, affidavit.Id);
            unit.AddParameter("FilePath", SqlDbType.NVarChar, ParameterDirection.Input, affidavit.FilePath);
            unit.AddParameter("FileName", SqlDbType.NVarChar, ParameterDirection.Input, affidavit.FileName);
            unit.AddParameter("FileExtension", SqlDbType.VarChar, ParameterDirection.Input, affidavit.FileExtension);
            unit.AddParameter("CreateUser", SqlDbType.BigInt, ParameterDirection.Input, affidavit.CreateUser);
            unit.AddParameter("ComplaintId", SqlDbType.BigInt, ParameterDirection.Input, affidavit.ComplaintId);
            unit.AddParameter("Mode", SqlDbType.NVarChar, ParameterDirection.Input, "InsertForMVC");

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<AffidavitCopy> List = new List<AffidavitCopy>();
                while (reader.Read())
                {
                    AffidavitCopy list = new AffidavitCopy();
                    list.ReturnStatus = DatabaseAccessEngine.GetString(reader, "ReturnStatus");
                    List.Add(list);
                }

                return List;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                reader.Close();
            }
        }

        public List<ComplaintNumberUpdate> ComplaintDelete(Int64 ComplaintId, Int64 LastUpdateUser, String Mode)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Complaint_Delete");

            unit.AddParameter("ComplaintId", SqlDbType.BigInt, ParameterDirection.Input, ComplaintId);
            unit.AddParameter("LastUpdateUser", SqlDbType.BigInt, ParameterDirection.Input, LastUpdateUser);
            unit.AddParameter("Mode", SqlDbType.VarChar, ParameterDirection.Input, Mode);

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<ComplaintNumberUpdate> List = new List<ComplaintNumberUpdate>();
                while (reader.Read())
                {
                    ComplaintNumberUpdate list = new ComplaintNumberUpdate();
                    list.ReturnStatus = DatabaseAccessEngine.GetString(reader, "ReturnStatus");
                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public List<ComplaintNumberUpdate> OnlineComplaintDelete(Int64 ComplaintId, Int64 LastUpdateUser, String Mode)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Online_Complaint_Delete");

            unit.AddParameter("ComplaintId", SqlDbType.BigInt, ParameterDirection.Input, ComplaintId);
            unit.AddParameter("LastUpdateUser", SqlDbType.BigInt, ParameterDirection.Input, LastUpdateUser);
            unit.AddParameter("Mode", SqlDbType.VarChar, ParameterDirection.Input, Mode);

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<ComplaintNumberUpdate> List = new List<ComplaintNumberUpdate>();
                while (reader.Read())
                {
                    ComplaintNumberUpdate list = new ComplaintNumberUpdate();
                    list.ReturnStatus = DatabaseAccessEngine.GetString(reader, "ReturnStatus");
                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public List<ComplaintDetailByNumber> GetComplaintDetailByComplaintNumber(String ComplaintNumber)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Complaint_Detail_By_Complaint_Number");

            unit.AddParameter("ComplaintNumber", SqlDbType.NVarChar, ParameterDirection.Input, string.IsNullOrEmpty(ComplaintNumber) ? string.Empty : ComplaintNumber);
            unit.AddParameter("Mode", SqlDbType.NVarChar, ParameterDirection.Input, "ComplaintDetailByComplaintNumber");

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<ComplaintDetailByNumber> List = new List<ComplaintDetailByNumber>();
                while (reader.Read())
                {
                    ComplaintDetailByNumber list = new ComplaintDetailByNumber();
                    list.Id = DatabaseAccessEngine.GetInt64(reader, "Id");
                    list.ComplaintNumber = DatabaseAccessEngine.GetString(reader, "ComplaintNumber");
                    list.FullName = DatabaseAccessEngine.GetString(reader, "FullName");
                    list.EmailAddress = DatabaseAccessEngine.GetString(reader, "EmailAddress");
                    list.ContactNumber = DatabaseAccessEngine.GetString(reader, "ContactNumber");
                    list.Address = DatabaseAccessEngine.GetString(reader, "Address");
                    list.TotalAmount = DatabaseAccessEngine.GetDecimal(reader, "TotalAmount");
                    list.PaidAmount = DatabaseAccessEngine.GetDecimal(reader, "PaidAmount");
                    list.DueAmount = DatabaseAccessEngine.GetDecimal(reader, "DueAmount");
                    list.Status = DatabaseAccessEngine.GetString(reader, "Status");
                    list.ComplaintAgainstList = DatabaseAccessEngine.GetString(reader, "ComplaintAgainstList");
                    list.ComplaintAffidavitCopyList = DatabaseAccessEngine.GetString(reader, "ComplaintAffidavitCopyList");
                    list.ComplaintAdditionalAttachmentsList = DatabaseAccessEngine.GetString(reader, "ComplaintAdditionalAttachmentsList");
                    list.ComplaintPaymentList = DatabaseAccessEngine.GetString(reader, "ComplaintPaymentList");
                    list.ComplainedAgainstReplyToList = DatabaseAccessEngine.GetString(reader, "ComplainedAgainstReplyToList");
                    list.SCreateDate = DatabaseAccessEngine.GetString(reader, "SCreateDate");
                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public List<ComplaintDetailByNumber> GetComplaintComplainantsDetailsByComplaintNumber(String ComplaintNumber)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Complaint_Detail_By_Complaint_Number");

            unit.AddParameter("ComplaintNumber", SqlDbType.NVarChar, ParameterDirection.Input, string.IsNullOrEmpty(ComplaintNumber) ? string.Empty : ComplaintNumber);
            unit.AddParameter("Mode", SqlDbType.NVarChar, ParameterDirection.Input, "ComplaintComplainantsDetailsByComplaintNumber");

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<ComplaintDetailByNumber> List = new List<ComplaintDetailByNumber>();
                while (reader.Read())
                {
                    ComplaintDetailByNumber list = new ComplaintDetailByNumber();
                    list.Id = DatabaseAccessEngine.GetInt64(reader, "Id");
                    list.ComplaintNumber = DatabaseAccessEngine.GetString(reader, "ComplaintNumber");
                    list.FullName = DatabaseAccessEngine.GetString(reader, "FullName");
                    list.EmailAddress = DatabaseAccessEngine.GetString(reader, "EmailAddress");
                    list.ContactNumber = DatabaseAccessEngine.GetString(reader, "ContactNumber");
                    list.Address = DatabaseAccessEngine.GetString(reader, "Address");
                    list.Status = DatabaseAccessEngine.GetString(reader, "Status");
                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public List<ComplaintAffidavitCopyByNumber> GetComplainAffidavitCopyByComplaintNumber(String ComplaintNumber)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Complaint_Detail_By_Complaint_Number");

            unit.AddParameter("ComplaintNumber", SqlDbType.NVarChar, ParameterDirection.Input, string.IsNullOrEmpty(ComplaintNumber) ? string.Empty : ComplaintNumber);
            unit.AddParameter("Mode", SqlDbType.NVarChar, ParameterDirection.Input, "ComplaintAffidavitCopyByComplaintNumber");

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<ComplaintAffidavitCopyByNumber> List = new List<ComplaintAffidavitCopyByNumber>();
                while (reader.Read())
                {
                    ComplaintAffidavitCopyByNumber list = new ComplaintAffidavitCopyByNumber();
                    list.Id = DatabaseAccessEngine.GetInt64(reader, "Id");
                    list.ComplaintId = DatabaseAccessEngine.GetInt64(reader, "ComplaintId");
                    list.FileName = DatabaseAccessEngine.GetString(reader, "FileName");
                    list.FilePath = DatabaseAccessEngine.GetString(reader, "FilePath");
                    list.IsOld = DatabaseAccessEngine.GetString(reader, "IsOld");
                    list.FileExtension = DatabaseAccessEngine.GetString(reader, "FileExtension");
                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public List<ComplaintAdditionalAttachmentsByNumber> GetComplainAdditionalAttachmentsByComplaintNumber(String ComplaintNumber)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Complaint_Detail_By_Complaint_Number");

            unit.AddParameter("ComplaintNumber", SqlDbType.NVarChar, ParameterDirection.Input, string.IsNullOrEmpty(ComplaintNumber) ? string.Empty : ComplaintNumber);
            unit.AddParameter("Mode", SqlDbType.NVarChar, ParameterDirection.Input, "ComplaintAdditionalAttachmentsByComplaintNumber");

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<ComplaintAdditionalAttachmentsByNumber> List = new List<ComplaintAdditionalAttachmentsByNumber>();
                while (reader.Read())
                {
                    ComplaintAdditionalAttachmentsByNumber list = new ComplaintAdditionalAttachmentsByNumber();
                    list.Id = DatabaseAccessEngine.GetInt64(reader, "Id");
                    list.ComplaintId = DatabaseAccessEngine.GetInt64(reader, "ComplaintId");
                    list.FileTitle = DatabaseAccessEngine.GetString(reader, "FileTitle");
                    list.FileName = DatabaseAccessEngine.GetString(reader, "FileName");
                    list.FilePath = DatabaseAccessEngine.GetString(reader, "FilePath");
                    list.Description = DatabaseAccessEngine.GetString(reader, "Description");
                    list.FileExtension = DatabaseAccessEngine.GetString(reader, "FileExtension");
                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public List<ComplaintMemberLogList> GetComplaintMemberLogList(Int64 Id, Int64 ComplaintId, Int64 CreateUser)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Complaint_Status_Log");

            unit.AddParameter("Id", SqlDbType.BigInt, ParameterDirection.Input, Id);
            unit.AddParameter("ComplaintId", SqlDbType.BigInt, ParameterDirection.Input, ComplaintId);
            unit.AddParameter("CreateUser", SqlDbType.BigInt, ParameterDirection.Input, CreateUser);
            unit.AddParameter("Mode", SqlDbType.NVarChar, ParameterDirection.Input, "ComplaintMemberLog");

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<ComplaintMemberLogList> List = new List<ComplaintMemberLogList>();
                while (reader.Read())
                {
                    ComplaintMemberLogList list = new ComplaintMemberLogList();
                    list.Id = DatabaseAccessEngine.GetInt64(reader, "Id");
                    list.MemberId = DatabaseAccessEngine.GetInt64(reader, "MemberId");
                    list.EnrollmentId = DatabaseAccessEngine.GetInt64(reader, "EnrollmentId");
                    list.EnroNumber = DatabaseAccessEngine.GetString(reader, "EnroNumber");
                    list.FullName = DatabaseAccessEngine.GetString(reader, "FullName");
                    list.ContactNumber = DatabaseAccessEngine.GetString(reader, "ContactNumber");
                    list.ComplaintDetailEncryptCode = DatabaseAccessEngine.GetString(reader, "ComplaintDetailEncryptCode");
                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public List<ComplaintGBMeetingLog> GetComplaintGBMeetingLogList(Int64 ComplaintId)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Get_Complaint_GB_Meeting_Log_List");

            unit.AddParameter("ComplaintId", SqlDbType.BigInt, ParameterDirection.Input, ComplaintId);
            unit.AddParameter("Mode", SqlDbType.NVarChar, ParameterDirection.Input, "SelectAll");

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<ComplaintGBMeetingLog> List = new List<ComplaintGBMeetingLog>();
                while (reader.Read())
                {
                    ComplaintGBMeetingLog list = new ComplaintGBMeetingLog();
                    list.SrNo = DatabaseAccessEngine.GetInt64(reader, "SrNo");
                    list.Id = DatabaseAccessEngine.GetInt64(reader, "Id");
                    list.MeetingTitle = DatabaseAccessEngine.GetString(reader, "MeetingTitle");
                    list.MeetingDate = DatabaseAccessEngine.GetString(reader, "MeetingDate");
                    list.MeetingNote = DatabaseAccessEngine.GetString(reader, "MeetingNote");
                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public List<ComplaintGBMeetingLogSaveReturn> ComplaintGBMeetingLogSave(Int64 Id, Int64 ComplaintId, String MeetingTitle, DateTime MeetingDate, String MeetingNote, Int64 CreatedUser, String ActionMode)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Get_Complaint_GB_Meeting_Log_Save");

            unit.AddParameter("Id", SqlDbType.BigInt, ParameterDirection.Input, Id);
            unit.AddParameter("ComplaintId", SqlDbType.BigInt, ParameterDirection.Input, ComplaintId);
            unit.AddParameter("MeetingTitle", SqlDbType.NVarChar, ParameterDirection.Input, MeetingTitle);
            unit.AddParameter("MeetingDate", SqlDbType.DateTime, ParameterDirection.Input, MeetingDate);
            unit.AddParameter("MeetingNote", SqlDbType.NVarChar, ParameterDirection.Input, MeetingNote);
            unit.AddParameter("CreatedUser", SqlDbType.BigInt, ParameterDirection.Input, CreatedUser);
            unit.AddParameter("Mode", SqlDbType.NVarChar, ParameterDirection.Input, ActionMode);

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<ComplaintGBMeetingLogSaveReturn> List = new List<ComplaintGBMeetingLogSaveReturn>();
                while (reader.Read())
                {
                    ComplaintGBMeetingLogSaveReturn list = new ComplaintGBMeetingLogSaveReturn();
                    list.ReturnStatus = DatabaseAccessEngine.GetString(reader, "ReturnStatus");
                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public List<ComplaintGBMeetingLogEdit> GetComplaintGBMeetingLogEdit(Int64 Id)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Get_Complaint_GB_Meeting_Log_Save");

            unit.AddParameter("Id", SqlDbType.BigInt, ParameterDirection.Input, Id);
            unit.AddParameter("ComplaintId", SqlDbType.BigInt, ParameterDirection.Input, 0);
            unit.AddParameter("MeetingTitle", SqlDbType.NVarChar, ParameterDirection.Input, "");
            unit.AddParameter("MeetingDate", SqlDbType.DateTime, ParameterDirection.Input, System.DateTime.Now);
            unit.AddParameter("MeetingNote", SqlDbType.NVarChar, ParameterDirection.Input, "");
            unit.AddParameter("CreatedUser", SqlDbType.BigInt, ParameterDirection.Input, 0);
            unit.AddParameter("Mode", SqlDbType.NVarChar, ParameterDirection.Input, "SelectDetail");

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<ComplaintGBMeetingLogEdit> List = new List<ComplaintGBMeetingLogEdit>();
                while (reader.Read())
                {
                    ComplaintGBMeetingLogEdit list = new ComplaintGBMeetingLogEdit();
                    list.ComplaintId = DatabaseAccessEngine.GetInt64(reader, "ComplaintId");
                    list.Id = DatabaseAccessEngine.GetInt64(reader, "Id");
                    list.MeetingTitle = DatabaseAccessEngine.GetString(reader, "MeetingTitle");
                    list.MeetingDate = DatabaseAccessEngine.GetString(reader, "MeetingDate");
                    list.MeetingNote = DatabaseAccessEngine.GetString(reader, "MeetingNote");
                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public List<ComplaintGBMeetingLogSaveReturn> ComplaintReplyDelete(Int64 Id, Int64 CreatedUser, String ActionMode)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Complaint_Reply_Delete");

            unit.AddParameter("Id", SqlDbType.BigInt, ParameterDirection.Input, Id);
            unit.AddParameter("CreatedUser", SqlDbType.BigInt, ParameterDirection.Input, CreatedUser);
            unit.AddParameter("Mode", SqlDbType.NVarChar, ParameterDirection.Input, ActionMode);

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<ComplaintGBMeetingLogSaveReturn> List = new List<ComplaintGBMeetingLogSaveReturn>();
                while (reader.Read())
                {
                    ComplaintGBMeetingLogSaveReturn list = new ComplaintGBMeetingLogSaveReturn();
                    list.ReturnStatus = DatabaseAccessEngine.GetString(reader, "ReturnStatus");
                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public List<ComplaintMemberEncryptCodeList> GetComplaintMemberEncryptCode(Int64 Id, String ActionMode)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Complaint_Member_Encrypt_Code");

            unit.AddParameter("Id", SqlDbType.BigInt, ParameterDirection.Input, Id);
            unit.AddParameter("Mode", SqlDbType.NVarChar, ParameterDirection.Input, ActionMode);

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<ComplaintMemberEncryptCodeList> List = new List<ComplaintMemberEncryptCodeList>();
                while (reader.Read())
                {
                    ComplaintMemberEncryptCodeList list = new ComplaintMemberEncryptCodeList();
                    list.Id = DatabaseAccessEngine.GetInt64(reader, "Id");
                    list.ComplaintId = DatabaseAccessEngine.GetInt64(reader, "ComplaintId");
                    list.Status = DatabaseAccessEngine.GetString(reader, "Status");
                    list.MemberId = DatabaseAccessEngine.GetInt64(reader, "MemberId");
                    list.IsChecked = DatabaseAccessEngine.GetString(reader, "IsChecked");
                    list.CommitteeId = DatabaseAccessEngine.GetInt64(reader, "CommitteeId");
                    list.ComplaintDetailEncryptCode = DatabaseAccessEngine.GetString(reader, "ComplaintDetailEncryptCode");
                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public List<ComplaintNumberUpdate> ComplaintStatusUpdate(Int64 ComplaintId,  Int64 LastUpdateUser, String Mode)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Complaint_Status_Update");

            unit.AddParameter("Id", SqlDbType.BigInt, ParameterDirection.Input, ComplaintId);
            unit.AddParameter("LastUpdateUser", SqlDbType.BigInt, ParameterDirection.Input, LastUpdateUser);
            unit.AddParameter("Mode", SqlDbType.VarChar, ParameterDirection.Input, Mode);

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<ComplaintNumberUpdate> List = new List<ComplaintNumberUpdate>();
                while (reader.Read())
                {
                    ComplaintNumberUpdate list = new ComplaintNumberUpdate();
                    list.ReturnStatus = DatabaseAccessEngine.GetString(reader, "ReturnStatus");
                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public List<ComplaintNumberUpdate> OnlineComplaintStatusUpdate(Int64 OnlineComplaintId, String Remark, Int64 LastUpdateUser, String Mode)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Online_Complaint_Status_Update");

            unit.AddParameter("OnlineComplaintId", SqlDbType.BigInt, ParameterDirection.Input, OnlineComplaintId);
            unit.AddParameter("Remark", SqlDbType.NVarChar, ParameterDirection.Input, string.IsNullOrEmpty(Remark) ? string.Empty : Remark);
            unit.AddParameter("CreateUser", SqlDbType.BigInt, ParameterDirection.Input, LastUpdateUser);
            unit.AddParameter("Mode", SqlDbType.NVarChar, ParameterDirection.Input, Mode);

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<ComplaintNumberUpdate> List = new List<ComplaintNumberUpdate>();
                while (reader.Read())
                {
                    ComplaintNumberUpdate list = new ComplaintNumberUpdate();
                    list.ReturnStatus = DatabaseAccessEngine.GetString(reader, "ReturnStatus");
                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }
    }
}

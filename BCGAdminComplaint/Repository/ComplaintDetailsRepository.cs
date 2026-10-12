using BCGAdminComplaint.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCGAdminComplaint.Repository
{
    public class ComplaintDetailsRepository
    {
        public int InsertComplaintStatusLog(ComplaintStatusLog log)
        {
            int result = 0;
            try
            {
                ExecutionUnit unit = new ExecutionUnit("SP_Complaint_Status_Log");

                unit.AddParameter("Mode", SqlDbType.VarChar, ParameterDirection.Input, "Insert");
                unit.AddParameter("Id", SqlDbType.BigInt, ParameterDirection.Input, log.Id);
                unit.AddParameter("ComplaintId", SqlDbType.BigInt, ParameterDirection.Input, log.ComplaintId);
                unit.AddParameter("NextDate", SqlDbType.DateTime, ParameterDirection.Input, log.NextDate);
                unit.AddParameter("OrderFileName", SqlDbType.NVarChar, ParameterDirection.Input, log.OrderFileName);
                unit.AddParameter("OrderFilePath", SqlDbType.NVarChar, ParameterDirection.Input, log.OrderFilePath);
                unit.AddParameter("Summary", SqlDbType.NVarChar, ParameterDirection.Input, log.Summary);
                unit.AddParameter("Status", SqlDbType.NVarChar, ParameterDirection.Input, log.Status);
                unit.AddParameter("CreateUser", SqlDbType.BigInt, ParameterDirection.Input, log.CreateUser);

                result = DatabaseAccessEngine.ExecuteNonQuery(unit);
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        public List<ComplaintStatusLog> GetComplaintDetails(Int64 ComplaintId,String Status)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Complaint_Status_Log");

            unit.AddParameter("ComplaintId", SqlDbType.BigInt, ParameterDirection.Input, ComplaintId);
            unit.AddParameter("Status", SqlDbType.NVarChar, ParameterDirection.Input, Status);
            unit.AddParameter("Mode", SqlDbType.VarChar, ParameterDirection.Input, "GetLogByStatus");

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<ComplaintStatusLog> List = new List<ComplaintStatusLog>();
                while (reader.Read())
                {
                    ComplaintStatusLog list = new ComplaintStatusLog();
                    list.Id = DatabaseAccessEngine.GetInt64(reader, "ID");
                    list.ComplaintId = DatabaseAccessEngine.GetInt64(reader, "ComplaintId");
                    list.NextDate = DatabaseAccessEngine.GetDateTime(reader, "NextDate");
                    list.Summary = DatabaseAccessEngine.GetString(reader, "Summary");
                    list.OrderFileName = DatabaseAccessEngine.GetString(reader, "OrderFileName");
                    list.OrderFilePath = DatabaseAccessEngine.GetString(reader, "OrderFilePath");
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

        public List<DcNumberList> GetDcNumberList()
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Get_DcList");

            unit.AddParameter("Mode", SqlDbType.VarChar, ParameterDirection.Input, "GetDcList");

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<DcNumberList> List = new List<DcNumberList>();
                while (reader.Read())
                {
                    DcNumberList list = new DcNumberList();
                    list.Id = DatabaseAccessEngine.GetInt64(reader, "ID");
                    list.DcTitle = DatabaseAccessEngine.GetString(reader, "DcCommitteeTitle");                   
                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public List<DcMemberList> GetDcMemberList(Int64 DcNo)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Get_DcList");

            unit.AddParameter("DcNo", SqlDbType.BigInt, ParameterDirection.Input, DcNo);
            unit.AddParameter("Mode", SqlDbType.VarChar, ParameterDirection.Input, "GetDcDetails");

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<DcMemberList> List = new List<DcMemberList>();
                while (reader.Read())
                {
                    DcMemberList list = new DcMemberList();
                    list.SrNo = DatabaseAccessEngine.GetNullableInt64(reader, "SrNo");
                    list.MemberId = DatabaseAccessEngine.GetInt64(reader, "MemberId");
                    list.EnroNumber = DatabaseAccessEngine.GetString(reader, "EnroNumber");                 
                    list.FullName = DatabaseAccessEngine.GetString(reader, "FullName");
                    list.ContactNumber = DatabaseAccessEngine.GetString(reader, "ContactNumber");
                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public List<DcMemberList> GetDcMemberListNotExist(Int64 DcNo)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Get_DcList");

            unit.AddParameter("DcNo", SqlDbType.BigInt, ParameterDirection.Input, DcNo);
            unit.AddParameter("Mode", SqlDbType.VarChar, ParameterDirection.Input, "GetDcDetailsNotExist");

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<DcMemberList> List = new List<DcMemberList>();
                while (reader.Read())
                {
                    DcMemberList list = new DcMemberList();
                    list.SrNo = DatabaseAccessEngine.GetNullableInt64(reader, "SrNo");
                    list.MemberId = DatabaseAccessEngine.GetInt64(reader, "MemberId");
                    list.EnroNumber = DatabaseAccessEngine.GetString(reader, "EnroNumber");
                    list.FullName = DatabaseAccessEngine.GetString(reader, "FullName");
                    list.ContactNumber = DatabaseAccessEngine.GetString(reader, "ContactNumber");
                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public List<ComplaintMemberLogSaveReturn> InsertComplaintMemberLog(ComplaintMemberLog log,String DCReferenceNumber)
        {
            try
            {
                ExecutionUnit unit = new ExecutionUnit("SP_Complaint_Member_Log");

                unit.AddParameter("Mode", SqlDbType.VarChar, ParameterDirection.Input, "Insert");
                unit.AddParameter("Id", SqlDbType.BigInt, ParameterDirection.Input, log.Id);
                unit.AddParameter("ComplaintId", SqlDbType.BigInt, ParameterDirection.Input, log.ComplaintId);
                unit.AddParameter("Status", SqlDbType.NVarChar, ParameterDirection.Input, log.Status);
                unit.AddParameter("MemberId", SqlDbType.BigInt, ParameterDirection.Input, log.MemberId);
                unit.AddParameter("IsChecked", SqlDbType.VarChar, ParameterDirection.Input, log.IsChecked);
                unit.AddParameter("CommitteeId", SqlDbType.BigInt, ParameterDirection.Input, log.CommitteeId);
                unit.AddParameter("DCReferenceNumber", SqlDbType.VarChar, ParameterDirection.Input, DCReferenceNumber);
                unit.AddParameter("ComplaintDetailEncryptCode", SqlDbType.VarChar, ParameterDirection.Input, log.ComplaintDetailEncryptCode);
                IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

                try
                {
                    List<ComplaintMemberLogSaveReturn> List = new List<ComplaintMemberLogSaveReturn>();
                    while (reader.Read())
                    {
                        ComplaintMemberLogSaveReturn list = new ComplaintMemberLogSaveReturn();
                        list.ComplaintId = DatabaseAccessEngine.GetInt64(reader, "ComplaintId");
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
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<ComplaintMemberLogSaveReturn> DCReferenceNumberUpdate(Int64 ComplaintId, String DCReferenceNumber)
        {
            try
            {
                ExecutionUnit unit = new ExecutionUnit("SP_Complaint_Member_Log");

                unit.AddParameter("Mode", SqlDbType.VarChar, ParameterDirection.Input, "DCReferenceNumberUpdate");
                unit.AddParameter("Id", SqlDbType.BigInt, ParameterDirection.Input, 0);
                unit.AddParameter("ComplaintId", SqlDbType.BigInt, ParameterDirection.Input, ComplaintId);
                unit.AddParameter("Status", SqlDbType.NVarChar, ParameterDirection.Input, "");
                unit.AddParameter("MemberId", SqlDbType.BigInt, ParameterDirection.Input, 0);
                unit.AddParameter("IsChecked", SqlDbType.VarChar, ParameterDirection.Input, "");
                unit.AddParameter("CommitteeId", SqlDbType.BigInt, ParameterDirection.Input, 0);
                unit.AddParameter("DCReferenceNumber", SqlDbType.VarChar, ParameterDirection.Input, DCReferenceNumber);
                unit.AddParameter("ComplaintDetailEncryptCode", SqlDbType.VarChar, ParameterDirection.Input, "");
                IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

                try
                {
                    List<ComplaintMemberLogSaveReturn> List = new List<ComplaintMemberLogSaveReturn>();
                    while (reader.Read())
                    {
                        ComplaintMemberLogSaveReturn list = new ComplaintMemberLogSaveReturn();
                        list.ComplaintId = DatabaseAccessEngine.GetInt64(reader, "ComplaintId");
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
            catch (Exception ex)
            {
                return null;
            }

        }

        public List<ComplaintMemberLog> GetDcMemberList(Int64 ComplaintId,String Status,Int64 CommitteeId)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Complaint_Member_Log");

            unit.AddParameter("ComplaintId", SqlDbType.BigInt, ParameterDirection.Input, ComplaintId);
            unit.AddParameter("Status", SqlDbType.NVarChar, ParameterDirection.Input, Status);
            unit.AddParameter("CommitteeId", SqlDbType.BigInt, ParameterDirection.Input, CommitteeId);
            unit.AddParameter("Mode", SqlDbType.VarChar, ParameterDirection.Input, "GetLogByStatus");


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

        public List<ComplaintStatusLog> GetLogList(Int64 ComplaintId) //Int64 ComplaintId, String Status
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Complaint_Status_Log");

            unit.AddParameter("ComplaintId", SqlDbType.BigInt, ParameterDirection.Input, ComplaintId);
            //unit.AddParameter("Status", SqlDbType.NVarChar, ParameterDirection.Input, Status);
            unit.AddParameter("Mode", SqlDbType.VarChar, ParameterDirection.Input, "LogList");

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<ComplaintStatusLog> List = new List<ComplaintStatusLog>();
                while (reader.Read())
                {
                    ComplaintStatusLog list = new ComplaintStatusLog();
                    list.Id = DatabaseAccessEngine.GetInt64(reader, "ID");
                    list.ComplaintId = DatabaseAccessEngine.GetInt64(reader, "ComplaintId");
                    list.NextDate = DatabaseAccessEngine.GetDateTime(reader, "NextDate");
                    list.Summary = DatabaseAccessEngine.GetString(reader, "Summary");
                    list.OrderFileName = DatabaseAccessEngine.GetString(reader, "OrderFileName");
                    list.OrderFilePath = DatabaseAccessEngine.GetString(reader, "OrderFilePath");
                    list.Status = DatabaseAccessEngine.GetString(reader, "Status");
                    list.Name = DatabaseAccessEngine.GetString(reader, "Name");
                    list.CommitteTitle = DatabaseAccessEngine.GetString(reader, "CommitteTitle");
                    list.CommitteeId = DatabaseAccessEngine.GetInt64(reader, "CommitteeId");
                    list.OldStatus = DatabaseAccessEngine.GetString(reader, "OldStatus");
                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public List<ComplaintStatusLog> GetLogListByMemberId(Int64 MemberId) //Int64 ComplaintId, String Status
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Complaint_Status_Log");

            unit.AddParameter("Id", SqlDbType.BigInt, ParameterDirection.Input, MemberId);            
            unit.AddParameter("Mode", SqlDbType.VarChar, ParameterDirection.Input, "LogListByMemberId");

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<ComplaintStatusLog> List = new List<ComplaintStatusLog>();
                while (reader.Read())
                {
                    ComplaintStatusLog list = new ComplaintStatusLog();
                    list.Id = DatabaseAccessEngine.GetInt64(reader, "ID");
                    list.ComplaintId = DatabaseAccessEngine.GetInt64(reader, "ComplaintId");
                    list.NextDate = DatabaseAccessEngine.GetDateTime(reader, "NextDate");
                    list.Summary = DatabaseAccessEngine.GetString(reader, "Summary");
                    list.OrderFileName = DatabaseAccessEngine.GetString(reader, "OrderFileName");
                    list.OrderFilePath = DatabaseAccessEngine.GetString(reader, "OrderFilePath");
                    list.Status = DatabaseAccessEngine.GetString(reader, "Status");
                    list.Name = DatabaseAccessEngine.GetString(reader, "Name");
                    list.CommitteTitle = DatabaseAccessEngine.GetString(reader, "CommitteTitle");
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

        public int UpdateStatus(Int64 ComplaintId, String Status)
        {
            int result = 0;
            try
            {
                ExecutionUnit unit = new ExecutionUnit("SP_Complaint_Status_Log");

                unit.AddParameter("Mode", SqlDbType.VarChar, ParameterDirection.Input, "UpdateStatus");
                unit.AddParameter("ComplaintId", SqlDbType.BigInt, ParameterDirection.Input, ComplaintId);
                unit.AddParameter("Status", SqlDbType.NVarChar, ParameterDirection.Input, Status);

                result = DatabaseAccessEngine.ExecuteNonQuery(unit);
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        public List<ComplaintListModel> GetComplaintReportList(Int64 ComplaintId)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_ComplaintList_Report");
            
            unit.AddParameter("ComplaintId", SqlDbType.BigInt, ParameterDirection.Input, ComplaintId);
            unit.AddParameter("Mode", SqlDbType.VarChar, ParameterDirection.Input, "ComplaintReport");

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<ComplaintListModel> List = new List<ComplaintListModel>();
                while (reader.Read())
                {
                    ComplaintListModel list = new ComplaintListModel();
                    list.SrNo = DatabaseAccessEngine.GetNullableInt64(reader, "SrNo");
                    list.FullName = DatabaseAccessEngine.GetString(reader, "FullName");
                    list.EmailAddress = DatabaseAccessEngine.GetString(reader, "EmailAddress");
                    list.ContactNumber = DatabaseAccessEngine.GetString(reader, "ContactNumber");
                    list.Summary = DatabaseAccessEngine.GetString(reader, "Summary");
                    list.DetailComplaint = DatabaseAccessEngine.GetString(reader, "DetailComplaint");
                    list.Status = DatabaseAccessEngine.GetString(reader, "Status");
                    list.PaymentStatus = DatabaseAccessEngine.GetString(reader, "PaymentStatus");
                    list.ComplaintNumber = DatabaseAccessEngine.GetString(reader, "ComplaintNumber");
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

        public List<ComplaintAgainstList> GetComplaintAgaintsReportList(Int64 ComplaintId)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_ComplaintList_Report");

            unit.AddParameter("ComplaintId", SqlDbType.BigInt, ParameterDirection.Input, ComplaintId);
            unit.AddParameter("Mode", SqlDbType.VarChar, ParameterDirection.Input, "ComplaintAgainstReport");

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

                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public List<ComplaintDocumentList> GetDocumentReportList(Int64 ComplaintId)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_ComplaintList_Report");

            unit.AddParameter("ComplaintId", SqlDbType.BigInt, ParameterDirection.Input, ComplaintId);
            unit.AddParameter("Mode", SqlDbType.VarChar, ParameterDirection.Input, "ComplaintFilesReport");

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<ComplaintDocumentList> List = new List<ComplaintDocumentList>();
                while (reader.Read())
                {
                    ComplaintDocumentList list = new ComplaintDocumentList();
                    list.SrNo = DatabaseAccessEngine.GetNullableInt64(reader, "SrNo");
                    list.ComplaintId = DatabaseAccessEngine.GetInt64(reader, "ComplaintId");
                    list.FileTitle = DatabaseAccessEngine.GetString(reader, "FileTitle");
                    list.FilePath = DatabaseAccessEngine.GetString(reader, "FilePath");
                    list.FileName = DatabaseAccessEngine.GetString(reader, "FileName");
                    list.Description = DatabaseAccessEngine.GetString(reader, "Description");
                    list.Name = DatabaseAccessEngine.GetString(reader, "Name");
                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public List<ComplaintStatusLog> GetLogListReport(Int64 ComplaintId) 
        {
            ExecutionUnit unit = new ExecutionUnit("SP_ComplaintList_Report");

            unit.AddParameter("ComplaintId", SqlDbType.BigInt, ParameterDirection.Input, ComplaintId);
            unit.AddParameter("Mode", SqlDbType.VarChar, ParameterDirection.Input, "ComplaintLogReport");

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<ComplaintStatusLog> List = new List<ComplaintStatusLog>();
                while (reader.Read())
                {
                    ComplaintStatusLog list = new ComplaintStatusLog();
                    list.SrNo = DatabaseAccessEngine.GetNullableInt64(reader, "SrNo");
                    list.NextDate = DatabaseAccessEngine.GetDateTime(reader, "NextDate");
                    list.Summary = DatabaseAccessEngine.GetString(reader, "Summary");
                    list.OrderFileName = DatabaseAccessEngine.GetString(reader, "OrderFileName");
                    list.OrderFilePath = DatabaseAccessEngine.GetString(reader, "OrderFilePath");
                    list.Status = DatabaseAccessEngine.GetString(reader, "Status");
                    list.CommitteTitle = DatabaseAccessEngine.GetString(reader, "CommitteTitle");
                    list.SNextDate = DatabaseAccessEngine.GetString(reader, "SNextDate");
                    list.OldStatus = DatabaseAccessEngine.GetString(reader, "OldStatus");
                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public List<ComplaintMemberLog> GetDcMemberLists(Int64 ComplaintId, Int64 CommitteeId)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Complaint_Member_Log");

            unit.AddParameter("Mode", SqlDbType.VarChar, ParameterDirection.Input, "GetDcMemberLists");
            unit.AddParameter("Id", SqlDbType.BigInt, ParameterDirection.Input, 0);
            unit.AddParameter("ComplaintId", SqlDbType.BigInt, ParameterDirection.Input, ComplaintId);
            unit.AddParameter("Status", SqlDbType.NVarChar, ParameterDirection.Input, "");
            unit.AddParameter("MemberId", SqlDbType.BigInt, ParameterDirection.Input, 0);
            unit.AddParameter("IsChecked", SqlDbType.VarChar, ParameterDirection.Input, "");
            unit.AddParameter("CommitteeId", SqlDbType.BigInt, ParameterDirection.Input, CommitteeId);
            unit.AddParameter("DCReferenceNumber", SqlDbType.VarChar, ParameterDirection.Input, "");
            unit.AddParameter("ComplaintDetailEncryptCode", SqlDbType.VarChar, ParameterDirection.Input, "");
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

        //public int InsertComplaintReply(ComplaintReply reply)
        //{
        //    int result = 0;
        //    try
        //    {
        //        ExecutionUnit unit = new ExecutionUnit("SP_Complaint_Reply_Mst");

        //        unit.AddParameter("Mode", SqlDbType.VarChar, ParameterDirection.Input, "Insert");
        //        unit.AddParameter("Id", SqlDbType.BigInt, ParameterDirection.Input, reply.Id);
        //        unit.AddParameter("ReceiverId", SqlDbType.BigInt, ParameterDirection.Input, reply.ReceiverId);
        //        unit.AddParameter("ReplyMessage", SqlDbType.NVarChar, ParameterDirection.Input, reply.ReplyMessage);
        //        unit.AddParameter("ComplaintId", SqlDbType.BigInt, ParameterDirection.Input, reply.ComplaintId);               
        //        unit.AddParameter("CreateUser", SqlDbType.BigInt, ParameterDirection.Input, reply.CreateUser);
        //        unit.AddParameter("SenderType", SqlDbType.NVarChar, ParameterDirection.Input, reply.SenderType);
        //        unit.AddParameter("ReceiverType", SqlDbType.NVarChar, ParameterDirection.Input, reply.ReceiverType);
        //        unit.AddParameter("FileName", SqlDbType.NVarChar, ParameterDirection.Input, reply.FileName);
        //        unit.AddParameter("FilePath", SqlDbType.NVarChar, ParameterDirection.Input, reply.FilePath);

        //        result = DatabaseAccessEngine.ExecuteNonQuery(unit);
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return result;
        //}

        public List<ComplaintReply> GetComplaintReplyList(Int64 SenderId, Int64 ComplaintId)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Complaint_Reply_Mst");

            unit.AddParameter("Id", SqlDbType.BigInt, ParameterDirection.Input, 0);
            unit.AddParameter("SenderId", SqlDbType.BigInt, ParameterDirection.Input, SenderId);
            unit.AddParameter("SenderType", SqlDbType.VarChar, ParameterDirection.Input, "");
            unit.AddParameter("Summery", SqlDbType.VarChar, ParameterDirection.Input, "");
            unit.AddParameter("Comprehensive", SqlDbType.VarChar, ParameterDirection.Input, "");
            unit.AddParameter("ComplaintId", SqlDbType.BigInt, ParameterDirection.Input, ComplaintId);
            unit.AddParameter("FileName", SqlDbType.VarChar, ParameterDirection.Input, "");
            unit.AddParameter("FilePath", SqlDbType.VarChar, ParameterDirection.Input, "");
            unit.AddParameter("CreateUser", SqlDbType.VarChar, ParameterDirection.Input, 0);
            unit.AddParameter("Mode", SqlDbType.VarChar, ParameterDirection.Input, "GetReplyListByComplaintId");

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<ComplaintReply> List = new List<ComplaintReply>();
                while (reader.Read())
                {
                    ComplaintReply list = new ComplaintReply();
                    list.Id = DatabaseAccessEngine.GetInt64(reader, "Id");
                    list.ComplaintId = DatabaseAccessEngine.GetInt64(reader, "ComplaintId");
                    list.Summery = DatabaseAccessEngine.GetString(reader, "Summery");
                    list.Comprehensive = DatabaseAccessEngine.GetString(reader, "Comprehensive");
                    list.FileName = DatabaseAccessEngine.GetString(reader, "FileName");
                    list.FilePath = DatabaseAccessEngine.GetString(reader, "FilePath");
                    list.SCreateDate = DatabaseAccessEngine.GetString(reader, "SCreateDate");
                    list.SCreateTime = DatabaseAccessEngine.GetString(reader, "SCreateTime");
                    list.SenderName = DatabaseAccessEngine.GetString(reader, "SenderName");
                    list.IsShowDeleteButton = DatabaseAccessEngine.GetString(reader, "IsShowDeleteButton");
                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public List<ComplaintAgainstDetail> ComplaintAgainstDetail(String EnrollmentNumber, String Mode)
        {
            try
            {
                ExecutionUnit unit = new ExecutionUnit("SP_Complaint_Against_Detail");

                unit.AddParameter("EnrollmentNumber", SqlDbType.VarChar, ParameterDirection.Input, EnrollmentNumber);
                unit.AddParameter("Mode", SqlDbType.VarChar, ParameterDirection.Input, Mode);

                IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

                try
                {
                    List<ComplaintAgainstDetail> List = new List<ComplaintAgainstDetail>();
                    while (reader.Read())
                    {
                        ComplaintAgainstDetail list = new ComplaintAgainstDetail();
                        list.EId = DatabaseAccessEngine.GetInt64(reader, "EId");
                        list.FullName = DatabaseAccessEngine.GetString(reader, "FullName");
                        list.CityName = DatabaseAccessEngine.GetString(reader, "CityName");
                        list.ContactNumber = DatabaseAccessEngine.GetString(reader, "ContactNumber");
                        List.Add(list);
                    }
                    return List;
                }
                finally
                {
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public List<ComplaintAgainstUpdate> ComplaintAgainstUpdate(Int64 Id, Int64 EId, String FullName, String CityName, String MobileNo, String Mode)
        {
            try
            {
                ExecutionUnit unit = new ExecutionUnit("SP_Complaint_Against_Update");

                unit.AddParameter("Id", SqlDbType.BigInt, ParameterDirection.Input, Id);
                unit.AddParameter("EId", SqlDbType.BigInt, ParameterDirection.Input, EId);
                unit.AddParameter("FullName", SqlDbType.VarChar, ParameterDirection.Input, FullName);
                unit.AddParameter("CityName", SqlDbType.VarChar, ParameterDirection.Input, CityName);
                unit.AddParameter("MobileNo", SqlDbType.VarChar, ParameterDirection.Input, MobileNo);
                unit.AddParameter("Mode", SqlDbType.VarChar, ParameterDirection.Input, Mode);

                IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

                try
                {
                    List<ComplaintAgainstUpdate> List = new List<ComplaintAgainstUpdate>();
                    while (reader.Read())
                    {
                        ComplaintAgainstUpdate list = new ComplaintAgainstUpdate();
                        list.ReturnStatus = DatabaseAccessEngine.GetString(reader, "ReturnStatus");
                        list.EnrollmentNumber = DatabaseAccessEngine.GetString(reader, "EnrollmentNumber");
                        List.Add(list);
                    }
                    return List;
                }
                finally
                {
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public List<ComplaintAgainstUpdate> ComplaintAgainstSave(Int64 ComplaintId, Int64 EId, Int64 CreateUser, String Mode)
        {
            try
            {
                ExecutionUnit unit = new ExecutionUnit("SP_Complaint_Against_Save");

                unit.AddParameter("ComplaintId", SqlDbType.BigInt, ParameterDirection.Input, ComplaintId);
                unit.AddParameter("EId", SqlDbType.BigInt, ParameterDirection.Input, EId);
                unit.AddParameter("CreateUser", SqlDbType.BigInt, ParameterDirection.Input, CreateUser);
                unit.AddParameter("Mode", SqlDbType.NVarChar, ParameterDirection.Input, Mode);

                IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

                try
                {
                    List<ComplaintAgainstUpdate> List = new List<ComplaintAgainstUpdate>();
                    while (reader.Read())
                    {
                        ComplaintAgainstUpdate list = new ComplaintAgainstUpdate();
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
            catch (Exception ex)
            {
                return null;
            }

        }

        public List<EnrollmentMstModel> GetAdvocateList(String SearchPara)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Enrollment_Details");
            unit.AddParameter("Mode", SqlDbType.NVarChar, ParameterDirection.Input, "SelectALL");
            unit.AddParameter("SearchPara", SqlDbType.NVarChar, ParameterDirection.Input, SearchPara);

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<EnrollmentMstModel> List = new List<EnrollmentMstModel>();
                while (reader.Read())
                {
                    EnrollmentMstModel list = new EnrollmentMstModel();
                    list.EId = DatabaseAccessEngine.GetInt64(reader, "EId");
                    list.EnroNumber = DatabaseAccessEngine.GetString(reader, "EnroNumber");
                    list.FullName = DatabaseAccessEngine.GetString(reader, "FullName");
                    list.ContactNumber = DatabaseAccessEngine.GetString(reader, "ContactNumber");
                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public List<EnrollmentMstModel> GetAdvocateSearchList(String SearchPara, int PageNo, int PageSize)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Search_Advocate_List");
            unit.AddParameter("SearchPara", SqlDbType.NVarChar, ParameterDirection.Input, string.IsNullOrEmpty(SearchPara) ? string.Empty : SearchPara);
            unit.AddParameter("PageNo", SqlDbType.Int, ParameterDirection.Input, PageNo);
            unit.AddParameter("PageSize", SqlDbType.Int, ParameterDirection.Input, PageSize);
            unit.AddParameter("Mode", SqlDbType.NVarChar, ParameterDirection.Input, "SearchAdvocate");

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<EnrollmentMstModel> List = new List<EnrollmentMstModel>();
                while (reader.Read())
                {
                    EnrollmentMstModel list = new EnrollmentMstModel();
                    list.EId = DatabaseAccessEngine.GetInt64(reader, "EId");
                    list.EnroNumber = DatabaseAccessEngine.GetString(reader, "EnroNumber");
                    list.FullName = DatabaseAccessEngine.GetString(reader, "FullName");
                    list.ContactNumber = DatabaseAccessEngine.GetString(reader, "ContactNumber");
                    list.CntGRNo = DatabaseAccessEngine.GetInt(reader, "TotalRecord");
                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public bool ShareComplaintWithMembersDynamic(Int64 complaintId, List<string> memberIds, Int64 userId)
        {
            bool isSuccess = false;
            try
            {
                foreach (var memberDataStr in memberIds)
                {
                    var parts = memberDataStr.Split('_');
                    Int64 memberId = Convert.ToInt64(parts[0]);
                    string flag = parts.Length > 1 ? parts[1] : "A";
                    
                    ExecutionUnit unit = new ExecutionUnit("SP_Complaint_Share_Member_Log_Insert");
                    unit.AddParameter("ComplaintId", SqlDbType.BigInt, ParameterDirection.Input, complaintId);
                    unit.AddParameter("MemberId", SqlDbType.BigInt, ParameterDirection.Input, memberId);
                    unit.AddParameter("Flag", SqlDbType.NChar, ParameterDirection.Input, flag);
                    unit.AddParameter("CreateUser", SqlDbType.BigInt, ParameterDirection.Input, userId);

                    DatabaseAccessEngine.ExecuteNonQuery(unit);
                }
                isSuccess = true;
            }
            catch (Exception)
            {
                isSuccess = false;
            }
            return isSuccess;
        }

        public List<DcMemberList> GetComplaintShareMembers(Int64 complaintId)
        {
            List<DcMemberList> members = GetDcMemberListNotExist(0);
            Dictionary<Int64, string> memberFlags = new Dictionary<Int64, string>();

            ExecutionUnit unit = new ExecutionUnit("SP_Get_Complaint_Share_Member_Log");
            unit.AddParameter("ComplaintId", SqlDbType.BigInt, ParameterDirection.Input, complaintId);

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);
            try
            {
                while (reader.Read())
                {
                    Int64 mId = DatabaseAccessEngine.GetInt64(reader, "MemberId");
                    string flag = DatabaseAccessEngine.GetString(reader, "Flag");
                    memberFlags[mId] = flag;
                }
            }
            finally
            {
                if (!reader.IsClosed)
                {
                    reader.Close();
                }
            }

            foreach (var m in members)
            {
                if (memberFlags.ContainsKey(m.MemberId))
                {
                    m.Flag = memberFlags[m.MemberId];
                }
                else
                {
                    m.Flag = null; // No record found
                }
            }
            
            return members;
        }

        public int UpdateComplaintMemberEmail(Int64 memberLogId, string emailAddress)
        {
            int result = 0;
            try
            {
                ExecutionUnit unit = new ExecutionUnit("SP_Complaint_Member_Email_Update");
                unit.AddParameter("MemberLogId", SqlDbType.BigInt, ParameterDirection.Input, memberLogId);
                unit.AddParameter("EmailAddress", SqlDbType.NVarChar, ParameterDirection.Input, emailAddress);
                result = DatabaseAccessEngine.ExecuteNonQuery(unit);
            }
            catch (Exception ex)
            {
            }
            return result;
        }
    }
}

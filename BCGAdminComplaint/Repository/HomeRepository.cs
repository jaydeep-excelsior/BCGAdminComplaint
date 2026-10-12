using BCGAdminComplaint.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCGAdminComplaint.Repository
{
    public class HomeRepository
    {
        public List<ComplaintUserLoginReturn> GetComplaintUserLogin(String UserName, String Password, String PCIPAddress, String Mode)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Complaint_User_Login");
            unit.AddParameter("UserName", SqlDbType.NVarChar, ParameterDirection.Input, UserName);
            unit.AddParameter("Password", SqlDbType.NVarChar, ParameterDirection.Input, Password);
            unit.AddParameter("PCIPAddress", SqlDbType.NVarChar, ParameterDirection.Input, PCIPAddress);
            unit.AddParameter("Mode", SqlDbType.NVarChar, ParameterDirection.Input, Mode);

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<ComplaintUserLoginReturn> List = new List<ComplaintUserLoginReturn>();
                while (reader.Read())
                {
                    ComplaintUserLoginReturn list = new ComplaintUserLoginReturn();
                    list.ReturnStatus = DatabaseAccessEngine.GetString(reader, "ReturnStatus");
                    list.RegisterId = DatabaseAccessEngine.GetString(reader, "RegisterId");
                    list.UserName = DatabaseAccessEngine.GetString(reader, "UserName");
                    list.IsDcAccess = DatabaseAccessEngine.GetString(reader, "IsDcAccess");
                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public List<ComplaintCount> GetComplaintcount(String FilterValue, String FromDate, String ToDate)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Dashboard");

            unit.AddParameter("FilterValue", SqlDbType.NVarChar, ParameterDirection.Input, FilterValue);
            unit.AddParameter("FromDate", SqlDbType.NVarChar, ParameterDirection.Input, FromDate);
            unit.AddParameter("ToDate", SqlDbType.NVarChar, ParameterDirection.Input, ToDate);
            unit.AddParameter("Mode", SqlDbType.NVarChar, ParameterDirection.Input, "GetComplaintCount");

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<ComplaintCount> List = new List<ComplaintCount>();
                while (reader.Read())
                {
                    ComplaintCount list = new ComplaintCount();
                    list.TotalCount = DatabaseAccessEngine.GetInt(reader, "TotalCount");
                    list.NewStatusCount = DatabaseAccessEngine.GetInt(reader, "NewStatusCount");
                    list.HearingStatusCount = DatabaseAccessEngine.GetInt(reader, "HearingStatusCount");
                    list.AdjuntStatusCount = DatabaseAccessEngine.GetInt(reader, "AdjuntStatusCount");
                    list.ApprovedStatusCount = DatabaseAccessEngine.GetInt(reader, "ApprovedStatusCount");
                    list.RejectedStatusCount = DatabaseAccessEngine.GetInt(reader, "RejectedStatusCount");
                    list.AcceptedStatusCount = DatabaseAccessEngine.GetInt(reader, "AcceptedStatusCount");
                    list.OrderPassedStatusCount = DatabaseAccessEngine.GetInt(reader, "OrderPassedStatusCount");
                    List.Add(list);
                }
                return List;
            }
            finally
            {
                reader.Close();
            }
        }

        public List<ComplaintListModel> GetComplaintList(String FilterValue, String FromDate, String ToDate)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Dashboard");

            unit.AddParameter("FilterValue", SqlDbType.NVarChar, ParameterDirection.Input, FilterValue);
            unit.AddParameter("FromDate", SqlDbType.NVarChar, ParameterDirection.Input, FromDate);
            unit.AddParameter("ToDate", SqlDbType.NVarChar, ParameterDirection.Input, ToDate);
            unit.AddParameter("Mode", SqlDbType.NVarChar, ParameterDirection.Input, "GetComplaintList");

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
                    list.Status = DatabaseAccessEngine.GetString(reader, "Status");
                    list.CreateDate = DatabaseAccessEngine.GetDateTime(reader, "CreateDate");
                    list.SCreateDate = DatabaseAccessEngine.GetString(reader, "SCreateDate");
                    list.ComplaintNumber = DatabaseAccessEngine.GetString(reader, "ComplaintNumber");
                    list.ComplaintsCreateDate = DatabaseAccessEngine.GetString(reader, "ComplaintsCreateDate");
                    list.PaymentStatus = DatabaseAccessEngine.GetString(reader, "PaymentStatus");
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

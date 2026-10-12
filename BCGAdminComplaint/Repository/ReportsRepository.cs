using BCGAdminComplaint.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCGAdminComplaint.Repository
{
    public class ReportsRepository
    {
        public List<ComplaintStatusLog> GetComplaintLogListReport(String Search,String Status,String OldStatus,String FromDate,String ToDate,String Committee)
        {
            ExecutionUnit unit = new ExecutionUnit("SP_Complaint_Status_Log");

            unit.AddParameter("Search", SqlDbType.VarChar, ParameterDirection.Input, Search);
            unit.AddParameter("Status", SqlDbType.VarChar, ParameterDirection.Input, Status);
            unit.AddParameter("OldStatus", SqlDbType.VarChar, ParameterDirection.Input, OldStatus);
            unit.AddParameter("FromDate", SqlDbType.VarChar, ParameterDirection.Input, FromDate);
            unit.AddParameter("ToDate", SqlDbType.VarChar, ParameterDirection.Input, ToDate);
            unit.AddParameter("Committee", SqlDbType.VarChar, ParameterDirection.Input, Committee);
            unit.AddParameter("Mode", SqlDbType.VarChar, ParameterDirection.Input, "LogListReport");

            IDataReader reader = DatabaseAccessEngine.ExecuteDataReader(unit);

            try
            {
                List<ComplaintStatusLog> List = new List<ComplaintStatusLog>();
                while (reader.Read())
                {
                    ComplaintStatusLog list = new ComplaintStatusLog();
                    list.CommitteTitle = DatabaseAccessEngine.GetString(reader, "CommitteTitle");
                    list.NextDate = DatabaseAccessEngine.GetDateTime(reader, "NextDate");
                    list.Summary = DatabaseAccessEngine.GetString(reader, "Summary");
                    list.OrderFileName = DatabaseAccessEngine.GetString(reader, "OrderFileName");
                    list.OrderFilePath = DatabaseAccessEngine.GetString(reader, "OrderFilePath");
                    list.Status = DatabaseAccessEngine.GetString(reader, "Status");
                    list.OldStatus = DatabaseAccessEngine.GetString(reader, "OldStatus");
                    list.SNextDate = DatabaseAccessEngine.GetString(reader, "SNextDate");
                    list.FullName = DatabaseAccessEngine.GetString(reader, "FullName");
                    list.ComplaintNumber = DatabaseAccessEngine.GetString(reader, "ComplaintNumber");
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

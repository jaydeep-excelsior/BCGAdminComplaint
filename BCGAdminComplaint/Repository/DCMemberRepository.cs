using BCGAdminComplaint.Models.DCMember;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace BCGAdminComplaint.Repository
{
    public class DCMemberRepository
    {
        private readonly string _connectionString;

        public DCMemberRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        }

        public List<DCMemberSave> DCMemberInsert(Int64 Id, Int64 EnrollmentId, String EmailAddress, String Password, String Flag, Int64 CreateUser, String ActionMode, String Name, String ContactNo, String BarAssociation, String MemberType = "")
        {
            List<DCMemberSave> ulr = new List<DCMemberSave>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_DCMember_Save", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", Id);
                    cmd.Parameters.AddWithValue("@EnrollmentId", EnrollmentId);
                    cmd.Parameters.AddWithValue("@EmailAddress", EmailAddress);
                    cmd.Parameters.AddWithValue("@Password", Password);
                    cmd.Parameters.AddWithValue("@Flag", Flag);
                    cmd.Parameters.AddWithValue("@CreateUser", CreateUser);
                    cmd.Parameters.AddWithValue("@ActionMode", ActionMode);
                    cmd.Parameters.AddWithValue("@Name", Name);
                    cmd.Parameters.AddWithValue("@ContactNo", ContactNo);
                    cmd.Parameters.AddWithValue("@BarAssociation", BarAssociation);
                    cmd.Parameters.AddWithValue("@MemberType", (object)MemberType ?? DBNull.Value);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ulr.Add(new DCMemberSave
                            {
                                ReturnStatus = reader["ReturnStatus"] == null ? "0" : reader["ReturnStatus"].ToString(),
                                Id = reader["Id"] == null ? "0" : reader["Id"].ToString() == "" ? "0" : reader["Id"].ToString()
                            });
                        }
                    }
                }
            }
            return ulr;
        }

        public List<DCMemberSave> DCMemberStatusUpdate(Int64 Id, String Flag, Int64 CreateUser, String ActionMode)
        {
            List<DCMemberSave> ulr = new List<DCMemberSave>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_DCMember_Save", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", Id);
                    cmd.Parameters.AddWithValue("@EnrollmentId", 0);
                    cmd.Parameters.AddWithValue("@EmailAddress", "");
                    cmd.Parameters.AddWithValue("@Password", "");
                    cmd.Parameters.AddWithValue("@Flag", Flag);
                    cmd.Parameters.AddWithValue("@CreateUser", CreateUser);
                    cmd.Parameters.AddWithValue("@ActionMode", ActionMode);
                    cmd.Parameters.AddWithValue("@Name", "");
                    cmd.Parameters.AddWithValue("@ContactNo", "");
                    cmd.Parameters.AddWithValue("@BarAssociation", "");
                    cmd.Parameters.AddWithValue("@MemberType", "");
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ulr.Add(new DCMemberSave
                            {
                                ReturnStatus = reader["ReturnStatus"] == null ? "0" : reader["ReturnStatus"].ToString(),
                                Id = reader["Id"] == null ? "0" : reader["Id"].ToString() == "" ? "0" : reader["Id"].ToString()
                            });
                        }
                    }
                }
            }
            return ulr;
        }

        public List<DCMemberList> DCMembersList(Int64 Id, String SearchValue, String FromDate, String ToDate, String RecordStatus, int PageNo, int PageSize, Int64 LoginUserId, Int64 SubMenuId, String SortBy, String ActionMode, String MemberType = "")
        {
            List<DCMemberList> ulr = new List<DCMemberList>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_DCMember_List", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", Id);
                    cmd.Parameters.AddWithValue("@SearchValue", SearchValue);
                    cmd.Parameters.AddWithValue("@FromDate", FromDate);
                    cmd.Parameters.AddWithValue("@ToDate", ToDate);
                    cmd.Parameters.AddWithValue("@RecordStatus", RecordStatus);
                    cmd.Parameters.AddWithValue("@PageNo", PageNo);
                    cmd.Parameters.AddWithValue("@PageSize", PageSize);
                    cmd.Parameters.AddWithValue("@LoginUserId", LoginUserId);
                    cmd.Parameters.AddWithValue("@SubMenuId", SubMenuId);
                    cmd.Parameters.AddWithValue("@SortBy", SortBy);
                    cmd.Parameters.AddWithValue("@ActionMode", ActionMode);
                    cmd.Parameters.AddWithValue("@MemberType", MemberType ?? "");
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        var columns = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();
                        while (reader.Read())
                        {
                            ulr.Add(new DCMemberList
                            {
                                SrNo = reader["SrNo"] == null ? "0" : reader["SrNo"].ToString() == "" ? "0" : reader["SrNo"].ToString(),
                                TotalRecord = reader["TotalRecord"] == null ? "0" : reader["TotalRecord"].ToString() == "" ? "0" : reader["TotalRecord"].ToString(),
                                Id = Convert.ToInt64(reader["Id"] == null ? "0" : reader["Id"].ToString() == "" ? "0" : reader["Id"].ToString()),
                                EnrollmentId = Convert.ToInt64(reader["EnrollmentId"] == null ? "0" : reader["EnrollmentId"].ToString() == "" ? "0" : reader["EnrollmentId"].ToString()),
                                EnroNumber = reader["EnroNumber"] == null ? "" : reader["EnroNumber"].ToString(),
                                FullName = reader["FullName"] == null ? "" : reader["FullName"].ToString(),
                                MemberType = columns.Contains("MemberType") ? (reader["MemberType"] == null ? "" : reader["MemberType"].ToString()) : "",
                                ContactNumber = reader["ContactNumber"] == null ? "" : reader["ContactNumber"].ToString(),
                                EmailAddress = reader["EmailAddress"] == null ? "" : reader["EmailAddress"].ToString(),
                                Password = reader["Password"] == null ? "" : reader["Password"].ToString(),
                                Flag = reader["Flag"] == null ? "" : reader["Flag"].ToString(),
                                EditRights = columns.Contains("EditRights") ? (reader["EditRights"] == null ? "" : reader["EditRights"].ToString()) : "A",
                                DeleteRights = columns.Contains("DeleteRights") ? (reader["DeleteRights"] == null ? "" : reader["DeleteRights"].ToString()) : "A",
                                CreateBy = reader["CreateBy"] == null ? "" : reader["CreateBy"].ToString(),
                                LastModifyBy = reader["LastModifyBy"] == null ? "" : reader["LastModifyBy"].ToString(),
                                SCreateDate = reader["SCreateDate"] == null ? "" : reader["SCreateDate"].ToString(),
                                SCreateTime = reader["SCreateTime"] == null ? "" : reader["SCreateTime"].ToString(),
                                LastModifyDate = reader["LastModifyDate"] == null ? "" : reader["LastModifyDate"].ToString(),
                                LastModifyTime = reader["LastModifyTime"] == null ? "" : reader["LastModifyTime"].ToString()
                            });
                        }
                    }
                }
            }
            return ulr;
        }

        public List<DCMemberEdit> DCMembersEdit(Int64 Id, String ActionMode)
        {
            List<DCMemberEdit> ulr = new List<DCMemberEdit>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_DCMember_List", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", Id);
                    cmd.Parameters.AddWithValue("@SearchValue", "");
                    cmd.Parameters.AddWithValue("@FromDate", "");
                    cmd.Parameters.AddWithValue("@ToDate", "");
                    cmd.Parameters.AddWithValue("@RecordStatus", "");
                    cmd.Parameters.AddWithValue("@PageNo", 0);
                    cmd.Parameters.AddWithValue("@PageSize", 0);
                    cmd.Parameters.AddWithValue("@LoginUserId", 0);
                    cmd.Parameters.AddWithValue("@SubMenuId", 0);
                    cmd.Parameters.AddWithValue("@SortBy", "");
                    cmd.Parameters.AddWithValue("@ActionMode", ActionMode);
                    cmd.Parameters.AddWithValue("@MemberType", "");
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        var columns = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();
                        while (reader.Read())
                        {
                            ulr.Add(new DCMemberEdit
                            {
                                Id = Convert.ToInt64(reader["Id"] == null ? "0" : reader["Id"].ToString() == "" ? "0" : reader["Id"].ToString()),
                                EnrollmentId = Convert.ToInt64(reader["EnrollmentId"] == null ? "0" : reader["EnrollmentId"].ToString() == "" ? "0" : reader["EnrollmentId"].ToString()),
                                EmailAddress = reader["EmailAddress"] == null ? "" : reader["EmailAddress"].ToString(),
                                Password = reader["Password"] == null ? "" : reader["Password"].ToString(),
                                Name = reader["Name"] == null ? "" : reader["Name"].ToString(),
                                ContactNo = reader["ContactNo"] == null ? "" : reader["ContactNo"].ToString(),
                                BarAssociation = reader["BarAssociation"] == null ? "" : reader["BarAssociation"].ToString(),
                                EnroNumber = reader["EnroNumber"] == null ? "" : reader["EnroNumber"].ToString(),
                                MemberType = columns.Contains("MemberType") ? (reader["MemberType"] == null ? "" : reader["MemberType"].ToString()) : "",
                            });
                        }
                    }
                }
            }
            return ulr;
        }

        public DCMemberEnrollmentDetails GetEnrollmentDetails(string enroNumber)
        {
            DCMemberEnrollmentDetails details = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetEnrollmentDetails_ByEnroNumber", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@EnroNumber", enroNumber);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            details = new DCMemberEnrollmentDetails
                            {
                                EnrollmentId = Convert.ToInt64(reader["EnrollmentId"] == null ? "0" : reader["EnrollmentId"].ToString() == "" ? "0" : reader["EnrollmentId"].ToString()),
                                FullName = reader["FullName"] == null ? "" : reader["FullName"].ToString(),
                                ContactNumber = reader["ContactNumber"] == null ? "" : reader["ContactNumber"].ToString(),
                                EmailAddress = reader["EmailAddress"] == null ? "" : reader["EmailAddress"].ToString(),
                                BarAssociation = reader["BarAssociation"] == null ? "" : reader["BarAssociation"].ToString()
                            };
                        }
                    }
                }
            }
            return details;
        }

        public List<BCGStaffModel> GetBCGStaffList()
        {
            List<BCGStaffModel> list = new List<BCGStaffModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT RegisterId, 
                           RTRIM(LTRIM(ISNULL(FirstName,'') + ' ' + ISNULL(MiddleName,'') + ' ' + ISNULL(Lastname,''))) + ' (' + ISNULL(UserName,'') + ')' AS DisplayText,
                           RTRIM(LTRIM(ISNULL(FirstName,'') + ' ' + ISNULL(MiddleName,'') + ' ' + ISNULL(Lastname,''))) AS FullName,
                           ISNULL(UserName,'') AS UserName,
                           CASE WHEN ISNULL(Mobile, 0) = 0 THEN '' ELSE CAST(Mobile AS NVARCHAR(50)) END AS ContactNumber,
                           ISNULL(Email,'') AS EmailAddress
                    FROM tbl_register_mst 
                    WHERE Flag = 'A' 
                    ORDER BY FirstName, Lastname";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new BCGStaffModel
                            {
                                RegisterId = Convert.ToInt64(reader["RegisterId"] == null ? "0" : reader["RegisterId"].ToString()),
                                DisplayText = reader["DisplayText"] == null ? "" : reader["DisplayText"].ToString(),
                                FullName = reader["FullName"] == null ? "" : reader["FullName"].ToString(),
                                UserName = reader["UserName"] == null ? "" : reader["UserName"].ToString(),
                                ContactNumber = reader["ContactNumber"] == null ? "" : reader["ContactNumber"].ToString(),
                                EmailAddress = reader["EmailAddress"] == null ? "" : reader["EmailAddress"].ToString()
                            });
                        }
                    }
                }
            }
            return list;
        }

        public BCGStaffModel GetBCGStaffDetails(Int64 registerId)
        {
            BCGStaffModel details = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT RegisterId, 
                           RTRIM(LTRIM(ISNULL(FirstName,'') + ' ' + ISNULL(MiddleName,'') + ' ' + ISNULL(Lastname,''))) + ' (' + ISNULL(UserName,'') + ')' AS DisplayText,
                           RTRIM(LTRIM(ISNULL(FirstName,'') + ' ' + ISNULL(MiddleName,'') + ' ' + ISNULL(Lastname,''))) AS FullName,
                           ISNULL(UserName,'') AS UserName,
                           CASE WHEN ISNULL(Mobile, 0) = 0 THEN '' ELSE CAST(Mobile AS NVARCHAR(50)) END AS ContactNumber,
                           ISNULL(Email,'') AS EmailAddress
                    FROM tbl_register_mst 
                    WHERE RegisterId = @RegisterId";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@RegisterId", registerId);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            details = new BCGStaffModel
                            {
                                RegisterId = Convert.ToInt64(reader["RegisterId"] == null ? "0" : reader["RegisterId"].ToString()),
                                DisplayText = reader["DisplayText"] == null ? "" : reader["DisplayText"].ToString(),
                                FullName = reader["FullName"] == null ? "" : reader["FullName"].ToString(),
                                UserName = reader["UserName"] == null ? "" : reader["UserName"].ToString(),
                                ContactNumber = reader["ContactNumber"] == null ? "" : reader["ContactNumber"].ToString(),
                                EmailAddress = reader["EmailAddress"] == null ? "" : reader["EmailAddress"].ToString()
                            };
                        }
                    }
                }
            }
            return details;
        }
    }
}

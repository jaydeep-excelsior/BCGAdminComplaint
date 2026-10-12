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

        public List<DCMemberSave> DCMemberInsert(Int64 Id, Int64 EnrollmentId, String EmailAddress, String Password, String Flag, Int64 CreateUser, String ActionMode, String Name, String ContactNo, String BarAssociation)
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

        public List<DCMemberList> DCMembersList(Int64 Id, String SearchValue, String FromDate, String ToDate, String RecordStatus, int PageNo, int PageSize, Int64 LoginUserId, Int64 SubMenuId, String SortBy, String ActionMode)
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
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
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
    }
}

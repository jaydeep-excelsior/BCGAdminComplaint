using BCGAdminComplaint.Models.DCCommittee;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace BCGAdminComplaint.Repository
{
    public class DCCommitteeRepository
    {
        private readonly string _connectionString;

        public DCCommitteeRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        }

        public List<DCCommitteeSave> DCCommitteeInsert(Int64 Id, String CommitteeTitle, Int64 DcCommitteeNo, String Note, String Flag, Int64 CreateUser, String ActionMode)
        {
            List<DCCommitteeSave> ulr = new List<DCCommitteeSave>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_DcCommittee_Mst", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", Id);
                    cmd.Parameters.AddWithValue("@DcCommitteeTitle", CommitteeTitle);
                    cmd.Parameters.AddWithValue("@Note", Note);
                    cmd.Parameters.AddWithValue("@Mode", "InsertUpdate");
                    cmd.Parameters.AddWithValue("@ShowRecord", 0);
                    cmd.Parameters.AddWithValue("@SearchTitle", "");
                    cmd.Parameters.AddWithValue("@SNOStart", 0);
                    cmd.Parameters.AddWithValue("@SNOEnd", 0);
                    cmd.Parameters.AddWithValue("@RoleId", 0);
                    cmd.Parameters.AddWithValue("@RightsId", 0);
                    cmd.Parameters.AddWithValue("@Flag", Flag);

                    // Add output parameters
                    SqlParameter intOutput = new SqlParameter("@intOutput", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Output };
                    SqlParameter numRowsChanged = new SqlParameter("@NumRowsChanged", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Output };
                    SqlParameter errorCode = new SqlParameter("@ErrorCode", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Output };
                    
                    cmd.Parameters.Add(intOutput);
                    cmd.Parameters.Add(numRowsChanged);
                    cmd.Parameters.Add(errorCode);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    string outputId = intOutput.Value?.ToString() ?? "0";
                    string rowsChanged = numRowsChanged.Value?.ToString() ?? "0";

                    // intOutput has the new Id (@@IDENTITY) for insert, or the existing Id for update
                    // Determine status: if rows changed > 0, it's a success
                    string status = "ER";
                    if (rowsChanged != "0" && !string.IsNullOrEmpty(outputId) && outputId != "0")
                    {
                        status = "IN";
                    }

                    ulr.Add(new DCCommitteeSave
                    {
                        ReturnStatus = status,
                        Id = outputId
                    });
                }
            }
            return ulr;
        }

        public List<DCCommitteeSave> DCCommitteeMemberInsert(Int64 Id, Int64 DcCommitteeId, Int64 MemberId, String ActionMode)
        {
            List<DCCommitteeSave> ulr = new List<DCCommitteeSave>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_DC_Commitee_Details_Mst", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", Id);
                    cmd.Parameters.AddWithValue("@DcCommitteeId", DcCommitteeId);
                    cmd.Parameters.AddWithValue("@MemberId", MemberId);
                    cmd.Parameters.AddWithValue("@Mode", ActionMode); // e.g. "InsertUpdate" or "Delete"

                    SqlParameter intOutput = new SqlParameter("@intOutput", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Output };
                    SqlParameter numRowsChanged = new SqlParameter("@NumRowsChanged", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Output };
                    SqlParameter errorCode = new SqlParameter("@ErrorCode", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Output };
                    
                    cmd.Parameters.Add(intOutput);
                    cmd.Parameters.Add(numRowsChanged);
                    cmd.Parameters.Add(errorCode);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    ulr.Add(new DCCommitteeSave
                    {
                        ReturnStatus = intOutput.Value?.ToString() ?? "ER",
                        Id = numRowsChanged.Value?.ToString() ?? "0"
                    });
                }
            }
            return ulr;
        }

        public List<DCCommitteeSave> DCCommitteeStatusUpdate(Int64 Id, String Flag)
        {
            List<DCCommitteeSave> ulr = new List<DCCommitteeSave>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_DcCommittee_Mst", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", Id);
                    cmd.Parameters.AddWithValue("@DcCommitteeTitle", "");
                    cmd.Parameters.AddWithValue("@Note", "");
                    cmd.Parameters.AddWithValue("@Flag", Flag);
                    cmd.Parameters.AddWithValue("@Mode", "Delete");
                    cmd.Parameters.AddWithValue("@ShowRecord", 0);
                    cmd.Parameters.AddWithValue("@SearchTitle", "");
                    cmd.Parameters.AddWithValue("@SNOStart", 0);
                    cmd.Parameters.AddWithValue("@SNOEnd", 0);
                    cmd.Parameters.AddWithValue("@RoleId", 0);
                    cmd.Parameters.AddWithValue("@RightsId", 0);

                    SqlParameter intOutput = new SqlParameter("@intOutput", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Output };
                    SqlParameter numRowsChanged = new SqlParameter("@NumRowsChanged", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Output };
                    SqlParameter errorCode = new SqlParameter("@ErrorCode", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Output };
                    
                    cmd.Parameters.Add(intOutput);
                    cmd.Parameters.Add(numRowsChanged);
                    cmd.Parameters.Add(errorCode);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    ulr.Add(new DCCommitteeSave
                    {
                        ReturnStatus = intOutput.Value?.ToString() ?? "ER",
                        Id = numRowsChanged.Value?.ToString() ?? "0"
                    });
                }
            }
            return ulr;
        }

        public List<DCCommitteeList> DCCommitteesList(Int64 Id, String SearchValue, int PageNo, int PageSize, Int64 LoginUserId, Int64 SubMenuId, String SortBy, String RecordStatus)
        {
            List<DCCommitteeList> ulr = new List<DCCommitteeList>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_DcCommittee_Mst", conn))
                {
                    int startNo = ((PageNo - 1) * PageSize) + 1;
                    int endNo = PageNo * PageSize;

                    string mode = "SelectAll";
                    string flag = "";
                    if (RecordStatus == "A")
                    {
                        mode = "ActiveSelectAll";
                        flag = "A";
                    }
                    else if (RecordStatus == "D")
                    {
                        mode = "DeactiveSelectAll";
                        flag = "D";
                    }

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", Id);
                    cmd.Parameters.AddWithValue("@DcCommitteeTitle", "");
                    cmd.Parameters.AddWithValue("@Note", "");
                    cmd.Parameters.AddWithValue("@Flag", flag);
                    cmd.Parameters.AddWithValue("@Mode", mode);
                    cmd.Parameters.AddWithValue("@ShowRecord", PageSize);
                    cmd.Parameters.AddWithValue("@SearchTitle", string.IsNullOrEmpty(SearchValue) ? "NA" : SearchValue);
                    cmd.Parameters.AddWithValue("@SNOStart", startNo);
                    cmd.Parameters.AddWithValue("@SNOEnd", endNo);
                    cmd.Parameters.AddWithValue("@RoleId", 1); // Mock RoleId
                    cmd.Parameters.AddWithValue("@RightsId", SubMenuId);

                    SqlParameter intOutput = new SqlParameter("@intOutput", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Output };
                    SqlParameter numRowsChanged = new SqlParameter("@NumRowsChanged", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Output };
                    SqlParameter errorCode = new SqlParameter("@ErrorCode", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Output };
                    
                    cmd.Parameters.Add(intOutput);
                    cmd.Parameters.Add(numRowsChanged);
                    cmd.Parameters.Add(errorCode);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ulr.Add(new DCCommitteeList
                            {
                                SrNo = reader["SNO"] == null ? "0" : reader["SNO"].ToString() == "" ? "0" : reader["SNO"].ToString(),
                                Id = Convert.ToInt64(reader["Id"] == null ? "0" : reader["Id"].ToString() == "" ? "0" : reader["Id"].ToString()),
                                DcCommitteeNo = reader["DcCommitteeNo"] != System.DBNull.Value ? System.Convert.ToInt64(reader["DcCommitteeNo"]) : 0,
                                CommitteeTitle = reader["Title"] == null ? "" : reader["Title"].ToString(),
                                Note = reader["Note"] == null ? "" : reader["Note"].ToString(),
                                Flag = reader["Flag"] == null ? "" : reader["Flag"].ToString(),
                                EditRights = reader["UpdateStatus"] == null ? "" : reader["UpdateStatus"].ToString(),
                                DeleteRights = reader["DeleteStatus"] == null ? "" : reader["DeleteStatus"].ToString(),
                                TotalRecord = "0",
                                SCreateDate = reader["CreateDate"] != System.DBNull.Value ? System.Convert.ToDateTime(reader["CreateDate"]).ToString("dd-MM-yyyy") : "",
                                SCreateTime = reader["CreateDate"] != System.DBNull.Value ? System.Convert.ToDateTime(reader["CreateDate"]).ToString("hh:mm tt") : "",
                                LastModifyDate = reader["UpdateDate"] != System.DBNull.Value ? System.Convert.ToDateTime(reader["UpdateDate"]).ToString("dd-MM-yyyy") : "",
                                LastModifyTime = reader["UpdateDate"] != System.DBNull.Value ? System.Convert.ToDateTime(reader["UpdateDate"]).ToString("hh:mm tt") : "",
                                CreateBy = "Admin",
                                LastModifyBy = reader["UpdateDate"] != System.DBNull.Value ? "Admin" : "-",
                                TotalMember = reader["TotalMember"] != System.DBNull.Value ? Convert.ToInt64(reader["TotalMember"]) : 0
                            });
                        }
                        
                        if (reader.NextResult())
                        {
                            if (reader.Read() && ulr.Count > 0)
                            {
                                string totalRec = reader["TotalRecord"]?.ToString() ?? "0";
                                foreach (var item in ulr) item.TotalRecord = totalRec;
                            }
                        }
                    }
                }
            }
            return ulr;
        }

        public List<DCCommitteeEdit> DCCommitteesEdit(Int64 Id)
        {
            List<DCCommitteeEdit> ulr = new List<DCCommitteeEdit>();
            var all = DCCommitteesList(0, "", 1, 9999, 0, 0,"", "");
            var item = all.Find(x => x.Id == Id);
            if (item != null)
            {
                ulr.Add(new DCCommitteeEdit
                {
                    Id = item.Id,
                    CommitteeTitle = item.CommitteeTitle,
                    DcCommitteeNo = item.DcCommitteeNo,
                    Note = item.Note
                });
            }
            return ulr;
        }

        public List<DCCommitteeMemberEdit> DCCommitteeMembersEdit(Int64 Id)
        {
            List<DCCommitteeMemberEdit> ulr = new List<DCCommitteeMemberEdit>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_DcCommittee_Mst", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", Id);
                    cmd.Parameters.AddWithValue("@DcCommitteeTitle", "");
                    cmd.Parameters.AddWithValue("@Note", "");
                    cmd.Parameters.AddWithValue("@Flag", "");
                    cmd.Parameters.AddWithValue("@Mode", "SelectById"); // Mode in SP_DcCommittee_Mst to get members
                    cmd.Parameters.AddWithValue("@ShowRecord", 0);
                    cmd.Parameters.AddWithValue("@SearchTitle", "");
                    cmd.Parameters.AddWithValue("@SNOStart", 0);
                    cmd.Parameters.AddWithValue("@SNOEnd", 0);
                    cmd.Parameters.AddWithValue("@RoleId", 0);
                    cmd.Parameters.AddWithValue("@RightsId", 0);

                    SqlParameter intOutput = new SqlParameter("@intOutput", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Output };
                    SqlParameter numRowsChanged = new SqlParameter("@NumRowsChanged", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Output };
                    SqlParameter errorCode = new SqlParameter("@ErrorCode", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Output };
                    
                    cmd.Parameters.Add(intOutput);
                    cmd.Parameters.Add(numRowsChanged);
                    cmd.Parameters.Add(errorCode);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ulr.Add(new DCCommitteeMemberEdit
                            {
                                Id = Id,
                                DC_ComplaintId = 0,
                                MemberId = Convert.ToInt64(reader["MemberId"] == null ? "0" : reader["MemberId"].ToString()),
                                MemberName = reader["FullName"] == null ? "" : reader["FullName"].ToString()
                            });
                        }
                    }
                }
            }
            return ulr;
        }

        public List<Dictionary<string, string>> GetCommitteeMembers(Int64 committeeId)
        {
            var members = new List<Dictionary<string, string>>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_DcCommittee_Mst", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", committeeId);
                    cmd.Parameters.AddWithValue("@DcCommitteeTitle", "");
                    cmd.Parameters.AddWithValue("@Note", "");
                    cmd.Parameters.AddWithValue("@Mode", "GetDetails");
                    cmd.Parameters.AddWithValue("@ShowRecord", 0);
                    cmd.Parameters.AddWithValue("@SearchTitle", "NA");
                    cmd.Parameters.AddWithValue("@SNOStart", 0);
                    cmd.Parameters.AddWithValue("@SNOEnd", 0);
                    cmd.Parameters.AddWithValue("@RoleId", 0);
                    cmd.Parameters.AddWithValue("@RightsId", 0);
                    cmd.Parameters.AddWithValue("@Flag", "");

                    SqlParameter intOutput = new SqlParameter("@intOutput", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Output };
                    SqlParameter numRowsChanged = new SqlParameter("@NumRowsChanged", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Output };
                    SqlParameter errorCode = new SqlParameter("@ErrorCode", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(intOutput);
                    cmd.Parameters.Add(numRowsChanged);
                    cmd.Parameters.Add(errorCode);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var member = new Dictionary<string, string>();
                            member["MemberId"] = reader["memberid"]?.ToString() ?? "0";
                            member["FullName"] = reader["Fullname"]?.ToString() ?? "";
                            member["EnroNumber"] = reader["EnroNumber"]?.ToString() ?? "-";
                            member["ContactNumber"] = reader["ContactNumber"]?.ToString() ?? "-";
                            member["EmailAddress"] = reader["EmailAddress"]?.ToString() ?? "-";
                            members.Add(member);
                        }
                    }
                }
            }
            return members;
        }
    }
}

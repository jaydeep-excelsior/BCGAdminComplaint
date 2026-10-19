using BCGAdminComplaint.Models.Notes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace BCGAdminComplaint.Repository
{
    public class NotesRepository
    {
        private readonly string _connectionString;

        public NotesRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        }

        public List<AdminComplaintNotesModel> GetNotesList(long loginUserId, string searchValue, string noteType, string fromDate, string toDate, int pageNo, int pageSize)
        {
            var list = new List<AdminComplaintNotesModel>();
            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand("SP_Admin_Complaint_Notes", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Mode", "SelectAll");
                    cmd.Parameters.AddWithValue("@LoginUserId", loginUserId);
                    cmd.Parameters.AddWithValue("@SearchValue", (object)searchValue ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NoteType", (object)noteType ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@FromDate", (object)fromDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ToDate", (object)toDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PageNo", pageNo <= 0 ? 1 : pageNo);
                    cmd.Parameters.AddWithValue("@PageSize", pageSize <= 0 ? 10 : pageSize);

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var item = new AdminComplaintNotesModel();
                            item.Id = reader["Id"] != DBNull.Value ? Convert.ToInt64(reader["Id"]) : 0;
                            item.NoteType = reader["NoteType"] != DBNull.Value ? reader["NoteType"].ToString() : "";
                            item.NoteTitle = reader["NoteTitle"] != DBNull.Value ? reader["NoteTitle"].ToString() : "";
                            item.NoteReferanceId = reader["NoteReferanceId"] != DBNull.Value ? (long?)Convert.ToInt64(reader["NoteReferanceId"]) : null;
                            item.ComplaintNumber = reader["ComplaintNumber"] != DBNull.Value ? reader["ComplaintNumber"].ToString() : "";
                            item.ComplainantName = reader["ComplainantName"] != DBNull.Value ? reader["ComplainantName"].ToString() : "";
                            item.NoteDescription = reader["NoteDescription"] != DBNull.Value ? reader["NoteDescription"].ToString() : "";
                            item.Flag = reader["Flag"] != DBNull.Value ? reader["Flag"].ToString() : "A";
                            item.CreateDate = reader["CreateDate"] != DBNull.Value ? Convert.ToDateTime(reader["CreateDate"]) : DateTime.MinValue;
                            item.SCreateDate = reader["SCreateDate"] != DBNull.Value ? reader["SCreateDate"].ToString() : "";
                            item.CreateUser = reader["CreateUser"] != DBNull.Value ? (long?)Convert.ToInt64(reader["CreateUser"]) : null;
                            item.CreateUserName = reader["CreateUserName"] != DBNull.Value ? reader["CreateUserName"].ToString() : "";
                            item.UpdateDate = reader["UpdateDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["UpdateDate"]) : null;
                            item.SUpdateDate = reader["SUpdateDate"] != DBNull.Value ? reader["SUpdateDate"].ToString() : "";
                            item.UpdateUser = reader["UpdateUser"] != DBNull.Value ? (long?)Convert.ToInt64(reader["UpdateUser"]) : null;
                            item.FileCount = reader["FileCount"] != DBNull.Value ? Convert.ToInt32(reader["FileCount"]) : 0;
                            item.TotalRecord = reader["TotalRecord"] != DBNull.Value ? Convert.ToInt32(reader["TotalRecord"]) : 0;
                            item.RowNum = reader["RowNum"] != DBNull.Value ? Convert.ToInt64(reader["RowNum"]) : 0;

                            list.Add(item);
                        }
                    }
                }
            }
            return list;
        }

        public AdminComplaintNotesModel GetNoteById(long id)
        {
            AdminComplaintNotesModel item = null;
            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand("SP_Admin_Complaint_Notes", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Mode", "SelectById");
                    cmd.Parameters.AddWithValue("@Id", id);

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            item = new AdminComplaintNotesModel();
                            item.Id = reader["Id"] != DBNull.Value ? Convert.ToInt64(reader["Id"]) : 0;
                            item.NoteType = reader["NoteType"] != DBNull.Value ? reader["NoteType"].ToString() : "";
                            item.NoteTitle = reader["NoteTitle"] != DBNull.Value ? reader["NoteTitle"].ToString() : "";
                            item.NoteReferanceId = reader["NoteReferanceId"] != DBNull.Value ? (long?)Convert.ToInt64(reader["NoteReferanceId"]) : null;
                            item.ComplaintNumber = reader["ComplaintNumber"] != DBNull.Value ? reader["ComplaintNumber"].ToString() : "";
                            item.ComplainantName = reader["ComplainantName"] != DBNull.Value ? reader["ComplainantName"].ToString() : "";
                            item.NoteDescription = reader["NoteDescription"] != DBNull.Value ? reader["NoteDescription"].ToString() : "";
                            item.Flag = reader["Flag"] != DBNull.Value ? reader["Flag"].ToString() : "A";
                            item.CreateDate = reader["CreateDate"] != DBNull.Value ? Convert.ToDateTime(reader["CreateDate"]) : DateTime.MinValue;
                            item.SCreateDate = reader["SCreateDate"] != DBNull.Value ? reader["SCreateDate"].ToString() : "";
                            item.CreateUser = reader["CreateUser"] != DBNull.Value ? (long?)Convert.ToInt64(reader["CreateUser"]) : null;
                            item.CreateUserName = reader["CreateUserName"] != DBNull.Value ? reader["CreateUserName"].ToString() : "";
                            item.UpdateDate = reader["UpdateDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["UpdateDate"]) : null;
                            item.SUpdateDate = reader["SUpdateDate"] != DBNull.Value ? reader["SUpdateDate"].ToString() : "";
                            item.UpdateUser = reader["UpdateUser"] != DBNull.Value ? (long?)Convert.ToInt64(reader["UpdateUser"]) : null;
                            item.FileCount = reader["FileCount"] != DBNull.Value ? Convert.ToInt32(reader["FileCount"]) : 0;
                        }

                        // Read files
                        if (item != null && reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                var file = new AdminComplaintNoteFileModel();
                                file.Id = reader["Id"] != DBNull.Value ? Convert.ToInt64(reader["Id"]) : 0;
                                file.NoteId = reader["NoteId"] != DBNull.Value ? Convert.ToInt64(reader["NoteId"]) : 0;
                                file.FileType = reader["FileType"] != DBNull.Value ? reader["FileType"].ToString() : "";
                                file.FileName = reader["FileName"] != DBNull.Value ? reader["FileName"].ToString() : "";
                                file.FilePath = reader["FilePath"] != DBNull.Value ? reader["FilePath"].ToString() : "";
                                file.Flag = reader["Flag"] != DBNull.Value ? reader["Flag"].ToString() : "A";
                                file.CreateDate = reader["CreateDate"] != DBNull.Value ? Convert.ToDateTime(reader["CreateDate"]) : DateTime.MinValue;
                                file.SCreateDate = reader["SCreateDate"] != DBNull.Value ? reader["SCreateDate"].ToString() : "";
                                file.CreateUser = reader["CreateUser"] != DBNull.Value ? (long?)Convert.ToInt64(reader["CreateUser"]) : null;

                                item.Files.Add(file);
                            }
                        }
                    }
                }
            }
            return item;
        }

        public List<AdminComplaintNoteFileModel> GetFilesByNoteId(long noteId)
        {
            var files = new List<AdminComplaintNoteFileModel>();
            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand("SP_Admin_Complaint_Notes", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Mode", "GetFilesByNoteId");
                    cmd.Parameters.AddWithValue("@NoteId", noteId);

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var file = new AdminComplaintNoteFileModel();
                            file.Id = reader["Id"] != DBNull.Value ? Convert.ToInt64(reader["Id"]) : 0;
                            file.NoteId = reader["NoteId"] != DBNull.Value ? Convert.ToInt64(reader["NoteId"]) : 0;
                            file.FileType = reader["FileType"] != DBNull.Value ? reader["FileType"].ToString() : "";
                            file.FileName = reader["FileName"] != DBNull.Value ? reader["FileName"].ToString() : "";
                            file.FilePath = reader["FilePath"] != DBNull.Value ? reader["FilePath"].ToString() : "";
                            file.Flag = reader["Flag"] != DBNull.Value ? reader["Flag"].ToString() : "A";
                            file.CreateDate = reader["CreateDate"] != DBNull.Value ? Convert.ToDateTime(reader["CreateDate"]) : DateTime.MinValue;
                            file.SCreateDate = reader["SCreateDate"] != DBNull.Value ? reader["SCreateDate"].ToString() : "";
                            file.CreateUser = reader["CreateUser"] != DBNull.Value ? (long?)Convert.ToInt64(reader["CreateUser"]) : null;

                            files.Add(file);
                        }
                    }
                }
            }
            return files;
        }

        public long SaveNote(long id, string noteType, string noteTitle, long? noteReferanceId, string noteDescription, long loginUserId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand("SP_Admin_Complaint_Notes", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Mode", "InsertUpdate");
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@NoteType", (object)noteType ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NoteTitle", (object)noteTitle ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NoteReferanceId", (object)noteReferanceId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NoteDescription", (object)noteDescription ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@LoginUserId", loginUserId);

                    var outputId = new SqlParameter("@OutputId", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(outputId);

                    conn.Open();
                    var result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToInt64(result);
                    }
                    if (outputId.Value != null && outputId.Value != DBNull.Value)
                    {
                        return Convert.ToInt64(outputId.Value);
                    }
                    return 0;
                }
            }
        }

        public long SaveNoteFile(long noteId, string fileType, string fileName, string filePath, long loginUserId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand("SP_Admin_Complaint_Notes", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Mode", "InsertFile");
                    cmd.Parameters.AddWithValue("@NoteId", noteId);
                    cmd.Parameters.AddWithValue("@FileType", (object)fileType ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@FileName", (object)fileName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@FilePath", (object)filePath ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@LoginUserId", loginUserId);

                    conn.Open();
                    var result = cmd.ExecuteScalar();
                    return result != null && result != DBNull.Value ? Convert.ToInt64(result) : 0;
                }
            }
        }

        public bool DeleteNote(long id, long loginUserId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand("SP_Admin_Complaint_Notes", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Mode", "Delete");
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@LoginUserId", loginUserId);

                    conn.Open();
                    var result = cmd.ExecuteScalar();
                    return result != null && Convert.ToInt32(result) == 1;
                }
            }
        }

        public bool DeleteNoteFile(long fileId, long loginUserId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand("SP_Admin_Complaint_Notes", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Mode", "DeleteFile");
                    cmd.Parameters.AddWithValue("@FileId", fileId);
                    cmd.Parameters.AddWithValue("@LoginUserId", loginUserId);

                    conn.Open();
                    var result = cmd.ExecuteScalar();
                    return result != null && Convert.ToInt32(result) == 1;
                }
            }
        }

        public NotesSummaryModel GetNotesSummary(long loginUserId)
        {
            var summary = new NotesSummaryModel();
            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand("SP_Admin_Complaint_Notes", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Mode", "Summary");
                    cmd.Parameters.AddWithValue("@LoginUserId", loginUserId);

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            summary.TotalCount = reader["TotalCount"] != DBNull.Value ? Convert.ToInt32(reader["TotalCount"]) : 0;
                            summary.ComplaintCount = reader["ComplaintCount"] != DBNull.Value ? Convert.ToInt32(reader["ComplaintCount"]) : 0;
                            summary.GeneralCount = reader["GeneralCount"] != DBNull.Value ? Convert.ToInt32(reader["GeneralCount"]) : 0;
                            try
                            {
                                if (reader["AttachedFilesCount"] != DBNull.Value)
                                    summary.AttachedFilesCount = Convert.ToInt32(reader["AttachedFilesCount"]);
                            }
                            catch { }
                            summary.MeetingCount = reader["MeetingCount"] != DBNull.Value ? Convert.ToInt32(reader["MeetingCount"]) : 0;
                            summary.ConfidentialCount = reader["ConfidentialCount"] != DBNull.Value ? Convert.ToInt32(reader["ConfidentialCount"]) : 0;
                            summary.OtherCount = reader["OtherCount"] != DBNull.Value ? Convert.ToInt32(reader["OtherCount"]) : 0;
                        }
                    }
                }
            }
            return summary;
        }

        public List<ComplaintLookupModel> SearchComplaints(string searchTerm)
        {
            var list = new List<ComplaintLookupModel>();
            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand("SP_Admin_Complaint_Notes", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Mode", "SearchComplaints");
                    cmd.Parameters.AddWithValue("@SearchValue", (object)searchTerm ?? "");

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new ComplaintLookupModel
                            {
                                Id = reader["Id"] != DBNull.Value ? Convert.ToInt64(reader["Id"]) : 0,
                                ComplaintNumber = reader["ComplaintNumber"] != DBNull.Value ? reader["ComplaintNumber"].ToString() : "",
                                FullName = reader["FullName"] != DBNull.Value ? reader["FullName"].ToString() : "",
                                Status = reader["Status"] != DBNull.Value ? reader["Status"].ToString() : ""
                            });
                        }
                    }
                }
            }
            return list;
        }
    }
}

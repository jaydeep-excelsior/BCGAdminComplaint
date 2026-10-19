using System;
using System.Collections.Generic;

namespace BCGAdminComplaint.Models.Notes
{
    public class AdminComplaintNotesModel
    {
        public long Id { get; set; }
        public string NoteType { get; set; }
        public string NoteTitle { get; set; }
        public long? NoteReferanceId { get; set; }
        public string ComplaintNumber { get; set; }
        public string ComplainantName { get; set; }
        public string NoteDescription { get; set; }
        public string Flag { get; set; }
        public DateTime CreateDate { get; set; }
        public string SCreateDate { get; set; }
        public long? CreateUser { get; set; }
        public string CreateUserName { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string SUpdateDate { get; set; }
        public long? UpdateUser { get; set; }
        public int FileCount { get; set; }
        public int TotalRecord { get; set; }
        public long RowNum { get; set; }
        public List<AdminComplaintNoteFileModel> Files { get; set; }

        public AdminComplaintNotesModel()
        {
            Files = new List<AdminComplaintNoteFileModel>();
        }
    }

    public class AdminComplaintNoteFileModel
    {
        public long Id { get; set; }
        public long NoteId { get; set; }
        public string FileType { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string Flag { get; set; }
        public DateTime CreateDate { get; set; }
        public string SCreateDate { get; set; }
        public long? CreateUser { get; set; }
    }

    public class NotesSummaryModel
    {
        public int TotalCount { get; set; }
        public int ComplaintCount { get; set; }
        public int GeneralCount { get; set; }
        public int AttachedFilesCount { get; set; }
        public int MeetingCount { get; set; }
        public int ConfidentialCount { get; set; }
        public int OtherCount { get; set; }
    }

    public class ComplaintLookupModel
    {
        public long Id { get; set; }
        public string ComplaintNumber { get; set; }
        public string FullName { get; set; }
        public string Status { get; set; }
    }
}

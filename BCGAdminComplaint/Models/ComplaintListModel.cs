using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCGAdminComplaint.Models
{
    public class ComplaintListModel
    {
        public Int64 Id { get; set; }
        public String FullName { get; set; }
        public String EmailAddress { get; set; }
        public String ContactNumber { get; set; }
        public String Summary { get; set; }
        public String DetailComplaint { get; set; }
        public String Status { get; set; }
        public String PaymentStatus { get; set; }
        public String ComplaintNumber { get; set; }
        public Decimal TotalAmount { get; set; }
        public Nullable<long> SrNo { get; set; }
        public Nullable<int> TotalRecord { get; set; }
        public String Address { get; set; }
        public DateTime CreateDate { get; set; }
        public String SCreateDate { get; set; }
        public String ComplaintsCreateDate { get; set; }
        public int NewStatusCount { get; set; }
        public int OldStatusCount { get; set; }
        public Decimal PaidAmount { get; set; }
        public Decimal DueAmount { get; set; }
        public String ComplainedAgainst { get; set; }
        public String ComplaintAffidavitCopyList { get; set; }
        public String ComplaintAdditionalAttachmentsList { get; set; }
        public String ComplaintPaymentList { get; set; }
        public String AffidavitFilesText { get; set; }
        public String AdditionalFilesText { get; set; }
        public String PaymentInfoText { get; set; }
    }

    public class ComplaintDocumentList
    {
        public Int64 Id { get; set; }
        public Nullable<long> SrNo { get; set; }
        public Int64 ComplaintId { get; set; }
        public String FileTitle { get; set; }
        public String FilePath { get; set; }
        public String FileName { get; set; }
        public String Description { get; set; }
        public String DocumetType { get; set; }
        public String Name { get; set; }
    }

    public class ComplaintAgainstList
    {        
        public Nullable<long> SrNo { get; set; }
        public Int64 Id { get; set; }
        public String Name { get; set; }
        public Int64 AdvocateId { get; set; }
        public String EnrollmentNo { get; set; }
        public String ContactNumber { get; set; }
        public Decimal Amount { get; set; }
        public Int64 CreateUser { get; set; }
        public Int64 ComplaintId { get; set; }
        public String ReturnStatus { get; set; }
        public String AdvocateName { get; set; }
        public String City { get; set; }
        public String MobileNo { get; set; }
        public String IsKnown { get; set; }
        public String FullName { get; set; }
        public String FinalContactNumber { get; set; }
    }

    public class ComplaintStatusLog
    {
        public Int64 Id { get; set; }
        public Int64 ComplaintId { get; set; }
        public String Status { get; set; }
        public DateTime NextDate { get; set; }
        public String OrderFileName { get; set; }
        public String OrderFilePath { get; set; }
        public String Summary { get; set; }
        public String SNextDate { get; set; }
        public String Name { get; set; }
        public String CommitteTitle { get; set; }
        public Int64 CommitteeId { get; set; }
        public String OldStatus { get; set; }
        public String FullName { get; set; }
        public String ComplaintNumber { get; set; }
        public Nullable<long> SrNo { get; set; }
        public Int64 CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public String SCreateDate { get; set; }
    }

    public class ComplaintMemberLog
    {
        public Int64 Id { get; set; }
        public Int64 ComplaintId { get; set; }
        public String Status { get; set; }
        public Int64 MemberId { get; set; }
        public String IsChecked { get; set; }
        public Nullable<long> SrNo { get; set; }
        public String FullName { get; set; }
        public String EnroNumber { get; set; }
        public String ContactNumber { get; set; }
        public Int64 CommitteeId { get; set; }
        public String ComplaintDetailEncryptCode { get; set; }
    }

    public class DcNumberList
    {
        public Int64 Id { get; set; }
        public String DcTitle { get; set; }
    }

    public class MemberNoteList
    {
        public Int64 Id { get; set; }
        public Int64 MemberId { get; set; }
        public String NoteType { get; set; }
        public String Note { get; set; }
        public String MemberName { get; set; }
        public DateTime CreateDate { get; set; }
        public String SCreateDate { get; set; }
        public Nullable<long> SrNo { get; set; }
    }

    public class ComplaintCount
    {
        public int TotalCount { get; set; }
        public int NewStatusCount { get; set; }
        public int HearingStatusCount { get; set; }
        public int AdjuntStatusCount { get; set; }
        public int ApprovedStatusCount { get; set; }
        public int RejectedStatusCount { get; set; }
        public int AcceptedStatusCount { get; set; }
        public int OrderPassedStatusCount { get; set; }
    }

    public class ComplaintReply
    {
        public Int64 Id { get; set; }
        public Int64 ComplaintId { get; set; }
        public String Summery { get; set; }
        public String Comprehensive { get; set; }
        public String FileName { get; set; }
        public String FilePath { get; set; }
        public String SCreateDate { get; set; }
        public String SCreateTime { get; set; }
        public String SenderName { get; set; }
        public String IsShowDeleteButton { get; set; }
    }
}

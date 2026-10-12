using System;

namespace BCGAdminComplaint.Models
{
    public class ComplaintDetailByNumber
    {
        public Int64 Id { get; set; }
        public String ComplaintNumber { get; set; }
        public String FullName { get; set; }
        public String EmailAddress { get; set; }
        public String ContactNumber { get; set; }
        public String Address { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal DueAmount { get; set; }
        public String Status { get; set; }
        public String ComplaintAgainstList { get; set; }
        public String ComplaintAffidavitCopyList { get; set; }
        public String ComplaintAdditionalAttachmentsList { get; set; }
        public String ComplaintPaymentList { get; set; }
        public String ComplainedAgainstReplyToList { get; set; }
        public String SCreateDate { get; set; }
    }

    public class ComplaintAffidavitCopyByNumber
    {
        public Int64 Id { get; set; }
        public Int64 ComplaintId { get; set; }
        public String FileName { get; set; }
        public String FilePath { get; set; }
        public String IsOld { get; set; }
        public String FileExtension { get; set; }
    }

    public class ComplaintAdditionalAttachmentsByNumber
    {
        public Int64 Id { get; set; }
        public Int64 ComplaintId { get; set; }
        public String FileTitle { get; set; }
        public String FileName { get; set; }
        public String FilePath { get; set; }
        public String Description { get; set; }
        public String FileExtension { get; set; }
    }

    public class ComplaintMemberLogList
    {
        public Int64 Id { get; set; }
        public Int64 MemberId { get; set; }
        public Int64 EnrollmentId { get; set; }
        public String EnroNumber { get; set; }
        public String FullName { get; set; }
        public String ContactNumber { get; set; }
        public String ComplaintDetailEncryptCode { get; set; }
        public String EmailAddress { get; set; }
    }

    public class ComplaintMemberEncryptCodeList
    {
        public Int64 Id { get; set; }
        public Int64 ComplaintId { get; set; }
        public String Status { get; set; }
        public Int64 MemberId { get; set; }
        public String IsChecked { get; set; }
        public Int64 CommitteeId { get; set; }
        public String ComplaintDetailEncryptCode { get; set; }
    }

}

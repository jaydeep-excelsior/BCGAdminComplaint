using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCGAdminComplaint.Models
{
    public class ComplaintDCReferredListModel
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
        public String DCComplaintNumber { get; set; }
        public String DCComplaintNumberCode { get; set; }
        public String CommitteTitle { get; set; }
        public String AssignCommitteeDate { get; set; }
        public String RespondentDetails { get; set; }
    }

    public class ComplaintDCReferredListReportModel
    {
        public Int64 SrNo { get; set; }
        public String FullName { get; set; }
        public String EmailAddress { get; set; }
        public String ContactNumber { get; set; }
        public String ComplaintNumber { get; set; }
        public String DCComplaintNumber { get; set; }
        public String RespondentDetails { get; set; }
        public String RespondentDetailsExcel { get; set; }
        public String MeetingDated { get; set; }
        public String Status { get; set; }
        public String CommitteTitle { get; set; }
        public String AssignCommitteeDate { get; set; }
    }
}


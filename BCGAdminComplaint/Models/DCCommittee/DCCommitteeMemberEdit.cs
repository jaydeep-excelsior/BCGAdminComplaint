using System;

namespace BCGAdminComplaint.Models.DCCommittee
{
    public class DCCommitteeMemberEdit
    {
        public Int64 Id { get; set; }
        public Int64 DC_ComplaintId { get; set; }
        public Int64 MemberId { get; set; }
        public String MemberName { get; set; }
        public String MemberType { get; set; }
    }
}

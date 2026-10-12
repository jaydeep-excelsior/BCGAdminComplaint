using System;

namespace BCGAdminComplaint.Models.DCCommittee
{
    public class DCCommitteeList
    {
        public String SrNo { get; set; }
        public String TotalRecord { get; set; }
        public Int64 Id { get; set; }
        public String CommitteeTitle { get; set; }
        public Int64 DcCommitteeNo { get; set; }
        public String Note { get; set; }
        public String Flag { get; set; }
        public String EditRights { get; set; }
        public String DeleteRights { get; set; }
        public String CreateBy { get; set; }
        public String LastModifyBy { get; set; }
        public String SCreateDate { get; set; }
        public String SCreateTime { get; set; }
        public String LastModifyDate { get; set; }
        public String LastModifyTime { get; set; }
        public Int64 TotalMember { get; set; }
    }
}

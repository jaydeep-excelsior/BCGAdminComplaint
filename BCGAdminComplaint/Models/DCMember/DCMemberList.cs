using System;

namespace BCGAdminComplaint.Models.DCMember
{
    public class DCMemberList
    {
        public String SrNo { get; set; }
        public String TotalRecord { get; set; }
        public Int64 Id { get; set; }
        public Int64 EnrollmentId { get; set; }
        public String EnroNumber { get; set; }
        public String FullName { get; set; }
        public String ContactNumber { get; set; }
        public String EmailAddress { get; set; }
        public String Password { get; set; }
        public String Flag { get; set; }
        public String EditRights { get; set; }
        public String DeleteRights { get; set; }
        public String CreateBy { get; set; }
        public String LastModifyBy { get; set; }
        public String SCreateDate { get; set; }
        public String SCreateTime { get; set; }
        public String LastModifyDate { get; set; }
        public String LastModifyTime { get; set; }
    }
}

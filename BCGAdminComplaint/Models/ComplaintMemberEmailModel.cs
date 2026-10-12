using System;

namespace BCGAdminComplaint.Models
{
    public class ComplaintMemberLogDetail
    {
        public long Id { get; set; }
        public long ComplaintId { get; set; }
        public string ComplaintDetailEncryptCode { get; set; }
        public string MemberEmail { get; set; }
        public string MemberName { get; set; }
        public string MemberMobile { get; set; }
        public string ComplaintNumber { get; set; }
        public string ComplainantName { get; set; }
    }

    public class EmailConfigurationModel
    {
        public long Id { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool SSL { get; set; }
        public string EmailType { get; set; }
        public string Flag { get; set; }
    }
}

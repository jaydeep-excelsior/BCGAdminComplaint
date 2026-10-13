using System;

namespace BCGAdminComplaint.Models.DCMember
{
    public class DCMemberEdit
    {
        public Int64 Id { get; set; }
        public Int64 EnrollmentId { get; set; }
        public String EmailAddress { get; set; }
        public String Password { get; set; }
        public String Name { get; set; }
        public String ContactNo { get; set; }
        public String BarAssociation { get; set; }
        public String EnroNumber { get; set; }
        public String MemberType { get; set; }
    }
}

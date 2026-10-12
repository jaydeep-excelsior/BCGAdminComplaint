using System;

namespace BCGAdminComplaint.Models.DCMember
{
    public class DCMemberEnrollmentDetails
    {
        public Int64 EnrollmentId { get; set; }
        public String FullName { get; set; }
        public String ContactNumber { get; set; }
        public String EmailAddress { get; set; }
        public String BarAssociation { get; set; }
    }
}

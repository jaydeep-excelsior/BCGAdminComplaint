using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCGAdminComplaint.Models
{
    public class DefendantDetailsList
    {
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
}

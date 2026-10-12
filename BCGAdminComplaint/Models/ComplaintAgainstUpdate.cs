using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCGAdminComplaint.Models
{
    public class ComplaintAgainstUpdate
    {
        public String ReturnStatus { get; set; }
        public String EnrollmentNumber { get; set; }
    }

    public class ComplaintAgainstDetail
    {
        public Int64 EId { get; set; }
        public String FullName { get; set; }
        public String CityName { get; set; }
        public String ContactNumber { get; set; }
    }
}

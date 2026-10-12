using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCGAdminComplaint.Models
{
    public class ComplaintModel
    {
        public Int64 Id { get; set; }
        public String FullName { get; set; }
        public String EmailAddress { get; set; }
        public String ContactNumber { get; set; }
        public String Address { get; set; }
        public String Summary { get; set; }
        public String DetailComplaint { get; set; }
        public Decimal TotalAmount { get; set; }
        public Int64 CreateUser { get; set; }
        public String Status { get; set; }
        public String PaymentStatus { get; set; }
        public String ReturnStatus { get; set; }
        public String ComplaintNumber { get; set; }
        public int Otp { get; set; }
    }
}

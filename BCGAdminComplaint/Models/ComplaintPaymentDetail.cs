using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCGAdminComplaint.Models
{
    public class ComplaintPaymentDetail
    {
        public Decimal TotalAmount { get; set; }
        public Decimal PaidAmount { get; set; }
        public Decimal DueAmount { get; set; }
        public String PaymentStatus { get; set; }
        public String Status { get; set; }
    }
}

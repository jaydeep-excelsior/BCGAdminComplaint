using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCGAdminComplaint.Models
{
    public class ComplaintPaymentModel
    {
        public String PaidAmount { get; set; }
        public String ReceiptNumber { get; set; }
        public String ReceiptDate { get; set; }
        public String PaidOn { get; set; }
        public String PaymentMode { get; set; }
        public String TransactionNumber { get; set; }
        public String TransactionDate { get; set; }
        public String BankId { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCGAdminComplaint.Models
{
    public class ComplaintPaymentReceived
    {
        public Int64 ComplaintId { get; set; }
        public Decimal PaidAmount { get; set; }
        public String PaidBy { get; set; }
        public String PaymentStatus { get; set; }
        public String ComplaintStatus { get; set; }
    }

    public class ComplaintPaymentReceivedLog
    {
        public Int64 ReceiptNumber { get; set; }
        public String ReceiptDate { get; set; }
        public decimal PaidAmount { get; set; }
        public String PaymentMode { get; set; }
        public String TransactionNumber { get; set; }
        public String TransactionDate { get; set; }
        public String BankName { get; set; }
    }

    public class ComplaintNumberUpdate
    {
        public String ReturnStatus { get; set; }
    }
}

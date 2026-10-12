using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCGAdminComplaint.Models
{
    public class DcMemberList
    {
        public Nullable<long> SrNo { get; set; }
        public Int64 MemberId { get; set; }
        public String EnroNumber { get; set; }
        public String FullName { get; set; }
        public String ContactNumber { get; set; }
        public String Flag { get; set; }
    }

    public class MemebrList
    {
        public String MemberId { get; set; }
        public String IsChecked { get; set; }
    }

    public class ComplaintMemberLogSaveReturn
    {
        public String ReturnStatus { get; set; }
        public Int64 ComplaintId { get; set; }
    }
}

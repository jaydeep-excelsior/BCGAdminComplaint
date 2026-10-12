using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCGAdminComplaint.Models
{
    public class AffidavitCopy
    {
        public Int64 Id { get; set; }
        public String FilePath { get; set; }
        public String FileName { get; set; }
        public Int64 ComplaintId { get; set; }
        public Int64 CreateUser { get; set; }
        public String ReturnStatus { get; set; }
        public String IsOld { get; set; }
        public Int64 SrNo { get; set; }
        public String FileExtension { get; set; }
    }
}

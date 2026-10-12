using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCGAdminComplaint.Models
{
    public class ComplaintUserLoginReturn
    {
        public String ReturnStatus { get; set; }
        public String RegisterId { get; set; }
        public String UserName { get; set; }
        public String IsDcAccess { get; set; }
    }
}

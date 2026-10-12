using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCGAdminComplaint.Models
{
    public class BaseFilter
    {
        public string searchValue { get; set; }
        public int pageNo { get; set; }
        public int pageSize { get; set; }
        public int sortColumn { get; set; }
        public string sortOrder { get; set; }
    }

    public class ExtraFilter
    {
        public String Status { get; set; }
        public String PaymentStatus { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int IsFilter { get; set; }
    }
}

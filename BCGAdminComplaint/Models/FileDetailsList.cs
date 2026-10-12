using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCGAdminComplaint.Models
{
    public class FileDetailsList
    {
        public Int64 Id { get; set; }
        public String FileTitle { get; set; }
        public String FilePath { get; set; }
        public String FileName { get; set; }
        public String Description { get; set; }
        public Int64 ComplaintId { get; set; }
        public Int64 CreateUser { get; set; }
        public String ReturnStatus { get; set; }
        public String FileExtension { get; set; }
    }
}

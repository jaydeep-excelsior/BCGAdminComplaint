using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCGAdminComplaint.Models
{
    public class ComplaintGBMeetingLog
    {
        public Int64 SrNo { get; set; }
        public Int64 Id { get; set; }
        public String MeetingTitle { get; set; }
        public String MeetingDate { get; set; }
        public String MeetingNote { get; set; }
    }

    public class ComplaintGBMeetingLogSaveReturn
    {
        public String ReturnStatus { get; set; }
    }

    public class ComplaintGBMeetingLogEdit
    {
        public Int64 Id { get; set; }
        public Int64 ComplaintId { get; set; }
        public String MeetingTitle { get; set; }
        public String MeetingDate { get; set; }
        public String MeetingNote { get; set; }
    }
}

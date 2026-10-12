using System;

namespace BCGAdminComplaint.Models.DCCommittee
{
    public class DCCommitteeEdit
    {
        public Int64 Id { get; set; }
        public String CommitteeTitle { get; set; }
        public Int64 DcCommitteeNo { get; set; }
        public String Note { get; set; }
    }
}

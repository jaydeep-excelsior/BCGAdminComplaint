using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCGAdminComplaint.Models
{
    public class EnrollmentMstModel
    {
        public Int64 EId { get; set; }
        public String EnroNumber { get; set; }
        public String FullName { get; set; }
        public String GAWFNumber { get; set; }
        public String ContactNumber { get; set; }
        public Int64 CntGRNo { get; set; }
        public int ENRollYear { get; set; }
        public int GAWFYear { get; set; }
        public String EmailAddress { get; set; }
        public String Gender { get; set; }
        public String Address { get; set; }
        public String PinCode { get; set; }
        public String ProfileImage { get; set; }
        public String SEnroNODate { get; set; }
        public String SGAWFNODates { get; set; }
        public String CastTitle { get; set; }
        public String CityName { get; set; }
        public String TalukaName { get; set; }
        public String StatusName { get; set; }
        public String DistrictName { get; set; }
        public String SDateOfBirth { get; set; }
        public String ReceiptFlag { get; set; }
        public String LastRenewalYear { get; set; }
        public String GAWFType { get; set; }
        public String PracticeVerificationShow { get; set; }
        public String PracticeVerificationStatus { get; set; }
        public String PracticeVerificationDate { get; set; }
        public String DeclarationShow { get; set; }
        public String DeclarationStatus { get; set; }
        public String GAWFSuspendFlag { get; set; }
        public String OneBarOneVoteAssocation { get; set; }
        public String IsEligibleForGAWFPayment { get; set; }
        public String ReturnStatus { get; set; }
        public int GenerateOTP { get; set; }
    }
}

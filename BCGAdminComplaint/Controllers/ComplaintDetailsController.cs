using BCGAdminComplaint;
using BCGAdminComplaint.Models;
using BCGAdminComplaint.Repository;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Data.SqlClient;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace BarCouncilComplaint.Controllers
{
    public class ComplaintDetailsController : BaseController
    {
        private readonly ComplaintRepository _ComplaintRepository;
        private readonly ComplaintDetailsRepository _ComplaintDetailsRepository;

        String ImageUrl = ConfigurationSettings.AppSettings["ImageUrl"].ToString();

        public ComplaintDetailsController()
        {
            _ComplaintRepository = new ComplaintRepository();
            _ComplaintDetailsRepository = new ComplaintDetailsRepository();
        }

        public ActionResult Index()
        {
            HttpCookie cookie = Request.Cookies["bcg"];
            if (cookie == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                return View();
            }
        }

        public ActionResult Details(Int64 Id)
        {
            HttpCookie cookie = Request.Cookies["bcg"];
            if (cookie == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                try
                {
                    List<ComplaintListModel> ComplaintDetail = new List<ComplaintListModel>();
                    List<ComplaintAgainstList> ComplainAgainstList = new List<ComplaintAgainstList>();
                    List<ComplaintDocumentList> ComplaintDocument = new List<ComplaintDocumentList>();
                    List<ComplaintMemberLog> DcMemberList = new List<ComplaintMemberLog>();
                    List<DcNumberList> result = new List<DcNumberList>();

                    ComplaintDetail = _ComplaintRepository.GetComplaintDetails(Id);
                    // ComplainAgainstList = _ComplaintRepository.GetAdjuntList(Id);
                    ComplaintDocument = _ComplaintRepository.GetDocumentList(Id);
                    DcMemberList = _ComplaintRepository.GetDcMemberList(Id);
                    result = _ComplaintDetailsRepository.GetDcNumberList();


                    ViewBag.ComplaintId = ComplaintDetail[0].Id;
                    ViewBag.FullName = ComplaintDetail[0].FullName;
                    ViewBag.EmailAddress = ComplaintDetail[0].EmailAddress;
                    ViewBag.ContactNumber = ComplaintDetail[0].ContactNumber;
                    ViewBag.Address = ComplaintDetail[0].Address;
                    ViewBag.Summary = ComplaintDetail[0].Summary;
                    ViewBag.DetailComplaint = ComplaintDetail[0].DetailComplaint;
                    ViewBag.Status = ComplaintDetail[0].Status;

                    //DataTable details = new DataTable();
                    //details.Columns.AddRange(new DataColumn[8] {
                    //         new DataColumn("SrNo",typeof(String)),
                    //        new DataColumn("FullName",typeof(String)),
                    //        new DataColumn("EnroNumber",typeof(String)),
                    //         new DataColumn("Amount",typeof(Decimal)),
                    //    new DataColumn("EnrollmentId",typeof(Decimal)),
                    //    new DataColumn("ContactNo",typeof(String)),
                    //     new DataColumn("IsKnown",typeof(String)),
                    //     new DataColumn("City",typeof(String))
                    //    });

                    //DataRow dr2 = null;
                    //if (ComplainAgainstList.Count > 0)
                    //{
                    //    for (int i = 0; i < ComplainAgainstList.Count; i++)
                    //    {
                    //        dr2 = details.NewRow();
                    //        dr2["SrNo"] = Convert.ToString(i + 1);
                    //        dr2["FullName"] = ComplainAgainstList[i].AdvocateName == null ? ComplainAgainstList[i].FullName : ComplainAgainstList[i].AdvocateName;
                    //        dr2["EnroNumber"] = ComplainAgainstList[i].EnrollmentNo;
                    //        dr2["Amount"] = ComplainAgainstList[i].Amount;
                    //        dr2["EnrollmentId"] = ComplainAgainstList[i].AdvocateId;
                    //        dr2["ContactNo"] = ComplainAgainstList[i].MobileNo == null ? ComplainAgainstList[i].ContactNumber : ComplainAgainstList[i].MobileNo;
                    //        dr2["IsKnown"] = ComplainAgainstList[i].IsKnown == null ? "" : ComplainAgainstList[i].IsKnown;
                    //        dr2["City"] = ComplainAgainstList[i].City == null ? "" : ComplainAgainstList[i].City;
                    //        details.Rows.Add(dr2);
                    //    }

                    //    ViewBag.AgainstList = details;

                    //    ViewBag.IsKnown = ComplainAgainstList[0].IsKnown == null ? "" : ComplainAgainstList[0].IsKnown;
                    //}

                    DataTable document = new DataTable();
                    document.Columns.AddRange(new DataColumn[5] {
                             new DataColumn("SrNo",typeof(String)),
                            new DataColumn("FileTitle",typeof(String)),
                            new DataColumn("FileName",typeof(String)),
                             new DataColumn("FilePath",typeof(String)),
                              new DataColumn("Description",typeof(String)),
                        });
                    DataRow drIND = null;
                    if (ComplaintDocument.Count > 0)
                    {
                        for (int i = 0; i < ComplaintDocument.Count; i++)
                        {
                            drIND = document.NewRow();
                            drIND["SrNo"] = Convert.ToString(i + 1);
                            drIND["FileTitle"] = ComplaintDocument[i].FileTitle;
                            drIND["FileName"] = ComplaintDocument[i].FileName;
                            drIND["FilePath"] = ComplaintDocument[i].FilePath;
                            drIND["Description"] = ComplaintDocument[i].Description;
                            document.Rows.Add(drIND);
                        }

                        ViewBag.DocumentList = document;
                    }

                    DataTable dcMember = new DataTable();
                    dcMember.Columns.AddRange(new DataColumn[7] {
                             new DataColumn("SrNo",typeof(String)),
                            new DataColumn("FullName",typeof(String)),
                            new DataColumn("EnroNumber",typeof(String)),
                             new DataColumn("ContactNumber",typeof(Decimal)),
                             new DataColumn("IsChecked",typeof(String)),
                             new DataColumn("MemberId",typeof(Int64)),
                              new DataColumn("CommitteeId",typeof(Int64)),
                        });

                    DataRow drMember = null;

                    if (DcMemberList.Count > 0)
                    {
                        for (int i = 0; i < DcMemberList.Count; i++)
                        {
                            drMember = dcMember.NewRow();
                            drMember["SrNo"] = Convert.ToString(i + 1);
                            drMember["FullName"] = DcMemberList[i].FullName;
                            drMember["EnroNumber"] = DcMemberList[i].EnroNumber;
                            drMember["ContactNumber"] = DcMemberList[i].ContactNumber;
                            drMember["IsChecked"] = DcMemberList[i].IsChecked;
                            drMember["MemberId"] = DcMemberList[i].MemberId;
                            drMember["CommitteeId"] = DcMemberList[i].CommitteeId;
                            dcMember.Rows.Add(drMember);
                        }
                        ViewBag.DcNumber = DcMemberList[0].CommitteeId;
                        ViewBag.MemberList = dcMember;
                    }
                    else
                    {
                        ViewBag.DcNumber = "0";
                        ViewBag.MemberList = "";
                    }

                    var query = result.Select(DcNumber => new
                    {
                        Id = DcNumber.Id,
                        DcTitle = DcNumber.DcTitle,

                    }).OrderBy(x => x.DcTitle)
                      .ToList();



                    return View(new Tuple<IEnumerable<ComplaintListModel>, IEnumerable<ComplaintDocumentList>, IEnumerable<ComplaintMemberLog>>(ComplaintDetail, ComplaintDocument, DcMemberList));
                }
                catch (Exception ex)
                {
                    return View();
                }
            }
        }

        public ActionResult DetailsNew(Int64 Id)
        {
            HttpCookie cookie = Request.Cookies["bcg"];
            if (cookie == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                if (Id != null)
                {
                    try
                    {

                        List<ComplaintListModel> ComplaintDetail = new List<ComplaintListModel>();
                        List<ComplaintAgainstList> ComplainAgainstList = new List<ComplaintAgainstList>();
                        List<ComplaintDocumentList> ComplaintDocument = new List<ComplaintDocumentList>();
                        List<ComplaintMemberLog> DcMemberList = new List<ComplaintMemberLog>();
                        List<DcNumberList> result = new List<DcNumberList>();
                        List<ComplaintGBMeetingLog> ComplaintGBMeeting = new List<ComplaintGBMeetingLog>();

                        ComplaintDetail = _ComplaintRepository.GetComplaintDetails(Id);
                        // ComplainAgainstList = _ComplaintRepository.GetAdjuntList(Id);
                        ComplaintDocument = _ComplaintRepository.GetDocumentList(Id);
                        ComplaintGBMeeting = _ComplaintRepository.GetComplaintGBMeetingLogList(Id);
                        DcMemberList = _ComplaintRepository.GetDcMemberList(Id);
                        result = _ComplaintDetailsRepository.GetDcNumberList();


                        ViewBag.ComplaintId = ComplaintDetail[0].Id;
                        ViewBag.FullName = ComplaintDetail[0].FullName;
                        ViewBag.EmailAddress = ComplaintDetail[0].EmailAddress;
                        ViewBag.ContactNumber = ComplaintDetail[0].ContactNumber;
                        ViewBag.Address = ComplaintDetail[0].Address;
                        ViewBag.Summary = ComplaintDetail[0].Summary;
                        ViewBag.DetailComplaint = ComplaintDetail[0].DetailComplaint;
                        ViewBag.Status = ComplaintDetail[0].Status;
                        ViewBag.ComplaintNumber = ComplaintDetail[0].ComplaintNumber;
                        ViewBag.SCreateDate = ComplaintDetail[0].SCreateDate;

                        DataTable document = new DataTable();
                        document.Columns.AddRange(new DataColumn[6] {
                             new DataColumn("SrNo",typeof(String)),
                             new DataColumn("Id",typeof(String)),
                             new DataColumn("FileTitle",typeof(String)),
                             new DataColumn("FileName",typeof(String)),
                             new DataColumn("FilePath",typeof(String)),
                             new DataColumn("Description",typeof(String))
                        });
                        DataRow drIND = null;
                        var AdditionalDocument = ComplaintDocument.Where(x => x.DocumetType == "Additional").ToList();
                        if (AdditionalDocument.Count > 0)
                        {
                            for (int i = 0; i < AdditionalDocument.Count; i++)
                            {
                                drIND = document.NewRow();
                                drIND["SrNo"] = Convert.ToString(i + 1);
                                drIND["Id"] = AdditionalDocument[i].Id;
                                drIND["FileTitle"] = AdditionalDocument[i].FileTitle;
                                drIND["FileName"] = AdditionalDocument[i].FileName;
                                drIND["FilePath"] = AdditionalDocument[i].FilePath;
                                drIND["Description"] = AdditionalDocument[i].Description;
                                document.Rows.Add(drIND);
                            }

                            ViewBag.AdditionalDocumentList = document;
                        }

                        DataTable dtComDC = new DataTable();
                        dtComDC.Columns.AddRange(new DataColumn[5] {
                             new DataColumn("SrNo",typeof(String)),
                             new DataColumn("Id",typeof(String)),
                             new DataColumn("MeetingTitle",typeof(String)),
                             new DataColumn("MeetingDate",typeof(String)),
                             new DataColumn("MeetingNote",typeof(String))
                        });

                        if (ComplaintGBMeeting != null)
                        {
                            if (ComplaintGBMeeting.ToList().Count > 0)
                            {
                                for (int i = 0; i < ComplaintGBMeeting.ToList().Count; i++)
                                {
                                    DataRow drComDC = null;
                                    drComDC = dtComDC.NewRow();
                                    drComDC["SrNo"] = ComplaintGBMeeting[i].SrNo == null ? "0" : ComplaintGBMeeting[i].SrNo.ToString() == "" ? "0" : ComplaintGBMeeting[i].SrNo.ToString();
                                    drComDC["Id"] = ComplaintGBMeeting[i].Id == null ? "0" : ComplaintGBMeeting[i].Id.ToString() == "" ? "0" : ComplaintGBMeeting[i].Id.ToString();
                                    drComDC["MeetingTitle"] = ComplaintGBMeeting[i].MeetingTitle == null ? "" : ComplaintGBMeeting[i].MeetingTitle.ToString();
                                    drComDC["MeetingDate"] = ComplaintGBMeeting[i].MeetingDate == null ? "" : ComplaintGBMeeting[i].MeetingDate.ToString();
                                    drComDC["MeetingNote"] = ComplaintGBMeeting[i].MeetingNote == null ? "" : ComplaintGBMeeting[i].MeetingNote.ToString();
                                    dtComDC.Rows.Add(drComDC);
                                }
                            }
                        }

                        ViewBag.ComplaintGBMeetingList = dtComDC;

                        DataTable AffidavitDoc = new DataTable();
                        AffidavitDoc.Columns.AddRange(new DataColumn[6] {
                             new DataColumn("SrNo",typeof(String)),
                             new DataColumn("Id",typeof(String)),
                             new DataColumn("FileTitle",typeof(String)),
                             new DataColumn("FileName",typeof(String)),
                             new DataColumn("FilePath",typeof(String)),
                             new DataColumn("Description",typeof(String))
                        });
                        DataRow drADC = null;
                        var AffidavitDocument = ComplaintDocument.Where(x => x.DocumetType == "Affidavit").ToList();
                        if (AffidavitDocument.Count > 0)
                        {
                            for (int i = 0; i < AffidavitDocument.Count; i++)
                            {
                                drADC = AffidavitDoc.NewRow();
                                drADC["SrNo"] = Convert.ToString(i + 1);
                                drADC["Id"] = AffidavitDocument[i].Id;
                                drADC["FileTitle"] = AffidavitDocument[i].FileTitle;
                                drADC["FileName"] = AffidavitDocument[i].FileName;
                                drADC["FilePath"] = AffidavitDocument[i].FilePath;
                                drADC["Description"] = AffidavitDocument[i].Description;
                                AffidavitDoc.Rows.Add(drADC);
                            }

                            ViewBag.AffidavitDocumentList = AffidavitDoc;
                        }

                        DataTable dcMember = new DataTable();
                        dcMember.Columns.AddRange(new DataColumn[7] {
                             new DataColumn("SrNo",typeof(String)),
                            new DataColumn("FullName",typeof(String)),
                            new DataColumn("EnroNumber",typeof(String)),
                             new DataColumn("ContactNumber",typeof(Decimal)),
                             new DataColumn("IsChecked",typeof(String)),
                             new DataColumn("MemberId",typeof(Int64)),
                              new DataColumn("CommitteeId",typeof(Int64)),
                        });

                        DataRow drMember = null;

                        if (DcMemberList.Count > 0)
                        {
                            for (int i = 0; i < DcMemberList.Count; i++)
                            {
                                drMember = dcMember.NewRow();
                                drMember["SrNo"] = Convert.ToString(i + 1);
                                drMember["FullName"] = DcMemberList[i].FullName;
                                drMember["EnroNumber"] = DcMemberList[i].EnroNumber;
                                drMember["ContactNumber"] = DcMemberList[i].ContactNumber;
                                drMember["IsChecked"] = DcMemberList[i].IsChecked;
                                drMember["MemberId"] = DcMemberList[i].MemberId;
                                drMember["CommitteeId"] = DcMemberList[i].CommitteeId;
                                dcMember.Rows.Add(drMember);
                            }
                            ViewBag.DcNumber = DcMemberList[0].CommitteeId;
                            ViewBag.MemberList = dcMember;
                        }
                        else
                        {
                            ViewBag.DcNumber = "0";
                            ViewBag.MemberList = "";
                        }

                        var query = result.Select(DcNumber => new
                        {
                            Id = DcNumber.Id,
                            DcTitle = DcNumber.DcTitle,

                        }).OrderBy(x => x.DcTitle)
                          .ToList();



                        return View(new Tuple<IEnumerable<ComplaintListModel>, IEnumerable<ComplaintDocumentList>, IEnumerable<ComplaintMemberLog>>(ComplaintDetail, ComplaintDocument, DcMemberList));
                    }
                    catch (Exception ex)
                    {
                        return View();
                    }
                }
                else
                {
                    return RedirectToAction("Index", "ComplaintList");
                }
            }
        }

        [HttpPost]
        public ActionResult UpdateComplainantDetails(Int64 Id, String FullName, String EmailAddress, String ContactNumber, String Address)
        {
            try
            {
                Int64 UpdateUser = Convert.ToInt64(Request.Cookies["CMPUserId"].Value);
                var result = _ComplaintRepository.ComplaintComplainantDetailsUpdate(Id, FullName, EmailAddress, ContactNumber, Address, UpdateUser, "ComplaintComplainantDetailsUpdate");

                if (result.Count > 0 && result[0].ReturnStatus == "UD")
                {
                    response.IsSuccess = true;
                    response.Message = "Complainant details updated successfully.";
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Failed to update complainant details.";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SendComplaintLinkSMS(int dcId, string mobileNumber, string emailAddress = "")
        {
            ResultResponse response = new ResultResponse();
            try
            {
                Int64 IId = Convert.ToInt64(dcId == null ? "0" : dcId.ToString() == "" ? "0" : dcId.ToString());

                string memberEmail = !string.IsNullOrEmpty(emailAddress) ? emailAddress.Trim() : "";
                string memberName = "";
                string memberMobile = "";
                string complaintNumber = "";
                string complainantName = "";
                string complaintDetailEncryptCode = "";

                // 1. Fetch Member & Complaint details via Repository (SP_Complaint_Member_Log_Detail_Get)
                ComplaintMemberLogDetail memberLog = _ComplaintDetailsRepository.GetComplaintMemberLogDetail(IId);
                if (memberLog != null)
                {
                    complaintDetailEncryptCode = (memberLog.ComplaintDetailEncryptCode ?? "").Trim();
                    if (string.IsNullOrEmpty(memberEmail))
                    {
                        memberEmail = (memberLog.MemberEmail ?? "").Trim();
                    }
                    memberName = (memberLog.MemberName ?? "").Trim();
                    memberMobile = (memberLog.MemberMobile ?? "").Trim();
                    complaintNumber = (memberLog.ComplaintNumber ?? "").Trim();
                    complainantName = (memberLog.ComplainantName ?? "").Trim();
                }

                // Fallback to encrypt code from repository if not found
                if (string.IsNullOrEmpty(complaintDetailEncryptCode))
                {
                    List<ComplaintMemberEncryptCodeList> getCode = _ComplaintRepository.GetComplaintMemberEncryptCode(IId, "ComplaintMemberLogDetail");
                    if (getCode != null && getCode.Count > 0 && !string.IsNullOrEmpty(getCode[0].ComplaintDetailEncryptCode))
                    {
                        complaintDetailEncryptCode = getCode[0].ComplaintDetailEncryptCode;
                    }
                }

                if (string.IsNullOrEmpty(complaintDetailEncryptCode))
                {
                    response.IsSuccess = false;
                    response.Message = "Complaint detail link could not be generated for this member.";
                    return Json(response, JsonRequestBehavior.AllowGet);
                }

                string complaintLinkBaseUrl = ConfigurationManager.AppSettings["ComplaintLinkBaseUrl"] != null
                    ? ConfigurationManager.AppSettings["ComplaintLinkBaseUrl"].ToString()
                    : "https://dcpanel.barcouncilofgujarat.org/BoardComplaint/Detail/";
                string link = complaintLinkBaseUrl + complaintDetailEncryptCode;

                bool emailSent = false;
                string emailError = "";

                // 2. Fetch Email Configuration via Repository (SP_Email_Configuration_Get)
                EmailConfigurationModel emailConfig = _ComplaintDetailsRepository.GetEmailConfiguration("NoReplay");
                string host = "";
                int port = 25;
                string smtpUser = "";
                string smtpPass = "";
                bool enableSsl = false;

                if (emailConfig != null)
                {
                    host = (emailConfig.Host ?? "").Trim();
                    if (host.Equals("barcouncilofgujarat.org", StringComparison.OrdinalIgnoreCase))
                    {
                        host = "mail.barcouncilofgujarat.org";
                    }
                    port = emailConfig.Port > 0 ? emailConfig.Port : 25;
                    smtpUser = (emailConfig.UserName ?? "").Trim();
                    smtpPass = (emailConfig.Password ?? "").Trim();
                    enableSsl = emailConfig.SSL;
                }

                    // 3. Send Email using tbl_EmailConfiguration_Mst
                    if (!string.IsNullOrEmpty(memberEmail) && !string.IsNullOrEmpty(host) && !string.IsNullOrEmpty(smtpUser))
                    {
                        string emailBody = 
                            "<div style=\"font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; max-width: 650px; margin: 0 auto; background: #ffffff; border: 1px solid #e2e8f0; border-radius: 12px; overflow: hidden;\">" +
                            "  <div style=\"background: linear-gradient(135deg, #1e3a8a 0%, #3b82f6 100%); color: #ffffff; padding: 24px; text-align: center;\">" +
                            "    <h2 style=\"margin: 0; font-size: 20px; font-weight: 700;\">Bar Council of Gujarat</h2>" +
                            "    <p style=\"margin: 6px 0 0; font-size: 13px; opacity: 0.9;\">Board Complaint Details Notification</p>" +
                            "  </div>" +
                            "  <div style=\"padding: 28px 24px;\">" +
                            "    <p style=\"font-size: 15px; color: #1e293b; margin-top: 0;\">Dear <strong>" + HttpUtility.HtmlEncode(memberName) + "</strong>,</p>" +
                            "    <p style=\"font-size: 14px; color: #475569; line-height: 1.6;\">" +
                            "      You have been assigned as a committee member for the following complaint. Please click the button below to view the board complaint details and proceed with the necessary action:" +
                            "    </p>" +
                            "    <div style=\"background: #f8fafc; border: 1px solid #e2e8f0; border-radius: 8px; padding: 16px; margin: 20px 0;\">" +
                            "      <table style=\"width: 100%; border-collapse: collapse; font-size: 13px;\">" +
                            "        <tr>" +
                            "          <td style=\"padding: 6px 0; color: #64748b; font-weight: 600; width: 40%;\">Complaint Number:</td>" +
                            "          <td style=\"padding: 6px 0; color: #0f172a; font-weight: 700;\">" + HttpUtility.HtmlEncode(complaintNumber) + "</td>" +
                            "        </tr>" +
                            "        <tr>" +
                            "          <td style=\"padding: 6px 0; color: #64748b; font-weight: 600;\">Complainant Name:</td>" +
                            "          <td style=\"padding: 6px 0; color: #0f172a;\">" + HttpUtility.HtmlEncode(complainantName) + "</td>" +
                            "        </tr>" +
                            "      </table>" +
                            "    </div>" +
                            "    <div style=\"text-align: center; margin: 30px 0;\">" +
                            "      <a href=\"" + link + "\" target=\"_blank\" style=\"display: inline-block; background: linear-gradient(135deg, #2563eb 0%, #1d4ed8 100%); color: #ffffff; text-decoration: none; font-size: 14px; font-weight: 600; padding: 12px 28px; border-radius: 8px;\">" +
                            "        View Board Complaint Details" +
                            "      </a>" +
                            "    </div>" +
                            "    <p style=\"font-size: 12px; color: #64748b; line-height: 1.5; margin-bottom: 0;\">" +
                            "      If the button above does not work, copy and paste this link into your browser:<br/>" +
                            "      <a href=\"" + link + "\" target=\"_blank\" style=\"color: #2563eb; word-break: break-all;\">" + link + "</a>" +
                            "    </p>" +
                            "  </div>" +
                            "  <div style=\"background: #f1f5f9; padding: 16px 24px; text-align: center; font-size: 12px; color: #64748b; border-top: 1px solid #e2e8f0;\">" +
                            "    This is an automated notification from Bar Council of Gujarat. Please do not reply directly to this email." +
                            "  </div>" +
                            "</div>";

                        string[] hostCandidates = new string[] { host, "mail.barcouncilofgujarat.org", "68.178.173.156" };
                        List<string> hostsToTry = new List<string>();
                        foreach (string h in hostCandidates)
                        {
                            if (!string.IsNullOrEmpty(h) && !hostsToTry.Contains(h))
                            {
                                hostsToTry.Add(h);
                            }
                        }

                        foreach (string currentHost in hostsToTry)
                        {
                            try
                            {
                                using (MailMessage mail = new MailMessage())
                                {
                                    mail.From = new MailAddress(smtpUser, "Bar Council of Gujarat");
                                    mail.To.Add(memberEmail);
                                    mail.Subject = "Board Complaint Details: " + (!string.IsNullOrEmpty(complaintNumber) ? complaintNumber : "Bar Council of Gujarat");
                                    mail.Body = emailBody;
                                    mail.IsBodyHtml = true;

                                    using (SmtpClient smtp = new SmtpClient(currentHost, port))
                                    {
                                        smtp.Credentials = new NetworkCredential(smtpUser, smtpPass);
                                        smtp.EnableSsl = enableSsl;
                                        smtp.Timeout = 10000;
                                        smtp.Send(mail);
                                        emailSent = true;
                                        emailError = "";
                                        break;
                                    }
                                }
                            }
                            catch (Exception mailEx)
                            {
                                emailError = mailEx.Message;
                            }
                        }
                    }

                    // 4. Send SMS
                    bool smsSent = false;
                    try
                    {
                        string sContactNumber = !string.IsNullOrEmpty(mobileNumber) ? mobileNumber.Trim() : memberMobile;
                        if (!string.IsNullOrEmpty(sContactNumber))
                        {
                            // Keep digits only
                            sContactNumber = System.Text.RegularExpressions.Regex.Replace(sContactNumber, @"[^\d]", "");
                            if (sContactNumber.Length == 12 && sContactNumber.StartsWith("91"))
                            {
                                sContactNumber = sContactNumber.Substring(2);
                            }

                            // Standard ASCII hyphen (-) to match registered DLT template
                            string message = "Complaint Assigned to You - The Bar Council of Gujarat. Please click the link below to view the complaint details and take the necessary action: " + link;
                            string url = "http://sms.hspsms.com/sendSMS?username=BCGSMS&message=" + HttpUtility.UrlEncode(message) + "&sendername=BCGMSG&smstype=TRANS&numbers=" + sContactNumber + "&apikey=8f381652-f622-4473-b934-212a9ff295f0&pe_id=1201160742985056427&template_id=1707173416339106900";
                            using (WebClient webClient = new WebClient())
                            {
                                string smsResponse = webClient.DownloadString(url);
                                if (!string.IsNullOrEmpty(smsResponse) && smsResponse.IndexOf("Success", StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    smsSent = true;
                                }
                            }
                        }
                    }
                    catch { }

                    response.IsSuccess = true;
                    if (emailSent && smsSent)
                    {
                        response.Message = "Complaint link sent successfully via Email (" + memberEmail + ") and SMS.";
                    }
                    else if (emailSent)
                    {
                        response.Message = "Complaint link sent successfully via Email (" + memberEmail + ").";
                    }
                    else if (smsSent)
                    {
                        response.Message = "Complaint link sent via SMS." + (!string.IsNullOrEmpty(emailError) ? " (Email status: " + emailError + ")" : "");
                    }
                    else
                    {
                        response.Message = "Complaint link generated: " + link + (!string.IsNullOrEmpty(emailError) ? " (Email: " + emailError + ")" : "");
                    }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SendComplaintLink(int dcId, string mobileNumber, string emailAddress = "")
        {
            return SendComplaintLinkSMS(dcId, mobileNumber, emailAddress);
        }

        [HttpPost]
        public ActionResult UpdateMemberEmail(int memberLogId, string emailAddress)
        {
            ResultResponse response = new ResultResponse();
            try
            {
                int rows = _ComplaintDetailsRepository.UpdateComplaintMemberEmail(memberLogId, (emailAddress ?? "").Trim());
                if (rows > 0 || rows == -1)
                {
                    response.IsSuccess = true;
                    response.Message = "Email address updated successfully.";
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Failed to update email address.";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Failed to update email: " + ex.Message;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UploadAffidavitCopy(String ComplaintId, String FileName, String Base64File)
        {
            try
            {
                String FilePath = string.Empty;
                String actualFileName = String.Empty;
                String fileExtension = String.Empty;

                if (!string.IsNullOrEmpty(Base64File))
                {
                    String[] fileParts = Base64File.Split(',');
                    String base64Data = fileParts.Length > 1 ? fileParts[1].Trim() : fileParts[0].Trim();

                    byte[] fileBytes = Convert.FromBase64String(base64Data);
                    MemoryStream fileMS = new MemoryStream(fileBytes);

                    String directoryPath = System.Web.Hosting.HostingEnvironment.MapPath("~/AffidavitFile/");
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    fileExtension = Path.GetExtension(FileName);
                    // generate unique file name to prevent overriding
                    actualFileName = Guid.NewGuid().ToString().Substring(0, 8) + "_" + FileName.Replace(' ', '_');
                    String physicalPath = directoryPath + actualFileName;
                    FilePath = ImageUrl + "AffidavitFile/" + actualFileName;

                    using (FileStream fileStream = new FileStream(physicalPath, FileMode.Create))
                    {
                        fileMS.WriteTo(fileStream);
                    }
                    fileMS.Close();
                }

                Int64 CreateUser = Convert.ToInt64(Request.Cookies["CMPUserId"].Value);
                Int64 IComplaintId = Convert.ToInt64(ComplaintId);

                var result = _ComplaintRepository.ComplaintAffidavitCopyUpdate(IComplaintId, FileName, FilePath, fileExtension, CreateUser, "ComplaintAffidavitCopyUpdate");

                if (result.Count > 0 && result[0].ReturnStatus == "UD")
                {
                    response.IsSuccess = true;
                    response.Message = "Affidavit document updated successfully.";
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Failed to update affidavit document.";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UploadAdditionalAttachment(String Id, String ComplaintId, String FileTitle, String FileName, String Base64File, String Description)
        {
            try
            {
                String FilePath = string.Empty;
                String actualFileName = String.Empty;
                String fileExtension = String.Empty;

                if (!string.IsNullOrEmpty(Base64File))
                {
                    String[] fileParts = Base64File.Split(',');
                    String base64Data = fileParts.Length > 1 ? fileParts[1].Trim() : fileParts[0].Trim();

                    byte[] fileBytes = Convert.FromBase64String(base64Data);
                    MemoryStream fileMS = new MemoryStream(fileBytes);

                    String directoryPath = System.Web.Hosting.HostingEnvironment.MapPath("~/AdditionalAttachment/");
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    fileExtension = Path.GetExtension(FileName);
                    actualFileName = Guid.NewGuid().ToString().Substring(0, 8) + "_" + FileName.Replace(' ', '_');
                    String physicalPath = directoryPath + actualFileName;
                    FilePath = ImageUrl + "AdditionalAttachment/" + actualFileName;

                    using (FileStream fileStream = new FileStream(physicalPath, FileMode.Create))
                    {
                        fileMS.WriteTo(fileStream);
                    }
                    fileMS.Close();
                }

                Int64 CreateUser = Convert.ToInt64(Request.Cookies["CMPUserId"] == null ? "0" : Request.Cookies["CMPUserId"].Value.ToString() == "" ? "0" : Request.Cookies["CMPUserId"].Value.ToString());
                Int64 IId = Convert.ToInt64(Id == null ? "0" : Id == "" ? "0" : Id);
                Int64 IComplaintId = Convert.ToInt64(ComplaintId == null ? "0" : ComplaintId == "" ? "0" : ComplaintId);

                var result = _ComplaintRepository.ComplaintAdditionalAttachmentsUpdate(IId, IComplaintId, FileTitle, FileName, FilePath, Description, fileExtension, CreateUser, "ComplaintAdditionalAttachmentsSave");

                if (result.Count > 0 && result[0].ReturnStatus == "IN")
                {
                    response.IsSuccess = true;
                    response.Message = "Additional attachment uploaded successfully.";
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Failed to upload additional attachment.";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteAdditionalAttachment(Int64 Id)
        {
            try
            {
                Int64 CreateUser = Convert.ToInt64(Request.Cookies["CMPUserId"].Value);

                var result = _ComplaintRepository.ComplaintAdditionalAttachmentsUpdate(Id, 0, "", "", "", "", "", CreateUser, "ComplaintAdditionalAttachmentDelete");

                if (result.Count > 0 && result[0].ReturnStatus == "DE")
                {
                    response.IsSuccess = true;
                    response.Message = "Attachment deleted successfully.";
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Failed to delete attachment.";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetSearchData(String SearchTitle, int PageNo = 1, int PageSize = 10)
        {
            try
            {
                if (string.IsNullOrEmpty(SearchTitle))
                {
                    return Json(new { data = new List<EnrollmentMstModel>(), totalRecord = 0 }, JsonRequestBehavior.AllowGet);
                }

                var result = _ComplaintDetailsRepository.GetAdvocateSearchList(SearchTitle, PageNo, PageSize);

                if (result != null && result.Count > 0)
                {
                    long totalRecord = result[0].CntGRNo;
                    return Json(new { data = result, totalRecord = totalRecord }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { data = new List<EnrollmentMstModel>(), totalRecord = 0 }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { data = new List<EnrollmentMstModel>(), totalRecord = 0, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SearchDetail()

        {
            HttpCookie cookie = Request.Cookies["bcg"];
            if (cookie == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult GetOtherList(String ComplaintId)
        {
            try
            {
                Int64 Id = Convert.ToInt64(ComplaintId);

                var ComplainAgainstList = _ComplaintRepository.GetAdjuntList(Id);

                if (ComplainAgainstList.Count > 0)
                {
                    response.IsSuccess = true;
                    response.Data = ComplainAgainstList;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveComplaintStatusLog(String NextDate, String Summary, String ComplaintId, String CompalintStatus, String OrderFileName, String OrderFilePath)
        {
            try
            {
                String FilePath = string.Empty;
                String NewDate = String.Empty;

                if (OrderFilePath != "")
                {
                    if (OrderFilePath != null)
                    {
                        String[] Imgarr = OrderFilePath.ToString().Split(',');
                        String NewIPImagePath = Imgarr[1].ToString().Trim();
                        String ImageName = String.Empty;

                        #region Byte Creation
                        byte[] ImageByte = String.IsNullOrEmpty(NewIPImagePath) ? new byte[0] : Convert.FromBase64String(NewIPImagePath);
                        #endregion

                        #region Memory Stream
                        MemoryStream ImageMS = new MemoryStream(ImageByte);
                        #endregion

                        #region Directory Paths
                        String FilePathImageS = System.Web.Hosting.HostingEnvironment.MapPath("~/OrderFile/");
                        #endregion

                        #region File Name + Directory Path
                        String FilePatDriver = String.Empty;
                        #endregion

                        #region Directory Create
                        if (!Directory.Exists(FilePathImageS))
                        {
                            Directory.CreateDirectory(FilePathImageS);
                        }
                        #endregion

                        #region File Stram Create To Memory Stream 1
                        if (OrderFilePath.Length != 0)
                        {
                            ImageName = OrderFileName.Replace(' ', '_');
                            FilePatDriver = FilePathImageS + ImageName;
                            FilePath = ImageUrl + "OrderFile/" + ImageName;
                            FileStream DriverImageFS = new FileStream(FilePatDriver, FileMode.Create);
                            ImageMS.WriteTo(DriverImageFS);
                            ImageMS.Close();
                            DriverImageFS.Close();
                            DriverImageFS.Dispose();
                        }
                        #endregion
                    }
                    else
                    {
                        FilePath = "";
                    }
                }
                else
                {
                    FilePath = "";
                }

                if (NextDate != "")
                {
                    if (NextDate != null)
                    {
                        NewDate = NextDate.ToString();
                    }
                    else
                    {
                        NewDate = System.DateTime.Now.ToString();
                    }
                }
                else
                {
                    NewDate = System.DateTime.Now.ToString();
                }

                ComplaintStatusLog data = new ComplaintStatusLog();
                data.ComplaintId = Convert.ToInt64(ComplaintId);
                data.Status = CompalintStatus.ToString();
                data.NextDate = Convert.ToDateTime(NewDate.ToString());
                data.Summary = Summary.ToString() == null ? "" : Summary.ToString() == "" ? "" : Summary.ToString();
                data.OrderFileName = OrderFileName.ToString() == null ? "" : OrderFileName.ToString() == "" ? "" : OrderFileName.ToString();
                data.OrderFilePath = FilePath;
                data.CreateUser = Convert.ToInt64(Request.Cookies["CMPUserId"].Value);

                var result = _ComplaintDetailsRepository.InsertComplaintStatusLog(data);

                if (result > 0)
                {
                    response.IsSuccess = true;
                }
                else
                {
                    response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetLogDetails(String ComplaintId, String Status, String CommitteeId)
        {
            try
            {
                if (Status == "Board Has Approved")
                {
                    var result = _ComplaintDetailsRepository.GetDcMemberList(Convert.ToInt64(ComplaintId), Status, Convert.ToInt64(CommitteeId));
                    if (result.Count > 0)
                    {
                        response.IsSuccess = true;
                        response.Data = result;
                    }
                    else
                    {
                        response.IsSuccess = false;
                    }
                }
                else
                {
                    var result = _ComplaintDetailsRepository.GetComplaintDetails(Convert.ToInt64(ComplaintId), Status);

                    if (result.Count > 0)
                    {
                        result[0].SNextDate = result[0].NextDate.ToString("dd-MM-yyyy");
                        response.IsSuccess = true;
                        response.Data = result;
                    }
                    else
                    {
                        response.IsSuccess = false;
                    }
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetDcMemberLists(String ComplaintId, String CommitteeId)
        {
            try
            {
                var result = _ComplaintDetailsRepository.GetDcMemberLists(Convert.ToInt64(ComplaintId), Convert.ToInt64(CommitteeId));

                if (result.Count > 0)
                {
                    // result[0].SNextDate = result[0].NextDate.ToString("dd-MM-yyyy");
                    response.IsSuccess = true;
                    response.Data = result;
                }
                else
                {
                    response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetDcNoList()
        {
            try
            {
                List<DcNumberList> result = _ComplaintDetailsRepository.GetDcNumberList();
                if (result.Count > 0)
                {
                    response.IsSuccess = true;
                    response.Data = result;
                }
                else
                {
                    response.IsSuccess = false;
                }

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetDcMemberList(String DcNo)
        {
            try
            {
                List<DcMemberList> result = _ComplaintDetailsRepository.GetDcMemberList(Convert.ToInt64(DcNo));
                if (result.Count > 0)
                {
                    response.IsSuccess = true;
                    response.Data = result;
                }
                else
                {
                    response.IsSuccess = false;
                }

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetDcMemberListNotExist(String DcNo)
        {
            try
            {
                List<DcMemberList> result = _ComplaintDetailsRepository.GetDcMemberListNotExist(Convert.ToInt64(DcNo));
                if (result.Count > 0)
                {
                    response.IsSuccess = true;
                    response.Data = result;
                }
                else
                {
                    response.IsSuccess = false;
                }

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveComplaintMemberLog(String ComplaintId, String CompalintStatus, MemebrList[] memberList, String CommitteeId, String DCReferenceNumber)
        {
            try
            {
                Int64 IComplaintId = Convert.ToInt64(ComplaintId == null ? "0" : ComplaintId == "" ? "0" : ComplaintId);
                List<ComplaintMemberLogSaveReturn> result = _ComplaintDetailsRepository.DCReferenceNumberUpdate(IComplaintId, DCReferenceNumber);
                if (result.ToList().Count > 0)
                {
                    string ReturnStatus = result[0].ReturnStatus == null ? "" : result[0].ReturnStatus;
                    if (ReturnStatus == "IN")
                    {
                        ComplaintMemberLog data = new ComplaintMemberLog();
                        foreach (var item in memberList)
                        {
                            String SComplaintId = ComplaintId == null ? "0" : ComplaintId == "" ? "0" : ComplaintId;
                            String SMemberId = item.MemberId == null ? "0" : item.MemberId == "" ? "0" : item.MemberId;
                            String SCommitteeId = CommitteeId == null ? "0" : CommitteeId == "" ? "0" : CommitteeId;
                            String URLID = SComplaintId + ":" + SMemberId + ":" + SCommitteeId;
                            String ComplaintDetailEncrypt = CryptoEngine.EncryptString(URLID, "sblw-3hn8-sqoy19");

                            data.ComplaintId = Convert.ToInt64(ComplaintId);
                            data.Status = CompalintStatus.ToString();
                            data.MemberId = Convert.ToInt64(item.MemberId.ToString());
                            data.CommitteeId = Convert.ToInt64(CommitteeId.ToString());
                            data.IsChecked = item.IsChecked.ToString();
                            data.ComplaintDetailEncryptCode = ComplaintDetailEncrypt;
                            List<ComplaintMemberLogSaveReturn> resultdetail = _ComplaintDetailsRepository.InsertComplaintMemberLog(data, DCReferenceNumber);
                        }
                    }
                    response.Message = ReturnStatus;
                    response.IsSuccess = true;
                }
                else
                {
                    response.IsSuccess = false;
                }


            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetLogList(String ComplaintId) //String ComplaintId, String Status
        {
            try
            {
                var result = _ComplaintDetailsRepository.GetLogList(Convert.ToInt64(ComplaintId)); //Convert.ToInt64(ComplaintId), Status

                if (result.Count > 0)
                {
                    for (int i = 0; i < result.Count; i++)
                    {
                        if (result[i].NextDate != null)
                        {
                            if (result[i].NextDate.ToString() != "01-01-0001 12:00:00 AM")
                            {
                                result[i].SNextDate = result[i].NextDate.ToString("dd-MM-yyyy hh:mm tt");
                            }
                            else
                            {
                                result[i].SNextDate = DateTime.Now.ToString("dd-MM-yyyy hh:mm tt");
                            }
                        }
                        else
                        {
                            result[i].SNextDate = DateTime.Now.ToString("dd-MM-yyyy hh:mm tt");
                        }

                        result[i].OrderFileName = result[i].OrderFileName == null ? "" : result[i].OrderFileName == "" ? "" : result[i].OrderFileName;
                        result[i].OrderFilePath = result[i].OrderFilePath == null ? "" : result[i].OrderFilePath == "" ? "" : result[i].OrderFilePath;
                        result[i].Summary = result[i].Summary == null ? "" : result[i].Summary == "" ? "" : result[i].Summary;
                        if (string.Equals(result[i].Status, "New", StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(result[i].Summary))
                        {
                            result[i].Summary = "Complaint has been successfully registered in the system. It is currently under initial administrative verification and scrutiny.";
                        }
                        else if (string.Equals(result[i].Status, "Accepted", StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(result[i].Summary))
                        {
                            result[i].Summary = "Complaint has been scrutinized and accepted for further proceedings. The matter will be placed before the General Board for consideration.";
                        }
                        else if ((string.Equals(result[i].Status, "Refer to Board", StringComparison.OrdinalIgnoreCase) || string.Equals(result[i].Status, "Next General Board", StringComparison.OrdinalIgnoreCase)) && string.IsNullOrWhiteSpace(result[i].Summary))
                        {
                            result[i].Summary = "Complaint has been referred to the General Board for consideration. Awaiting the Board's review and decision in the upcoming meeting.";
                        }
                        else if (string.Equals(result[i].Status, "Board Accept", StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(result[i].Summary))
                        {
                            result[i].Summary = "The General Board has accepted the complaint. Pending formal board approval and committee allocation.";
                        }
                        result[i].OldStatus = result[i].OldStatus == null ? "" : result[i].OldStatus == "" ? "" : result[i].OldStatus;
                    }

                    response.IsSuccess = true;
                    response.Data = result;
                }
                else
                {
                    response.IsSuccess = false;
                }
                //}
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult StatusUpdate(String ComplaintId, String Status) //String ComplaintId, String Status
        {
            try
            {
                var result = _ComplaintDetailsRepository.UpdateStatus(Convert.ToInt64(ComplaintId), Status); //Convert.ToInt64(ComplaintId), Status

                if (result > 0)
                {
                    response.IsSuccess = true;
                }
                else
                {
                    response.IsSuccess = false;
                }
                //}
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetAllComplaintDetailsReportsFilterSet(String ComplaintId)
        {
            try
            {
                Response.Cookies["ComplaintId"].Value = ComplaintId;
                return Json(new { data = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //ExceptionInsert("2013", ex.GetType().ToString(), ex.Message.ToString(), ex.StackTrace.ToString(), ex.Source.ToString(), "GetAllInstitutesProgramsReportsListFilterSet()");
                return View("Error");
            }
        }

        [HttpGet]
        public ActionResult DownloadDetailReport()
        {
            try
            {

                String ComplaintId = "";

                #region ComplaintId
                if (Request.Cookies["ComplaintId"].Value != null)
                {
                    if (Request.Cookies["ComplaintId"].Value.ToString() != "")
                    {
                        ComplaintId = Request.Cookies["ComplaintId"].Value.ToString();
                    }
                    else
                    {
                        ComplaintId = "";
                    }
                }
                else
                {
                    ComplaintId = "";
                }
                #endregion

                List<ComplaintListModel> ComplaintDetail = new List<ComplaintListModel>();
                List<ComplaintAgainstList> ComplainAgainstList = new List<ComplaintAgainstList>();
                List<ComplaintDocumentList> ComplaintDocument = new List<ComplaintDocumentList>();
                List<ComplaintStatusLog> ActionLog = new List<ComplaintStatusLog>();

                ComplaintDetail = _ComplaintDetailsRepository.GetComplaintReportList(Convert.ToInt64(ComplaintId));
                ComplainAgainstList = _ComplaintDetailsRepository.GetComplaintAgaintsReportList(Convert.ToInt64(ComplaintId));
                ComplaintDocument = _ComplaintDetailsRepository.GetDocumentReportList(Convert.ToInt64(ComplaintId));
                ActionLog = _ComplaintDetailsRepository.GetLogListReport(Convert.ToInt64(ComplaintId));

                DataTable details = new DataTable();
                details.Columns.AddRange(new DataColumn[11] {
                        new DataColumn("SrNo", typeof(String)),
                        new DataColumn("FullName", typeof(String)),
                        new DataColumn("EmailAddress", typeof(String)),
                        new DataColumn("ContactNumber", typeof(String)),
                        new DataColumn("Summary", typeof(String)),
                        new DataColumn("DetailComplaint", typeof(String)),
                        new DataColumn("PaymentStatus", typeof(String)),
                        new DataColumn("Status", typeof(String)),
                        new DataColumn("ComplaintNumber", typeof(String)),
                        new DataColumn("TotalAmount", typeof(String)),
                        new DataColumn("Address", typeof(String)),
                    });

                if (ComplaintDetail.Count > 0)
                {
                    details.Rows.Add(ComplaintDetail[0].SrNo, ComplaintDetail[0].FullName, ComplaintDetail[0].EmailAddress, ComplaintDetail[0].ContactNumber, ComplaintDetail[0].Summary,
                        ComplaintDetail[0].DetailComplaint, ComplaintDetail[0].PaymentStatus, ComplaintDetail[0].Status, ComplaintDetail[0].ComplaintNumber, ComplaintDetail[0].TotalAmount,
                        ComplaintDetail[0].Address);
                }

                DataTable complaintAgainst = new DataTable();
                complaintAgainst.Columns.AddRange(new DataColumn[8] {
                         new DataColumn("SrNo",typeof(String)),
                        new DataColumn("Name",typeof(String)),
                        new DataColumn("EnrollmentNumber",typeof(String)),
                         new DataColumn("Amount",typeof(Decimal)),
                    new DataColumn("EnrollmentId",typeof(Decimal)),
                    new DataColumn("ContactNo",typeof(String)),
                     new DataColumn("IsKnown",typeof(String)),
                     new DataColumn("City",typeof(String)),
                });
                DataRow dr2 = null;

                if (ComplainAgainstList.Count > 0)
                {
                    for (int i = 0; i < ComplainAgainstList.Count; i++)
                    {
                        dr2 = complaintAgainst.NewRow();
                        dr2["SrNo"] = Convert.ToString(i + 1);
                        dr2["Amount"] = ComplainAgainstList[i].Amount;
                        dr2["Name"] = ComplainAgainstList[i].AdvocateName == null ? ComplainAgainstList[i].FullName : ComplainAgainstList[i].AdvocateName;
                        dr2["EnrollmentNumber"] = ComplainAgainstList[i].EnrollmentNo;
                        dr2["ContactNo"] = ComplainAgainstList[i].ContactNumber == null ? ComplainAgainstList[i].MobileNo : ComplainAgainstList[i].ContactNumber;
                        dr2["City"] = ComplainAgainstList[i].City == null ? "" : ComplainAgainstList[i].City;
                        complaintAgainst.Rows.Add(dr2);
                    }

                }

                DataTable document = new DataTable();
                document.Columns.AddRange(new DataColumn[6] {
                             new DataColumn("SrNo",typeof(String)),
                            new DataColumn("FileTitle",typeof(String)),
                            new DataColumn("FileName",typeof(String)),
                             new DataColumn("FilePath",typeof(String)),
                              new DataColumn("Description",typeof(String)),
                              new DataColumn("Name",typeof(String))
                        });
                DataRow drIND = null;




                if (ComplaintDocument.Count > 0)
                {
                    for (int i = 0; i < ComplaintDocument.Count; i++)
                    {

                        var file = "<a href=" + ComplaintDocument[i].FilePath + ">" + ComplaintDocument[i].FileName + "</a>";

                        drIND = document.NewRow();
                        drIND["SrNo"] = Convert.ToString(i + 1);
                        drIND["FileTitle"] = ComplaintDocument[i].FileTitle;
                        drIND["FileName"] = ComplaintDocument[i].FileName;
                        drIND["FilePath"] = file.ToString();
                        drIND["Description"] = ComplaintDocument[i].Description;
                        drIND["Name"] = ComplaintDocument[i].Name;
                        document.Rows.Add(drIND);
                    }
                }

                DataTable actionLog = new DataTable();
                actionLog.Columns.AddRange(new DataColumn[8] {
                    new DataColumn("SrNo",typeof(String)),
                    new DataColumn("Status",typeof(String)),
                    new DataColumn("NextDate",typeof(String)),
                    new DataColumn("FilePath",typeof(String)),
                    new DataColumn("FileName",typeof(String)),
                    new DataColumn("Summary",typeof(String)),
                    new DataColumn("CommitteeTitle",typeof(String)),
                    new DataColumn("OldStatus",typeof(String)),
                });

                DataRow drActionLog = null;

                if (ActionLog.Count > 0)
                {
                    for (int i = 0; i < ActionLog.Count; i++)
                    {
                        var file = "<a href=" + ActionLog[i].OrderFilePath + ">" + ActionLog[i].OrderFileName + "</a>";

                        if (ActionLog[i].SNextDate == "")
                        {
                            ActionLog[i].SNextDate = DateTime.Now.ToString("dd-MM-yyyy");
                        }
                        else
                        {
                            ActionLog[i].SNextDate = ActionLog[i].SNextDate;
                        }

                        drActionLog = actionLog.NewRow();
                        drActionLog["SrNo"] = Convert.ToString(i + 1);
                        drActionLog["Status"] = ActionLog[i].Status;
                        drActionLog["NextDate"] = ActionLog[i].SNextDate;
                        drActionLog["FilePath"] = ActionLog[i].OrderFilePath;
                        drActionLog["FileName"] = file;
                        drActionLog["Summary"] = ActionLog[i].Summary;
                        drActionLog["CommitteeTitle"] = ActionLog[i].CommitteTitle;
                        drActionLog["OldStatus"] = ActionLog[i].OldStatus;
                        actionLog.Rows.Add(drActionLog);
                    }

                }

                Complaint cmp = new Complaint();


                #region Company Detail

                DataTable dtCom = new DataTable();
                dtCom.Columns.Add("CompanyName", typeof(String));
                dtCom.Columns.Add("Address", typeof(String));
                dtCom.Columns.Add("ContactNumber", typeof(String));
                dtCom.Columns.Add("EmailAddress", typeof(String));
                dtCom.Columns.Add("Website", typeof(String));
                dtCom.Columns.Add("Logo", typeof(String));

                DataRow drCom = null;
                drCom = dtCom.NewRow();
                drCom["CompanyName"] = "The Bar Council of Gujarat";
                drCom["Address"] = "3rd Floor, Satyamev Complex, Opp High Court Of Gujarat, S.G. Highway, Ahmedabad - 380060, India.";
                drCom["ContactNumber"] = "079-27434073";
                drCom["EmailAddress"] = "mail@barcouncilofgujarat.org";
                drCom["Website"] = "https://www.barcouncilofgujarat.org/";
                drCom["Logo"] = "~/assets/img/logo.png";
                dtCom.Rows.Add(drCom);
                #endregion

                #region Report Header Title

                DataTable dtHT = new DataTable();
                dtHT.Columns.Add("ReportHeaderTitle", typeof(String));

                DataRow drHT = null;
                drHT = dtHT.NewRow();
                drHT["ReportHeaderTitle"] = "Complaintant Details";
                dtHT.Rows.Add(drHT);
                #endregion

                cmp.Tables["CompanyDetail"].Merge(dtCom);

                #region QAIcon Image
                for (int index = 0; index < cmp.Tables["CompanyDetail"].Rows.Count; index++)
                {
                    if (cmp.Tables["CompanyDetail"].Rows[index]["Logo"].ToString() != "")
                    {
                        string s = cmp.Tables["CompanyDetail"].Rows[index]["Logo"].ToString();
                        string[] pathArr = s.Split('/');
                        String fileNames = pathArr.Last().ToString();

                        string pdfPath = "~\\" + pathArr[1].ToString() + "\\" + pathArr[2].ToString() + "\\" + pathArr[3].ToString();
                        string p = "\\";
                        string path = Server.MapPath(pdfPath);
                        if (System.IO.File.Exists(path))
                        {
                            LoadImage(cmp.Tables["CompanyDetail"].Rows[index], "LogoStreem", path);
                        }
                        else
                        {
                            LoadImage(cmp.Tables["CompanyDetail"].Rows[index], "LogoStreem", "Logo");
                        }
                    }
                }
                #endregion

                cmp.Tables["ReportHeader"].Merge(dtHT);
                cmp.Tables["ComplaintDetail"].Merge(details);
                cmp.Tables["ComplaintAgainst"].Merge(complaintAgainst);
                cmp.Tables["ComplaintFiles"].Merge(document);
                cmp.Tables["ActionLog"].Merge(actionLog);


                ReportDocument rd = new ReportDocument();
                rd.Load(Path.Combine(Server.MapPath("~/Report/"), "ComplaintDetailsReport.rpt"));
                rd.SetDataSource(cmp);
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();

                Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                rd.Close();
                rd.Dispose();
                String fileName = "ComplaintDetailsReport" + DateTime.Now.ToString("ddMMyyyyHHmmss").ToString() + ".pdf";
                return File(stream, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return null;
        }

        #region Report Image load
        private void LoadImage(DataRow objDataRow, string strImageField, string FilePath)
        {
            try
            {
                if (FilePath != null)
                {
                    if (FilePath != "")
                    {
                        FileStream fs = new FileStream(FilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                        byte[] Image = new byte[fs.Length];
                        fs.Read(Image, 0, Convert.ToInt32(fs.Length));
                        fs.Close();
                        objDataRow[strImageField] = Image;
                    }
                }
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        [HttpPost]
        public ActionResult InsertComplaintReply(String ComplaintId, String ReplyMessage, String FileName, String FilePath)
        {
            try
            {
                String NewFilePath = String.Empty;
                String NewFileName = String.Empty;

                if (FilePath != null && FilePath != "")
                {
                    String[] Imgarr = FilePath.ToString().Split(',');
                    if (Imgarr.Length > 1)
                    {
                        String NewIPImagePath = Imgarr[1].ToString().Trim();

                        #region Byte Creation
                        byte[] ImageByte = String.IsNullOrEmpty(NewIPImagePath) ? new byte[0] : Convert.FromBase64String(NewIPImagePath);
                        #endregion

                        #region Memory Stream
                        MemoryStream ImageMS = new MemoryStream(ImageByte);
                        #endregion

                        #region Directory Paths
                        String FilePathImageS = System.Web.Hosting.HostingEnvironment.MapPath("~/ReplayDocuments/");
                        #endregion

                        #region Directory Create
                        if (!Directory.Exists(FilePathImageS))
                        {
                            Directory.CreateDirectory(FilePathImageS);
                        }
                        #endregion

                        if (FileName.Length != 0)
                        {
                            String ImageName = FileName.Replace(' ', '_');
                            string justFileName = Path.GetFileNameWithoutExtension(ImageName);
                            string extension = Path.GetExtension(ImageName);
                            NewFileName = justFileName + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss").ToString() + extension;

                            String FilePatDriver = Path.Combine(FilePathImageS, NewFileName);
                            NewFilePath = ImageUrl + "/ReplayDocuments/" + NewFileName;

                            FileStream DriverImageFS = new FileStream(FilePatDriver, FileMode.Create);
                            ImageMS.WriteTo(DriverImageFS);
                            ImageMS.Close();
                            DriverImageFS.Close();
                            DriverImageFS.Dispose();
                        }
                    }
                }

                long SenderId = Convert.ToInt64(Request.Cookies["CMPUserId"].Value);
                string SenderType = "A";
                string ReceiverType = "M";
                string Summery = ReplyMessage;
                string Comprehensive = "";

                var result = _ComplaintRepository.ComplaintComplaintReplySave(
                    Convert.ToInt64(ComplaintId),
                    SenderId,
                    SenderType,
                    ReceiverType,
                    Summery,
                    Comprehensive,
                    NewFileName,
                    NewFilePath,
                    "ComplaintComplaintReplySave"
                );

                if (result != null && result.Count > 0 && result[0].ReturnStatus == "IN")
                {
                    response.IsSuccess = true;
                }
                else
                {
                    response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                // Log exception
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetReplyList(String SenderId, String ComplaintId)
        {
            try
            {
                //Int64 ISenderId = Convert.ToInt64(SenderId == null ? "0" : SenderId == "" ? "0" : SenderId);
                long ISenderId = Convert.ToInt64(Request.Cookies["CMPUserId"] == null ? "0" : Request.Cookies["CMPUserId"].Value.ToString() == "" ? "0" : Request.Cookies["CMPUserId"].Value.ToString());
                Int64 IComplaintId = Convert.ToInt64(ComplaintId == null ? "0" : ComplaintId == "" ? "0" : ComplaintId);
                var result = _ComplaintDetailsRepository.GetComplaintReplyList(ISenderId, IComplaintId);

                if (result.Count > 0)
                {
                    response.IsSuccess = true;
                    response.Data = result;
                }
                else
                {
                    response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetComplaintCurrentStatus(String ComplaintId)
        {
            try
            {
                Int64 Id = Convert.ToInt64(ComplaintId);

                var ComplaintCurrentStatus = _ComplaintRepository.GetComplaintCurrentStatus(Id);

                response.IsSuccess = ComplaintCurrentStatus.ToList().Count > 0 ? true : false;
                response.Data = ComplaintCurrentStatus;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetComplaintPaymentDetail(String ComplaintId)
        {
            try
            {
                Int64 Id = Convert.ToInt64(ComplaintId);

                var ComplaintPaymentDetail = _ComplaintRepository.GetComplaintPaymentDetail(Id);

                response.IsSuccess = ComplaintPaymentDetail.ToList().Count > 0 ? true : false;
                response.Data = ComplaintPaymentDetail;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetComplaintPaymentReceived(String ComplaintId, String PaidAmount, String PaidOn, String PaymentMode, String TransactionNumber, String TransactionDate, String BankId, String ReceiptNumber, String ReceiptDate)
        {
            try
            {
                Int64 Id = Convert.ToInt64(ComplaintId);

                Int64 IComplaintId = Convert.ToInt64(ComplaintId == null ? "0" : ComplaintId == "" ? "0" : ComplaintId);
                Decimal DPaidAmount = Convert.ToDecimal(PaidAmount == null ? "0" : PaidAmount == "" ? "0" : PaidAmount);
                Int64 IBankId = Convert.ToInt64(BankId == null ? "0" : BankId == "" ? "0" : BankId);
                Int64 IPaidById = Convert.ToInt64(Request.Cookies["CMPUserId"] == null ? "0" : Request.Cookies["CMPUserId"].Value.ToString() == "" ? "0" : Request.Cookies["CMPUserId"].Value.ToString());
                String SPaidOn = PaidOn == null ? "" : PaidOn;
                String SPaymentMode = PaymentMode == null ? "" : PaymentMode;
                String STransactionNumber = TransactionNumber == null ? "" : TransactionNumber;
                DateTime DTransactionDate = Convert.ToDateTime(TransactionDate == null ? System.DateTime.Now.ToString() : TransactionDate == "" ? System.DateTime.Now.ToString() : TransactionDate);
                Int64 IReceiptNumber = Convert.ToInt64(ReceiptNumber == null ? "0" : ReceiptNumber == "" ? "0" : ReceiptNumber);
                DateTime DReceiptDate = Convert.ToDateTime(ReceiptDate == null ? System.DateTime.Now.ToString() : ReceiptDate == "" ? System.DateTime.Now.ToString() : ReceiptDate);

                var ComplaintPaymentDetail = _ComplaintRepository.GetComplaintPaymentReceived(IComplaintId, IReceiptNumber, DReceiptDate, DPaidAmount, IPaidById, SPaidOn, SPaymentMode, STransactionNumber, DTransactionDate, IBankId, IPaidById);

                response.IsSuccess = ComplaintPaymentDetail.ToList().Count > 0 ? true : false;
                response.Data = ComplaintPaymentDetail;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetComplaintBankList()
        {
            try
            {
                var ComplaintBankList = _ComplaintRepository.GetComplaintBankList();

                response.IsSuccess = ComplaintBankList.ToList().Count > 0 ? true : false;
                response.Data = ComplaintBankList;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ComplaintAgainstUpdate(String Id, String EnrollmentNumber)
        {
            try
            {
                Int64 IID = Convert.ToInt64(Id == null ? "0" : Id == "" ? "0" : Id);

                List<ComplaintAgainstDetail> ComplaintAgainstDetail = _ComplaintDetailsRepository.ComplaintAgainstDetail(EnrollmentNumber, "ComplaintAgainstDetail");

                if (ComplaintAgainstDetail.ToList().Count > 0)
                {
                    Int64 IEId = Convert.ToInt64(ComplaintAgainstDetail.ToList().Count == 0 ? "0" : ComplaintAgainstDetail[0].EId == null ? "0" : ComplaintAgainstDetail[0].EId.ToString() == "" ? "0" : ComplaintAgainstDetail[0].EId.ToString());
                    String SFullName = ComplaintAgainstDetail.ToList().Count == 0 ? "" : ComplaintAgainstDetail[0].FullName == null ? "" : ComplaintAgainstDetail[0].FullName.ToString();
                    String SCityName = ComplaintAgainstDetail.ToList().Count == 0 ? "" : ComplaintAgainstDetail[0].CityName == null ? "" : ComplaintAgainstDetail[0].CityName.ToString();
                    String SContactNumber = ComplaintAgainstDetail.ToList().Count == 0 ? "" : ComplaintAgainstDetail[0].ContactNumber == null ? "" : ComplaintAgainstDetail[0].ContactNumber.ToString();

                    List<ComplaintAgainstUpdate> result = _ComplaintDetailsRepository.ComplaintAgainstUpdate(IID, IEId, SFullName, SCityName, SContactNumber, "ComplaintAgainstUpdate");

                    if (result.ToList().Count > 0)
                    {
                        response.Data = result.ToList();
                        response.IsSuccess = true;
                    }
                    else
                    {
                        response.IsSuccess = false;
                    }
                }
                else
                {
                    response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ComplaintAgainstSave(String Id, String EnrollmentNumber)
        {
            try
            {
                Int64 IID = Convert.ToInt64(Id == null || Id == "" ? "0" : Id);
                Int64 CreateUser = Convert.ToInt64(Request.Cookies["CMPUserId"] == null || Request.Cookies["CMPUserId"].Value.ToString() == "" ? "0" : Request.Cookies["CMPUserId"].Value.ToString());

                List<ComplaintAgainstDetail> ComplaintAgainstDetail = _ComplaintDetailsRepository.ComplaintAgainstDetail(EnrollmentNumber, "ComplaintAgainstDetail");

                if (ComplaintAgainstDetail.ToList().Count > 0)
                {
                    Int64 IEId = Convert.ToInt64(ComplaintAgainstDetail.ToList().Count == 0 || ComplaintAgainstDetail[0].EId == null || ComplaintAgainstDetail[0].EId.ToString() == "" ? "0" : ComplaintAgainstDetail[0].EId.ToString());

                    List<ComplaintAgainstUpdate> result = _ComplaintDetailsRepository.ComplaintAgainstSave(IID, IEId, CreateUser, "ComplaintAgainstAdd");

                    if (result != null && result.Count > 0)
                    {
                        response.Data = result.ToList();
                        response.IsSuccess = true;
                    }
                    else
                    {
                        response.IsSuccess = false;
                    }
                }
                else
                {
                    response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetComplaintPaymentReceivedLog(String ComplaintId)
        {
            try
            {
                Int64 Id = Convert.ToInt64(ComplaintId);
                List<ComplaintPaymentReceivedLog> cpr = new List<ComplaintPaymentReceivedLog>();
                Int64 IComplaintId = Convert.ToInt64(ComplaintId == null ? "0" : ComplaintId == "" ? "0" : ComplaintId);
                cpr = _ComplaintRepository.GetComplaintPaymentReceivedLog(IComplaintId);

                response.IsSuccess = cpr.ToList().Count > 0 ? true : false;
                response.Data = cpr.ToList();
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ComplaintGBMeetingLogSave(String Id, String ComplaintId, String MeetingTitle, String MeetingDate, String MeetingNote, String ActionMode)
        {
            try
            {
                Int64 IId = Convert.ToInt64(Id == null ? "0" : Id.ToString() == "" ? "0" : Id.ToString());
                Int64 IComplaintId = Convert.ToInt64(ComplaintId == null ? "0" : ComplaintId.ToString() == "" ? "0" : ComplaintId.ToString());
                DateTime DMeetingDate = Convert.ToDateTime(MeetingDate == null ? System.DateTime.Now.ToString() : MeetingDate.ToString() == "" ? System.DateTime.Now.ToString() : MeetingDate.ToString());
                Int64 CreatedUserId = Convert.ToInt64(Request.Cookies["CMPUserId"] == null ? "0" : Request.Cookies["CMPUserId"].Value.ToString() == "" ? "0" : Request.Cookies["CMPUserId"].Value.ToString());
                String action = String.IsNullOrEmpty(ActionMode) ? (IId > 0 ? "Update" : "Save") : ActionMode;
                List<ComplaintGBMeetingLogSaveReturn> result = _ComplaintRepository.ComplaintGBMeetingLogSave(IId, IComplaintId, MeetingTitle, DMeetingDate, MeetingNote, CreatedUserId, action); //Convert.ToInt64(ComplaintId), Status
                if (result != null)
                {
                    if (result.ToList().Count > 0)
                    {
                        response.Data = result;
                        response.IsSuccess = true;
                    }
                    else
                    {
                        response.Data = null;
                        response.IsSuccess = false;
                    }
                }
                else
                {
                    response.Data = null;
                    response.IsSuccess = false;
                }
                //}
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetComplaintGBMeetingLogDetail(String Id)
        {
            try
            {
                Int64 IId = Convert.ToInt64(Id == null ? "0" : Id.ToString() == "" ? "0" : Id.ToString());
                List<ComplaintGBMeetingLogEdit> cpr = new List<ComplaintGBMeetingLogEdit>();
                cpr = _ComplaintRepository.GetComplaintGBMeetingLogEdit(IId);
                response.IsSuccess = cpr.ToList().Count > 0 ? true : false;
                response.Data = cpr.ToList();
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetComplaintGBMeetingLogList(String ComplaintId)
        {
            try
            {
                Int64 IComplaintId = Convert.ToInt64(ComplaintId == null ? "0" : ComplaintId.ToString() == "" ? "0" : ComplaintId.ToString());
                List<ComplaintGBMeetingLog> cpr = new List<ComplaintGBMeetingLog>();
                cpr = _ComplaintRepository.GetComplaintGBMeetingLogList(IComplaintId);
                response.IsSuccess = cpr.ToList().Count > 0 ? true : false;
                response.Data = cpr.ToList();
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ComplaintReplyDelete(String Id)
        {
            try
            {
                Int64 IId = Convert.ToInt64(Id == null ? "0" : Id.ToString() == "" ? "0" : Id.ToString());
                Int64 CreatedUserId = Convert.ToInt64(Request.Cookies["CMPUserId"] == null ? "0" : Request.Cookies["CMPUserId"].Value.ToString() == "" ? "0" : Request.Cookies["CMPUserId"].Value.ToString());
                List<ComplaintGBMeetingLogSaveReturn> result = _ComplaintRepository.ComplaintReplyDelete(IId, CreatedUserId, "DeleteUpdate");
                if (result != null)
                {
                    if (result.ToList().Count > 0)
                    {
                        response.Data = result;
                        response.IsSuccess = true;
                    }
                    else
                    {
                        response.Data = null;
                        response.IsSuccess = false;
                    }
                }
                else
                {
                    response.Data = null;
                    response.IsSuccess = false;
                }
                //}
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ShareComplaintWithMembers(String ComplaintId, List<String> MemberIds)
        {
            try
            {
                Int64 userId = 0;
                if (Session["UserId"] != null)
                {
                    userId = Convert.ToInt64(Session["UserId"]);
                }

                if (MemberIds != null && MemberIds.Count > 0)
                {
                    bool result = _ComplaintDetailsRepository.ShareComplaintWithMembersDynamic(Convert.ToInt64(ComplaintId), MemberIds, userId);
                    response.IsSuccess = result;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "No members selected.";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetComplaintShareMembers(String ComplaintId)
        {
            try
            {
                List<DcMemberList> result = _ComplaintDetailsRepository.GetComplaintShareMembers(Convert.ToInt64(ComplaintId));
                if (result.Count > 0)
                {
                    response.IsSuccess = true;
                    response.Data = result;
                }
                else
                {
                    response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
    }
}

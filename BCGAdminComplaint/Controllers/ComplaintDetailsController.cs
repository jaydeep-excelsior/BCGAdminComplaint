using BCGAdminComplaint;
using BCGAdminComplaint.Models;
using BCGAdminComplaint.Repository;
using System.Text;
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
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing;

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
                        var dcObj = result.FirstOrDefault(x => x.Id == DcMemberList[0].CommitteeId);
                        ViewBag.DcCommitteeTitle = dcObj != null ? dcObj.DcTitle : DcMemberList[0].CommitteeId.ToString();
                        ViewBag.MemberList = dcMember;
                    }
                    else
                    {
                        ViewBag.DcNumber = "0";
                        ViewBag.DcCommitteeTitle = "";
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

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult DetailsNew(Int64? Id = null)
        {
            HttpCookie cookie = Request.Cookies["bcg"];
            if (cookie == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                if (Id.HasValue && Id.Value > 0)
                {
                    Session["ComplaintId"] = Id.Value;
                    HttpCookie compCookie = new HttpCookie("ComplaintId", Id.Value.ToString());
                    compCookie.Path = "/";
                    Response.Cookies.Add(compCookie);

                    if (Request.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase))
                    {
                        return RedirectToAction("DetailsNew", "ComplaintDetails");
                    }
                }

                Int64 targetId = 0;
                Int64 sId = 0;
                Int64 cId = 0;
                if (Id.HasValue && Id.Value > 0)
                {
                    targetId = Id.Value;
                }
                else if (Session["ComplaintId"] != null && Int64.TryParse(Session["ComplaintId"].ToString(), out sId) && sId > 0)
                {
                    targetId = sId;
                }
                else if (Request.Cookies["ComplaintId"] != null && Int64.TryParse(Request.Cookies["ComplaintId"].Value, out cId) && cId > 0)
                {
                    targetId = cId;
                    Session["ComplaintId"] = targetId;
                }

                if (targetId > 0)
                {
                    Session["ComplaintId"] = targetId;
                    HttpCookie compCookie = new HttpCookie("ComplaintId", targetId.ToString());
                    compCookie.Path = "/";
                    Response.Cookies.Add(compCookie);

                    try
                    {

                        List<ComplaintListModel> ComplaintDetail = new List<ComplaintListModel>();
                        List<ComplaintAgainstList> ComplainAgainstList = new List<ComplaintAgainstList>();
                        List<ComplaintDocumentList> ComplaintDocument = new List<ComplaintDocumentList>();
                        List<ComplaintMemberLog> DcMemberList = new List<ComplaintMemberLog>();
                        List<DcNumberList> result = new List<DcNumberList>();
                        List<ComplaintGBMeetingLog> ComplaintGBMeeting = new List<ComplaintGBMeetingLog>();

                        ComplaintDetail = _ComplaintRepository.GetComplaintDetails(targetId);
                        if (ComplaintDetail == null || ComplaintDetail.Count == 0)
                        {
                            return RedirectToAction("Index", "ComplaintList");
                        }
                        // ComplainAgainstList = _ComplaintRepository.GetAdjuntList(targetId);
                        ComplaintDocument = _ComplaintRepository.GetDocumentList(targetId);
                        ComplaintGBMeeting = _ComplaintRepository.GetComplaintGBMeetingLogList(targetId);
                        DcMemberList = _ComplaintRepository.GetDcMemberList(targetId);
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
                            var dcObj = result.FirstOrDefault(x => x.Id == DcMemberList[0].CommitteeId);
                            ViewBag.DcCommitteeTitle = dcObj != null ? dcObj.DcTitle : DcMemberList[0].CommitteeId.ToString();
                            ViewBag.MemberList = dcMember;
                        }
                        else
                        {
                            ViewBag.DcNumber = "0";
                            ViewBag.DcCommitteeTitle = "";
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
                        result[i].Summary = result[i].Summary == null ? "" : result[i].Summary.Trim();
                        if (string.IsNullOrWhiteSpace(result[i].Summary))
                        {
                            if (string.Equals(result[i].Status, "New", StringComparison.OrdinalIgnoreCase))
                            {
                                result[i].Summary = "Complaint has been successfully registered in the system. It is currently under initial administrative verification and scrutiny.";
                            }
                            else if (string.Equals(result[i].Status, "Accepted", StringComparison.OrdinalIgnoreCase))
                            {
                                result[i].Summary = "Complaint has been scrutinized and accepted for further proceedings. The matter will be placed before the General Board for consideration.";
                            }
                            else if (string.Equals(result[i].Status, "Refer to Board", StringComparison.OrdinalIgnoreCase) || string.Equals(result[i].Status, "Next General Board", StringComparison.OrdinalIgnoreCase))
                            {
                                result[i].Summary = "Complaint has been referred to the General Board for consideration. Awaiting the Board's review and decision in the upcoming meeting.";
                            }
                            else if (string.Equals(result[i].Status, "Board Accept", StringComparison.OrdinalIgnoreCase))
                            {
                                result[i].Summary = "The General Board has accepted the complaint. Pending formal board approval and committee allocation.";
                            }
                            else if (string.Equals(result[i].Status, "Board Has Approved", StringComparison.OrdinalIgnoreCase))
                            {
                                result[i].Summary = "The General Board has formally approved the complaint. The matter has been referred to the Disciplinary Committee for conducting inquiry and hearing proceedings.";
                            }
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
                HttpCookie c = new HttpCookie("ComplaintId", ComplaintId ?? "");
                c.Path = "/";
                Response.Cookies.Add(c);
                Session["ComplaintId"] = ComplaintId;
                return Json(new { data = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //ExceptionInsert("2013", ex.GetType().ToString(), ex.Message.ToString(), ex.StackTrace.ToString(), ex.Source.ToString(), "GetAllInstitutesProgramsReportsListFilterSet()");
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult SetComplaintId(Int64 Id)
        {
            try
            {
                if (Id > 0)
                {
                    Session["ComplaintId"] = Id;
                    HttpCookie compCookie = new HttpCookie("ComplaintId", Id.ToString());
                    compCookie.Path = "/";
                    Response.Cookies.Add(compCookie);
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, message = "Invalid Id" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult DownloadDetailReport(long? id = null, bool withAttachments = true)
        {
            try
            {
                long complaintId = 0;
                if (id.HasValue && id.Value > 0)
                {
                    complaintId = id.Value;
                }
                else if (Request.Cookies["ComplaintId"] != null && Request.Cookies["ComplaintId"].Value != null)
                {
                    long.TryParse(Request.Cookies["ComplaintId"].Value.ToString(), out complaintId);
                }

                if (complaintId <= 0)
                {
                    return Content("Invalid Complaint ID specified.");
                }

                List<ComplaintListModel> complaintDetail = _ComplaintDetailsRepository.GetComplaintReportList(complaintId);
                List<ComplaintAgainstList> complainAgainstList = _ComplaintDetailsRepository.GetComplaintAgaintsReportList(complaintId);
                List<ComplaintDocumentList> complaintDocument = GetAllComplaintDocumentsWithAffidavit(complaintId, (complaintDetail != null && complaintDetail.Count > 0) ? complaintDetail[0] : null);
                List<ComplaintStatusLog> actionLog = _ComplaintDetailsRepository.GetLogListReport(complaintId);
                if (actionLog != null && actionLog.Count > 0)
                {
                    actionLog = actionLog.OrderBy(x => x.CreateDate).ThenByDescending(x => x.SrNo ?? 0).ToList();
                }

                if (complaintDetail == null || complaintDetail.Count == 0)
                {
                    return Content("Complaint record not found for ID: " + complaintId);
                }

                string html = GenerateComplaintDetailsPdfHtml(complaintDetail[0], complainAgainstList, complaintDocument, actionLog);
                byte[] pdfBytes = ConvertHtmlToPdf(html);

                byte[] finalPdfBytes = pdfBytes;
                if (withAttachments)
                {
                    finalPdfBytes = MergeDossierWithAttachments(pdfBytes, complaintDocument, actionLog, complaintDetail[0]);
                }

                string safeCmpNumber = (!string.IsNullOrEmpty(complaintDetail[0].ComplaintNumber)
                    ? complaintDetail[0].ComplaintNumber.Replace("/", "_").Replace("\\", "_")
                    : complaintId.ToString());

                string suffix = withAttachments ? "_WithAttached" : "_WithoutAttached";
                string fileName = "Complaint_" + safeCmpNumber + suffix + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".pdf";
                return File(finalPdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                return Content("Error generating Complaint PDF report: " + ex.Message);
            }
        }

        private List<ComplaintDocumentList> GetAllComplaintDocumentsWithAffidavit(long complaintId, ComplaintListModel complaint)
        {
            var combinedList = new List<ComplaintDocumentList>();
            var seenFilePaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // 1. Get from _ComplaintRepository.GetDocumentList (includes both Additional and Affidavit)
            try
            {
                var repoDocs = _ComplaintRepository.GetDocumentList(complaintId);
                if (repoDocs != null && repoDocs.Count > 0)
                {
                    foreach (var d in repoDocs)
                    {
                        if (!string.IsNullOrWhiteSpace(d.FilePath) && !seenFilePaths.Contains(d.FilePath.Trim()))
                        {
                            seenFilePaths.Add(d.FilePath.Trim());
                            if (string.Equals(d.DocumetType, "Affidavit", StringComparison.OrdinalIgnoreCase))
                            {
                                d.DocumetType = "Complaint Affidavit Copy";
                                if (string.IsNullOrEmpty(d.FileTitle)) d.FileTitle = "Complaint Affidavit Copy";
                            }
                            combinedList.Add(d);
                        }
                    }
                }
            }
            catch { }

            // 2. Get from _ComplaintDetailsRepository.GetDocumentReportList (ComplaintFilesReport)
            try
            {
                var reportDocs = _ComplaintDetailsRepository.GetDocumentReportList(complaintId);
                if (reportDocs != null && reportDocs.Count > 0)
                {
                    foreach (var d in reportDocs)
                    {
                        if (!string.IsNullOrWhiteSpace(d.FilePath) && !seenFilePaths.Contains(d.FilePath.Trim()))
                        {
                            seenFilePaths.Add(d.FilePath.Trim());
                            if (string.IsNullOrEmpty(d.DocumetType)) d.DocumetType = "Additional";
                            combinedList.Add(d);
                        }
                    }
                }
            }
            catch { }

            // 3. Get affidavit from _ComplaintRepository.GetComplainAffidavitCopyByComplaintNumber
            if (complaint != null && !string.IsNullOrEmpty(complaint.ComplaintNumber))
            {
                try
                {
                    var affCopies = _ComplaintRepository.GetComplainAffidavitCopyByComplaintNumber(complaint.ComplaintNumber);
                    if (affCopies != null && affCopies.Count > 0)
                    {
                        var activeAffs = affCopies.Where(x => !string.Equals(x.IsOld, "Y", StringComparison.OrdinalIgnoreCase)).ToList();
                        if (activeAffs.Count == 0) activeAffs = affCopies;

                        foreach (var aff in activeAffs)
                        {
                            if (!string.IsNullOrWhiteSpace(aff.FilePath) && !seenFilePaths.Contains(aff.FilePath.Trim()))
                            {
                                seenFilePaths.Add(aff.FilePath.Trim());
                                combinedList.Add(new ComplaintDocumentList
                                {
                                    ComplaintId = complaintId,
                                    DocumetType = "Complaint Affidavit Copy",
                                    FileTitle = "Complaint Affidavit Copy",
                                    FileName = !string.IsNullOrEmpty(aff.FileName) ? aff.FileName : "Complaint_Affidavit.pdf",
                                    FilePath = aff.FilePath,
                                    Description = "Official sworn affidavit submitted by complainant."
                                });
                            }
                            else
                            {
                                var existing = combinedList.FirstOrDefault(x => string.Equals(x.FilePath?.Trim(), aff.FilePath?.Trim(), StringComparison.OrdinalIgnoreCase));
                                if (existing != null)
                                {
                                    existing.DocumetType = "Complaint Affidavit Copy";
                                    existing.FileTitle = "Complaint Affidavit Copy";
                                }
                            }
                        }
                    }
                }
                catch { }
            }

            // 4. Online documents fallback
            try
            {
                var onlineDocs = _ComplaintRepository.GetOnlineDocumentList(complaintId);
                if (onlineDocs != null && onlineDocs.Count > 0)
                {
                    foreach (var d in onlineDocs)
                    {
                        if (!string.IsNullOrWhiteSpace(d.FilePath) && !seenFilePaths.Contains(d.FilePath.Trim()))
                        {
                            seenFilePaths.Add(d.FilePath.Trim());
                            combinedList.Add(d);
                        }
                    }
                }
            }
            catch { }

            // Sort: Complaint Affidavit Copy appears first, followed by additional attachments
            var sortedList = combinedList
                .OrderBy(x => (string.Equals(x.DocumetType, "Complaint Affidavit Copy", StringComparison.OrdinalIgnoreCase) || string.Equals(x.DocumetType, "Affidavit", StringComparison.OrdinalIgnoreCase)) ? 0 : 1)
                .ThenBy(x => x.SrNo ?? 0)
                .ToList();

            return sortedList;
        }

        private byte[] MergeDossierWithAttachments(byte[] dossierPdfBytes, List<ComplaintDocumentList> documents, List<ComplaintStatusLog> logs, ComplaintListModel complaint)
        {
            if (dossierPdfBytes == null || dossierPdfBytes.Length == 0)
            {
                return dossierPdfBytes;
            }

            try
            {
                using (MemoryStream outputMs = new MemoryStream())
                {
                    using (PdfDocument mergedDoc = new PdfDocument())
                    {
                        // 1. Add dossier summary report pages
                        using (MemoryStream dossierMs = new MemoryStream(dossierPdfBytes))
                        {
                            using (PdfDocument dossierDoc = PdfReader.Open(dossierMs, PdfDocumentOpenMode.Import))
                            {
                                for (int p = 0; p < dossierDoc.PageCount; p++)
                                {
                                    PdfPage page = mergedDoc.AddPage(dossierDoc.Pages[p]);
                                    if (p == 0)
                                    {
                                        string cmpNum = (complaint != null && !string.IsNullOrEmpty(complaint.ComplaintNumber)) ? complaint.ComplaintNumber : "Case Dossier";
                                        mergedDoc.Outlines.Add("Case Dossier: " + cmpNum, page, true);
                                    }
                                }
                            }
                        }

                        // 2. Add uploaded complaint documents & affidavit
                        if (documents != null && documents.Count > 0)
                        {
                            int docIndex = 1;
                            foreach (var doc in documents)
                            {
                                string filePath = doc.FilePath;
                                string fileName = !string.IsNullOrEmpty(doc.FileName) ? doc.FileName : (!string.IsNullOrEmpty(doc.FileTitle) ? doc.FileTitle : ("Document_" + docIndex));

                                bool isAffidavit = string.Equals(doc.DocumetType, "Affidavit", StringComparison.OrdinalIgnoreCase)
                                                || string.Equals(doc.DocumetType, "Complaint Affidavit Copy", StringComparison.OrdinalIgnoreCase)
                                                || string.Equals(doc.FileTitle, "Complaint Affidavit Copy", StringComparison.OrdinalIgnoreCase);

                                string outlineTitle;
                                if (isAffidavit)
                                {
                                    outlineTitle = "Complaint Affidavit Copy: " + fileName;
                                }
                                else
                                {
                                    string fileTitle = !string.IsNullOrEmpty(doc.FileTitle) ? doc.FileTitle : (!string.IsNullOrEmpty(doc.FileName) ? doc.FileName : (!string.IsNullOrEmpty(doc.DocumetType) ? doc.DocumetType : fileName));
                                    outlineTitle = "Attachment " + docIndex + ": " + fileTitle;
                                    docIndex++;
                                }

                                AppendFileToPdfDocument(mergedDoc, filePath, fileName, outlineTitle);
                            }
                        }

                        // 3. Add order files from status logs
                        if (logs != null && logs.Count > 0)
                        {
                            int logDocIndex = 1;
                            foreach (var log in logs)
                            {
                                if (!string.IsNullOrEmpty(log.OrderFilePath) && !string.IsNullOrEmpty(log.OrderFileName))
                                {
                                    string orderTitle = "Order Document " + logDocIndex + " (" + (!string.IsNullOrEmpty(log.Status) ? log.Status : "Order") + ")";
                                    AppendFileToPdfDocument(mergedDoc, log.OrderFilePath, log.OrderFileName, orderTitle);
                                    logDocIndex++;
                                }
                            }
                        }

                        mergedDoc.Save(outputMs, false);
                        return outputMs.ToArray();
                    }
                }
            }
            catch
            {
                return dossierPdfBytes;
            }
        }

        private void AppendFileToPdfDocument(PdfDocument targetDoc, string filePath, string fileName, string outlineTitle)
        {
            if (string.IsNullOrWhiteSpace(filePath)) return;

            byte[] fileBytes = FetchAttachmentBytes(filePath);
            if (fileBytes == null || fileBytes.Length == 0) return;

            bool isPdf = (fileBytes.Length >= 4 && fileBytes[0] == 0x25 && fileBytes[1] == 0x50 && fileBytes[2] == 0x44 && fileBytes[3] == 0x46)
                         || (!string.IsNullOrEmpty(fileName) && fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                         || (!string.IsNullOrEmpty(filePath) && filePath.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase));

            if (isPdf)
            {
                try
                {
                    using (MemoryStream ms = new MemoryStream(fileBytes))
                    {
                        using (PdfDocument attachDoc = PdfReader.Open(ms, PdfDocumentOpenMode.Import))
                        {
                            for (int i = 0; i < attachDoc.PageCount; i++)
                            {
                                PdfPage newPage = targetDoc.AddPage(attachDoc.Pages[i]);
                                if (i == 0 && !string.IsNullOrEmpty(outlineTitle))
                                {
                                    targetDoc.Outlines.Add(outlineTitle, newPage, true);
                                }
                            }
                        }
                    }
                    return;
                }
                catch { }
            }

            bool isImage = (!string.IsNullOrEmpty(fileName) && (fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".gif", StringComparison.OrdinalIgnoreCase)))
                           || (!string.IsNullOrEmpty(filePath) && (filePath.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || filePath.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) || filePath.EndsWith(".png", StringComparison.OrdinalIgnoreCase) || filePath.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase) || filePath.EndsWith(".gif", StringComparison.OrdinalIgnoreCase)));

            if (isImage)
            {
                try
                {
                    PdfPage imgPage = targetDoc.AddPage();
                    imgPage.Size = PdfSharp.PageSize.A4;
                    using (MemoryStream imgMs = new MemoryStream(fileBytes))
                    {
                        using (XImage img = XImage.FromStream(imgMs))
                        {
                            using (XGraphics gfx = XGraphics.FromPdfPage(imgPage))
                            {
                                double margin = 36.0;
                                double availW = imgPage.Width.Point - (2 * margin);
                                double availH = imgPage.Height.Point - (2 * margin);

                                double scale = Math.Min(availW / img.PointWidth, availH / img.PointHeight);
                                if (scale > 1.0) scale = 1.0;

                                double drawW = img.PointWidth * scale;
                                double drawH = img.PointHeight * scale;
                                double x = margin + ((availW - drawW) / 2.0);
                                double y = margin + ((availH - drawH) / 2.0);

                                gfx.DrawImage(img, x, y, drawW, drawH);
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(outlineTitle))
                    {
                        targetDoc.Outlines.Add(outlineTitle, imgPage, true);
                    }
                    return;
                }
                catch { }
            }
        }

        private byte[] FetchAttachmentBytes(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) return null;
            string path = filePath.Trim();

            // 1. If absolute HTTP/HTTPS URL
            if (path.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || path.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                // If URL points to localhost, resolve directly from disk first
                try
                {
                    Uri uri = new Uri(path);
                    if (uri.IsLoopback || string.Equals(uri.Host, "localhost", StringComparison.OrdinalIgnoreCase))
                    {
                        string localRel = uri.AbsolutePath;
                        if (!string.IsNullOrEmpty(localRel))
                        {
                            string mapped = System.Web.Hosting.HostingEnvironment.MapPath("~" + localRel);
                            if (!string.IsNullOrEmpty(mapped) && System.IO.File.Exists(mapped))
                            {
                                return System.IO.File.ReadAllBytes(mapped);
                            }
                        }
                    }
                }
                catch { }

                try
                {
                    ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    using (WebClient wc = new WebClient())
                    {
                        return wc.DownloadData(path);
                    }
                }
                catch
                {
                    try
                    {
                        Uri uri = new Uri(path);
                        string localRel = uri.AbsolutePath;
                        if (!string.IsNullOrEmpty(localRel))
                        {
                            string mapped = System.Web.Hosting.HostingEnvironment.MapPath("~" + localRel);
                            if (!string.IsNullOrEmpty(mapped) && System.IO.File.Exists(mapped))
                            {
                                return System.IO.File.ReadAllBytes(mapped);
                            }
                        }
                    }
                    catch { }
                }
                return null;
            }

            // 2. If virtual or relative path
            try
            {
                string virtualPath = path;
                if (virtualPath.StartsWith("/")) virtualPath = "~" + virtualPath;
                else if (!virtualPath.StartsWith("~")) virtualPath = "~/" + virtualPath;

                string mapped = System.Web.Hosting.HostingEnvironment.MapPath(virtualPath);
                if (!string.IsNullOrEmpty(mapped) && System.IO.File.Exists(mapped))
                {
                    return System.IO.File.ReadAllBytes(mapped);
                }
            }
            catch { }

            // 3. If direct physical path
            try
            {
                if (System.IO.File.Exists(path))
                {
                    return System.IO.File.ReadAllBytes(path);
                }
            }
            catch { }

            // 4. Fallback to portal hosts
            string[] baseHosts = new string[] {
                "https://www.barcouncilofgujarat.org",
                "https://dcoffice.barcouncilofgujarat.org"
            };

            foreach (var baseHost in baseHosts)
            {
                try
                {
                    string cleanRel = path.TrimStart('~').TrimStart('/');
                    string testUrl = baseHost + "/" + cleanRel;
                    ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    using (WebClient wc = new WebClient())
                    {
                        return wc.DownloadData(testUrl);
                    }
                }
                catch { }
            }

            return null;
        }

        private string RenderComplaintStatusPill(string status)
        {
            if (string.IsNullOrEmpty(status)) return "-";
            string s = status.Trim();
            string lower = s.ToLower();
            string badgeClass = "status-default";
            if (lower.Contains("accept") || lower.Contains("approved")) badgeClass = "status-accepted";
            else if (lower.Contains("reject")) badgeClass = "status-rejected";
            else if (lower.Contains("more detail") || lower.Contains("pending") || lower.Contains("payment")) badgeClass = "status-warning";
            else if (lower.Contains("board")) badgeClass = "status-info";

            return string.Format("<span class=\"status-pill {0}\">{1}</span>", badgeClass, HttpUtility.HtmlEncode(s.ToUpper()));
        }

        private string FormatPdfFileLink(string fileName, string filePath)
        {
            if (string.IsNullOrEmpty(fileName) || fileName.Trim() == "-")
            {
                return "-";
            }

            string rawPath = !string.IsNullOrEmpty(filePath) ? filePath.Trim() : "";
            if (string.IsNullOrEmpty(rawPath))
            {
                return "<strong>" + HttpUtility.HtmlEncode(fileName) + "</strong>";
            }

            string targetUrl = rawPath;
            if (!targetUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && !targetUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                if (targetUrl.StartsWith("~/"))
                {
                    targetUrl = targetUrl.Substring(1);
                }
                if (!targetUrl.StartsWith("/"))
                {
                    targetUrl = "/" + targetUrl;
                }
                string baseUrl = Request != null && Request.Url != null ? Request.Url.GetLeftPart(UriPartial.Authority) : "http://localhost:53316";
                targetUrl = baseUrl + targetUrl;
            }

            return string.Format("<a href=\"{0}\" target=\"_blank\" class=\"pdf-doc-link\" title=\"Open file in new tab\">{1}</a>",
                HttpUtility.HtmlAttributeEncode(targetUrl),
                HttpUtility.HtmlEncode(fileName));
        }

        private string GenerateComplaintDetailsPdfHtml(
            ComplaintListModel complaint,
            List<ComplaintAgainstList> advocates,
            List<ComplaintDocumentList> documents,
            List<ComplaintStatusLog> logs)
        {
            string logoBase64 = GetLogoBase64();
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang=\"en\">");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta charset=\"utf-8\" />");
            sb.AppendLine("<title>Complaint Dossier - " + HttpUtility.HtmlEncode(complaint.ComplaintNumber ?? "") + "</title>");
            sb.AppendLine("<style>");
            sb.AppendLine("@page { size: A4 portrait; margin: 10mm 12mm 10mm 12mm; }");
            sb.AppendLine("* { box-sizing: border-box; -webkit-print-color-adjust: exact !important; print-color-adjust: exact !important; }");
            sb.AppendLine("body { font-family: 'Segoe UI', 'Helvetica Neue', Arial, sans-serif; font-size: 10.5px; color: #1e293b; background: #fff; margin: 0; padding: 0; line-height: 1.4; }");

            // Header styling
            sb.AppendLine(".rpt-header { width: 100%; border-collapse: collapse; margin-bottom: 6px; }");
            sb.AppendLine(".rpt-header td { vertical-align: middle; padding: 0; }");
            sb.AppendLine(".rpt-header .logo-td { width: 56px; }");
            sb.AppendLine(".rpt-header .logo-td img { height: 48px; width: auto; display: block; }");
            sb.AppendLine(".rpt-header .title-td { text-align: center; padding: 0 10px; }");
            sb.AppendLine(".rpt-header .badge-td { width: 190px; text-align: right; }");
            sb.AppendLine(".org-name { font-size: 15px; font-weight: 800; color: #002b5c; text-transform: uppercase; letter-spacing: 0.5px; margin: 0 0 2px 0; }");
            sb.AppendLine(".org-addr { font-size: 8px; color: #64748b; margin: 0; line-height: 1.25; }");
            sb.AppendLine(".doc-badge { display: inline-block; background: #002b5c; color: #fff; padding: 3px 10px; border-radius: 4px; font-weight: 700; font-size: 9.5px; letter-spacing: 0.4px; text-transform: uppercase; }");
            sb.AppendLine(".doc-badge-sub { font-size: 8px; color: #64748b; font-weight: 600; margin-top: 2px; text-transform: uppercase; letter-spacing: 0.2px; }");

            // Divider
            sb.AppendLine(".hdr-divider { border: none; border-top: 2px solid #002b5c; margin: 4px 0 10px 0; }");

            // Case Hero Banner
            sb.AppendLine(".case-banner { width: 100%; border-collapse: collapse; background: #f8fafc; border: 1.5px solid #cbd5e1; border-radius: 6px; margin-bottom: 12px; }");
            sb.AppendLine(".case-banner td { padding: 8px 12px; vertical-align: middle; }");
            sb.AppendLine(".case-num { font-size: 14px; font-weight: 800; color: #002b5c; letter-spacing: 0.5px; font-family: 'Consolas', monospace; }");
            sb.AppendLine(".meta-label { font-size: 8.5px; font-weight: 600; color: #64748b; text-transform: uppercase; letter-spacing: 0.3px; display: block; margin-bottom: 2px; }");
            sb.AppendLine(".meta-val { font-size: 10.5px; font-weight: 700; color: #0f172a; }");

            // Section Header & Card
            sb.AppendLine(".sec-header { background: #002b5c; color: #ffffff; padding: 5px 10px; font-size: 10px; font-weight: 700; text-transform: uppercase; letter-spacing: 0.4px; border-radius: 4px 4px 0 0; margin-top: 10px; margin-bottom: 0; }");
            sb.AppendLine(".sec-card { border: 1px solid #cbd5e1; border-top: none; padding: 10px; margin-bottom: 10px; background: #ffffff; border-radius: 0 0 4px 4px; }");

            // Info Grid Table
            sb.AppendLine(".info-table { width: 100%; border-collapse: collapse; }");
            sb.AppendLine(".info-table td { padding: 5px 8px; vertical-align: top; font-size: 10px; border-bottom: 1px dashed #e2e8f0; }");
            sb.AppendLine(".info-table tr:last-child td { border-bottom: none; }");
            sb.AppendLine(".info-lbl { font-weight: 600; color: #475569; width: 22%; }");
            sb.AppendLine(".info-val { font-weight: 700; color: #0f172a; }");

            // Narrative Block
            sb.AppendLine(".narrative-title { font-weight: 700; font-size: 9.5px; color: #475569; text-transform: uppercase; letter-spacing: 0.3px; margin: 8px 0 4px 0; }");
            sb.AppendLine(".narrative-box { background: #f8fafc; border: 1px solid #e2e8f0; border-radius: 4px; padding: 8px 10px; font-size: 9.5px; color: #1e293b; line-height: 1.45; white-space: pre-wrap; }");

            // Data Tables
            sb.AppendLine(".data-tbl { width: 100%; border-collapse: collapse; margin-top: 0; font-size: 9.5px; }");
            sb.AppendLine(".data-tbl thead { display: table-header-group; }");
            sb.AppendLine(".data-tbl tr { page-break-inside: avoid; }");
            sb.AppendLine(".data-tbl th { background: #f1f5f9; color: #0f172a; font-weight: 700; font-size: 8.5px; text-transform: uppercase; letter-spacing: 0.3px; padding: 6px 8px; border: 1px solid #cbd5e1; text-align: left; }");
            sb.AppendLine(".data-tbl th.tc { text-align: center; }");
            sb.AppendLine(".data-tbl td { border: 1px solid #cbd5e1; padding: 5px 8px; vertical-align: middle; }");
            sb.AppendLine(".data-tbl tbody tr:nth-child(even) { background-color: #f8fafc; }");

            // Status Pill
            sb.AppendLine(".status-pill { display: inline-block; padding: 2px 8px; border-radius: 10px; font-size: 8.5px; font-weight: 700; text-transform: uppercase; letter-spacing: 0.3px; white-space: nowrap; }");
            sb.AppendLine(".status-accepted { background: #dcfce7; color: #15803d; border: 1px solid #86efac; }");
            sb.AppendLine(".status-rejected { background: #fee2e2; color: #b91c1c; border: 1px solid #fca5a5; }");
            sb.AppendLine(".status-warning { background: #fef3c7; color: #b45309; border: 1px solid #fde68a; }");
            sb.AppendLine(".status-info { background: #e0f2fe; color: #0369a1; border: 1px solid #bae6fd; }");
            sb.AppendLine(".status-default { background: #e0e7ff; color: #3730a3; border: 1px solid #c7d2fe; }");

            // Empty row
            // Empty row
            sb.AppendLine(".empty-row { text-align: center; color: #64748b; font-style: italic; padding: 14px !important; }");
            sb.AppendLine(".pdf-doc-link { color: #0056b3; text-decoration: underline; font-weight: 700; word-break: break-all; }");
            sb.AppendLine(".data-tbl tr.log-mainrow td { border-bottom: 1px dashed #cbd5e1; }");
            sb.AppendLine(".data-tbl tr.log-subrow td { border-top: none; border-bottom: 1.5px solid #94a3b8; background-color: #f8fafc; padding: 5px 8px; font-size: 9.5px; color: #1e293b; line-height: 1.4; }");
            sb.AppendLine(".log-subrow-lbl { font-weight: 700; color: #002b5c; font-size: 8.5px; text-transform: uppercase; letter-spacing: 0.3px; margin-right: 4px; }");

            // Footer
            sb.AppendLine(".rpt-footer { width: 100%; border-collapse: collapse; margin-top: 15px; border-top: 1px solid #002b5c; }");
            sb.AppendLine(".rpt-footer td { padding: 4px 0; font-size: 8px; color: #64748b; vertical-align: middle; }");

            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");

            // Report Header
            sb.AppendLine("<table class=\"rpt-header\">");
            sb.AppendLine("  <tr>");
            sb.AppendLine("    <td class=\"logo-td\">");
            if (!string.IsNullOrEmpty(logoBase64))
            {
                sb.AppendLine("      <img src=\"data:image/png;base64," + logoBase64 + "\" alt=\"BCG\" />");
            }
            sb.AppendLine("    </td>");
            sb.AppendLine("    <td class=\"title-td\">");
            sb.AppendLine("      <div class=\"org-name\">The Bar Council of Gujarat</div>");
            sb.AppendLine("      <div class=\"org-addr\">3rd Floor, Satyamev Complex, Opp. High Court of Gujarat, S.G. Highway, Ahmedabad - 380060</div>");
            sb.AppendLine("      <div class=\"org-addr\">Phone: (079) 27434073 &nbsp;|&nbsp; Email: mail@barcouncilofgujarat.org &nbsp;|&nbsp; Web: www.barcouncilofgujarat.org</div>");
            sb.AppendLine("    </td>");
            sb.AppendLine("    <td class=\"badge-td\">");
            sb.AppendLine("      <div class=\"doc-badge\">Complaint Case Dossier</div>");
            sb.AppendLine("      <div class=\"doc-badge-sub\">Official Proceedings Record</div>");
            sb.AppendLine("    </td>");
            sb.AppendLine("  </tr>");
            sb.AppendLine("</table>");
            sb.AppendLine("<hr class=\"hdr-divider\" />");

            // Case Hero Banner
            string cmpNo = !string.IsNullOrEmpty(complaint.ComplaintNumber) ? HttpUtility.HtmlEncode(complaint.ComplaintNumber) : "-";
            string filedDate = !string.IsNullOrEmpty(complaint.SCreateDate) ? HttpUtility.HtmlEncode(complaint.SCreateDate) : "-";
            string statusPill = RenderComplaintStatusPill(complaint.Status);
            string payStatus = !string.IsNullOrEmpty(complaint.PaymentStatus) ? HttpUtility.HtmlEncode(complaint.PaymentStatus) : "Unpaid";
            string totalAmt = complaint.TotalAmount.ToString("N2");

            sb.AppendLine("<table class=\"case-banner\">");
            sb.AppendLine("  <tr>");
            sb.AppendLine("    <td style=\"width:32%;\"><span class=\"meta-label\">Complaint Number</span><span class=\"case-num\">" + cmpNo + "</span></td>");
            sb.AppendLine("    <td style=\"width:22%;\"><span class=\"meta-label\">Filing Date</span><span class=\"meta-val\">" + filedDate + "</span></td>");
            sb.AppendLine("    <td style=\"width:24%;\"><span class=\"meta-label\">Current Status</span><div>" + statusPill + "</div></td>");
            sb.AppendLine("    <td style=\"width:22%;\"><span class=\"meta-label\">Payment Status / Fee</span><span class=\"meta-val\">₹ " + totalAmt + " (" + payStatus + ")</span></td>");
            sb.AppendLine("  </tr>");
            sb.AppendLine("</table>");

            // Section 1: Complainant Profile
            sb.AppendLine("<div class=\"sec-header\">1. Complainant's Information (ફરિયાદીની વિગત)</div>");
            sb.AppendLine("<div class=\"sec-card\">");
            sb.AppendLine("  <table class=\"info-table\">");
            sb.AppendLine("    <tr>");
            sb.AppendLine("      <td class=\"info-lbl\" style=\"width:16%;\">Full Name:</td>");
            sb.AppendLine("      <td class=\"info-val\" style=\"width:34%;\">" + (!string.IsNullOrEmpty(complaint.FullName) ? HttpUtility.HtmlEncode(complaint.FullName) : "-") + "</td>");
            sb.AppendLine("      <td class=\"info-lbl\" style=\"width:16%;\">Contact Number:</td>");
            sb.AppendLine("      <td class=\"info-val\" style=\"width:34%;\">" + (!string.IsNullOrEmpty(complaint.ContactNumber) ? HttpUtility.HtmlEncode(complaint.ContactNumber) : "-") + "</td>");
            sb.AppendLine("    </tr>");
            sb.AppendLine("    <tr>");
            sb.AppendLine("      <td class=\"info-lbl\">Email Address:</td>");
            sb.AppendLine("      <td class=\"info-val\" colspan=\"3\">" + (!string.IsNullOrEmpty(complaint.EmailAddress) ? HttpUtility.HtmlEncode(complaint.EmailAddress) : "-") + "</td>");
            sb.AppendLine("    </tr>");
            sb.AppendLine("    <tr>");
            sb.AppendLine("      <td class=\"info-lbl\">Postal Address:</td>");
            sb.AppendLine("      <td class=\"info-val\" colspan=\"3\" style=\"line-height:1.45;\">" + (!string.IsNullOrEmpty(complaint.Address) ? HttpUtility.HtmlEncode(complaint.Address) : "-") + "</td>");
            sb.AppendLine("    </tr>");
            sb.AppendLine("  </table>");

            if (!string.IsNullOrEmpty(complaint.Summary) && complaint.Summary.Trim() != "")
            {
                sb.AppendLine("  <div class=\"narrative-title\">Summary / Short Description:</div>");
                sb.AppendLine("  <div class=\"narrative-box\">" + HttpUtility.HtmlEncode(complaint.Summary) + "</div>");
            }

            if (!string.IsNullOrEmpty(complaint.DetailComplaint) && complaint.DetailComplaint.Trim() != "")
            {
                sb.AppendLine("  <div class=\"narrative-title\">Detailed Complaint Narrative (વિસ્તૃત ફરિયાદ):</div>");
                sb.AppendLine("  <div class=\"narrative-box\">" + HttpUtility.HtmlEncode(complaint.DetailComplaint) + "</div>");
            }
            sb.AppendLine("</div>");

            // Section 2: Advocates Complained Against
            sb.AppendLine("<div class=\"sec-header\">2. Details of Advocate - Complained Against (જે વકીલ વિરુદ્ધ ફરિયાદ છે તેની વિગતો)</div>");
            sb.AppendLine("<div class=\"sec-card\">");
            sb.AppendLine("  <table class=\"data-tbl\">");
            sb.AppendLine("    <thead>");
            sb.AppendLine("      <tr>");
            sb.AppendLine("        <th style=\"width:30px;\" class=\"tc\">#</th>");
            sb.AppendLine("        <th>Advocate Name</th>");
            sb.AppendLine("        <th style=\"width:130px;\">Enrollment Number</th>");
            sb.AppendLine("        <th style=\"width:100px;\">City</th>");
            sb.AppendLine("        <th style=\"width:100px;\">Contact Number</th>");
            sb.AppendLine("        <th style=\"width:85px; text-align:right;\">Fee Amount</th>");
            sb.AppendLine("      </tr>");
            sb.AppendLine("    </thead>");
            sb.AppendLine("    <tbody>");
            if (advocates != null && advocates.Count > 0)
            {
                for (int i = 0; i < advocates.Count; i++)
                {
                    var adv = advocates[i];
                    string advName = !string.IsNullOrEmpty(adv.AdvocateName) ? adv.AdvocateName : (!string.IsNullOrEmpty(adv.FullName) ? adv.FullName : adv.Name);
                    string advEnro = !string.IsNullOrEmpty(adv.EnrollmentNo) ? adv.EnrollmentNo : (adv.IsKnown == "N" ? "Unknown" : "-");
                    string advCity = !string.IsNullOrEmpty(adv.City) ? adv.City : "-";
                    string advContact = !string.IsNullOrEmpty(adv.ContactNumber) ? adv.ContactNumber : (!string.IsNullOrEmpty(adv.MobileNo) ? adv.MobileNo : "-");
                    string advAmt = adv.Amount > 0 ? ("₹ " + adv.Amount.ToString("N2")) : "₹ 0.00";

                    sb.AppendLine("      <tr>");
                    sb.AppendLine("        <td class=\"tc\">" + (i + 1) + "</td>");
                    sb.AppendLine("        <td><strong>" + HttpUtility.HtmlEncode(advName ?? "-") + "</strong></td>");
                    sb.AppendLine("        <td>" + HttpUtility.HtmlEncode(advEnro) + "</td>");
                    sb.AppendLine("        <td>" + HttpUtility.HtmlEncode(advCity) + "</td>");
                    sb.AppendLine("        <td>" + HttpUtility.HtmlEncode(advContact) + "</td>");
                    sb.AppendLine("        <td style=\"text-align:right;\">" + advAmt + "</td>");
                    sb.AppendLine("      </tr>");
                }
            }
            else
            {
                sb.AppendLine("      <tr><td colspan=\"6\" class=\"empty-row\">No advocates listed for this complaint.</td></tr>");
            }
            sb.AppendLine("    </tbody>");
            sb.AppendLine("  </table>");
            sb.AppendLine("</div>");

            // Section 3: Attached Documents
            sb.AppendLine("<div class=\"sec-header\">3. Documents & Evidence Attachments (જોડાણ દસ્તાવેજો)</div>");
            sb.AppendLine("<div class=\"sec-card\">");
            sb.AppendLine("  <table class=\"data-tbl\">");
            sb.AppendLine("    <thead>");
            sb.AppendLine("      <tr>");
            sb.AppendLine("        <th style=\"width:25px;\" class=\"tc\">#</th>");
            sb.AppendLine("        <th style=\"width:145px;\">Document Category</th>");
            sb.AppendLine("        <th style=\"width:235px;\">File Name</th>");
            sb.AppendLine("        <th>Description / Notes</th>");
            sb.AppendLine("      </tr>");
            sb.AppendLine("    </thead>");
            sb.AppendLine("    <tbody>");
            if (documents != null && documents.Count > 0)
            {
                for (int i = 0; i < documents.Count; i++)
                {
                    var doc = documents[i];
                    bool isAffidavit = string.Equals(doc.DocumetType, "Affidavit", StringComparison.OrdinalIgnoreCase)
                                    || string.Equals(doc.DocumetType, "Complaint Affidavit Copy", StringComparison.OrdinalIgnoreCase)
                                    || string.Equals(doc.FileTitle, "Complaint Affidavit Copy", StringComparison.OrdinalIgnoreCase);

                    string docTitle = isAffidavit
                        ? "Complaint Affidavit Copy"
                        : (!string.IsNullOrEmpty(doc.FileTitle) ? doc.FileTitle : (!string.IsNullOrEmpty(doc.DocumetType) ? doc.DocumetType : "Document"));

                    string docName = !string.IsNullOrEmpty(doc.FileName) ? doc.FileName : "-";
                    string docDesc = !string.IsNullOrEmpty(doc.Description) ? doc.Description : (isAffidavit ? "Official complaint sworn affidavit submitted by complainant." : "-");
                    string fileCellHtml = FormatPdfFileLink(docName, doc.FilePath);

                    sb.AppendLine("      <tr>");
                    sb.AppendLine("        <td class=\"tc\">" + (i + 1) + "</td>");
                    sb.AppendLine("        <td><strong>" + HttpUtility.HtmlEncode(docTitle) + "</strong></td>");
                    sb.AppendLine("        <td>" + fileCellHtml + "</td>");
                    sb.AppendLine("        <td>" + HttpUtility.HtmlEncode(docDesc) + "</td>");
                    sb.AppendLine("      </tr>");
                }
            }
            else
            {
                sb.AppendLine("      <tr><td colspan=\"4\" class=\"empty-row\">No attachments or affidavit records uploaded.</td></tr>");
            }
            sb.AppendLine("    </tbody>");
            sb.AppendLine("  </table>");
            sb.AppendLine("</div>");

            // Section 4: Action Log & Proceedings History
            sb.AppendLine("<div class=\"sec-header\">4. Case Proceedings & Activity History Log (કેસ કાર્યવાહી ઇતિહાસ)</div>");
            sb.AppendLine("<div class=\"sec-card\">");
            sb.AppendLine("  <table class=\"data-tbl\">");
            sb.AppendLine("    <thead>");
            sb.AppendLine("      <tr>");
            sb.AppendLine("        <th style=\"width:30px;\" class=\"tc\">#</th>");
            sb.AppendLine("        <th style=\"width:130px;\">Log Date &amp; Time</th>");
            sb.AppendLine("        <th style=\"width:115px;\">Status</th>");
            sb.AppendLine("        <th style=\"width:95px;\">Next Date</th>");
            sb.AppendLine("        <th style=\"width:125px;\">Committee</th>");
            sb.AppendLine("        <th>Order File</th>");
            sb.AppendLine("      </tr>");
            sb.AppendLine("    </thead>");
            sb.AppendLine("    <tbody>");
            if (logs != null && logs.Count > 0)
            {
                var sortedLogs = logs.OrderBy(x => x.CreateDate).ThenByDescending(x => x.SrNo ?? 0).ToList();
                for (int i = 0; i < sortedLogs.Count; i++)
                {
                    var log = sortedLogs[i];
                    string logCreated = !string.IsNullOrEmpty(log.SCreateDate)
                        ? log.SCreateDate
                        : (log.CreateDate != DateTime.MinValue ? log.CreateDate.ToString("dd-MM-yyyy hh:mm tt") : "-");
                    string logStatus = RenderComplaintStatusPill(log.Status);
                    string logDate = !string.IsNullOrEmpty(log.SNextDate) ? log.SNextDate : "-";
                    string logComm = !string.IsNullOrEmpty(log.CommitteTitle) ? ("Committee " + log.CommitteTitle) : "-";
                    string logSummary = !string.IsNullOrEmpty(log.Summary) ? log.Summary.Trim() : "";
                    if (string.IsNullOrEmpty(logSummary))
                    {
                        if (string.Equals(log.Status, "New", StringComparison.OrdinalIgnoreCase))
                        {
                            logSummary = "Complaint has been successfully registered in the system. It is currently under initial administrative verification and scrutiny.";
                        }
                        else if (string.Equals(log.Status, "Accepted", StringComparison.OrdinalIgnoreCase))
                        {
                            logSummary = "Complaint has been scrutinized and accepted for further proceedings. The matter will be placed before the General Board for consideration.";
                        }
                        else if (string.Equals(log.Status, "Refer to Board", StringComparison.OrdinalIgnoreCase) || string.Equals(log.Status, "Next General Board", StringComparison.OrdinalIgnoreCase))
                        {
                            logSummary = "Complaint has been referred to the General Board for consideration. Awaiting the Board's review and decision in the upcoming meeting.";
                        }
                        else if (string.Equals(log.Status, "Board Accept", StringComparison.OrdinalIgnoreCase))
                        {
                            logSummary = "The General Board has accepted the complaint. Pending formal board approval and committee allocation.";
                        }
                        else if (string.Equals(log.Status, "Board Has Approved", StringComparison.OrdinalIgnoreCase))
                        {
                            logSummary = "The General Board has formally approved the complaint. The matter has been referred to the Disciplinary Committee for conducting inquiry and hearing proceedings.";
                        }
                        else
                        {
                            logSummary = "-";
                        }
                    }
                    string logFile = !string.IsNullOrEmpty(log.OrderFileName) ? log.OrderFileName : "-";
                    string logFileHtml = FormatPdfFileLink(logFile, log.OrderFilePath);

                    sb.AppendLine("      <tr class=\"log-mainrow\">");
                    sb.AppendLine("        <td class=\"tc\">" + (i + 1) + "</td>");
                    sb.AppendLine("        <td style=\"white-space:nowrap; font-weight:600; color:#334155;\">" + HttpUtility.HtmlEncode(logCreated) + "</td>");
                    sb.AppendLine("        <td>" + logStatus + "</td>");
                    sb.AppendLine("        <td>" + HttpUtility.HtmlEncode(logDate) + "</td>");
                    sb.AppendLine("        <td>" + HttpUtility.HtmlEncode(logComm) + "</td>");
                    sb.AppendLine("        <td>" + logFileHtml + "</td>");
                    sb.AppendLine("      </tr>");
                    sb.AppendLine("      <tr class=\"log-subrow\">");
                    sb.AppendLine("        <td colspan=\"6\">");
                    sb.AppendLine("          <span class=\"log-subrow-lbl\">Remarks / Summary:</span> " + HttpUtility.HtmlEncode(logSummary));
                    sb.AppendLine("        </td>");
                    sb.AppendLine("      </tr>");
                }
            }
            else
            {
                sb.AppendLine("      <tr><td colspan=\"6\" class=\"empty-row\">No status log records available.</td></tr>");
            }
            sb.AppendLine("    </tbody>");
            sb.AppendLine("  </table>");
            sb.AppendLine("</div>");

            // Report Footer
            sb.AppendLine("<table class=\"rpt-footer\">");
            sb.AppendLine("  <tr>");
            sb.AppendLine("    <td>The Bar Council of Gujarat &bull; Statutory Body Constituted under the Advocates Act, 1961 &bull; Confidential Official Case Record</td>");
            sb.AppendLine("    <td style=\"text-align:right;\">Generated on " + DateTime.Now.ToString("dd-MM-yyyy hh:mm tt") + "</td>");
            sb.AppendLine("  </tr>");
            sb.AppendLine("</table>");

            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

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

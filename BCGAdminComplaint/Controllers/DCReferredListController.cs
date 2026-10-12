using BarCouncilComplaint.Controllers;
using BCGAdminComplaint.Models;
using BCGAdminComplaint.Repository;
using ClosedXML.Excel;
using System.Text;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace BCGAdminComplaint.Controllers
{
    public class DCReferredListController : BaseController
    {
        // GET: DCReferredList
        private readonly ComplaintRepository _ComplaintRepository;

        public DCReferredListController()
        {
            _ComplaintRepository = new ComplaintRepository();
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

        [HttpPost]
        public ActionResult GetComplaintDCReferredList(BaseFilter baseFilter, String Status, String FromDate, String ToDate, String DCCommittee)
        {
            try
            {
                HttpCookie cookie = Request.Cookies["bcg"];
                if (cookie == null)
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    List<ComplaintDCReferredListModel> result = _ComplaintRepository.GetComplaintDCReferredList(baseFilter, Status, FromDate, ToDate, DCCommittee);
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
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetDCReferredListFilterSet(string SearchValue, string FromDate, string ToDate, string SelectComplaintIds)
        {
            try
            {
                Response.Cookies["SearchValue"].Value = SearchValue == null ? "" : SearchValue;
                Response.Cookies["FromDate"].Value = FromDate == null ? "" : FromDate;
                Response.Cookies["fromDate"].Value = FromDate == null ? "" : FromDate;
                Response.Cookies["ToDate"].Value = ToDate == null ? "" : ToDate;
                Response.Cookies["SelectComplaintIds"].Value = SelectComplaintIds == null ? "" : SelectComplaintIds;
                return Json(new { data = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { data = "Error", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult GetDCReferredListExcelReport()
        {
            try
            {
                string SearchValue = "";
                string FromDate = "";
                string ToDate = "";
                string SelectComplaintIds = "";

                if (Request.Cookies["SearchValue"] != null && Request.Cookies["SearchValue"].Value != null)
                {
                    SearchValue = Request.Cookies["SearchValue"].Value;
                }
                if (Request.Cookies["FromDate"] != null && Request.Cookies["FromDate"].Value != null)
                {
                    FromDate = Request.Cookies["FromDate"].Value;
                }
                else if (Request.Cookies["fromDate"] != null && Request.Cookies["fromDate"].Value != null)
                {
                    FromDate = Request.Cookies["fromDate"].Value;
                }
                if (Request.Cookies["ToDate"] != null && Request.Cookies["ToDate"].Value != null)
                {
                    ToDate = Request.Cookies["ToDate"].Value;
                }
                if (Request.Cookies["SelectComplaintIds"] != null && Request.Cookies["SelectComplaintIds"].Value != null)
                {
                    SelectComplaintIds = Request.Cookies["SelectComplaintIds"].Value;
                }

                List<ComplaintDCReferredListReportModel> result = _ComplaintRepository.GetComplaintDCReferredListReport(SearchValue, FromDate, ToDate, SelectComplaintIds);

                DataTable dt = new DataTable("DC Referred List");
                dt.Columns.AddRange(new DataColumn[10] {
                    new DataColumn("#"),
                    new DataColumn("Complaint Number"),
                    new DataColumn("DC Case No."),
                    new DataColumn("Assign DC Committee"),
                    new DataColumn("Assign DC Committee Date"),
                    new DataColumn("Full Name"),
                    new DataColumn("Email"),
                    new DataColumn("Mobile No."),
                    new DataColumn("Complained Against (Advocate)"),
                    new DataColumn("Complaint Status")
                });

                for (int i = 0; i < result.Count; i++)
                {
                    var customer = result[i];
                    string commTitle = "";
                    if (!string.IsNullOrEmpty(customer.CommitteTitle))
                    {
                        commTitle = customer.CommitteTitle.Trim().StartsWith("Committee", StringComparison.OrdinalIgnoreCase)
                            ? customer.CommitteTitle.Trim()
                            : ("Committee " + customer.CommitteTitle.Trim());
                    }

                    string respExcel = !string.IsNullOrEmpty(customer.RespondentDetailsExcel)
                        ? customer.RespondentDetailsExcel
                        : (!string.IsNullOrEmpty(customer.RespondentDetails) ? customer.RespondentDetails.Replace("<br />", ", ").Replace("<br/>", ", ") : "");

                    dt.Rows.Add(
                        customer.SrNo > 0 ? customer.SrNo.ToString() : (i + 1).ToString(),
                        customer.ComplaintNumber ?? "",
                        customer.DCComplaintNumber ?? "",
                        commTitle,
                        customer.AssignCommitteeDate ?? "",
                        customer.FullName ?? "",
                        customer.EmailAddress ?? "",
                        customer.ContactNumber ?? "",
                        respExcel,
                        customer.Status ?? ""
                    );
                }

                string fileName = "DC_Referred_List_Excel_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".xlsx";

                using (XLWorkbook wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add(dt);

                    // Header styling
                    var headerRange = ws.Range(1, 1, 1, 10);
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Font.FontSize = 11;
                    headerRange.Style.Font.FontColor = XLColor.White;
                    headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#002b5c");
                    headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Row(1).Height = 26;

                    // Column alignments and padding
                    ws.Column(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Column(2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Column(3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Column(4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Column(5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Column(6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Column(7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Column(8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Column(9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Column(10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    // Row formatting & borders
                    int totalRows = dt.Rows.Count + 1;
                    for (int r = 2; r <= totalRows; r++)
                    {
                        ws.Row(r).Height = 20;
                        ws.Row(r).Style.Font.FontSize = 10;
                        ws.Row(r).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    }

                    var dataRange = ws.Range(1, 1, totalRows, 10);
                    dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    dataRange.Style.Border.OutsideBorderColor = XLColor.FromHtml("#cbd5e1");
                    dataRange.Style.Border.InsideBorderColor = XLColor.FromHtml("#e2e8f0");

                    ws.Columns().AdjustToContents();

                    // Minimum column widths for readability
                    if (ws.Column(1).Width < 6) ws.Column(1).Width = 6;
                    if (ws.Column(2).Width < 18) ws.Column(2).Width = 18;
                    if (ws.Column(3).Width < 16) ws.Column(3).Width = 16;
                    if (ws.Column(4).Width < 20) ws.Column(4).Width = 20;
                    if (ws.Column(5).Width < 22) ws.Column(5).Width = 22;
                    if (ws.Column(6).Width < 25) ws.Column(6).Width = 25;
                    if (ws.Column(7).Width < 25) ws.Column(7).Width = 25;
                    if (ws.Column(8).Width < 15) ws.Column(8).Width = 15;
                    if (ws.Column(9).Width < 30) ws.Column(9).Width = 30;
                    if (ws.Column(10).Width < 16) ws.Column(10).Width = 16;

                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }

        [HttpGet]
        public ActionResult GetDCReferredListPDFReport()
        {
            try
            {
                string SearchValue = "";
                string FromDate = "";
                string ToDate = "";
                string SelectComplaintIds = "";

                if (Request.Cookies["SearchValue"] != null && Request.Cookies["SearchValue"].Value != null)
                {
                    SearchValue = Request.Cookies["SearchValue"].Value;
                }
                if (Request.Cookies["FromDate"] != null && Request.Cookies["FromDate"].Value != null)
                {
                    FromDate = Request.Cookies["FromDate"].Value;
                }
                else if (Request.Cookies["fromDate"] != null && Request.Cookies["fromDate"].Value != null)
                {
                    FromDate = Request.Cookies["fromDate"].Value;
                }
                if (Request.Cookies["ToDate"] != null && Request.Cookies["ToDate"].Value != null)
                {
                    ToDate = Request.Cookies["ToDate"].Value;
                }
                if (Request.Cookies["SelectComplaintIds"] != null && Request.Cookies["SelectComplaintIds"].Value != null)
                {
                    SelectComplaintIds = Request.Cookies["SelectComplaintIds"].Value;
                }

                List<ComplaintDCReferredListReportModel> result = _ComplaintRepository.GetComplaintDCReferredListReport(SearchValue, FromDate, ToDate, SelectComplaintIds);

                string html = GenerateDCReferredReportHtml(result, SearchValue, FromDate, ToDate);
                byte[] pdfBytes = ConvertHtmlToPdf(html);

                string fileName = "DC_Referred_List_Report_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".pdf";
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                return Content("Error generating PDF: " + ex.Message);
            }
        }

        private string RenderStatusPill(string status)
        {
            if (string.IsNullOrEmpty(status)) return "-";
            string s = status.Trim();
            string lower = s.ToLower();
            string badgeClass = "status-new";
            if (lower.Contains("approv") || lower.Contains("accept") || lower.Contains("paid")) badgeClass = "status-approved";
            else if (lower.Contains("hear") || lower.Contains("adj") || lower.Contains("pend") || lower.Contains("part")) badgeClass = "status-hearing";
            else if (lower.Contains("reject") || lower.Contains("dismiss") || lower.Contains("due")) badgeClass = "status-rejected";

            return string.Format("<span class=\"status-pill {0}\">{1}</span>", badgeClass, HttpUtility.HtmlEncode(s.ToUpper()));
        }

        private string GenerateDCReferredReportHtml(List<ComplaintDCReferredListReportModel> result, string searchValue, string fromDate, string toDate)
        {
            string logoBase64 = GetLogoBase64();
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang=\"en\">");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta charset=\"utf-8\" />");
            sb.AppendLine("<title>DC Referred List Report - Bar Council of Gujarat</title>");
            sb.AppendLine("<style>");
            sb.AppendLine("@page { size: A4 landscape; margin: 8mm 10mm 8mm 10mm; }");
            sb.AppendLine("* { box-sizing: border-box; -webkit-print-color-adjust: exact !important; print-color-adjust: exact !important; }");
            sb.AppendLine("body { font-family: 'Segoe UI', 'Helvetica Neue', Arial, sans-serif; font-size: 10px; color: #1e293b; background: #fff; margin: 0; padding: 0; line-height: 1.35; }");

            // Report Header
            sb.AppendLine(".rpt-header { width: 100%; border-collapse: collapse; margin-bottom: 4px; }");
            sb.AppendLine(".rpt-header td { vertical-align: middle; padding: 0; }");
            sb.AppendLine(".rpt-header .logo-td { width: 56px; }");
            sb.AppendLine(".rpt-header .logo-td img { height: 48px; width: auto; display: block; }");
            sb.AppendLine(".rpt-header .title-td { text-align: center; padding: 0 10px; }");
            sb.AppendLine(".rpt-header .badge-td { width: 220px; text-align: right; }");
            sb.AppendLine(".org-name { font-size: 16px; font-weight: 800; color: #002b5c; text-transform: uppercase; letter-spacing: 0.6px; margin: 0 0 1px 0; }");
            sb.AppendLine(".org-addr { font-size: 8.5px; color: #64748b; margin: 0; line-height: 1.25; }");
            sb.AppendLine(".doc-badge { display: inline-block; background: #002b5c; color: #fff; padding: 3px 10px; border-radius: 4px; font-weight: 700; font-size: 10px; letter-spacing: 0.4px; text-transform: uppercase; }");
            sb.AppendLine(".doc-badge-sub { font-size: 8px; color: #64748b; font-weight: 600; margin-top: 2px; text-align: right; text-transform: uppercase; letter-spacing: 0.2px; }");

            // Divider & Meta Bar
            sb.AppendLine(".hdr-divider { border: none; border-top: 2px solid #002b5c; margin: 0 0 5px 0; }");
            sb.AppendLine(".meta-bar { width: 100%; border-collapse: collapse; background: #f1f5f9; border: 1px solid #cbd5e1; border-radius: 4px; margin-bottom: 8px; }");
            sb.AppendLine(".meta-bar td { padding: 4px 8px; font-size: 9.5px; vertical-align: middle; }");
            sb.AppendLine(".meta-bar .ml { font-weight: 600; color: #64748b; }");
            sb.AppendLine(".meta-bar .mv { font-weight: 700; color: #0f172a; }");
            sb.AppendLine(".cnt-badge { display: inline-block; background: #002b5c; color: #fff; padding: 2px 8px; border-radius: 10px; font-size: 9px; font-weight: 700; }");

            // Table Styling
            sb.AppendLine(".rpt-table { width: 100%; border-collapse: collapse; border: 1px solid #94a3b8; font-size: 10px; background: #fff; }");
            sb.AppendLine(".rpt-table thead { display: table-header-group; }");
            sb.AppendLine(".rpt-table tr { page-break-inside: avoid; }");
            sb.AppendLine(".rpt-table th { background: #002b5c !important; color: #ffffff !important; font-weight: 700; font-size: 9px; text-transform: uppercase; letter-spacing: 0.4px; padding: 7px 6px; border: 1px solid #001f42; text-align: center; vertical-align: middle; }");
            sb.AppendLine(".rpt-table th.th-left { text-align: left; }");
            sb.AppendLine(".rpt-table td { border: 1px solid #cbd5e1; padding: 6px 6px; vertical-align: middle; color: #1e293b; }");
            sb.AppendLine(".rpt-table tbody tr:nth-child(even) { background-color: #f8fafc; }");

            // Badges & Value Formats
            sb.AppendLine(".col-sr { text-align: center; font-weight: 700; color: #64748b; font-size: 10px; width: 35px; }");
            sb.AppendLine(".badge-cmp { display: inline-block; background: #eef2ff; color: #1e3a8a; border: 1px solid #c7d2fe; border-radius: 3px; padding: 2px 6px; font-weight: 700; font-family: 'Consolas', monospace; font-size: 10px; white-space: nowrap; }");
            sb.AppendLine(".badge-dc { display: inline-block; background: #fef9c3; color: #854d0e; border: 1px solid #fde047; border-radius: 3px; padding: 2px 6px; font-weight: 700; font-family: 'Consolas', monospace; font-size: 10px; white-space: nowrap; }");
            sb.AppendLine(".badge-comm { display: inline-block; background: #e0f2fe; color: #0369a1; border: 1px solid #bae6fd; border-radius: 3px; padding: 2px 6px; font-weight: 700; font-size: 9.5px; white-space: nowrap; }");
            sb.AppendLine(".name-text { font-weight: 700; color: #0f172a; font-size: 10.5px; }");
            sb.AppendLine(".email-text { color: #334155; font-size: 9.5px; word-break: break-all; }");
            sb.AppendLine(".phone-text { font-weight: 700; color: #0f172a; font-size: 10px; text-align: center; font-family: 'Consolas', monospace; }");
            sb.AppendLine(".resp-text { font-size: 9.5px; color: #1e293b; line-height: 1.35; }");
            sb.AppendLine(".date-text { text-align: center; font-size: 9.5px; font-weight: 600; color: #334155; white-space: nowrap; }");

            // Status Pill
            sb.AppendLine(".status-pill { display: inline-block; padding: 2px 7px; border-radius: 10px; font-size: 8.5px; font-weight: 700; text-transform: uppercase; letter-spacing: 0.3px; white-space: nowrap; text-align: center; }");
            sb.AppendLine(".status-new { background: #e0f2fe; color: #0369a1; border: 1px solid #bae6fd; }");
            sb.AppendLine(".status-approved { background: #dcfce7; color: #15803d; border: 1px solid #86efac; }");
            sb.AppendLine(".status-hearing { background: #fef3c7; color: #b45309; border: 1px solid #fde68a; }");
            sb.AppendLine(".status-rejected { background: #fee2e2; color: #b91c1c; border: 1px solid #fca5a5; }");

            // Report Footer
            sb.AppendLine(".rpt-footer { width: 100%; border-collapse: collapse; margin-top: 6px; border-top: 1px solid #002b5c; }");
            sb.AppendLine(".rpt-footer td { padding: 4px 0; font-size: 8.5px; color: #64748b; vertical-align: middle; }");

            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");

            // ═══════ REPORT HEADER ═══════
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
            sb.AppendLine("      <div class=\"doc-badge\">DC Referred List</div>");
            sb.AppendLine("      <div class=\"doc-badge-sub\">Disciplinary Committee Register</div>");
            sb.AppendLine("    </td>");
            sb.AppendLine("  </tr>");
            sb.AppendLine("</table>");
            sb.AppendLine("<hr class=\"hdr-divider\" />");

            // ═══════ META SUMMARY BAR ═══════
            string dateRangeStr = (!string.IsNullOrEmpty(fromDate) || !string.IsNullOrEmpty(toDate))
                ? string.Format("{0} to {1}", !string.IsNullOrEmpty(fromDate) ? fromDate : "-", !string.IsNullOrEmpty(toDate) ? toDate : "-")
                : "All Registered Dates";

            string recLabel = result.Count == 1 ? "1 Record" : (result.Count + " Records");

            sb.AppendLine("<table class=\"meta-bar\">");
            sb.AppendLine("  <tr>");
            sb.AppendLine("    <td><span class=\"ml\">Search:</span> <span class=\"mv\">" + (!string.IsNullOrEmpty(searchValue) ? HttpUtility.HtmlEncode(searchValue) : "All") + "</span></td>");
            sb.AppendLine("    <td><span class=\"ml\">Date Range:</span> <span class=\"mv\">" + HttpUtility.HtmlEncode(dateRangeStr) + "</span></td>");
            sb.AppendLine("    <td><span class=\"ml\">Generated:</span> <span class=\"mv\">" + DateTime.Now.ToString("dd-MM-yyyy hh:mm tt") + "</span></td>");
            sb.AppendLine("    <td style=\"text-align:right;\"><span class=\"ml\">Total Records:</span> <span class=\"cnt-badge\">" + recLabel + "</span></td>");
            sb.AppendLine("  </tr>");
            sb.AppendLine("</table>");

            // ═══════ TABLE ═══════
            if (result.Count == 0)
            {
                sb.AppendLine("<div style=\"text-align:center; padding:40px; font-size:13px; color:#64748b; font-style:italic;\">No Disciplinary Committee referred complaints found matching the criteria.</div>");
            }
            else
            {
                sb.AppendLine("<table class=\"rpt-table\">");
                sb.AppendLine("  <thead>");
                sb.AppendLine("    <tr>");
                sb.AppendLine("      <th style=\"width:30px;\">#</th>");
                sb.AppendLine("      <th style=\"width:125px;\">Complaint Number</th>");
                sb.AppendLine("      <th style=\"width:115px;\">DC Case No.</th>");
                sb.AppendLine("      <th style=\"width:95px;\">Assign DC Committee</th>");
                sb.AppendLine("      <th style=\"width:95px;\">Assign DC Committee Date</th>");
                sb.AppendLine("      <th class=\"th-left\" style=\"width:125px;\">Full Name</th>");
                sb.AppendLine("      <th class=\"th-left\" style=\"width:120px;\">Email</th>");
                sb.AppendLine("      <th style=\"width:85px;\">Mobile No.</th>");
                sb.AppendLine("      <th class=\"th-left\">Complained Against (Advocate)</th>");
                sb.AppendLine("      <th style=\"width:85px;\">Complaint Status</th>");
                sb.AppendLine("    </tr>");
                sb.AppendLine("  </thead>");
                sb.AppendLine("  <tbody>");

                for (int i = 0; i < result.Count; i++)
                {
                    var item = result[i];
                    string srNo = item.SrNo > 0 ? item.SrNo.ToString() : (i + 1).ToString();
                    string cmpNum = !string.IsNullOrEmpty(item.ComplaintNumber) ? HttpUtility.HtmlEncode(item.ComplaintNumber) : "-";
                    string dcNum = !string.IsNullOrEmpty(item.DCComplaintNumber) ? HttpUtility.HtmlEncode(item.DCComplaintNumber) : "-";
                    string commTitle = !string.IsNullOrEmpty(item.CommitteTitle) ? ("Committee " + HttpUtility.HtmlEncode(item.CommitteTitle)) : "-";
                    string commDate = !string.IsNullOrEmpty(item.AssignCommitteeDate) ? HttpUtility.HtmlEncode(item.AssignCommitteeDate) : "-";
                    string fullName = !string.IsNullOrEmpty(item.FullName) ? HttpUtility.HtmlEncode(item.FullName) : "-";
                    string email = !string.IsNullOrEmpty(item.EmailAddress) ? HttpUtility.HtmlEncode(item.EmailAddress) : "-";
                    string contact = !string.IsNullOrEmpty(item.ContactNumber) ? HttpUtility.HtmlEncode(item.ContactNumber) : "-";
                    string respDetails = !string.IsNullOrEmpty(item.RespondentDetails)
                        ? HttpUtility.HtmlEncode(item.RespondentDetails).Replace(", ", "<br/>")
                        : "-";
                    string statusHtml = RenderStatusPill(item.Status);

                    sb.AppendLine("    <tr>");
                    sb.AppendLine("      <td class=\"col-sr\">" + srNo + "</td>");
                    sb.AppendLine("      <td style=\"text-align:center;\"><span class=\"badge-cmp\">" + cmpNum + "</span></td>");
                    sb.AppendLine("      <td style=\"text-align:center;\"><span class=\"badge-dc\">" + dcNum + "</span></td>");
                    sb.AppendLine("      <td style=\"text-align:center;\"><span class=\"badge-comm\">" + commTitle + "</span></td>");
                    sb.AppendLine("      <td class=\"date-text\">" + commDate + "</td>");
                    sb.AppendLine("      <td><span class=\"name-text\">" + fullName + "</span></td>");
                    sb.AppendLine("      <td><span class=\"email-text\">" + email + "</span></td>");
                    sb.AppendLine("      <td class=\"phone-text\">" + contact + "</td>");
                    sb.AppendLine("      <td><div class=\"resp-text\">" + respDetails + "</div></td>");
                    sb.AppendLine("      <td style=\"text-align:center;\">" + statusHtml + "</td>");
                    sb.AppendLine("    </tr>");
                }

                sb.AppendLine("  </tbody>");
                sb.AppendLine("</table>");
            }

            // ═══════ REPORT FOOTER ═══════
            sb.AppendLine("<table class=\"rpt-footer\">");
            sb.AppendLine("  <tr>");
            sb.AppendLine("    <td>Bar Council of Gujarat &bull; Statutory Body Constituted under Advocates Act, 1961 &bull; Confidential Official Record</td>");
            sb.AppendLine("    <td style=\"text-align:right;\">Generated by Admin Portal</td>");
            sb.AppendLine("  </tr>");
            sb.AppendLine("</table>");

            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

    }
}
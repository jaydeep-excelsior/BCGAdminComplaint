using BCGAdminComplaint;
using BCGAdminComplaint.Models;
using BCGAdminComplaint.Repository;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace BarCouncilComplaint.Controllers
{
    public class ReportsController : BaseController
    {
        private readonly ComplaintRepository _ComplaintRepository;
        // GET: Reports

        public ReportsController()
        {
            _ComplaintRepository = new ComplaintRepository();
        }

        public ActionResult ComplaintReport()
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

        public ActionResult ComplaintListReport()
        {
            HttpCookie cookie = Request.Cookies["bcg"];
            if (cookie == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                return RedirectToAction("ComplaintReport");
            }
        }

        [HttpPost]
        public ActionResult GetReportComplaintList(BaseFilter baseFilter, String Status, String PaymentStatus, String FromDate, String ToDate, String Year, String FromNo, String ToNo)
        {
            ResultResponse response = new ResultResponse();
            try
            {
                HttpCookie cookie = Request.Cookies["bcg"];
                if (cookie == null)
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    List<ComplaintListModel> result = _ComplaintRepository.GetComplaintList(baseFilter, Status, PaymentStatus, FromDate, ToDate, Year, FromNo, ToNo, "ReportExcePDFSelectAll");
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

        private class FileEntry
        {
            public string FileName { get; set; }
            public string FilePath { get; set; }
        }

        private List<FileEntry> ParseFileList(string json)
        {
            var list = new List<FileEntry>();
            if (string.IsNullOrWhiteSpace(json) || json.Trim() == "[]") return list;
            try
            {
                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var rawList = serializer.Deserialize<List<Dictionary<string, object>>>(json);
                if (rawList != null)
                {
                    foreach (var dict in rawList)
                    {
                        string fn = dict.ContainsKey("FileName") && dict["FileName"] != null ? dict["FileName"].ToString() : "";
                        string fp = dict.ContainsKey("FilePath") && dict["FilePath"] != null ? dict["FilePath"].ToString() : "";
                        if (!string.IsNullOrEmpty(fn) || !string.IsNullOrEmpty(fp))
                        {
                            list.Add(new FileEntry
                            {
                                FileName = !string.IsNullOrEmpty(fn) ? fn : Path.GetFileName(fp),
                                FilePath = fp
                            });
                        }
                    }
                }
            }
            catch { }
            return list;
        }

        private static readonly Dictionary<string, Tuple<string, Func<ComplaintListModel, string>>> ReportColumnDefinitions =
            new Dictionary<string, Tuple<string, Func<ComplaintListModel, string>>>(StringComparer.OrdinalIgnoreCase)
        {
            { "SrNo", Tuple.Create("Sr No", new Func<ComplaintListModel, string>(item => item.SrNo.ToString())) },
            { "ComplaintNumber", Tuple.Create("Complaint Number", new Func<ComplaintListModel, string>(item => item.ComplaintNumber ?? "")) },
            { "ComplaintsCreateDate", Tuple.Create("Created Date", new Func<ComplaintListModel, string>(item => item.ComplaintsCreateDate ?? "")) },
            { "ComplainedAgainst", Tuple.Create("Complained Against", new Func<ComplaintListModel, string>(item => item.ComplainedAgainst ?? "-")) },
            { "FullName", Tuple.Create("Full Name", new Func<ComplaintListModel, string>(item => item.FullName ?? "")) },
            { "ContactNumber", Tuple.Create("Contact Number", new Func<ComplaintListModel, string>(item => item.ContactNumber ?? "")) },
            { "EmailAddress", Tuple.Create("Email", new Func<ComplaintListModel, string>(item => item.EmailAddress ?? "")) },
            { "AffidavitFilesText", Tuple.Create("Complaint Affidavit Copy", new Func<ComplaintListModel, string>(item => {
                var files = ParseFileListStatic(item.ComplaintAffidavitCopyList);
                if (files.Count > 0) return string.Join("\n", files.Select(f => f.FileName));
                return !string.IsNullOrEmpty(item.AffidavitFilesText) ? item.AffidavitFilesText : "-";
            })) },
            { "AdditionalFilesText", Tuple.Create("If Any Additional Attachments", new Func<ComplaintListModel, string>(item => {
                var files = ParseFileListStatic(item.ComplaintAdditionalAttachmentsList);
                if (files.Count > 0) return string.Join("\n", files.Select(f => f.FileName));
                return !string.IsNullOrEmpty(item.AdditionalFilesText) ? item.AdditionalFilesText : "-";
            })) },
            { "TotalAmount", Tuple.Create("Total Amount", new Func<ComplaintListModel, string>(item => item.TotalAmount == null ? "0" : item.TotalAmount.ToString())) },
            { "PaidAmount", Tuple.Create("Paid Amount", new Func<ComplaintListModel, string>(item => item.PaidAmount == null ? "0" : item.PaidAmount.ToString())) },
            { "DueAmount", Tuple.Create("Due Amount", new Func<ComplaintListModel, string>(item => item.DueAmount == null ? "0" : item.DueAmount.ToString())) },
            { "PaymentStatus", Tuple.Create("Payment Status", new Func<ComplaintListModel, string>(item => item.PaymentStatus ?? "")) },
            { "PaymentInfoText", Tuple.Create("Pay Payment Information", new Func<ComplaintListModel, string>(item => string.IsNullOrEmpty(item.PaymentInfoText) ? "-" : item.PaymentInfoText)) },
            { "Status", Tuple.Create("Status", new Func<ComplaintListModel, string>(item => item.Status ?? "")) }
        };

        private static List<FileEntry> ParseFileListStatic(string json)
        {
            var list = new List<FileEntry>();
            if (string.IsNullOrWhiteSpace(json) || json.Trim() == "[]") return list;
            try
            {
                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var rawList = serializer.Deserialize<List<Dictionary<string, object>>>(json);
                if (rawList != null)
                {
                    foreach (var dict in rawList)
                    {
                        string fn = dict.ContainsKey("FileName") && dict["FileName"] != null ? dict["FileName"].ToString() : "";
                        string fp = dict.ContainsKey("FilePath") && dict["FilePath"] != null ? dict["FilePath"].ToString() : "";
                        if (!string.IsNullOrEmpty(fn) || !string.IsNullOrEmpty(fp))
                        {
                            list.Add(new FileEntry
                            {
                                FileName = !string.IsNullOrEmpty(fn) ? fn : Path.GetFileName(fp),
                                FilePath = fp
                            });
                        }
                    }
                }
            }
            catch { }
            return list;
        }



        private string RenderStatusPill(string status)
        {
            if (string.IsNullOrEmpty(status)) return "-";
            string s = status.Trim();
            string lower = s.ToLower();
            string badgeClass = "status-new";
            if (lower.Contains("approv") || lower.Contains("accept") || lower.Contains("paid")) badgeClass = "status-approved";
            else if (lower.Contains("hear") || lower.Contains("adj") || lower.Contains("pend") || lower.Contains("part")) badgeClass = "status-hearing";
            else if (lower.Contains("reject") || lower.Contains("due")) badgeClass = "status-rejected";

            return string.Format("<span class=\"status-pill {0}\">{1}</span>", badgeClass, HttpUtility.HtmlEncode(s.ToUpper()));
        }

        private string RenderPaymentStatusPill(string paymentStatus)
        {
            if (string.IsNullOrEmpty(paymentStatus)) return "-";
            string s = paymentStatus.Trim();
            string lower = s.ToLower();
            string badgeClass = "status-hearing";
            if (lower == "paid") badgeClass = "status-approved";
            else if (lower.Contains("unpaid") || lower.Contains("due") || lower.Contains("fail")) badgeClass = "status-rejected";

            return string.Format("<span class=\"status-pill {0}\">{1}</span>", badgeClass, HttpUtility.HtmlEncode(s.ToUpper()));
        }

        private string GeneratePrintReportHtml(List<ComplaintListModel> result, List<string> selectedCols, string searchTerm, string status, string paymentStatus, string fromDate, string toDate)
        {
            string logoBase64 = GetLogoBase64();
            StringBuilder sb = new StringBuilder();

            // ── Helper booleans for selected columns ──
            bool showSrNo = selectedCols.Contains("SrNo");
            bool showComplaintNumber = selectedCols.Contains("ComplaintNumber");
            bool showCreateDate = selectedCols.Contains("ComplaintsCreateDate");
            bool showAgainst = selectedCols.Contains("ComplainedAgainst");
            bool showFullName = selectedCols.Contains("FullName");
            bool showContact = selectedCols.Contains("ContactNumber");
            bool showEmail = selectedCols.Contains("EmailAddress");
            bool showAffidavit = selectedCols.Contains("AffidavitFilesText");
            bool showAdditional = selectedCols.Contains("AdditionalFilesText");
            bool showTotalAmt = selectedCols.Contains("TotalAmount");
            bool showPaidAmt = selectedCols.Contains("PaidAmount");
            bool showDueAmt = selectedCols.Contains("DueAmount");
            bool showPayStatus = selectedCols.Contains("PaymentStatus");
            bool showPayInfo = selectedCols.Contains("PaymentInfoText");
            bool showStatus = selectedCols.Contains("Status");
            bool showFinance = showTotalAmt || showPaidAmt || showDueAmt || showPayStatus || showPayInfo;

            bool hasLeftFields = showFullName || showContact || showEmail || showAffidavit;
            bool hasRightFields = showAgainst || showAdditional;
            bool hasBody = hasLeftFields || hasRightFields;

            // ── Selected Columns Mode & Page Limit Rule ──
            // If ALL fields are selected (15 columns), enforce exactly 3 per page.
            // Otherwise, NO limit per page — cards flow naturally to fill pages without forced breaks.
            bool isAllFieldsSelected = selectedCols.Count >= 15;

            string cardMargin = isAllFieldsSelected ? "5px" : "8px";
            string hdrPadding = isAllFieldsSelected ? "4px 10px" : "5px 10px";
            string codeFontSize = isAllFieldsSelected ? "12.5px" : "13px";
            string bodyPadding = isAllFieldsSelected ? "4px 10px" : "6px 12px";
            string bodyMinH = "";
            string labelFontSize = isAllFieldsSelected ? "8px" : "8.5px";
            string labelMargin = isAllFieldsSelected ? "1px" : "2px";
            string valFontSize = isAllFieldsSelected ? "12px" : "12.5px";
            string valLineHeight = isAllFieldsSelected ? "1.3" : "1.35";
            string valSubFontSize = isAllFieldsSelected ? "10px" : "10.5px";
            string pillPadding = "2px 8px";
            string pillFontSize = "8.5px";

            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang=\"en\">");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta charset=\"utf-8\" />");
            sb.AppendLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />");
            sb.AppendLine("<title>Complaint List Report - Bar Council of Gujarat</title>");
            sb.AppendLine("<style>");
            sb.AppendLine("@import url('https://fonts.googleapis.com/css2?family=Noto+Sans+Gujarati:wght@400;500;600;700&display=swap');");
            sb.AppendLine("@page { size: A4 landscape; margin: 7mm 10mm 7mm 10mm; }");
            sb.AppendLine("* { box-sizing: border-box; -webkit-print-color-adjust: exact !important; print-color-adjust: exact !important; }");
            sb.AppendLine("body, table, td, th, div, span, p { font-family: 'Noto Sans Gujarati', 'Shruti', 'Nirmala UI', 'Gujarati Sangam MN', 'Segoe UI', 'Helvetica Neue', Arial, sans-serif; }");
            sb.AppendLine("body { font-size: 11px; color: #1e293b; background: #fff; margin: 0; padding: 0; line-height: 1.35; }");

            // ── Report Header ──
            sb.AppendLine(".rpt-header { width: 100%; border-collapse: collapse; margin-bottom: 3px; }");
            sb.AppendLine(".rpt-header td { vertical-align: middle; padding: 0; }");
            sb.AppendLine(".rpt-header .logo-td { width: 48px; max-width: 48px; padding: 0; text-align: left; white-space: nowrap; }");
            sb.AppendLine(".rpt-header .logo-td img { height: 46px; width: 46px; display: block; margin: 0; }");
            sb.AppendLine(".rpt-header .title-td { text-align: left; padding: 0 0 0 6px; }");
            sb.AppendLine(".rpt-header .badge-td { width: 150px; text-align: right; }");
            sb.AppendLine(".org-name { font-size: 16px; font-weight: 800; color: #002b5c; text-transform: uppercase; letter-spacing: 0.6px; margin: 0 0 1px 0; text-align: left; }");
            sb.AppendLine(".org-addr { font-size: 8.5px; color: #64748b; margin: 0; line-height: 1.25; text-align: left; }");
            sb.AppendLine(".doc-badge { display: inline-block; background: #002b5c; color: #fff; padding: 3px 10px; border-radius: 4px; font-weight: 700; font-size: 10.5px; letter-spacing: 0.4px; text-transform: uppercase; }");
            sb.AppendLine(".doc-badge-sub { font-size: 8px; color: #64748b; font-weight: 600; margin-top: 2px; text-align: right; text-transform: uppercase; letter-spacing: 0.2px; }");

            // ── Header Divider ──
            sb.AppendLine(".hdr-divider { border: none; border-top: 2px solid #002b5c; margin: 0 0 4px 0; }");

            // ── Meta Summary Bar ──
            sb.AppendLine(".meta-bar { width: 100%; border-collapse: collapse; background: #f1f5f9; border: 1px solid #cbd5e1; border-radius: 4px; margin-bottom: 5px; }");
            sb.AppendLine(".meta-bar td { padding: 3px 8px; font-size: 9.5px; vertical-align: middle; }");
            sb.AppendLine(".meta-bar .ml { font-weight: 600; color: #64748b; }");
            sb.AppendLine(".meta-bar .mv { font-weight: 700; color: #0f172a; }");
            sb.AppendLine(".cnt-badge { display: inline-block; background: #002b5c; color: #fff; padding: 2px 8px; border-radius: 10px; font-size: 9.5px; font-weight: 700; }");

            // ── Form Card ──
            sb.AppendLine(string.Format(".form-card {{ border: 1px solid #94a3b8; border-left: 4px solid #002b5c; border-radius: 4px; margin-bottom: {0}; page-break-inside: avoid; overflow: hidden; background: #fff; }}", cardMargin));
            sb.AppendLine(".page-break { page-break-after: always; height: 0; margin: 0; padding: 0; border: none; }");

            // ── Card Header ──
            sb.AppendLine(".card-hdr { width: 100%; border-collapse: collapse; background: #002b5c !important; }");
            sb.AppendLine(string.Format(".card-hdr td {{ padding: {0}; vertical-align: middle; }}", hdrPadding));
            sb.AppendLine(".card-hdr .sr-circle { display: inline-block; width: 20px; height: 20px; line-height: 20px; text-align: center; border-radius: 50%; background: rgba(255,255,255,0.25) !important; color: #fff; font-weight: 800; font-size: 10px; margin-right: 8px; vertical-align: middle; }");
            sb.AppendLine(string.Format(".card-hdr .cmp-code {{ font-family: 'Consolas', 'Courier New', monospace; font-size: {0}; font-weight: 800; color: #fff; vertical-align: middle; letter-spacing: 0.3px; }}", codeFontSize));
            sb.AppendLine(".card-hdr .cmp-date { font-size: 9.5px; color: rgba(255,255,255,0.85); margin-left: 10px; vertical-align: middle; }");
            sb.AppendLine(".card-hdr .pills-td { text-align: right; }");

            // ── Status Pills ──
            sb.AppendLine(string.Format(".pill {{ display: inline-block; padding: {0}; border-radius: 10px; font-size: {1}; font-weight: 700; text-transform: uppercase; letter-spacing: 0.3px; margin-left: 5px; }}", pillPadding, pillFontSize));
            sb.AppendLine(".pill-green { background: #dcfce7 !important; color: #15803d !important; border: 1px solid #86efac; }");
            sb.AppendLine(".pill-amber { background: #fef3c7 !important; color: #b45309 !important; border: 1px solid #fde68a; }");
            sb.AppendLine(".pill-red { background: #fee2e2 !important; color: #b91c1c !important; border: 1px solid #fca5a5; }");
            sb.AppendLine(".pill-blue { background: #e0f2fe !important; color: #0369a1 !important; border: 1px solid #bae6fd; }");

            // ── Card Body (Form Fields) ──
            sb.AppendLine(".card-body-tbl { width: 100%; border-collapse: collapse; }");
            sb.AppendLine(string.Format(".card-body-tbl td {{ padding: {0}; vertical-align: top; border-bottom: 1px solid #e2e8f0; {1} }}", bodyPadding, bodyMinH));
            if (hasLeftFields && hasRightFields)
            {
                sb.AppendLine(".card-body-tbl td.left-col { width: 50%; border-right: 1px dashed #cbd5e1; }");
                sb.AppendLine(".card-body-tbl td.right-col { width: 50%; }");
            }
            else if (hasLeftFields && !hasRightFields)
            {
                sb.AppendLine(".card-body-tbl td.left-col { width: 100%; border-right: none; }");
            }
            else if (!hasLeftFields && hasRightFields)
            {
                sb.AppendLine(".card-body-tbl td.right-col { width: 100%; }");
            }
            sb.AppendLine(".field-row { margin-bottom: 3px; }");
            sb.AppendLine(".field-row:last-child { margin-bottom: 0; }");
            sb.AppendLine(string.Format(".f-label {{ font-size: {0}; font-weight: 700; color: #64748b; text-transform: uppercase; letter-spacing: 0.4px; margin-bottom: {1}; }}", labelFontSize, labelMargin));
            sb.AppendLine(string.Format(".f-value {{ font-size: {0}; font-weight: 700; color: #0f172a; line-height: {1}; }}", valFontSize, valLineHeight));
            sb.AppendLine(string.Format(".f-value-sub {{ font-size: {0}; color: #334155; font-weight: 500; line-height: {1}; }}", valSubFontSize, valLineHeight));
            sb.AppendLine(".f-value-muted { font-size: 9.5px; color: #94a3b8; font-style: italic; }");

            // ── Amount styling ──
            sb.AppendLine(".amt { font-family: 'Consolas', 'Courier New', monospace; font-weight: 700; font-size: 11px; }");
            sb.AppendLine(".amt-green { color: #15803d; }");
            sb.AppendLine(".amt-red { color: #dc2626; }");
            sb.AppendLine(".amt-dark { color: #0f172a; }");

            // ── Document Link ──
            sb.AppendLine(".doc-link { display: inline-block; background: #f0f9ff; border: 1px solid #7dd3fc; border-radius: 3px; padding: 2px 6px; font-size: 9px; font-weight: 600; color: #0369a1; text-decoration: none; margin: 1px 3px 1px 0; line-height: 1.25; }");
            sb.AppendLine(".doc-link-icon { color: #dc2626; font-weight: 700; margin-right: 3px; }");

            // ── Card Finance Footer ──
            sb.AppendLine(".card-fin { width: 100%; border-collapse: collapse; background: #f8fafc !important; border-top: 1px dashed #cbd5e1; }");
            sb.AppendLine(".card-fin td { padding: 3px 10px; font-size: 9.5px; vertical-align: middle; }");
            sb.AppendLine(".card-fin .fin-label { font-weight: 600; color: #64748b; font-size: 8px; text-transform: uppercase; letter-spacing: 0.2px; }");
            sb.AppendLine(".card-fin .fin-val { font-family: 'Consolas', monospace; font-weight: 700; font-size: 11.5px; }");
            sb.AppendLine(".card-fin .receipt-info { font-size: 9.5px; color: #64748b; font-style: italic; text-align: right; }");

            // ── Report Footer ──
            sb.AppendLine(".rpt-footer { width: 100%; border-collapse: collapse; margin-top: 4px; border-top: 1px solid #002b5c; }");
            sb.AppendLine(".rpt-footer td { padding: 3px 0; font-size: 8.5px; color: #64748b; vertical-align: middle; }");

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
            sb.AppendLine("      <div class=\"doc-badge\">Complaint Register</div>");
            sb.AppendLine("      <div class=\"doc-badge-sub\">Disciplinary Proceedings Docket</div>");
            sb.AppendLine("    </td>");
            sb.AppendLine("  </tr>");
            sb.AppendLine("</table>");
            sb.AppendLine("<hr class=\"hdr-divider\" />");

            // ═══════ META SUMMARY BAR ═══════
            string dateRangeStr = (!string.IsNullOrEmpty(fromDate) || !string.IsNullOrEmpty(toDate))
                ? string.Format("{0} to {1}", !string.IsNullOrEmpty(fromDate) ? fromDate : "-", !string.IsNullOrEmpty(toDate) ? toDate : "-")
                : "All Registered Dates";

            sb.AppendLine("<table class=\"meta-bar\">");
            sb.AppendLine("  <tr>");
            sb.AppendLine("    <td><span class=\"ml\">Status:</span> <span class=\"mv\">" + (!string.IsNullOrEmpty(status) ? HttpUtility.HtmlEncode(status) : "All") + "</span></td>");
            sb.AppendLine("    <td><span class=\"ml\">Payment:</span> <span class=\"mv\">" + (!string.IsNullOrEmpty(paymentStatus) ? HttpUtility.HtmlEncode(paymentStatus) : "All") + "</span></td>");
            sb.AppendLine("    <td><span class=\"ml\">Date Range:</span> <span class=\"mv\">" + HttpUtility.HtmlEncode(dateRangeStr) + "</span></td>");
            sb.AppendLine("    <td><span class=\"ml\">Generated:</span> <span class=\"mv\">" + DateTime.Now.ToString("dd-MM-yyyy hh:mm tt") + "</span></td>");
            sb.AppendLine("    <td style=\"text-align:right;\"><span class=\"ml\">Total:</span> <span class=\"cnt-badge\">" + result.Count + " Complaints</span></td>");
            sb.AppendLine("  </tr>");
            sb.AppendLine("</table>");

            // ═══════ FORM CARDS ═══════
            for (int cardIndex = 0; cardIndex < result.Count; cardIndex++)
            {
                var item = result[cardIndex];
                sb.AppendLine("<div class=\"form-card\">");

                // ── Card Header Bar ──
                sb.AppendLine("  <table class=\"card-hdr\">");
                sb.AppendLine("    <tr>");
                sb.AppendLine("      <td>");
                if (showSrNo)
                {
                    sb.Append("        <span class=\"sr-circle\">" + (item.SrNo != null ? item.SrNo.ToString() : "-") + "</span>");
                }
                if (showComplaintNumber)
                {
                    sb.Append("<span class=\"cmp-code\">" + HttpUtility.HtmlEncode(item.ComplaintNumber ?? "-") + "</span>");
                }
                if (showCreateDate)
                {
                    sb.Append("<span class=\"cmp-date\">Filed: " + HttpUtility.HtmlEncode(item.ComplaintsCreateDate ?? "-") + "</span>");
                }
                sb.AppendLine();
                sb.AppendLine("      </td>");
                sb.AppendLine("      <td class=\"pills-td\">");
                if (showPayStatus)
                {
                    string ps = (item.PaymentStatus ?? "").Trim().ToLower();
                    string psCls = "pill-amber";
                    if (ps == "completed" || ps == "paid") psCls = "pill-green";
                    else if (ps == "pending" || ps.Contains("due")) psCls = "pill-red";
                    sb.Append("<span class=\"pill " + psCls + "\">" + HttpUtility.HtmlEncode(item.PaymentStatus ?? "-") + "</span>");
                }
                if (showStatus)
                {
                    string st = (item.Status ?? "").Trim();
                    string stLower = st.ToLower();
                    string stCls = "pill-blue";
                    if (stLower == "new") stCls = "pill-blue";
                    else if (stLower == "old") stCls = "pill-blue";
                    else if (stLower.Contains("accept") || stLower.Contains("approv")) stCls = "pill-green";
                    else if (stLower.Contains("reject")) stCls = "pill-red";
                    else if (stLower.Contains("hear") || stLower.Contains("adj")) stCls = "pill-amber";
                    sb.Append("<span class=\"pill " + stCls + "\">" + HttpUtility.HtmlEncode(st) + "</span>");
                }
                sb.AppendLine();
                sb.AppendLine("      </td>");
                sb.AppendLine("    </tr>");
                sb.AppendLine("  </table>");

                // ── Card Body (Form Fields) ──
                if (hasBody)
                {
                    sb.AppendLine("  <table class=\"card-body-tbl\">");
                    sb.AppendLine("    <tr>");

                    // Left Column: Complainant Info
                    if (hasLeftFields)
                    {
                        sb.AppendLine("      <td class=\"left-col\">");
                        if (showFullName)
                        {
                            sb.AppendLine("        <div class=\"field-row\">");
                            sb.AppendLine("          <div class=\"f-label\">Complainant Name</div>");
                            sb.AppendLine("          <div class=\"f-value\">" + HttpUtility.HtmlEncode(item.FullName ?? "-") + "</div>");
                            sb.AppendLine("        </div>");
                        }
                        if (showContact || showEmail)
                        {
                            sb.AppendLine("        <div class=\"field-row\">");
                            sb.AppendLine("          <div class=\"f-label\">Contact Information</div>");
                            sb.AppendLine("          <div class=\"f-value-sub\">");
                            if (showContact)
                            {
                                sb.Append("            &#128222; " + HttpUtility.HtmlEncode(item.ContactNumber ?? "-"));
                            }
                            if (showContact && showEmail)
                            {
                                sb.Append(" &nbsp;&nbsp;|&nbsp;&nbsp; ");
                            }
                            if (showEmail)
                            {
                                string emailVal = !string.IsNullOrEmpty(item.EmailAddress) ? HttpUtility.HtmlEncode(item.EmailAddress) : "<em class=\"f-value-muted\">Not Provided</em>";
                                sb.Append("&#9993; " + emailVal);
                            }
                            sb.AppendLine();
                            sb.AppendLine("          </div>");
                            sb.AppendLine("        </div>");
                        }
                        if (showAffidavit)
                        {
                            sb.AppendLine("        <div class=\"field-row\">");
                            sb.AppendLine("          <div class=\"f-label\">Complaint Affidavit Copy</div>");
                            var affFiles = ParseFileList(item.ComplaintAffidavitCopyList);
                            if (affFiles.Count > 0)
                            {
                                foreach (var f in affFiles)
                                {
                                    sb.AppendLine("          <a href=\"" + HttpUtility.HtmlAttributeEncode(f.FilePath) + "\" target=\"_blank\" class=\"doc-link\"><span class=\"doc-link-icon\">&#128196;</span>" + HttpUtility.HtmlEncode(f.FileName) + "</a>");
                                }
                            }
                            else
                            {
                                sb.AppendLine("          <div class=\"f-value-muted\">No affidavit uploaded</div>");
                            }
                            sb.AppendLine("        </div>");
                        }
                        sb.AppendLine("      </td>");
                    }

                    // Right Column: Complained Against + Additional Attachments
                    if (hasRightFields)
                    {
                        sb.AppendLine("      <td class=\"right-col\">");
                        if (showAgainst)
                        {
                            sb.AppendLine("        <div class=\"field-row\">");
                            sb.AppendLine("          <div class=\"f-label\">Complained Against (Advocate)</div>");
                            sb.AppendLine("          <div class=\"f-value\">" + HttpUtility.HtmlEncode(item.ComplainedAgainst ?? "-") + "</div>");
                            sb.AppendLine("        </div>");
                        }
                        if (showAdditional)
                        {
                            sb.AppendLine("        <div class=\"field-row\">");
                            sb.AppendLine("          <div class=\"f-label\">Additional Attachments</div>");
                            var addFiles = ParseFileList(item.ComplaintAdditionalAttachmentsList);
                            if (addFiles.Count > 0)
                            {
                                foreach (var f in addFiles)
                                {
                                    sb.AppendLine("          <a href=\"" + HttpUtility.HtmlAttributeEncode(f.FilePath) + "\" target=\"_blank\" class=\"doc-link\"><span class=\"doc-link-icon\">&#128196;</span>" + HttpUtility.HtmlEncode(f.FileName) + "</a>");
                                }
                            }
                            else
                            {
                                sb.AppendLine("          <div class=\"f-value-muted\">No additional attachments</div>");
                            }
                            sb.AppendLine("        </div>");
                        }
                        sb.AppendLine("      </td>");
                    }

                    sb.AppendLine("    </tr>");
                    sb.AppendLine("  </table>");
                }

                // ── Card Finance Footer ──
                if (showFinance)
                {
                    sb.AppendLine("  <table class=\"card-fin\">");
                    sb.AppendLine("    <tr>");
                    if (showTotalAmt)
                    {
                        sb.AppendLine("      <td><span class=\"fin-label\">Total Amount</span><br/><span class=\"fin-val amt-dark\">&#8377; " + item.TotalAmount.ToString("N2") + "</span></td>");
                    }
                    if (showPaidAmt)
                    {
                        sb.AppendLine("      <td><span class=\"fin-label\">Paid Amount</span><br/><span class=\"fin-val amt-green\">&#8377; " + item.PaidAmount.ToString("N2") + "</span></td>");
                    }
                    if (showDueAmt)
                    {
                        decimal due = item.DueAmount;
                        string dueClass = due > 0 ? "amt-red" : "amt-green";
                        sb.AppendLine("      <td><span class=\"fin-label\">Due Amount</span><br/><span class=\"fin-val " + dueClass + "\">&#8377; " + due.ToString("N2") + "</span></td>");
                    }
                    if (showPayInfo)
                    {
                        string payInfoHtml = !string.IsNullOrEmpty(item.PaymentInfoText) ? HttpUtility.HtmlEncode(item.PaymentInfoText).Replace("\n", "<br/>") : "<em>No payment details</em>";
                        sb.AppendLine("      <td class=\"receipt-info\">" + payInfoHtml + "</td>");
                    }
                    sb.AppendLine("    </tr>");
                    sb.AppendLine("  </table>");
                }

                sb.AppendLine("</div>"); // .form-card

                // If all fields are selected, enforce exactly 3 per page; otherwise no limit per page (cards flow naturally)
                if (isAllFieldsSelected && (cardIndex + 1) % 3 == 0 && cardIndex + 1 < result.Count)
                {
                    sb.AppendLine("<div class=\"page-break\"></div>");
                }
            }

            // ═══════ REPORT FOOTER ═══════
            sb.AppendLine("<table class=\"rpt-footer\">");
            sb.AppendLine("  <tr>");
            sb.AppendLine("    <td><strong>Bar Council of Gujarat</strong> &bull; Statutory Body Constituted under Advocates Act, 1961 &bull; Confidential Official Record</td>");
            sb.AppendLine("    <td style=\"text-align:right;\">Generated: " + DateTime.Now.ToString("dd-MM-yyyy hh:mm tt") + " &bull; Page may contain sensitive personal information</td>");
            sb.AppendLine("  </tr>");
            sb.AppendLine("</table>");

            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

        [HttpPost]
        public ActionResult GetAllComplaintReportsListFilterSet(String searchTerm, String Status, String PaymentStatus, String FromDate, String ToDate, String Year, String FromNo, String ToNo, String selectedColumns = "")
        {
            try
            {
                Response.Cookies["searchTerm"].Value = searchTerm;
                Response.Cookies["Status"].Value = Status.ToString();
                Response.Cookies["PaymentStatus"].Value = PaymentStatus.ToString();
                Response.Cookies["FromDate"].Value = FromDate.ToString();
                Response.Cookies["ToDate"].Value = ToDate.ToString();
                Response.Cookies["Year"].Value = Year != null ? Year.ToString() : "";
                Response.Cookies["FromNo"].Value = FromNo != null ? FromNo.ToString() : "";
                Response.Cookies["ToNo"].Value = ToNo != null ? ToNo.ToString() : "";
                Response.Cookies["SelectedColumns"].Value = selectedColumns != null ? selectedColumns : "";
                Session["SelectedColumns"] = selectedColumns != null ? selectedColumns : "";
                return Json(new { data = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }

        [HttpGet]
        public ActionResult GetAllComplaintPDFReport()
        {
            try
            {
                #region Variable Set
                String searchTerm = "";
                String status = "";
                String paymentStatus = "";
                String fromDate = "";
                String toDate = "";

                #region searchTerm
                if (Request.Cookies["searchTerm"] != null && Request.Cookies["searchTerm"].Value != null)
                {
                    searchTerm = Request.Cookies["searchTerm"].Value.ToString();
                }
                #endregion

                #region Status
                if (Request.Cookies["Status"] != null && Request.Cookies["Status"].Value != null)
                {
                    status = Request.Cookies["Status"].Value.ToString();
                }
                #endregion

                #region PaymentStatus
                if (Request.Cookies["PaymentStatus"] != null && Request.Cookies["PaymentStatus"].Value != null)
                {
                    paymentStatus = Request.Cookies["PaymentStatus"].Value.ToString();
                }
                #endregion

                #region FromDate
                if (Request.Cookies["FromDate"] != null && Request.Cookies["FromDate"].Value != null)
                {
                    fromDate = Request.Cookies["FromDate"].Value.ToString();
                }
                #endregion

                #region ToDate
                if (Request.Cookies["ToDate"] != null && Request.Cookies["ToDate"].Value != null)
                {
                    toDate = Request.Cookies["ToDate"].Value.ToString();
                }
                #endregion

                BaseFilter baseFilter = new BaseFilter();
                baseFilter.searchValue = searchTerm;
                baseFilter.pageNo = 1;
                baseFilter.pageSize = 0;
                baseFilter.sortColumn = 1;
                baseFilter.sortOrder = "desc";
                #endregion

                List<ComplaintListModel> result = _ComplaintRepository.GetComplaintList(baseFilter, status, paymentStatus, fromDate, toDate, Request.Cookies["Year"] != null ? Request.Cookies["Year"].Value : "", Request.Cookies["FromNo"] != null ? Request.Cookies["FromNo"].Value : "", Request.Cookies["ToNo"] != null ? Request.Cookies["ToNo"].Value : "", "ReportExcePDFSelectAll");

                string selectedColsStr = "";
                if (Request.Cookies["SelectedColumns"] != null && !string.IsNullOrEmpty(Request.Cookies["SelectedColumns"].Value))
                {
                    selectedColsStr = Request.Cookies["SelectedColumns"].Value;
                }
                else if (Session["SelectedColumns"] != null)
                {
                    selectedColsStr = Session["SelectedColumns"].ToString();
                }

                List<string> selectedCols = string.IsNullOrEmpty(selectedColsStr)
                    ? new List<string>()
                    : selectedColsStr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                if (selectedCols.Count == 0)
                {
                    selectedCols = new List<string> {
                        "SrNo", "ComplaintNumber", "ComplaintsCreateDate", "ComplainedAgainst",
                        "FullName", "ContactNumber", "EmailAddress", "AffidavitFilesText",
                        "AdditionalFilesText", "TotalAmount", "PaidAmount", "DueAmount",
                        "PaymentStatus", "PaymentInfoText", "Status"
                    };
                }

                string html = GeneratePrintReportHtml(result, selectedCols, searchTerm, status, paymentStatus, fromDate, toDate);
                byte[] pdfBytes = ConvertHtmlToPdf(html);
                string fileName = "ComplaintListReport_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".pdf";
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }

        [HttpGet]
        public ActionResult GetAllComplaintExcelReport()
        {
            try
            {
                #region Variable Set
                String searchTerm = "";
                String status = "";
                String paymentStatus = "";
                String fromDate = "";
                String toDate = "";

                #region searchTerm
                if (Request.Cookies["searchTerm"] != null && Request.Cookies["searchTerm"].Value != null)
                {
                    if (Request.Cookies["searchTerm"].Value.ToString() != "")
                    {
                        searchTerm = Request.Cookies["searchTerm"].Value.ToString();
                    }
                    else
                    {
                        searchTerm = "";
                    }
                }
                else
                {
                    searchTerm = "";
                }
                #endregion

                #region Status
                if (Request.Cookies["Status"] != null && Request.Cookies["Status"].Value != null)
                {
                    if (Request.Cookies["Status"].Value.ToString() != "")
                    {
                        status = Request.Cookies["Status"].Value.ToString();
                    }
                    else
                    {
                        status = "";
                    }
                }
                else
                {
                    status = "";
                }
                #endregion

                #region PaymentStatus
                if (Request.Cookies["PaymentStatus"] != null && Request.Cookies["PaymentStatus"].Value != null)
                {
                    if (Request.Cookies["PaymentStatus"].Value.ToString() != "")
                    {
                        paymentStatus = Request.Cookies["PaymentStatus"].Value.ToString();
                    }
                    else
                    {
                        paymentStatus = "";
                    }
                }
                else
                {
                    paymentStatus = "";
                }
                #endregion

                #region FromDate
                if (Request.Cookies["FromDate"] != null && Request.Cookies["FromDate"].Value != null)
                {
                    if (Request.Cookies["FromDate"].Value.ToString() != "")
                    {
                        fromDate = Request.Cookies["FromDate"].Value.ToString();
                    }
                    else
                    {
                        fromDate = "";
                    }
                }
                else
                {
                    fromDate = "";
                }
                #endregion

                #region ToDate
                if (Request.Cookies["ToDate"] != null && Request.Cookies["ToDate"].Value != null)
                {
                    if (Request.Cookies["ToDate"].Value.ToString() != "")
                    {
                        toDate = Request.Cookies["ToDate"].Value.ToString();
                    }
                    else
                    {
                        toDate = "";
                    }
                }
                else
                {
                    toDate = "";
                }
                #endregion

                BaseFilter baseFilter = new BaseFilter();
                baseFilter.searchValue = searchTerm;
                baseFilter.pageNo = 1;
                baseFilter.pageSize = 0; //parseInt($('#ddlPageSize :selected').val())
                baseFilter.sortColumn = 1;
                baseFilter.sortOrder = "desc";
                #endregion

                List<ComplaintListModel> result = _ComplaintRepository.GetComplaintList(baseFilter, status, paymentStatus, fromDate, toDate, Request.Cookies["Year"] != null ? Request.Cookies["Year"].Value : "", Request.Cookies["FromNo"] != null ? Request.Cookies["FromNo"].Value : "", Request.Cookies["ToNo"] != null ? Request.Cookies["ToNo"].Value : "", "ReportExcePDFSelectAll");

                string selectedColsStr = "";
                if (Request.Cookies["SelectedColumns"] != null && !string.IsNullOrEmpty(Request.Cookies["SelectedColumns"].Value))
                {
                    selectedColsStr = Request.Cookies["SelectedColumns"].Value;
                }
                else if (Session["SelectedColumns"] != null)
                {
                    selectedColsStr = Session["SelectedColumns"].ToString();
                }

                List<string> selectedCols = string.IsNullOrEmpty(selectedColsStr)
                    ? new List<string>()
                    : selectedColsStr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                if (selectedCols.Count == 0)
                {
                    selectedCols = new List<string> {
                        "SrNo", "ComplaintNumber", "ComplaintsCreateDate", "ComplainedAgainst",
                        "FullName", "EmailAddress", "ContactNumber", "AffidavitFilesText",
                        "AdditionalFilesText", "TotalAmount", "PaidAmount", "DueAmount",
                        "PaymentStatus", "PaymentInfoText", "Status"
                    };
                }

                if (result.Count > 0)
                {
                    DataTable compList = new DataTable("Complaint List Excel");
                    foreach (var colKey in selectedCols)
                    {
                        if (ReportColumnDefinitions.ContainsKey(colKey))
                        {
                            compList.Columns.Add(new DataColumn(ReportColumnDefinitions[colKey].Item1, typeof(String)));
                        }
                    }

                    foreach (var item in result)
                    {
                        DataRow row = compList.NewRow();
                        foreach (var colKey in selectedCols)
                        {
                            if (ReportColumnDefinitions.ContainsKey(colKey))
                            {
                                string header = ReportColumnDefinitions[colKey].Item1;
                                row[header] = ReportColumnDefinitions[colKey].Item2(item) ?? "";
                            }
                        }
                        compList.Rows.Add(row);
                    }

                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        var ws = wb.Worksheets.Add(compList);

                        for (int r = 0; r < result.Count; r++)
                        {
                            var item = result[r];
                            int excelRow = r + 2;

                            for (int c = 0; c < selectedCols.Count; c++)
                            {
                                string colKey = selectedCols[c];
                                int excelCol = c + 1;

                                if (colKey == "AffidavitFilesText")
                                {
                                    var files = ParseFileList(item.ComplaintAffidavitCopyList);
                                    if (files.Count > 0)
                                    {
                                        var cell = ws.Cell(excelRow, excelCol);
                                        cell.Value = files[0].FileName;
                                        if (!string.IsNullOrEmpty(files[0].FilePath))
                                        {
                                            cell.Hyperlink = new XLHyperlink(files[0].FilePath);
                                            cell.Style.Font.FontColor = XLColor.Blue;
                                            cell.Style.Font.Underline = XLFontUnderlineValues.Single;
                                        }
                                    }
                                }
                                else if (colKey == "AdditionalFilesText")
                                {
                                    var files = ParseFileList(item.ComplaintAdditionalAttachmentsList);
                                    if (files.Count > 0)
                                    {
                                        var cell = ws.Cell(excelRow, excelCol);
                                        cell.Value = files[0].FileName;
                                        if (!string.IsNullOrEmpty(files[0].FilePath))
                                        {
                                            cell.Hyperlink = new XLHyperlink(files[0].FilePath);
                                            cell.Style.Font.FontColor = XLColor.Blue;
                                            cell.Style.Font.Underline = XLFontUnderlineValues.Single;
                                        }
                                    }
                                }
                            }
                        }

                        using (MemoryStream stream = new MemoryStream())
                        {
                            wb.SaveAs(stream);
                            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ComplaintListExcel.xlsx");
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                //ExceptionInsert("2013", ex.GetType().ToString(), ex.Message.ToString(), ex.StackTrace.ToString(), ex.Source.ToString(), "GetAllInstitutesProgramsReportsListFilterSet()");
                return View("Error");
            }
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

        public ActionResult ComplaintActionLogReport()
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
    }
}
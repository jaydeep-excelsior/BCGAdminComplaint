using BCGAdminComplaint.Models.DCCommittee;
using BCGAdminComplaint.Models.DCMember;
using BCGAdminComplaint.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace BCGAdminComplaint.Controllers
{
    public class DCCommitteeController : Controller
    {
        private readonly DCCommitteeRepository _dCCommittee;
        private readonly DCMemberRepository _dCMemberRepository;

        public DCCommitteeController()
        {
            _dCCommittee = new DCCommitteeRepository();
            _dCMemberRepository = new DCMemberRepository();
        }

        public ActionResult Index()
        {
            if (Request.Cookies["bcg"] != null)
            {
                ViewBag.IsAdd = "A"; // Mocking rights since old project lacks UserMenuRightsDetail
                return View();
            }
            else
            {
                return Redirect("/Home/Login");
            }
        }

        [HttpPost]
        public ActionResult DCCommitteeLists(string Id, string SearchValue, string FromDate, string ToDate, string RecordStatus, string PageNo, string PageSize, string SortBy)
        {
            try
            {
                if (Request.Cookies["bcg"] == null)
                    return Redirect("/Home/Login");

                Int64 IId = Convert.ToInt64(string.IsNullOrEmpty(Id) ? "0" : Id);
                String SSearchValue = SearchValue ?? "";
                int IPageNo = Convert.ToInt32(string.IsNullOrEmpty(PageNo) ? "1" : PageNo);
                int IPageSize = Convert.ToInt32(string.IsNullOrEmpty(PageSize) ? "25" : PageSize);
                Int64 BCGV2UserId = 1; // Default
                Int64 ISubMenuId = 115;
                String SSortBy = string.IsNullOrEmpty(SortBy) ? "ByNewest" : SortBy;

                var UTL = _dCCommittee.DCCommitteesList(IId, SSearchValue, IPageNo, IPageSize, BCGV2UserId, ISubMenuId, SSortBy, RecordStatus ?? "");

                return Json(new { data = UTL }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { data = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Add()
        {
            if (Request.Cookies["bcg"] != null)
            {
                var UTL = _dCMemberRepository.DCMembersList(0, "", "", "", "A", 1, 9999, 1, 115, "ByNewest", "SelectList");
                var DCMemberQuery = UTL.Select(DCMemberL => new
                {
                    Id = DCMemberL.Id,
                    Title = DCMemberL.FullName + (!string.IsNullOrEmpty(DCMemberL.MemberType) ? (DCMemberL.MemberType == "B" ? " (BCG MEMBER)" : DCMemberL.MemberType == "O" ? " (CO-OPT MEMBER)" : " (" + DCMemberL.MemberType + ")") : "")
                }).ToList();
                ViewBag.DCMemberList = new SelectList(DCMemberQuery, "Id", "Title");
                ViewBag.IsAdd = "A";
                return View(UTL);
            }
            else
            {
                return Redirect("/Home/Login");
            }
        }

        public ActionResult Edit()
        {
            if (Request.Cookies["bcg"] != null)
            {
                var UTL = _dCMemberRepository.DCMembersList(0, "", "", "", "A", 1, 9999, 1, 115, "ByNewest", "SelectList");
                var DCMemberQuery = UTL.Select(DCMemberL => new
                {
                    Id = DCMemberL.Id,
                    Title = DCMemberL.FullName + (!string.IsNullOrEmpty(DCMemberL.MemberType) ? (DCMemberL.MemberType == "B" ? " (BCG MEMBER)" : DCMemberL.MemberType == "O" ? " (CO-OPT MEMBER)" : " (" + DCMemberL.MemberType + ")") : "")
                }).ToList();
                ViewBag.DCMemberList = new SelectList(DCMemberQuery, "Id", "Title");
                ViewBag.IsUpdate = "A";
                return View(UTL);
            }
            else
            {
                return Redirect("/Home/Login");
            }
        }

        [HttpPost]
        public ActionResult DCCommitteeMembersInserts(Int64? Id, string CommitteeTitle, Int64 DcCommitteeNo, string Note, Int64 DC_ComplaintId, List<Int64> MemberIds)
        {
            try
            {
                if (Request.Cookies["bcg"] == null)
                    return Json(new { data = "ER", error = "User not authenticated" }, JsonRequestBehavior.AllowGet);

                Int64 committeeId = Id ?? 0;
                string committeeActionMode = "InsertUpdate";
                Int64 BCGV2UserId = 1;

                var committeeResult = _dCCommittee.DCCommitteeInsert(
                    committeeId,
                    CommitteeTitle?.Trim() ?? "",
                    DcCommitteeNo,
                    Note?.Trim() ?? "",
                    "A",
                    BCGV2UserId,
                    committeeActionMode
                );

                if (committeeResult == null || !committeeResult.Any())
                    return Json(new { data = "ER", error = "Committee insert/update failed" }, JsonRequestBehavior.AllowGet);

                var committeeStatus = committeeResult.First().ReturnStatus?.ToUpper() ?? "ER";
                if (committeeStatus != "IN" && committeeStatus != "UP" && committeeStatus != "UD" && committeeStatus != "EX")
                    return Json(new { data = "ER", error = "Unhandled Committee status: " + committeeStatus }, JsonRequestBehavior.AllowGet);

                long newCommitteeId = 0;
                long.TryParse(committeeResult.First().Id, out newCommitteeId);
                if (newCommitteeId == 0) newCommitteeId = committeeId;

                // Delete existing members before inserting new ones
                if (committeeId > 0)
                {
                    _dCCommittee.DCCommitteeMemberInsert(0, newCommitteeId, 0, "Delete");
                }

                if (MemberIds != null && MemberIds.Count > 0)
                {
                    foreach (var memberId in MemberIds)
                    {
                        var memberResult = _dCCommittee.DCCommitteeMemberInsert(
                            0,              
                            newCommitteeId, 
                            memberId,
                            "Insert"
                        );

                        string memberStatus = memberResult?.FirstOrDefault()?.ReturnStatus ?? "ER";
                        // if (memberStatus != "IN") handle error if needed
                    }
                }

                return Json(new { data = "OK", committeeStatus, memberStatus = "DONE" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { data = "ER", error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GetCommitteeMembers(Int64 committeeId)
        {
            try
            {
                var members = _dCCommittee.GetCommitteeMembers(committeeId);
                return Json(new { success = true, data = members }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DCCommitteeMembersDataEdit(string id)
        {
            try
            {
                if (Request.Cookies["bcg"] == null)
                    return Redirect("/Home/Login");

                Int64 IId = Convert.ToInt64(string.IsNullOrEmpty(id) ? "0" : id);

                List<DCCommitteeEdit> mainData = _dCCommittee.DCCommitteesEdit(IId);
                List<DCCommitteeMemberEdit> attachmentData = _dCCommittee.DCCommitteeMembersEdit(IId);

                return Json(new
                {
                    success = true,
                    mainData = mainData,
                    attachmentData = attachmentData
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error occurred while fetching data.",
                    error = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DCCommitteeMembersStatusUpdate(string id, string Flag)
        {
            try
            {
                if (Request.Cookies["bcg"] == null)
                    return Redirect("/Home/Login");

                Int64 IId = Convert.ToInt64(string.IsNullOrEmpty(id) ? "0" : id);
                String SFlag = Flag ?? "";
                
                var UTSR = _dCCommittee.DCCommitteeStatusUpdate(IId, SFlag);
                String SRecordStatus = (UTSR != null && UTSR.Any()) ? UTSR[0].ReturnStatus : "";
                
                return Json(new { data = SRecordStatus, Flag = SFlag }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { data = "" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
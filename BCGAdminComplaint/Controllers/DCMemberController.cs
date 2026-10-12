using BCGAdminComplaint.Models.DCMember;
using BCGAdminComplaint.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BCGAdminComplaint.Controllers
{
    public class DCMemberController : Controller
    {
        private readonly DCMemberRepository _dCMemberRepository;

        public DCMemberController()
        {
            _dCMemberRepository = new DCMemberRepository();
        }

        public ActionResult Index()
        {
            if (Request.Cookies["bcg"] != null)
            {
                return View();
            }
            else
            {
                Response.Redirect("/Home/Login");
                return null;
            }
        }

        [HttpPost]
        public ActionResult DCMemberLists(string Id, string SearchValue, string FromDate, string ToDate, string RecordStatus, string PageNo, string PageSize, string SortBy)
        {
            try
            {
                if (Request.Cookies["bcg"] == null)
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    List<DCMemberList> UTL = new List<DCMemberList>();
                    Int64 IId = Convert.ToInt64(Id == null ? "0" : Id.ToString() == "" ? "0" : Id);
                    String SSearchValue = SearchValue == null ? "" : SearchValue;
                    String SFromDate = FromDate == null ? "" : FromDate;
                    String SToDate = ToDate == null ? "" : ToDate;
                    String SRecordStatus = RecordStatus == null ? "" : RecordStatus;
                    int IPageNo = Convert.ToInt32(PageNo == null ? "1" : PageNo.ToString() == "" ? "1" : PageNo);
                    int IPageSize = Convert.ToInt32(PageSize == null ? "0" : PageSize.ToString() == "" ? "0" : PageSize);
                    Int64 ISubMenuId = 114;
                    String SSortBy = SortBy == null ? "ByNewest" : SortBy.ToString() == "" ? "ByNewest" : SortBy;
                    Int64 LoginUserId = 0;

                    UTL = _dCMemberRepository.DCMembersList(IId, SSearchValue, SFromDate, SToDate, SRecordStatus, IPageNo, IPageSize, LoginUserId, ISubMenuId, SSortBy, "SelectList");

                    return Json(new { data = UTL }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { data = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Add()
        {
            if (Request.Cookies["bcg"] != null)
            {
                return View();
            }
            else
            {
                Response.Redirect("/Home/Login");
                return null;
            }
        }

        public ActionResult Edit()
        {
            if (Request.Cookies["bcg"] != null)
            {
                return View();
            }
            else
            {
                Response.Redirect("/Home/Login");
                return null;
            }
        }

        [HttpPost]
        public ActionResult DCMembersInsert(string Id, string EnrollmentId, string EmailAddress, string Password, string Name, string ContactNo, string BarAssociation)
        {
            try
            {
                if (false && Request.Cookies["bcg"] == null)
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    Int64 IId = Convert.ToInt64(Id == null ? "0" : Id.ToString() == "" ? "0" : Id.ToString());
                    List<DCMemberSave> UTSR = new List<DCMemberSave>();
                    Int64 SEnrollmentId = Convert.ToInt64(EnrollmentId == null ? "0" : EnrollmentId.ToString() == "" ? "0" : EnrollmentId.ToString());
                    String SEmailAddress = EmailAddress == null ? "" : EmailAddress.ToString();
                    String SPassword = Password == null ? "" : Password.ToString();
                    String SName = Name == null ? "" : Name.ToString();
                    String SContactNo = ContactNo == null ? "" : ContactNo.ToString();
                    String SBarAssociation = BarAssociation == null ? "" : BarAssociation.ToString();
                    Int64 LoginUserId = 0;

                    if (IId > 0)
                    {
                        UTSR = _dCMemberRepository.DCMemberInsert(IId, SEnrollmentId, SEmailAddress, SPassword, "A", LoginUserId, "UpdateRecord", SName, SContactNo, SBarAssociation);
                    }
                    else
                    {
                        UTSR = _dCMemberRepository.DCMemberInsert(IId, SEnrollmentId, SEmailAddress, SPassword, "A", LoginUserId, "InsertRecord", SName, SContactNo, SBarAssociation);
                    }
                    String SRecordStatus = UTSR == null ? "" : UTSR[0].ReturnStatus == null ? "" : UTSR[0].ReturnStatus;
                    return Json(new { data = SRecordStatus }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { data = ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DCMemberDataEdit(string id)
        {
            try
            {
                if (Request.Cookies["bcg"] == null)
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    Int64 IDCMemberId = Convert.ToInt64(id == null ? "0" : id.ToString() == "" ? "0" : id.ToString());
                    List<DCMemberEdit> UTE = new List<DCMemberEdit>();
                    UTE = _dCMemberRepository.DCMembersEdit(IDCMemberId, "EditRecord");
                    return Json(new { data = UTE }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { data = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DCMembersStatusUpdate(string id, string Flag)
        {
            try
            {
                if (Request.Cookies["bcg"] == null)
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    Int64 IDCMemberId = Convert.ToInt64(id == null ? "0" : id.ToString() == "" ? "0" : id.ToString());
                    List<DCMemberSave> UTSR = new List<DCMemberSave>();
                    String SFlag = Flag == null ? "" : Flag;
                    Int64 LoginUserId = 0;
                    UTSR = _dCMemberRepository.DCMemberStatusUpdate(IDCMemberId, SFlag, LoginUserId, "StatusUpdateRecord");
                    String SRecordStatus = UTSR == null ? "" : UTSR[0].ReturnStatus == null ? "" : UTSR[0].ReturnStatus;
                    return Json(new { data = SRecordStatus, Flag = SFlag }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { data = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GetEnrollmentDetails(string enroNumber)
        {
            try
            {
                if (Request.Cookies["bcg"] == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    DCMemberEnrollmentDetails details = _dCMemberRepository.GetEnrollmentDetails(enroNumber);
                    return Json(new { data = details }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { data = "" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
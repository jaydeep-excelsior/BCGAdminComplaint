using BCGAdminComplaint.Models;
using BCGAdminComplaint.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BarCouncilComplaint.Controllers
{
    public class MemberComplaintListController : BaseController
    {
        private readonly ComplaintRepository _ComplaintRepository;
        private readonly ComplaintDetailsRepository _ComplaintDetailsRepository;

        public MemberComplaintListController()
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
                ViewBag.MemberId = Request.Cookies["CMPUserId"].Value;
                return View();
            }
        }

        [HttpPost]
        public ActionResult GetMemberComplaintList(BaseFilter baseFilter, String MemberId)
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
                    List<ComplaintListModel> result = _ComplaintRepository.GetMemberComplaintList(baseFilter, Convert.ToInt64(MemberId));
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
        public ActionResult SaveMemberNote(string NoteType, string Note,Int64 ComplaintId)
        {
            try
            {
                Int64 CreateUser = Convert.ToInt64(Request.Cookies["CMPUserId"].Value);
                Int64 MemberId = Convert.ToInt64(Request.Cookies["CMPUserId"].Value);
                var result = _ComplaintRepository.InsertMemberNote(MemberId, NoteType, Note, CreateUser, ComplaintId);
                if (result.Count > 0)
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
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetMemberNoteList(Int64 ComplaintId)
        {
            try
            {
                Int64 MemberId = Convert.ToInt64(Request.Cookies["CMPUserId"].Value);
                var result = _ComplaintRepository.GetMemberNoteList(MemberId, ComplaintId);
                if (result.Count > 0)
                {
                    for (int i = 0; i < result.Count; i++)
                    {
                        result[i].SCreateDate = result[i].CreateDate.ToString("yyyy-MM-dd");
                    }
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
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MemberDetails(Int64 Id)
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
                    List<ComplaintDocumentList> ComplaintDocument = new List<ComplaintDocumentList>();
                    List<ComplaintMemberLog> DcMemberList = new List<ComplaintMemberLog>();
                    List<DcNumberList> result = new List<DcNumberList>();

                    ComplaintDetail = _ComplaintRepository.GetComplaintDetails(Id);
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

                        ViewBag.MemberList = dcMember;
                    }

                    var query = result.Select(DcNumber => new
                    {
                        Id = DcNumber.Id,
                        DcTitle = DcNumber.DcTitle,

                    }).OrderBy(x => x.DcTitle)
                     .ToList();

                    ViewBag.DcNumber = DcMemberList[0].CommitteeId;

                    return View(new Tuple<IEnumerable<ComplaintListModel>, IEnumerable<ComplaintDocumentList>, IEnumerable<ComplaintMemberLog>>(ComplaintDetail, ComplaintDocument, DcMemberList));
                }
                catch (Exception ex)
                {
                    return View();
                }
            }
        }

        [HttpPost]
        public ActionResult GetLogList() 
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
                    Int64 MemberId = Convert.ToInt64(Request.Cookies["CMPUserId"].Value);

                    var result = _ComplaintDetailsRepository.GetLogListByMemberId(MemberId);

                    if (result.Count > 0)
                    {
                        for (int i = 0; i < result.Count; i++)
                        {
                            result[i].SNextDate = result[i].NextDate.ToString("dd-MM-yyyy hh:mm tt");
                        }

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

            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
    }
}
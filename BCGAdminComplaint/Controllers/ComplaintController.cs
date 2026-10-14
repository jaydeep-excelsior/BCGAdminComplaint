using BCGAdminComplaint.Models;
using BCGAdminComplaint.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BarCouncilComplaint.Controllers
{
    public class ComplaintListController : BaseController
    {
        // GET: ComplaintList
        private readonly ComplaintRepository _ComplaintRepository;
        private readonly ComplaintDetailsRepository _ComplaintDetailsRepository;

        public ComplaintListController()
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

        public ActionResult OnlineComplaintList()
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

        public ActionResult AddComplaint()
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

        public ActionResult SearchComplaint()
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
        public ActionResult GetComplaintList(BaseFilter baseFilter, String Status, String PaymentStatus, String FromDate, String ToDate, String Year, String FromNo, String ToNo)
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
                    List<ComplaintListModel> result = _ComplaintRepository.GetComplaintList(baseFilter, Status, PaymentStatus, FromDate, ToDate, Year, FromNo, ToNo, "SelectAll");
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
        public ActionResult GetOnlineComplaintList(BaseFilter baseFilter, String Status, String PaymentStatus, String FromDate, String ToDate, String Year, String FromNo, String ToNo)
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
                    List<ComplaintListModel> result = _ComplaintRepository.GetOnlineComplaintList(baseFilter, Status, PaymentStatus, FromDate, ToDate, Year, FromNo, ToNo, 0, 0, "SelectAll");
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
        public ActionResult GetDocument(String ComplaintId)
        {
            try
            {
                Int64 IComplaintId = Convert.ToInt64(ComplaintId == null ? "0" : ComplaintId == "" ? "0" : ComplaintId);
                List<ComplaintDocumentList> result = _ComplaintRepository.GetDocumentList(IComplaintId);
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
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetOnlineDocument(String ComplaintId)
        {
            try
            {
                Int64 IComplaintId = Convert.ToInt64(ComplaintId == null ? "0" : ComplaintId == "" ? "0" : ComplaintId);
                List<ComplaintDocumentList> result = _ComplaintRepository.GetOnlineDocumentList(IComplaintId);
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
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetAdjuntDetails(String ComplaintId)
        {
            try
            {
                Int64 IComplaintId = Convert.ToInt64(ComplaintId == null ? "0" : ComplaintId == "" ? "0" : ComplaintId);
                List<ComplaintAgainstList> result = _ComplaintRepository.GetAdjuntList(IComplaintId);
                if (result.Count > 0)
                {
                    for (int i = 0; i < result.Count; i++)
                    {
                        result[i].IsKnown = result[i].IsKnown == null ? "" : result[i].IsKnown;
                        result[i].Name = result[i].AdvocateName == null ? result[i].FullName : result[i].AdvocateName;
                        result[i].FinalContactNumber = result[i].MobileNo == null ? result[i].ContactNumber : result[i].MobileNo;
                        result[i].EnrollmentNo = result[i].EnrollmentNo == null ? "" : result[i].EnrollmentNo;
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

        [HttpPost]
        public ActionResult GetOnlineAdjuntDetails(String ComplaintId)
        {
            try
            {
                Int64 IComplaintId = Convert.ToInt64(ComplaintId == null ? "0" : ComplaintId == "" ? "0" : ComplaintId);
                List<ComplaintAgainstList> result = _ComplaintRepository.GetOnlineAdjuntList(IComplaintId);
                if (result.Count > 0)
                {
                    for (int i = 0; i < result.Count; i++)
                    {
                        result[i].IsKnown = result[i].IsKnown == null ? "" : result[i].IsKnown;
                        result[i].Name = result[i].AdvocateName == null ? result[i].FullName : result[i].AdvocateName;
                        result[i].FinalContactNumber = result[i].MobileNo == null ? result[i].ContactNumber : result[i].MobileNo;
                        result[i].EnrollmentNo = result[i].EnrollmentNo == null ? "" : result[i].EnrollmentNo;
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

        [HttpPost]
        public ActionResult ComplaintNumberUpdate(String ComplaintId, String ComplaintNumberCode, String ComplaintNumber)
        {
            try
            {
                Int64 IComplaintId = Convert.ToInt64(ComplaintId == null ? "0" : ComplaintId == "" ? "0" : ComplaintId);
                Int64 IComplaintNumberCode = Convert.ToInt64(ComplaintNumberCode == null ? "0" : ComplaintNumberCode == "" ? "0" : ComplaintNumberCode);
                Int64 LastUpdateUser = Convert.ToInt64(Request.Cookies["CMPUserId"] == null ? "0" : Request.Cookies["CMPUserId"].Value.ToString() == "" ? "0" : Request.Cookies["CMPUserId"].Value.ToString());
                List<ComplaintNumberUpdate> result = _ComplaintRepository.ComplaintNumberUpdate(IComplaintId, IComplaintNumberCode, ComplaintNumber, LastUpdateUser, "ComplaintNumberUpdate");
                String ReturnStatus = result == null ? "" : result.ToList().Count == 0 ? "" : result[0].ReturnStatus == null ? "" : result[0].ReturnStatus;
                if (ReturnStatus == "UD")
                {
                    response.Data = ReturnStatus;
                    response.IsSuccess = true;
                }
                else
                {
                    response.Data = "";
                    response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                response.Data = "";
                response.IsSuccess = false;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteComplaint(String ComplaintId)
        {
            try
            {
                Int64 IComplaintId = Convert.ToInt64(ComplaintId == null ? "0" : ComplaintId == "" ? "0" : ComplaintId);
                Int64 LastUpdateUser = Convert.ToInt64(Request.Cookies["CMPUserId"] == null ? "0" : Request.Cookies["CMPUserId"].Value.ToString() == "" ? "0" : Request.Cookies["CMPUserId"].Value.ToString());
                List<ComplaintNumberUpdate> result = _ComplaintRepository.ComplaintDelete(IComplaintId, LastUpdateUser, "ComplaintDelete");
                String ReturnStatus = result == null ? "" : result.ToList().Count == 0 ? "" : result[0].ReturnStatus == null ? "" : result[0].ReturnStatus;
                if (ReturnStatus == "DE")
                {
                    response.Data = ReturnStatus;
                    response.IsSuccess = true;
                }
                else
                {
                    response.Data = "";
                    response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                response.Data = "";
                response.IsSuccess = false;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteOnlineComplaint(String ComplaintId)
        {
            try
            {
                Int64 IComplaintId = Convert.ToInt64(ComplaintId == null ? "0" : ComplaintId == "" ? "0" : ComplaintId);
                Int64 LastUpdateUser = Convert.ToInt64(Request.Cookies["CMPUserId"] == null ? "0" : Request.Cookies["CMPUserId"].Value.ToString() == "" ? "0" : Request.Cookies["CMPUserId"].Value.ToString());
                List<ComplaintNumberUpdate> result = _ComplaintRepository.OnlineComplaintDelete(IComplaintId, LastUpdateUser, "ComplaintDelete");
                String ReturnStatus = result == null ? "" : result.ToList().Count == 0 ? "" : result[0].ReturnStatus == null ? "" : result[0].ReturnStatus;
                if (ReturnStatus == "DE")
                {
                    response.Data = ReturnStatus;
                    response.IsSuccess = true;
                }
                else
                {
                    response.Data = "";
                    response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                response.Data = "";
                response.IsSuccess = false;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AdditionalFilesUpload()
        {
            try
            {
                #region Directory Paths
                String FilePathImageS = System.Web.Hosting.HostingEnvironment.MapPath("~/ComplaintAdditional/");
                #endregion

                #region Directory Create
                if (!Directory.Exists(FilePathImageS))
                {
                    Directory.CreateDirectory(FilePathImageS);
                }
                #endregion

                HttpFileCollectionBase files = Request.Files;
                String FName = String.Empty;
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase postedFile = files[i];
                    String CFPath = Path.Combine(HttpRuntime.AppDomainAppPath, "ComplaintAdditional");
                    FName = postedFile.FileName.Replace(" ", "_").Trim();
                    postedFile.SaveAs(Path.Combine(CFPath, FName.Trim()));
                }

                string Durl = Request.Url.Scheme + "://" + Request.Url.Authority;
                String FinalUrl = Durl + "/ComplaintAdditional/" + FName;
                response.IsSuccess = true;
                response.Data = FinalUrl;
                response.Data2 = FName;
            }
            catch (Exception ex)
            {
                response.Data = "";
                response.Data2 = "";
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AffidavitCopyFiles()
        {
            try
            {
                #region Directory Paths
                String FilePathImageS = System.Web.Hosting.HostingEnvironment.MapPath("~/ComplaintAffidavit/");
                #endregion

                #region Directory Create
                if (!Directory.Exists(FilePathImageS))
                {
                    Directory.CreateDirectory(FilePathImageS);
                }
                #endregion

                HttpFileCollectionBase files = Request.Files;
                String FName = String.Empty;
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase postedFile = files[i];
                    String CFPath = Path.Combine(HttpRuntime.AppDomainAppPath, "ComplaintAffidavit");
                    FName = postedFile.FileName.Replace(" ", "_").Trim();
                    postedFile.SaveAs(Path.Combine(CFPath, FName.Trim()));
                }

                string Durl = Request.Url.Scheme + "://" + Request.Url.Authority;
                String FinalUrl = Durl + "/ComplaintAffidavit/" + FName;
                response.IsSuccess = true;
                response.Data = FinalUrl;
                response.Data2 = FName;
            }
            catch (Exception ex)
            {
                response.Data = "";
                response.Data2 = "";
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddFile(String FilePath, String FileName, String FileTitle, String Description, String ComplaintId, String FileExtension)
        {
            try
            {
                DataTable dt = new DataTable();

                if (ComplaintId == "0" || ComplaintId == "")
                {
                    FileDetailsList fdl = new FileDetailsList();
                    fdl.ComplaintId = Convert.ToInt64(ComplaintId == "" ? "0" : ComplaintId == null ? "0" : ComplaintId);
                    fdl.FileName = FileName;
                    fdl.FileTitle = FileTitle;
                    fdl.FilePath = FilePath == null ? "" : FilePath;
                    fdl.Description = Description;
                    fdl.FileExtension = FileExtension;

                    response.IsSuccess = true;
                    response.Data = fdl;
                }
                else
                {
                    FileDetailsList fdl = new FileDetailsList();
                    fdl.ComplaintId = Convert.ToInt64(ComplaintId == "" ? "0" : ComplaintId == null ? "0" : ComplaintId);
                    fdl.FileName = FileName;
                    fdl.FileTitle = FileTitle;
                    fdl.FilePath = FilePath == null ? "" : FilePath;
                    fdl.Description = Description;
                    fdl.FileExtension = FileExtension;
                    _ComplaintRepository.SaveFileDetails(fdl);

                    response.IsSuccess = true;
                    response.Data = fdl;
                }
            }
            catch (Exception ex)
            {

            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveComplaint(String Name, String Email, String MobileNo, String Address, String Summary, String DetailComplaint, String TotalAmount, DefendantDetailsList[] DefendantDetails, FileDetailsList[] FileDetails, ComplaintPaymentModel[] ComplaintPayment, String AffidavitFileName, String AffidavitFilePath)
        {
            try
            {
                ComplaintModel cmp = new ComplaintModel();
                cmp.FullName = Name;
                cmp.EmailAddress = Email;
                cmp.ContactNumber = MobileNo;
                cmp.Address = Address;
                cmp.Summary = Summary;
                cmp.DetailComplaint = DetailComplaint;
                cmp.TotalAmount = Convert.ToDecimal(TotalAmount);
                cmp.Status = "New";
                cmp.CreateUser = 1;

                List<ComplaintModel> result = _ComplaintRepository.SaveComplaint(cmp);

                if (result.Count > 0)
                {
                    Int64 NewComplaintId = Convert.ToInt64(result[0].Id == null ? "0" : result[0].Id.ToString() == "" ? "0" : result[0].Id.ToString());
                    if (DefendantDetails != null)
                    {
                        foreach (var item in DefendantDetails)
                        {
                            DefendantDetailsList ddl = new DefendantDetailsList();
                            ddl.AdvocateId = item.AdvocateId;
                            ddl.Amount = item.Amount;
                            ddl.CreateUser = 1;
                            ddl.ComplaintId = NewComplaintId;
                            ddl.AdvocateName = item.AdvocateName == null ? "" : item.AdvocateName;
                            ddl.ContactNumber = item.ContactNumber == null ? "" : item.ContactNumber;
                            ddl.City = item.City == null ? "" : item.City;
                            ddl.IsKnown = item.IsKnown == null ? "N" : item.IsKnown;
                            _ComplaintRepository.SaveDefendantDetails(ddl);
                        }
                    }

                    if (FileDetails != null)
                    {
                        foreach (var item in FileDetails)
                        {
                            FileDetailsList fdl = new FileDetailsList();
                            fdl.ComplaintId = NewComplaintId;
                            fdl.FileName = item.FileName;
                            fdl.FileTitle = item.FileTitle;
                            fdl.FilePath = item.FilePath;
                            fdl.Description = item.Description;
                            fdl.FileExtension = item.FileExtension;

                            _ComplaintRepository.SaveFileDetails(fdl);
                        }
                    }

                    if (AffidavitFilePath != null)
                    {
                        String[] FileExT = AffidavitFileName.Split('.');
                        String FileExtension = FileExT.Length > 1 ? FileExT[1] == null ? "" : FileExT[1].ToString() : "";
                        AffidavitCopy fdl = new AffidavitCopy();
                        fdl.ComplaintId = NewComplaintId;
                        fdl.FileName = AffidavitFileName;
                        fdl.FilePath = AffidavitFilePath;
                        fdl.FileExtension = FileExtension;
                        fdl.CreateUser = 1;

                        _ComplaintRepository.SaveAffidavitCopy(fdl);
                    }

                    if (ComplaintPayment != null)
                    {
                        for (int i = 0; i < ComplaintPayment.ToList().Count; i++)
                        {
                            Decimal DPaidAmount = Convert.ToDecimal(ComplaintPayment[i].PaidAmount == null ? "0" : ComplaintPayment[i].PaidAmount == "" ? "0" : ComplaintPayment[i].PaidAmount);
                            Int64 IReceiptNumber = Convert.ToInt64(ComplaintPayment[i].ReceiptNumber == null ? "0" : ComplaintPayment[i].ReceiptNumber == "" ? "0" : ComplaintPayment[i].ReceiptNumber);
                            DateTime DReceiptDate = Convert.ToDateTime(ComplaintPayment[i].ReceiptDate == null ? System.DateTime.Now.ToString() : ComplaintPayment[i].ReceiptDate == "" ? System.DateTime.Now.ToString() : ComplaintPayment[i].ReceiptDate);
                            String SPaidOn = ComplaintPayment[i].PaidOn == null ? System.DateTime.Now.ToString() : ComplaintPayment[i].PaidOn == "" ? System.DateTime.Now.ToString() : ComplaintPayment[i].PaidOn;
                            String SPaymentMode = ComplaintPayment[i].PaymentMode == null ? "" : ComplaintPayment[i].PaymentMode;
                            String STransactionNumber = ComplaintPayment[i].TransactionNumber == null ? "" : ComplaintPayment[i].TransactionNumber;
                            DateTime DTransactionDate = Convert.ToDateTime(ComplaintPayment[i].TransactionDate == null ? System.DateTime.Now.ToString() : ComplaintPayment[i].TransactionDate == "" ? System.DateTime.Now.ToString() : ComplaintPayment[i].TransactionDate);
                            Int64 IBankId = Convert.ToInt64(ComplaintPayment[i].BankId == null ? "0" : ComplaintPayment[i].BankId == "" ? "0" : ComplaintPayment[i].BankId);
                            Int64 IPaidById = Convert.ToInt64(Request.Cookies["CMPUserId"] == null ? "0" : Request.Cookies["CMPUserId"].Value.ToString() == "" ? "0" : Request.Cookies["CMPUserId"].Value.ToString());
                            var ComplaintPaymentDetail = _ComplaintRepository.GetComplaintPaymentReceived(NewComplaintId, IReceiptNumber, DReceiptDate, DPaidAmount, IPaidById, SPaidOn, SPaymentMode, STransactionNumber, DTransactionDate, IBankId, IPaidById);
                        }
                    }

                    response.IsSuccess = true;
                }
                else
                {
                    response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {

            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SearchComplaintByNumber(String ComplaintNumber)
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
                    List<ComplaintDetailByNumber> result = _ComplaintRepository.GetComplaintDetailByComplaintNumber(ComplaintNumber);
                    if (result.Count > 0)
                    {
                        var comp = result[0];
                        try
                        {
                            var dcMemberList = _ComplaintRepository.GetDcMemberList(comp.Id);
                            if (dcMemberList != null && dcMemberList.Count > 0 && dcMemberList[0].CommitteeId > 0)
                            {
                                comp.DcNumber = dcMemberList[0].CommitteeId;
                                var dcList = _ComplaintDetailsRepository.GetDcNumberList();
                                var dcObj = dcList.FirstOrDefault(x => x.Id == dcMemberList[0].CommitteeId);
                                comp.DcCommitteeTitle = dcObj != null ? dcObj.DcTitle : dcMemberList[0].CommitteeId.ToString();
                            }
                        }
                        catch { }

                        response.IsSuccess = true;
                        response.Data = comp;
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = "Complaint not found.";
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
        public ActionResult SearchComplaintComplainantsDetailsByComplaintNumber(String ComplaintNumber)
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
                    List<ComplaintDetailByNumber> result = _ComplaintRepository.GetComplaintComplainantsDetailsByComplaintNumber(ComplaintNumber);
                    if (result.Count > 0)
                    {
                        var comp = result[0];
                        try
                        {
                            var dcMemberList = _ComplaintRepository.GetDcMemberList(comp.Id);
                            if (dcMemberList != null && dcMemberList.Count > 0 && dcMemberList[0].CommitteeId > 0)
                            {
                                comp.DcNumber = dcMemberList[0].CommitteeId;
                                var dcList = _ComplaintDetailsRepository.GetDcNumberList();
                                var dcObj = dcList.FirstOrDefault(x => x.Id == dcMemberList[0].CommitteeId);
                                comp.DcCommitteeTitle = dcObj != null ? dcObj.DcTitle : dcMemberList[0].CommitteeId.ToString();
                            }
                        }
                        catch { }

                        response.IsSuccess = true;
                        response.Data = comp;
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = "Complaint not found.";
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
        public ActionResult GetComplaintAffidavitCopyLog(String ComplaintNumber)
        {
            try
            {
                List<ComplaintAffidavitCopyByNumber> cpr = new List<ComplaintAffidavitCopyByNumber>();
                cpr = _ComplaintRepository.GetComplainAffidavitCopyByComplaintNumber(ComplaintNumber);

                response.IsSuccess = cpr.ToList().Count > 0 ? true : false;
                response.Data = cpr.ToList();
            }
            catch (Exception ex)
            {

            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetComplaintAdditionalAttachmentsLog(String ComplaintNumber)
        {
            try
            {
                List<ComplaintAdditionalAttachmentsByNumber> cpr = new List<ComplaintAdditionalAttachmentsByNumber>();
                cpr = _ComplaintRepository.GetComplainAdditionalAttachmentsByComplaintNumber(ComplaintNumber);

                response.IsSuccess = cpr.ToList().Count > 0 ? true : false;
                response.Data = cpr.ToList();
            }
            catch (Exception ex)
            {

            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetComplaintMemberLogList(String ComplaintId)
        {
            try
            {
                List<ComplaintMemberLogList> cpr = new List<ComplaintMemberLogList>();
                Int64 IComplaintId = Convert.ToInt64(ComplaintId == null ? "0" : ComplaintId == "" ? "0" : ComplaintId);
                Int64 CMPUserId = Convert.ToInt64(Request.Cookies["CMPUserId"] == null ? "0" : Request.Cookies["CMPUserId"].Value.ToString() == "" ? "0" : Request.Cookies["CMPUserId"].Value.ToString());
                cpr = _ComplaintRepository.GetComplaintMemberLogList(0, IComplaintId, CMPUserId);

                response.IsSuccess = cpr.ToList().Count > 0 ? true : false;
                response.Data = cpr.ToList();
            }
            catch (Exception ex)
            {

            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ComplaintOldStatusUpdate(String ComplaintId)
        {
            try
            {
                Int64 IComplaintId = Convert.ToInt64(ComplaintId == null ? "0" : ComplaintId == "" ? "0" : ComplaintId);
                Int64 LastUpdateUser = Convert.ToInt64(Request.Cookies["CMPUserId"] == null ? "0" : Request.Cookies["CMPUserId"].Value.ToString() == "" ? "0" : Request.Cookies["CMPUserId"].Value.ToString());
                List<ComplaintNumberUpdate> result = _ComplaintRepository.ComplaintStatusUpdate(IComplaintId, LastUpdateUser, "ComplaintOldStatusUpdate");
                String ReturnStatus = result == null ? "" : result.ToList().Count == 0 ? "" : result[0].ReturnStatus == null ? "" : result[0].ReturnStatus;
                if (ReturnStatus == "UD")
                {
                    response.Data = ReturnStatus;
                    response.IsSuccess = true;
                }
                else
                {
                    response.Data = "";
                    response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                response.Data = "";
                response.IsSuccess = false;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ComplaintNewStatusUpdate(String ComplaintId)
        {
            try
            {
                Int64 IComplaintId = Convert.ToInt64(ComplaintId == null ? "0" : ComplaintId == "" ? "0" : ComplaintId);
                Int64 LastUpdateUser = Convert.ToInt64(Request.Cookies["CMPUserId"] == null ? "0" : Request.Cookies["CMPUserId"].Value.ToString() == "" ? "0" : Request.Cookies["CMPUserId"].Value.ToString());
                List<ComplaintNumberUpdate> result = _ComplaintRepository.ComplaintStatusUpdate(IComplaintId, LastUpdateUser, "ComplaintNewStatusUpdate");
                String ReturnStatus = result == null ? "" : result.ToList().Count == 0 ? "" : result[0].ReturnStatus == null ? "" : result[0].ReturnStatus;
                if (ReturnStatus == "UD")
                {
                    response.Data = ReturnStatus;
                    response.IsSuccess = true;
                }
                else
                {
                    response.Data = "";
                    response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                response.Data = "";
                response.IsSuccess = false;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult OnlineComplaintStatusUpdate(String ComplaintId, String Remark, String Mode)
        {
            try
            {
                Int64 IComplaintId = Convert.ToInt64(ComplaintId == null ? "0" : ComplaintId == "" ? "0" : ComplaintId);
                Int64 LastUpdateUser = Convert.ToInt64(Request.Cookies["CMPUserId"] == null ? "0" : Request.Cookies["CMPUserId"].Value.ToString() == "" ? "0" : Request.Cookies["CMPUserId"].Value.ToString());
                List<ComplaintNumberUpdate> result = _ComplaintRepository.OnlineComplaintStatusUpdate(IComplaintId, Remark, LastUpdateUser, Mode);
                String ReturnStatus = result == null ? "" : result.ToList().Count == 0 ? "" : result[0].ReturnStatus == null ? "" : result[0].ReturnStatus;
                if (ReturnStatus == "I" || ReturnStatus == "R" || ReturnStatus == "UD")
                {
                    response.Data = ReturnStatus;
                    response.IsSuccess = true;
                }
                else
                {
                    response.Data = "";
                    response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                response.Data = "";
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
    }
}
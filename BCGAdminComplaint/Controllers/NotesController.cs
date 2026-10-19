using BCGAdminComplaint.Models.Notes;
using BCGAdminComplaint.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BCGAdminComplaint.Controllers
{
    public class NotesController : Controller
    {
        private readonly NotesRepository _notesRepository;

        public NotesController()
        {
            _notesRepository = new NotesRepository();
        }

        private long GetLoginUserId()
        {
            long userId = 1; // Default admin
            if (Request.Cookies["CMPUserId"] != null && !string.IsNullOrEmpty(Request.Cookies["CMPUserId"].Value))
            {
                long.TryParse(Request.Cookies["CMPUserId"].Value, out userId);
            }
            return userId;
        }

        private string GetLoginUserName()
        {
            if (Request.Cookies["CMPUserName"] != null && !string.IsNullOrEmpty(Request.Cookies["CMPUserName"].Value))
            {
                return Request.Cookies["CMPUserName"].Value;
            }
            return "Admin";
        }

        private string GetBaseDomainUrl()
        {
            try
            {
                if (Request != null && Request.Url != null)
                {
                    return Request.Url.Scheme + "://" + Request.Url.Authority;
                }
            }
            catch { }

            string imgUrl = ConfigurationManager.AppSettings["ImageUrl"];
            if (!string.IsNullOrWhiteSpace(imgUrl))
            {
                return imgUrl.TrimEnd('/');
            }

            return "http://localhost:53316";
        }

        public ActionResult Index()
        {
            if (Request.Cookies["bcg"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));

            ViewBag.UserName = GetLoginUserName();
            ViewBag.UserId = GetLoginUserId();
            return View();
        }

        [HttpPost]
        public ActionResult GetNotesList(string searchValue, string noteType, string fromDate, string toDate, int pageNo = 1, int pageSize = 12)
        {
            try
            {
                if (Request.Cookies["bcg"] == null)
                {
                    return Json(new { success = false, message = "Session expired" }, JsonRequestBehavior.AllowGet);
                }

                long loginUserId = GetLoginUserId();
                var notes = _notesRepository.GetNotesList(loginUserId, searchValue, noteType, fromDate, toDate, pageNo, pageSize);
                int totalRecords = notes.Count > 0 ? notes[0].TotalRecord : 0;

                return Json(new
                {
                    success = true,
                    data = notes,
                    total = totalRecords,
                    page = pageNo,
                    pageSize = pageSize,
                    totalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GetNoteById(long? id = null)
        {
            try
            {
                if (Request.Cookies["bcg"] == null)
                {
                    return Json(new { success = false, message = "Session expired" }, JsonRequestBehavior.AllowGet);
                }

                if (!id.HasValue || id.Value <= 0)
                {
                    return Json(new { success = false, message = "Invalid Note Id." }, JsonRequestBehavior.AllowGet);
                }

                var note = _notesRepository.GetNoteById(id.Value);
                if (note == null)
                {
                    return Json(new { success = false, message = "Note not found." }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = true, data = note }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SaveNote(long id, string noteType, string noteTitle, long? noteReferanceId, string noteDescription)
        {
            try
            {
                if (Request.Cookies["bcg"] == null)
                {
                    return Json(new { success = false, message = "Session expired" }, JsonRequestBehavior.AllowGet);
                }

                if (string.IsNullOrWhiteSpace(noteTitle))
                {
                    return Json(new { success = false, message = "Note Title is required." }, JsonRequestBehavior.AllowGet);
                }

                long loginUserId = GetLoginUserId();

                string normalizedType = noteType;
                if (string.IsNullOrWhiteSpace(normalizedType))
                {
                    normalizedType = "General Note";
                }
                else if (normalizedType.Equals("Complaint", StringComparison.OrdinalIgnoreCase))
                {
                    normalizedType = "Complaint Note";
                }
                else if (normalizedType.Equals("General", StringComparison.OrdinalIgnoreCase))
                {
                    normalizedType = "General Note";
                }

                // If General Note, complaint reference is not linked
                if (normalizedType.Equals("General Note", StringComparison.OrdinalIgnoreCase))
                {
                    noteReferanceId = null;
                }

                long noteId = _notesRepository.SaveNote(id, normalizedType, noteTitle.Trim(), noteReferanceId, noteDescription ?? "", loginUserId);

                if (noteId <= 0)
                {
                    return Json(new { success = false, message = "Failed to save note." }, JsonRequestBehavior.AllowGet);
                }

                // Collect all uploaded files directly from Request.Files (contains every posted file)
                List<HttpPostedFileBase> allUploadedFiles = new List<HttpPostedFileBase>();
                if (Request.Files != null && Request.Files.Count > 0)
                {
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        HttpPostedFileBase postedFile = Request.Files[i];
                        if (postedFile != null && !string.IsNullOrWhiteSpace(postedFile.FileName))
                        {
                            allUploadedFiles.Add(postedFile);
                        }
                    }
                }

                // Handle file attachments if any uploaded
                if (allUploadedFiles.Count > 0)
                {
                    string uploadFolder = Server.MapPath("~/Uploads/NotesFiles/");
                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }

                    foreach (HttpPostedFileBase file in allUploadedFiles)
                    {
                        if (file == null) continue;

                        string origFileName = Path.GetFileName(file.FileName);
                        string ext = Path.GetExtension(origFileName).ToLowerInvariant();
                        string uniqueName = string.Format("Note_{0}_{1:yyyyMMddHHmmss}_{2}{3}", noteId, DateTime.Now, Guid.NewGuid().ToString("N").Substring(0, 6), ext);
                        string serverPath = Path.Combine(uploadFolder, uniqueName);
                        string baseDomain = GetBaseDomainUrl();
                        string virtualPath = baseDomain + "/Uploads/NotesFiles/" + uniqueName;

                        file.SaveAs(serverPath);

                        string fileCategory = "Document";
                        if (ext == ".pdf") fileCategory = "PDF";
                        else if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".gif" || ext == ".webp") fileCategory = "Image";
                        else if (ext == ".doc" || ext == ".docx") fileCategory = "Word";
                        else if (ext == ".xls" || ext == ".xlsx") fileCategory = "Excel";

                        _notesRepository.SaveNoteFile(noteId, fileCategory, origFileName, virtualPath, loginUserId);
                    }
                }

                string msg = id > 0 ? "Note updated successfully!" : "Note created successfully!";
                if (allUploadedFiles.Count > 0)
                {
                    msg += string.Format(" ({0} file{1} attached)", allUploadedFiles.Count, allUploadedFiles.Count > 1 ? "s" : "");
                }
                return Json(new { success = true, noteId = noteId, filesCount = allUploadedFiles.Count, message = msg }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteNote(long id)
        {
            try
            {
                if (Request.Cookies["bcg"] == null)
                {
                    return Json(new { success = false, message = "Session expired" }, JsonRequestBehavior.AllowGet);
                }

                long loginUserId = GetLoginUserId();
                bool deleted = _notesRepository.DeleteNote(id, loginUserId);

                if (deleted)
                {
                    return Json(new { success = true, message = "Note deleted successfully!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "Could not delete note." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteNoteFile(long fileId)
        {
            try
            {
                if (Request.Cookies["bcg"] == null)
                {
                    return Json(new { success = false, message = "Session expired" }, JsonRequestBehavior.AllowGet);
                }

                long loginUserId = GetLoginUserId();
                bool deleted = _notesRepository.DeleteNoteFile(fileId, loginUserId);

                if (deleted)
                {
                    return Json(new { success = true, message = "File removed successfully!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "Could not remove file." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult GetNotesSummary()
        {
            try
            {
                long loginUserId = GetLoginUserId();
                var summary = _notesRepository.GetNotesSummary(loginUserId);
                return Json(new { success = true, summary = summary }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult GetAllComplaints()
        {
            try
            {
                var list = _notesRepository.SearchComplaints("");
                var results = list.Select(c => new
                {
                    id = c.Id,
                    complaintNumber = c.ComplaintNumber,
                    fullName = c.FullName,
                    status = c.Status,
                    text = c.ComplaintNumber + (!string.IsNullOrEmpty(c.FullName) ? " - " + c.FullName : "") + (!string.IsNullOrEmpty(c.Status) ? " (" + c.Status + ")" : "")
                }).ToList();

                return Json(new { success = true, results = results }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message, results = new List<object>() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult SearchComplaints(string term)
        {
            try
            {
                var list = _notesRepository.SearchComplaints(term ?? "");
                var results = list.Select(c => new
                {
                    id = c.Id,
                    complaintNumber = c.ComplaintNumber,
                    fullName = c.FullName,
                    status = c.Status,
                    text = c.ComplaintNumber + (!string.IsNullOrEmpty(c.FullName) ? " - " + c.FullName : "") + (!string.IsNullOrEmpty(c.Status) ? " (" + c.Status + ")" : "")
                }).ToList();

                return Json(new { results = results }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { results = new List<object>() }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}

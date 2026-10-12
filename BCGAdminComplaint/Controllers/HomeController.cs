using BCGAdminComplaint.Models;
using BCGAdminComplaint.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BarCouncilComplaint.Controllers
{
    public class HomeController : Controller
    {
        private readonly HomeRepository _HomeRepository;

        public HomeController()
        {
            _HomeRepository = new HomeRepository();
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

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UserLogin(string UserName, string Password, string PCIPAddress)
        {
            try
            {

                List<ComplaintUserLoginReturn> result = _HomeRepository.GetComplaintUserLogin(UserName, Password, PCIPAddress, "UserLogin");

                if (result.ToList().Count > 0)
                {
                    String SReturnStatus = result[0].ReturnStatus == null ? "" : result[0].ReturnStatus;

                    if (SReturnStatus == "LO")
                    {
                        HttpCookie cookie = new HttpCookie("bcg");
                        Response.Cookies["CMPUserId"].Value = result[0].RegisterId.ToString();
                        Response.Cookies["CMPUserName"].Value = result[0].UserName.ToString();
                        Response.Cookies["CMPUserId"].Expires = DateTime.Now.AddDays(1);
                        cookie.Expires = DateTime.Now.AddDays(1);
                        Response.Cookies.Add(cookie);                     
                        return Json(new { data = "Success", ReturnStatus = SReturnStatus }, JsonRequestBehavior.AllowGet);
                    }
                    else if (SReturnStatus == "DE")
                    {
                        return Json(new { data = "", ReturnStatus = result[0].IsDcAccess }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { data = "", ReturnStatus = SReturnStatus }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { data = "", ReturnStatus = "" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult GetDashBoardCount(String FilterValue, String FromDate, String ToDate)
        {
            try
            {
                HttpCookie cookie = Request.Cookies["bcg"];
                if (cookie == null)
                {
                    return RedirectToAction("Login");
                }
                else
                {
                    var result = _HomeRepository.GetComplaintcount(FilterValue, FromDate, ToDate);

                    if (result.Count > 0)
                    {
                        return Json(new { data = result }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { data = "" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult GetDashBoardList(String FilterValue, String FromDate, String ToDate)
        {
            try
            {
                HttpCookie cookie = Request.Cookies["bcg"];
                if (cookie == null)
                {
                    return RedirectToAction("Login");
                }
                else
                {
                    var result = _HomeRepository.GetComplaintList(FilterValue, FromDate, ToDate);

                    if (result.Count > 0)
                    {
                        //for (int i = 0; i < result.Count; i++)
                        //{
                        //    result[i].SCreateDate = result[i].CreateDate.ToString("dd-MM-yyyy HH:mm:ss");
                        //}
                        return Json(new { data = result }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { data = "" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }
    }
}
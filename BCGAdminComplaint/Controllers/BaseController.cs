using BCGAdminComplaint.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BarCouncilComplaint.Controllers
{
    public class BaseController : Controller
    {
        public ResultResponse response;

        public BaseController()
        {
            response = new ResultResponse();
        }

        protected string GetBrowserPath()
        {
            string[] candidatePaths = new string[]
            {
                @"C:\Program Files\Google\Chrome\Application\chrome.exe",
                @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe",
                @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe",
                @"C:\Program Files\Microsoft\Edge\Application\msedge.exe"
            };

            foreach (string path in candidatePaths)
            {
                if (System.IO.File.Exists(path))
                {
                    return path;
                }
            }
            return null;
        }

        protected byte[] ConvertHtmlToPdf(string html)
        {
            string browserPath = GetBrowserPath();
            if (string.IsNullOrEmpty(browserPath))
            {
                throw new FileNotFoundException("Neither Google Chrome nor Microsoft Edge was found on the server for PDF conversion.");
            }

            string tempHtml = Path.Combine(Path.GetTempPath(), "BCG_Report_" + Guid.NewGuid().ToString("N") + ".html");
            string tempPdf = Path.Combine(Path.GetTempPath(), "BCG_Report_" + Guid.NewGuid().ToString("N") + ".pdf");

            try
            {
                System.IO.File.WriteAllText(tempHtml, html, Encoding.UTF8);

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = browserPath,
                    Arguments = string.Format("--headless --disable-gpu --no-pdf-header-footer --run-all-compositor-stages-before-draw --print-to-pdf=\"{0}\" \"{1}\"", tempPdf, tempHtml),
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false
                };

                using (Process process = Process.Start(psi))
                {
                    if (process != null)
                    {
                        bool finished = process.WaitForExit(30000);
                        if (!finished)
                        {
                            try { process.Kill(); } catch { }
                            throw new TimeoutException("PDF generation process timed out.");
                        }
                    }
                }

                if (System.IO.File.Exists(tempPdf))
                {
                    return System.IO.File.ReadAllBytes(tempPdf);
                }
                else
                {
                    throw new Exception("PDF output file was not created by the browser engine.");
                }
            }
            finally
            {
                try { if (System.IO.File.Exists(tempHtml)) System.IO.File.Delete(tempHtml); } catch { }
                try { if (System.IO.File.Exists(tempPdf)) System.IO.File.Delete(tempPdf); } catch { }
            }
        }

        protected string GetLogoBase64()
        {
            try
            {
                string logoPath = Server != null
                    ? Server.MapPath("~/assets/img/barcouncilofgujarat_logo.png")
                    : @"d:\Project\ASP .Net Project\BCG\Complaint\BCGAdminComplaint\BCGAdminComplaint\assets\img\barcouncilofgujarat_logo.png";

                if (System.IO.File.Exists(logoPath))
                {
                    byte[] bytes = System.IO.File.ReadAllBytes(logoPath);
                    return Convert.ToBase64String(bytes);
                }
            }
            catch { }
            return "";
        }
    }
}
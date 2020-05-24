using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Uch_PracticeV3.Models;
using ClosedXML.Excel;
using System.IO;
using Uch_PracticeV3.QueriesExecution;
using Uch_PracticeV3.OfficeInteraction;
using DocumentFormat.OpenXml.InkML;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.Owin;
using Uch_PracticeV3.Models.Identity;
using System.Data.Entity;

namespace Uch_PracticeV3.Controllers
{
    public class QueriesController : Controller
    {
        private ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        Uch_PracticeEntities db = new Uch_PracticeEntities();
        // GET: Queries
        public ActionResult Index()
        {
            ViewBag.url = Request.Url.AbsolutePath;
            ViewBag.authed = AuthenticationManager.User.Identity.IsAuthenticated;
            return View();
        }

        public async Task<ActionResult> Marks()
        {
            ViewBag.url = Request.Url.AbsolutePath;
            ViewBag.authed = AuthenticationManager.User.Identity.IsAuthenticated;
            return await HandleRequest(Queries.Students_Marks, "Оценки студентов");
        }



        private async Task<ActionResult> HandleRequest(Queries queryName, string templateTitle)
        {
            switch (Request.QueryString.Get("format"))
            {
                case "tab":
                    {
                        string param = Request.QueryString.Get("param");
                        var data = await QueriesExecution.QueriesExecution.
                            ExecuteQuery(queryName, param);
                        if (data.rows.Count == 0)
                        {
                            ViewBag.isResultEmpty = true;
                        }
                        ViewBag.Title = templateTitle;
                        if (param != null)
                        {
                            ViewBag.Param = param;
                        }
                        else if (queryName == Queries.Students_Marks)
                        {
                            return View("StudentsWithFiles", data);
                        }
                        return View("Result", data);
                    }
                case "excel":
                    {
                        var data = await QueriesExecution.QueriesExecution.
                            ExecuteQuery(queryName, Request.QueryString.Get("param"));
                        if (data.rows.Count == 0)
                        {
                            ViewBag.Title = templateTitle;
                            ViewBag.isResultEmpty = true;
                            return View("Result", data);
                        }
                        if(queryName==Queries.Students_Marks && Request.QueryString.Get("param") == null)
                        {
                            return View("Error");
                        }
                        return ExcelInteraction.ConvertToXlsx(data);
                    }
                case "word":
                    {
                        var data = await QueriesExecution.QueriesExecution.
                            ExecuteQuery(queryName, Request.QueryString.Get("param"));
                        if (data.rows.Count == 0)
                        {
                            ViewBag.Title = templateTitle;
                            ViewBag.isResultEmpty = true;
                            return View("Result", data);
                        }
                        if (queryName == Queries.Students_Marks && Request.QueryString.Get("param") == null)
                        {
                            return View("Error");
                        }
                        return File(WordInteraction.ConvertToWord(data), 
                            "application/vnd.openxmlformats-officedocument.wordprocessingml.document", 
                            data.sheetName + ".docx");
                    }
                default:
                    {
                        return View("Error");
                    }
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
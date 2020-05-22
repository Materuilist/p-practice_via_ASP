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

namespace Uch_PracticeV3.Controllers
{
    public class QueriesController : Controller
    {

        UCH_PracticeEntities db = new UCH_PracticeEntities();
        // GET: Queries
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Marks()
        {
            return await HandleRequest(Queries.Students_Marks, "Оценки студентов");
        }

        private async Task<ActionResult> HandleRequest(Queries queryName, string templateTitle)
        {
            switch (Request.QueryString.Get("format"))
            {
                case "tab":
                    {
                        var data = await QueriesExecution.QueriesExecution.ExecuteQuery(queryName);
                        ViewBag.Title = templateTitle;
                        return View("Result", data);
                    }
                case "excel":
                    {
                        return ExcelInteraction.ConvertToXlsx(
                            await QueriesExecution.QueriesExecution.ExecuteQuery(queryName));
                    }
                case "word":
                    {
                        return File(WordInteraction.ConvertToWord(
                            await QueriesExecution.QueriesExecution.ExecuteQuery(queryName)), 
                            "application/vnd.openxmlformats-officedocument.wordprocessingml.document", 
                            templateTitle+".docx");
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
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Uch_PracticeV3.Models;
using Uch_PracticeV3.Models.Identity;

namespace Uch_PracticeV3.Controllers
{
    public class StudentsController : Controller
    {
        private const int ITEMS_PER_PAGE = 3;
        private Uch_PracticeEntities db = new Uch_PracticeEntities();

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void FillViewBag()
        {
            ViewBag.Table = "/Students";
            ViewBag.Title = "Студенты";
            ViewBag.url = Request.Url.AbsolutePath;
            ViewBag.authed = AuthenticationManager.User.Identity.IsAuthenticated;
            ViewBag.isSuperUser = AuthenticationManager.User.Identity.GetUserRole() == "superuser";
        }

        [Authorize(Roles = "manager")]
        public async Task<ActionResult> Index()
        {
            FillViewBag();
            int page;
            bool pageWasPassed = Int32.TryParse(Request.QueryString.Get("page"), out page);
            if (!pageWasPassed)
            {
                page = 1;
            }

            ViewBag.ItemsPerPage = ITEMS_PER_PAGE;
            ViewBag.PageIndex = page;
            ViewBag.NextPageExists = await db.Students.CountAsync() > ITEMS_PER_PAGE * page;
            return View(await db.Students.OrderBy(l => l.Id).Skip((page - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE)
                .Include(s => s.Contract).Include(s => s.Leader).Include(s => s.Group).ToListAsync());
        }

        [Authorize(Roles = "manager")]
        public async Task<ActionResult> Create()
        {
            FillViewBag();
            ViewBag.Action = "Create";
            ViewBag.Groups = await db.Groups.ToListAsync();
            ViewBag.Contracts = await db.Contracts.ToListAsync();
            ViewBag.Leaders = await db.Leaders.ToListAsync();
            return View("ModifyStudent", new Student());
        }

        [Authorize(Roles = "manager")]
        public async Task<ActionResult> Update()
        {
            FillViewBag();
            string rowId = Request.QueryString.Get("rowId");
            if (rowId == null)
            {
                return Redirect("/");
            }
            ViewBag.Action = "Update";
            ViewBag.Groups = await db.Groups.ToListAsync();
            ViewBag.Contracts = await db.Contracts.ToListAsync();
            ViewBag.Leaders = await db.Leaders.ToListAsync();
            return View("ModifyStudent", await db.Students.FindAsync(rowId));
        }

        [Authorize(Roles = "manager")]
        public async Task<ActionResult> Delete()
        {
            FillViewBag();
            string deletedItemId = Request.QueryString.Get("rowId");
            if (deletedItemId != null)
            {
                db.Entry(new Student() { Id = deletedItemId }).State = EntityState.Deleted;
                await db.SaveChangesAsync();
            }
            return Redirect("/Students?page=1");
        }


        [Authorize(Roles = "manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Student student)
        {
            if ((from _student in db.Students where _student.Id == student.Id select _student)
                   .FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Студент с таким кодом уже есть");
            }
            if (!ModelState.IsValid)
            {
                FillViewBag();
                ViewBag.Action = "Create";
                ViewBag.Errors = ModelState.Values.Select(ms => ms.Errors.FirstOrDefault())
                    .Where(error => error != null);
                ViewBag.Groups = await db.Groups.ToListAsync();
                ViewBag.Contracts = await db.Contracts.ToListAsync();
                ViewBag.Leaders = await db.Leaders.ToListAsync();
                return View("ModifyStudent", student);
            }
            db.Entry(student).State = EntityState.Added;
            await db.SaveChangesAsync();
            return Redirect("/Students?page=1");
        }

        [Authorize(Roles = "manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update(Student student)
        {
            if (!ModelState.IsValid)
            {
                FillViewBag();
                ViewBag.Action = "Update";
                ViewBag.Errors = ModelState.Values.Select(ms => ms.Errors.FirstOrDefault())
                    .Where(error => error != null);
                ViewBag.Groups = await db.Groups.ToListAsync();
                ViewBag.Contracts = await db.Contracts.ToListAsync();
                ViewBag.Leaders = await db.Leaders.ToListAsync();
                return View("ModifyStudent", student);
            }
            db.Entry(student).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return Redirect("/Students?page=1");
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
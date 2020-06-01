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
    public class SpecialtiesController : Controller
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
            ViewBag.Table = "/Specialties";
            ViewBag.Title = "Специальности";
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
            ViewBag.NextPageExists = await db.Specialties.CountAsync() > ITEMS_PER_PAGE * page;
            return View(await db.Specialties.OrderBy(s => s.Id).Skip((page - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE)
                .Include(s => s.Groups).ToListAsync());
        }

        [Authorize(Roles = "manager")]
        public async Task<ActionResult> Create()
        {
            FillViewBag();
            ViewBag.Action = "Create";
            return View("ModifySpecialty", new Specialty());
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
            return View("ModifySpecialty", await db.Specialties.FindAsync(rowId));
        }

        [Authorize(Roles = "manager")]
        public async Task<ActionResult> Delete()
        {
            FillViewBag();
            string deletedItemId = Request.QueryString.Get("rowId");
            if (deletedItemId != null)
            {
                db.Entry(new Specialty() { Id = deletedItemId }).State = EntityState.Deleted;
                await db.SaveChangesAsync();
            }
            return Redirect("/Specialties?page=1");
        }


        [Authorize(Roles = "manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Specialty specialty)
        {
            if ((from spec in db.Specialties where spec.Id == specialty.Id select spec)
                   .FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Специальность с таким кодом уже есть");
            }
            if ((from spec in db.Specialties
                 where spec.Educational_Program == specialty.Educational_Program && spec.Id != specialty.Id
                 select
                 spec)
                   .FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Образовательная программа должна быть уникальной");
            }
            if (!ModelState.IsValid)
            {
                FillViewBag();
                ViewBag.Action = "Create";
                ViewBag.Errors = ModelState.Values.Select(ms => ms.Errors.FirstOrDefault())
                    .Where(error => error != null);
                return View("ModifySpecialty", specialty);
            }
            db.Entry(specialty).State = EntityState.Added;
            await db.SaveChangesAsync();
            return Redirect("/Specialties?page=1");
        }

        [Authorize(Roles = "manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update(Specialty specialty)
        {
            if ((from spec in db.Specialties
                 where spec.Educational_Program == specialty.Educational_Program && spec.Id != specialty.Id
                 select
                 spec)
                   .FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Образовательная программа должна быть уникальной");
            }
            if (!ModelState.IsValid)
            {
                FillViewBag();
                ViewBag.Action = "Update";
                ViewBag.Errors = ModelState.Values.Select(ms => ms.Errors.FirstOrDefault())
                    .Where(error => error != null);
                return View("ModifySpecialty", specialty);
            }
            db.Entry(specialty).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return Redirect("/Specialties?page=1");
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
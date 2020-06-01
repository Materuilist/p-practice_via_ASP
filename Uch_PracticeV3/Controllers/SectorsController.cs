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
    public class SectorsController : Controller
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
            ViewBag.Table = "/Sectors";
            ViewBag.Title = "Отрасли";
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
            ViewBag.NextPageExists = await db.Sectors.CountAsync() > ITEMS_PER_PAGE * page;
            return View(await db.Sectors.OrderBy(s => s.Id).Skip((page - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE)
                .Include(s => s.Organizations).ToListAsync());
        }

        [Authorize(Roles = "manager")]
        public async Task<ActionResult> Create()
        {
            FillViewBag();
            ViewBag.Action = "Create";
            return View("ModifySector", new Sector());
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
            return View("ModifySector", await db.Sectors.FindAsync(rowId));
        }

        [Authorize(Roles = "manager")]
        public async Task<ActionResult> Delete()
        {
            FillViewBag();
            string deletedItemId = Request.QueryString.Get("rowId");
            if (deletedItemId != null)
            {
                db.Entry(new Sector() { Id = deletedItemId }).State = EntityState.Deleted;
                await db.SaveChangesAsync();
            }
            return Redirect("/Sectors?page=1");
        }


        [Authorize(Roles = "manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Sector sector)
        {
            if ((from sect in db.Sectors where sect.Id == sector.Id select sect)
                      .FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Отрасль с таким кодом уже есть");
            }
            if ((from sect in db.Sectors where sect.Naming == sector.Naming && sect.Id != sector.Id select sect)
                      .FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Отрасль с таким названием уже есть");
            }
            if (!ModelState.IsValid)
            {
                FillViewBag();
                ViewBag.Action = "Create";
                ViewBag.Errors = ModelState.Values.Select(ms => ms.Errors.FirstOrDefault())
                    .Where(error => error != null);
                return View("ModifySector", sector);
            }
            db.Entry(sector).State = EntityState.Added;
            await db.SaveChangesAsync();
            return Redirect("/Sectors?page=1");
        }

        [Authorize(Roles = "manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update(Sector sector)
        {
            if ((from sect in db.Sectors where sect.Naming == sector.Naming && sect.Id != sector.Id select sect)
                      .FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Отрасль с таким названием уже есть");
            }
            if (!ModelState.IsValid)
            {
                FillViewBag();
                ViewBag.Action = "Update";
                ViewBag.Errors = ModelState.Values.Select(ms => ms.Errors.FirstOrDefault())
                    .Where(error => error != null);
                return View("ModifySector", sector);
            }
            db.Entry(sector).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return Redirect("/Sectors?page=1");
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
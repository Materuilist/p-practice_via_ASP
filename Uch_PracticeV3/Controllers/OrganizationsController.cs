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
    public class OrganizationsController : Controller
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
            ViewBag.Table = "/Organizations";
            ViewBag.Title = "Организации";
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
            ViewBag.NextPageExists = await db.Organizations.CountAsync() > ITEMS_PER_PAGE * page;
            return View(await db.Organizations.OrderBy(l => l.Id).Skip((page - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE)
                .Include(o => o.Contracts).Include(o => o.Sector).ToListAsync());
        }

        [Authorize(Roles = "manager")]
        public async Task<ActionResult> Create()
        {
            FillViewBag();
            ViewBag.Action = "Create";
            ViewBag.Sectors = await db.Sectors.ToListAsync();
            return View("ModifyOrganization", new Organization());
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
            ViewBag.Sectors = await db.Sectors.ToListAsync();
            return View("ModifyOrganization", await db.Organizations.FindAsync(rowId));
        }

        [Authorize(Roles = "manager")]
        public async Task<ActionResult> Delete()
        {
            FillViewBag();
            string deletedItemId = Request.QueryString.Get("rowId");
            if (deletedItemId != null)
            {
                db.Entry(new Organization() { Id = deletedItemId }).State = EntityState.Deleted;
                await db.SaveChangesAsync();
            }
            return Redirect("/Organizations?page=1");
        }


        [Authorize(Roles = "manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Organization organization)
        {
            if ((from org in db.Organizations where org.Id == organization.Id select org)
                   .FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Организация с таким кодом уже есть");
            }
            if ((from org in db.Organizations
                 where org.FullNaming == organization.FullNaming && org.Id != organization.Id
                 select org)
                   .FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Организация с таким полным названием уже есть");
            }
            if (!ModelState.IsValid)
            {
                FillViewBag();
                ViewBag.Action = "Create";
                ViewBag.Errors = ModelState.Values.Select(ms => ms.Errors.FirstOrDefault())
                    .Where(error => error != null);
                ViewBag.Sectors = await db.Sectors.ToListAsync();
                return View("ModifyOrganization", organization);
            }
            db.Entry(organization).State = EntityState.Added;
            await db.SaveChangesAsync();
            return Redirect("/Organizations?page=1");
        }

        [Authorize(Roles = "manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update(Organization organization)
        {
            if ((from org in db.Organizations
                 where org.FullNaming == organization.FullNaming && org.Id != organization.Id
                 select org)
                   .FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Организация с таким полным названием уже есть");
            }
            if (!ModelState.IsValid)
            {
                FillViewBag();
                ViewBag.Action = "Update";
                ViewBag.Errors = ModelState.Values.Select(ms => ms.Errors.FirstOrDefault())
                    .Where(error => error != null);
                ViewBag.Sectors = await db.Sectors.ToListAsync();
                return View("ModifyOrganization", organization);
            }
            db.Entry(organization).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return Redirect("/Organizations?page=1");
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
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
    public class GroupsController : Controller
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
            ViewBag.Table = "/Groups";
            ViewBag.Title = "Группы";
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
            ViewBag.NextPageExists = await db.Groups.CountAsync() > ITEMS_PER_PAGE * page;
            return View(await db.Groups.OrderBy(g => g.Id).Skip((page - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE)
                .Include(g => g.Specialty).Include(g => g.Students).ToListAsync());
        }

        [Authorize(Roles = "manager")]
        public async Task<ActionResult> Create()
        {
            FillViewBag();
            ViewBag.Action = "Create";
            ViewBag.Specialties = await db.Specialties.ToListAsync();
            return View("ModifyGroup", new Group());
        }

        [Authorize(Roles = "manager")]
        public async Task<ActionResult> Update()
        {
            FillViewBag();
            int rowId = Request.QueryString.Get("rowId") != null ? Convert.ToInt32(
                Request.QueryString.Get("rowId"))
                : 1;
            ViewBag.Action = "Update";
            ViewBag.Specialties = await db.Specialties.ToListAsync();
            return View("ModifyGroup", await db.Groups.FindAsync(rowId));
        }

        [Authorize(Roles = "manager")]
        public async Task<ActionResult> Delete()
        {
            FillViewBag();
            Int32 deletedItemId;
            bool idWasPassed = Int32.TryParse(Request.QueryString.Get("rowId"), out deletedItemId);
            if (idWasPassed)
            {
                db.Entry(new Group() { Id = deletedItemId }).State = EntityState.Deleted;
                await db.SaveChangesAsync();
            }
            return Redirect("/Groups?page=1");
        }


        [Authorize(Roles = "manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Group _group)
        {
            if ((from gr in db.Groups where gr.Id == _group.Id select gr)
                   .FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Группа с таким кодом уже есть");
            }
            if ((from gr in db.Groups where gr.Naming == _group.Naming && gr.Id != _group.Id select gr)
                   .FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Группа с таким названием уже есть");
            }
            if (!ModelState.IsValid)
            {
                FillViewBag();
                ViewBag.Action = "Create";
                ViewBag.Errors = ModelState.Values.Select(ms => ms.Errors.FirstOrDefault()).Where(error => error != null);
                ViewBag.Specialties = await db.Specialties.ToListAsync();
                return View("ModifyGroup", _group);
            }
            db.Entry(_group).State = EntityState.Added;
            await db.SaveChangesAsync();
            return Redirect("/Groups?page=1");
        }

        [Authorize(Roles = "manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update(Group _group)
        {
            if ((from gr in db.Groups where gr.Naming == _group.Naming && gr.Id != _group.Id select gr)
                   .FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Группа с таким названием уже есть");
            }
            if (!ModelState.IsValid)
            {
                FillViewBag();
                ViewBag.Action = "Update";
                ViewBag.Errors = ModelState.Values.Select(ms => ms.Errors.FirstOrDefault()).Where(error => error != null);
                ViewBag.Specialties = await db.Specialties.ToListAsync();
                return View("ModifyGroup", _group);
            }
            db.Entry(_group).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return Redirect("/Groups?page=1");
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
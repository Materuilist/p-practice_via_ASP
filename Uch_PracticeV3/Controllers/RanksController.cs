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
    public class RanksController : Controller
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
            ViewBag.Table = "/Ranks";
            ViewBag.Title = "Должности";
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
            ViewBag.NextPageExists = await db.Ranks.CountAsync() > ITEMS_PER_PAGE * page;
            return View(await db.Ranks.OrderBy(r => r.Id).Skip((page - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE)
                .Include(r => r.Leaders).ToListAsync());
        }

        [Authorize(Roles = "manager")]
        public async Task<ActionResult> Create()
        {
            FillViewBag();
            ViewBag.Action = "Create";
            return View("ModifyRank", new Rank());
        }

        [Authorize(Roles = "manager")]
        public async Task<ActionResult> Update()
        {
            FillViewBag();
            int rowId = Request.QueryString.Get("rowId") != null ? Convert.ToInt32(
                Request.QueryString.Get("rowId"))
                : 1;
            ViewBag.Action = "Update";
            return View("ModifyRank", await db.Ranks.FindAsync(rowId));
        }

        [Authorize(Roles = "manager")]
        public async Task<ActionResult> Delete()
        {
            FillViewBag();
            Int32 deletedItemId;
            bool idWasPassed = Int32.TryParse(Request.QueryString.Get("rowId"), out deletedItemId);
            if (idWasPassed)
            {
                db.Entry(new Rank() { Id = deletedItemId }).State = EntityState.Deleted;
                await db.SaveChangesAsync();
            }
            return Redirect("/Ranks?page=1");
        }


        [Authorize(Roles = "manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Rank rank)
        {
            if ((from r in db.Ranks where r.Id == rank.Id select r)
                   .FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Должность с таким кодом уже есть");
            }
            if ((from r in db.Ranks where r.Naming == rank.Naming && r.Id != rank.Id select r)
                   .FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Должность с таким названием уже есть");
            }
            if (!ModelState.IsValid)
            {
                FillViewBag();
                ViewBag.Action = "Create";
                ViewBag.Errors = ModelState.Values.Select(ms => ms.Errors.FirstOrDefault())
                    .Where(error => error != null);
                return View("ModifyRank", rank);
            }
            db.Entry(rank).State = EntityState.Added;
            await db.SaveChangesAsync();
            return Redirect("/Ranks?page=1");
        }

        [Authorize(Roles = "manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update(Rank rank)
        {
            if ((from r in db.Ranks where r.Naming == rank.Naming && r.Id != rank.Id select r)
                   .FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Должность с таким названием уже есть");
            }
            if (!ModelState.IsValid)
            {
                FillViewBag();
                ViewBag.Action = "Update";
                ViewBag.Errors = ModelState.Values.Select(ms => ms.Errors.FirstOrDefault())
                    .Where(error => error != null);
                return View("ModifyRank", rank);
            }
            db.Entry(rank).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return Redirect("/Ranks?page=1");
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
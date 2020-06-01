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
    public class LeadersController : Controller
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
            ViewBag.Table = "/Leaders";
            ViewBag.Title = "Руководители";
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
            ViewBag.NextPageExists = await db.Leaders.CountAsync() > ITEMS_PER_PAGE * page;
            return View(await db.Leaders.OrderBy(l => l.Id).Skip((page - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE)
                .Include(l => l.Rank).Include(l => l.Students).ToListAsync());
        }

        [Authorize(Roles = "manager")]
        public async Task<ActionResult> Create()
        {
            FillViewBag();
            ViewBag.Action = "Create";
            ViewBag.Ranks = await db.Ranks.ToListAsync();
            return View("ModifyLeader", new Leader());
        }

        [Authorize(Roles = "manager")]
        public async Task<ActionResult> Update()
        {
            FillViewBag();
            int rowId = Request.QueryString.Get("rowId") != null ? Convert.ToInt32(
                Request.QueryString.Get("rowId"))
                : 1;
            ViewBag.Action = "Update";
            ViewBag.Ranks = await db.Ranks.ToListAsync();
            return View("ModifyLeader", await db.Leaders.FindAsync(rowId));
        }

        [Authorize(Roles = "manager")]
        public async Task<ActionResult> Delete()
        {
            FillViewBag();
            Int32 deletedItemId;
            bool idWasPassed = Int32.TryParse(Request.QueryString.Get("rowId"), out deletedItemId);
            if (idWasPassed)
            {
                db.Entry(new Leader() { Id = deletedItemId }).State = EntityState.Deleted;
                await db.SaveChangesAsync();
            }
            return Redirect("/Leaders?page=1");
        }


        [Authorize(Roles = "manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Leader leader)
        {
            //email не уникален
            if ((from lead in db.Leaders where lead.Email == leader.Email && lead.Id != leader.Id select lead)
                .FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Руководитель с таким Email уже есть");
            }
            if ((from lead in db.Leaders where lead.Phone == leader.Phone && lead.Id != leader.Id select lead)
                .FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Руководитель с таким телефоном уже есть");
            }
            if (!ModelState.IsValid)
            {
                FillViewBag();
                ViewBag.Action = "Create";
                ViewBag.Errors = ModelState.Values.Select(ms => ms.Errors.FirstOrDefault())
                    .Where(error => error != null);
                ViewBag.Ranks = await db.Ranks.ToListAsync();
                return View("ModifyLeader", leader);
            }
            db.Entry(leader).State = EntityState.Added;
            await db.SaveChangesAsync();
            return Redirect("/Leaders?page=1");
        }

        [Authorize(Roles = "manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update(Leader leader)
        {

            //email не уникален
            if ((from lead in db.Leaders where lead.Email == leader.Email && lead.Id != leader.Id select lead)
                .FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Руководитель с таким Email уже есть");
            }
            if ((from lead in db.Leaders where lead.Phone == leader.Phone && lead.Id != leader.Id select lead)
                .FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Руководитель с таким телефоном уже есть");
            }
            if (!ModelState.IsValid)
            {
                FillViewBag();
                ViewBag.Action = "Update";
                ViewBag.Errors = ModelState.Values.Select(ms => ms.Errors.FirstOrDefault())
                    .Where(error => error != null);
                ViewBag.Ranks = await db.Ranks.ToListAsync();
                return View("ModifyLeader", leader);
            }
            db.Entry(leader).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return Redirect("/Leaders?page=1");
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
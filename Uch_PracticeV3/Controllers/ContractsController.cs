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
    public class ContractsController : Controller
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
            ViewBag.Table = "/Contracts";
            ViewBag.Title = "Договоры";
            ViewBag.url = Request.Url.AbsolutePath;
            ViewBag.authed = AuthenticationManager.User.Identity.IsAuthenticated;
            ViewBag.isSuperUser = AuthenticationManager.User.Identity.GetUserRole() == "superuser";
        }

        // GET: Contracts
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
            ViewBag.NextPageExists = await db.Contracts.CountAsync() > ITEMS_PER_PAGE * page;
            return View(await db.Contracts.OrderBy(c => c.Id).Skip((page - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE)
                .Include(c => c.Organization).Include(c => c.Students).ToListAsync());
        }

        [Authorize(Roles = "manager")]
        public async Task<ActionResult> Create()
        {
            FillViewBag();
            ViewBag.Action = "Create";
            ViewBag.Organizations = await db.Organizations.ToListAsync();
            return View("ModifyContract", new Contract());
        }

        [Authorize(Roles = "manager")]
        public async Task<ActionResult> Update()
        {
            FillViewBag();
            int rowId = Request.QueryString.Get("rowId") != null ? Convert.ToInt32(
                Request.QueryString.Get("rowId"))
                : 1;
            ViewBag.Action = "Update";
            ViewBag.Organizations = await db.Organizations.ToListAsync();
            return View("ModifyContract", await db.Contracts.FindAsync(rowId));
        }

        [Authorize(Roles = "manager")]
        public async Task<ActionResult> Delete()
        {
            FillViewBag();
            Int32 deletedItemId;
            bool idWasPassed = Int32.TryParse(Request.QueryString.Get("rowId"), out deletedItemId);
            if (idWasPassed)
            {
                db.Entry(new Contract() { Id = deletedItemId }).State = EntityState.Deleted;
                await db.SaveChangesAsync();
            }
            return Redirect("/Contracts?page=1");
        }


        [Authorize(Roles = "manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Contract contract)
        {
            if (contract.Termination_Date != null && contract.Enter_Date > contract.Termination_Date)
            {
                ModelState.AddModelError("", "Дата заключения договора не должна быть больше даты его расторжения");
            }
            if (!ModelState.IsValid)
            {
                FillViewBag();
                ViewBag.Action = "Create";
                ViewBag.Errors = ModelState.Values.Select(ms => ms.Errors.FirstOrDefault()).Where(error => error != null);
                ViewBag.Organizations = await db.Organizations.ToListAsync();
                return View("ModifyContract", contract);
            }
            db.Entry(contract).State = EntityState.Added;
            await db.SaveChangesAsync();
            return Redirect("/Contracts?page=1");
        }

        [Authorize(Roles = "manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update(Contract contract)
        {
            if (contract.Termination_Date != null && contract.Enter_Date > contract.Termination_Date)
            {
                ModelState.AddModelError("", "Дата заключения договора не должна быть больше даты его расторжения");
            }
            if (!ModelState.IsValid)
            {
                FillViewBag();
                ViewBag.Action = "Update";
                ViewBag.Errors = ModelState.Values.Select(ms => ms.Errors.FirstOrDefault()).Where(error => error != null);
                ViewBag.Organizations = await db.Organizations.ToListAsync();
                return View("ModifyContract", contract);
            }
            db.Entry(contract).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return Redirect("/Contracts?page=1");
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
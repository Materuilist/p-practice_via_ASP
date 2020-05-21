using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Uch_PracticeV3.Models.Identity;
using Uch_PracticeV3.Models;
using Uch_PracticeV3.Filters;
using System.Threading.Tasks;
using System.Data.Entity;
using Microsoft.Extensions.Configuration;

namespace Uch_PracticeV3.Controllers
{
    public class AdminController : Controller
    {
        private const int ITEMS_PER_PAGE = 3;
        private UCH_PracticeEntities db = new UCH_PracticeEntities();
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
        [Auth]
        public ActionResult Index()
        {
            return View();
        }

        [Auth]
        public async Task<ActionResult> Contracts()
        {
            switch (Request.QueryString.Get("action"))
            {
                case "edit":
                    {
                        int rowId = Request.QueryString.Get("rowId") != null ? Convert.ToInt32(
                            Request.QueryString.Get("rowId"))
                            : 1;
                        ViewBag.Action = "edit";
                        ViewBag.Organizations = await db.Organizations.ToListAsync();
                        ViewBag.Contract = await db.Contracts.FindAsync(rowId);
                        return View("ModifyContract");
                    }
                case "add":
                    {
                        ViewBag.Action = "add";
                        ViewBag.Organizations = await db.Organizations.ToListAsync();
                        return View("ModifyContract", new Contract());
                    }
                case "delete":
                    {
                        Int32 deletedItemId;
                        bool idWasPassed = Int32.TryParse(Request.QueryString.Get("rowId"), out deletedItemId);
                        if (idWasPassed)
                        {
                            db.Entry(new Contract() { Id = deletedItemId }).State = EntityState.Deleted;
                            await db.SaveChangesAsync();
                        }
                        return Redirect("/Admin/Contracts?page=1");
                    }
                default:
                    {
                        break;
                    }
            }

            int page;
            bool pageWasPassed = Int32.TryParse(Request.QueryString.Get("page"), out page);
            if (!pageWasPassed)
            {
                page = 1;
            }

            ViewBag.ItemsPerPage = ITEMS_PER_PAGE;
            ViewBag.PageIndex = page;
            ViewBag.NextPageExists = await db.Contracts.CountAsync() > ITEMS_PER_PAGE * page;
            return View(await db.Contracts.OrderBy(c=>c.Id).Skip((page - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE)
                .Include(c=>c.Organization).Include(c=>c.Students).ToListAsync());
        }

        [Auth]
        [HttpPost]
        [Route("Admin/Contracts/{operation}")]
        public async Task<ActionResult> Contracts(string operation, Contract contract)
        {
            switch (operation)
            {
                case "edit":
                    {
                        db.Entry(contract).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        break;
                    }
                case "add":
                    {
                        db.Entry(contract).State = EntityState.Added;
                        await db.SaveChangesAsync();
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            return Redirect("/Admin/Contracts?page=1");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
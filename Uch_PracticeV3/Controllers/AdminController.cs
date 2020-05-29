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
        private Uch_PracticeEntities db = new Uch_PracticeEntities();
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
            ViewBag.url = Request.Url.AbsolutePath;
            ViewBag.authed = AuthenticationManager.User.Identity.IsAuthenticated;
            return View();
        }

        //public ActionResult ButtonsScript()
        //{
        //    return PartialView("ButtonsScript");
        //}
        //public ActionResult Pagination()
        //{
        //    return PartialView("Pagination");
        //}

        [Auth]
        public async Task<ActionResult> Contracts()
        {
            ViewBag.Table = "Contracts";
            ViewBag.Title = "Договоры";
            ViewBag.url = Request.Url.AbsolutePath;
            ViewBag.authed = AuthenticationManager.User.Identity.IsAuthenticated;
            switch (Request.QueryString.Get("action"))
            {
                case "edit":
                    {
                        int rowId = Request.QueryString.Get("rowId") != null ? Convert.ToInt32(
                            Request.QueryString.Get("rowId"))
                            : 1;
                        ViewBag.Action = "edit";
                        ViewBag.Organizations = await db.Organizations.ToListAsync();
                        return View("ModifyContract", await db.Contracts.FindAsync(rowId));
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
        [ValidateAntiForgeryToken]
        [Route("Admin/Contracts/{operation}")]
        public async Task<ActionResult> Contracts(string operation, Contract contract)
        {
            if (contract.Termination_Date!=null && contract.Enter_Date>contract.Termination_Date)
            {
                ModelState.AddModelError("", "Дата заключения договора не должен быть больше даты его расторжения");
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Title = "Контракты";
                ViewBag.url = Request.Url.AbsolutePath;
                ViewBag.authed = AuthenticationManager.User.Identity.IsAuthenticated;
                ViewBag.Action = operation;
                ViewBag.Errors = ModelState.Values.Select(ms => ms.Errors.FirstOrDefault()).Where(error => error != null);
                ViewBag.Organizations = await db.Organizations.ToListAsync();
                return View("ModifyContract", contract);
            }
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

        [Auth]
        public async Task<ActionResult> Groups()
        {
            ViewBag.Table = "Groups";
            ViewBag.Title = "Группы";
            ViewBag.url = Request.Url.AbsolutePath;
            ViewBag.authed = AuthenticationManager.User.Identity.IsAuthenticated;
            switch (Request.QueryString.Get("action"))
            {
                case "edit":
                    {
                        int rowId = Request.QueryString.Get("rowId") != null ? Convert.ToInt32(
                            Request.QueryString.Get("rowId"))
                            : 1;
                        ViewBag.Action = "edit";
                        ViewBag.Specialties = await db.Specialties.ToListAsync();
                        return View("ModifyGroup", await db.Groups.FindAsync(rowId));
                    }
                case "add":
                    {
                        ViewBag.Action = "add";
                        ViewBag.Specialties = await db.Specialties.ToListAsync();
                        return View("ModifyGroup", new Group());
                    }
                case "delete":
                    {
                        Int32 deletedItemId;
                        bool idWasPassed = Int32.TryParse(Request.QueryString.Get("rowId"), out deletedItemId);
                        if (idWasPassed)
                        {
                            db.Entry(new Group() { Id = deletedItemId }).State = EntityState.Deleted;
                            await db.SaveChangesAsync();
                        }
                        return Redirect("/Admin/Groups?page=1");
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
            ViewBag.NextPageExists = await db.Groups.CountAsync() > ITEMS_PER_PAGE * page;
            return View(await db.Groups.OrderBy(g => g.Id).Skip((page - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE)
                .Include(g => g.Specialty).Include(g=>g.Students).ToListAsync());
        }

        [Auth]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Groups/{operation}")]
        public async Task<ActionResult> Groups(string operation, Group _group)
        {
            if ((from gr in db.Groups where gr.Naming == _group.Naming && gr.Id!=_group.Id select gr)
                   .FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Группа с таким названием уже есть");
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Title = "Группы";
                ViewBag.url = Request.Url.AbsolutePath;
                ViewBag.authed = AuthenticationManager.User.Identity.IsAuthenticated;
                ViewBag.Action = operation;
                ViewBag.Errors = ModelState.Values.Select(ms => ms.Errors.FirstOrDefault()).Where(error=>error!=null);
                ViewBag.Specialties = await db.Specialties.ToListAsync();
                return View("ModifyGroup", _group);
            }
            switch (operation)
            {
                case "edit":
                    {
                        db.Entry(_group).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        break;
                    }
                case "add":
                    {
                        db.Entry(_group).State = EntityState.Added;
                        await db.SaveChangesAsync();
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            return Redirect("/Admin/Groups?page=1");
        }

        [Auth]
        public async Task<ActionResult> Leaders()
        {
            ViewBag.Table = "Leaders";
            ViewBag.Title = "Руководители";
            ViewBag.url = Request.Url.AbsolutePath;
            ViewBag.authed = AuthenticationManager.User.Identity.IsAuthenticated;
            switch (Request.QueryString.Get("action"))
            {
                case "edit":
                    {
                        int rowId = Request.QueryString.Get("rowId") != null ? Convert.ToInt32(
                            Request.QueryString.Get("rowId"))
                            : 1;
                        ViewBag.Action = "edit";
                        ViewBag.Ranks = await db.Ranks.ToListAsync();
                        return View("ModifyLeader", await db.Leaders.FindAsync(rowId));
                    }
                case "add":
                    {
                        ViewBag.Action = "add";
                        ViewBag.Ranks = await db.Ranks.ToListAsync();
                        return View("ModifyLeader", new Leader());
                    }
                case "delete":
                    {
                        Int32 deletedItemId;
                        bool idWasPassed = Int32.TryParse(Request.QueryString.Get("rowId"), out deletedItemId);
                        if (idWasPassed)
                        {
                            db.Entry(new Leader() { Id = deletedItemId }).State = EntityState.Deleted;
                            await db.SaveChangesAsync();
                        }
                        return Redirect("/Admin/Leaders?page=1");
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
            ViewBag.NextPageExists = await db.Leaders.CountAsync() > ITEMS_PER_PAGE * page;
            return View(await db.Leaders.OrderBy(l => l.Id).Skip((page - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE)
                .Include(l => l.Rank).Include(l => l.Students).ToListAsync());
        }

        [Auth]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Leaders/{operation}")]
        public async Task<ActionResult> Leaders(string operation, Leader leader)
        {
            //email не уникален
            if ((from lead in db.Leaders where lead.Email == leader.Email && lead.Id!=leader.Id select lead)
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
                ViewBag.Title = "Руководители";
                ViewBag.url = Request.Url.AbsolutePath;
                ViewBag.authed = AuthenticationManager.User.Identity.IsAuthenticated;
                ViewBag.Action = operation;
                ViewBag.Errors = ModelState.Values.Select(ms => ms.Errors.FirstOrDefault())
                    .Where(error => error != null);
                ViewBag.Ranks = await db.Ranks.ToListAsync();
                return View("ModifyLeader", leader);
            }
            switch (operation)
            {
                case "edit":
                    {
                        db.Entry(leader).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        break;
                    }
                case "add":
                    {
                        db.Entry(leader).State = EntityState.Added;
                        await db.SaveChangesAsync();
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            return Redirect("/Admin/Leaders?page=1");
        }

        [Auth]
        public async Task<ActionResult> Organizations()
        {
            ViewBag.Table = "Organizations";
            ViewBag.Title = "Организации";
            ViewBag.url = Request.Url.AbsolutePath;
            ViewBag.authed = AuthenticationManager.User.Identity.IsAuthenticated;
            switch (Request.QueryString.Get("action"))
            {
                case "edit":
                    {
                        string rowId = Request.QueryString.Get("rowId");
                        if(rowId == null)
                        {
                            return Redirect("/");
                        }
                        ViewBag.Action = "edit";
                        ViewBag.Sectors = await db.Sectors.ToListAsync();
                        return View("ModifyOrganization", await db.Organizations.FindAsync(rowId));
                    }
                case "add":
                    {
                        ViewBag.Action = "add";
                        ViewBag.Sectors = await db.Sectors.ToListAsync();
                        return View("ModifyOrganization", new Organization());
                    }
                case "delete":
                    {
                        string deletedItemId = Request.QueryString.Get("rowId");
                        if (deletedItemId!=null)
                        {
                            db.Entry(new Organization() { Id = deletedItemId }).State = EntityState.Deleted;
                            await db.SaveChangesAsync();
                        }
                        return Redirect("/Admin/Organizations?page=1");
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
            ViewBag.NextPageExists = await db.Organizations.CountAsync() > ITEMS_PER_PAGE * page;
            return View(await db.Organizations.OrderBy(l => l.Id).Skip((page - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE)
                .Include(o => o.Contracts).Include(o => o.Sector).ToListAsync());
        }

        [Auth]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Organizations/{operation}")]
        public async Task<ActionResult> Organizations(string operation, Organization organization)
        {
            if ((from org in db.Organizations where org.FullNaming == organization.FullNaming && org.Id!=organization.Id
                 select org)
                   .FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Организация с таким полным названием уже есть");
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Title = "Организации";
                ViewBag.url = Request.Url.AbsolutePath;
                ViewBag.authed = AuthenticationManager.User.Identity.IsAuthenticated;
                ViewBag.Action = operation;
                ViewBag.Errors = ModelState.Values.Select(ms => ms.Errors.FirstOrDefault())
                    .Where(error => error != null);
                ViewBag.Sectors = await db.Sectors.ToListAsync();
                return View("ModifyOrganization", organization);
            }
            switch (operation)
            {
                case "edit":
                    {
                        db.Entry(organization).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        break;
                    }
                case "add":
                    {
                        db.Entry(organization).State = EntityState.Added;
                        await db.SaveChangesAsync();
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            return Redirect("/Admin/Organizations?page=1");
        }

        [Auth]
        public async Task<ActionResult> Ranks()
        {
            ViewBag.Table = "Ranks";
            ViewBag.Title = "Должности";
            ViewBag.url = Request.Url.AbsolutePath;
            ViewBag.authed = AuthenticationManager.User.Identity.IsAuthenticated;
            switch (Request.QueryString.Get("action"))
            {
                case "edit":
                    {
                        int rowId = Request.QueryString.Get("rowId") != null ? Convert.ToInt32(
                            Request.QueryString.Get("rowId"))
                            : 1;
                        ViewBag.Action = "edit";
                        return View("ModifyRank", await db.Ranks.FindAsync(rowId));
                    }
                case "add":
                    {
                        ViewBag.Action = "add";
                        return View("ModifyRank", new Rank());
                    }
                case "delete":
                    {
                        Int32 deletedItemId;
                        bool idWasPassed = Int32.TryParse(Request.QueryString.Get("rowId"), out deletedItemId);
                        if (idWasPassed)
                        {
                            db.Entry(new Rank() { Id = deletedItemId }).State = EntityState.Deleted;
                            await db.SaveChangesAsync();
                        }
                        return Redirect("/Admin/Ranks?page=1");
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
            ViewBag.NextPageExists = await db.Ranks.CountAsync() > ITEMS_PER_PAGE * page;
            return View(await db.Ranks.OrderBy(r => r.Id).Skip((page - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE)
                .Include(r => r.Leaders).ToListAsync());
        }

        [Auth]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Ranks/{operation}")]
        public async Task<ActionResult> Ranks(string operation, Rank rank)
        {
            if ((from r in db.Ranks where r.Naming == rank.Naming && r.Id!=rank.Id select r)
                   .FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Должность с таким названием уже есть");
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Title = "Должности";
                ViewBag.url = Request.Url.AbsolutePath;
                ViewBag.authed = AuthenticationManager.User.Identity.IsAuthenticated;
                ViewBag.Action = operation;
                ViewBag.Errors = ModelState.Values.Select(ms => ms.Errors.FirstOrDefault())
                    .Where(error => error != null);
                return View("ModifyRank", rank);
            }
            switch (operation)
            {
                case "edit":
                    {
                        db.Entry(rank).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        break;
                    }
                case "add":
                    {
                        db.Entry(rank).State = EntityState.Added;
                        await db.SaveChangesAsync();
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            return Redirect("/Admin/Ranks?page=1");
        }

        [Auth]
        public async Task<ActionResult> Sectors()
        {
            ViewBag.Table = "Sectors";
            ViewBag.Title = "Отрасли";
            ViewBag.url = Request.Url.AbsolutePath;
            ViewBag.authed = AuthenticationManager.User.Identity.IsAuthenticated;
            switch (Request.QueryString.Get("action"))
            {
                case "edit":
                    {
                        string rowId = Request.QueryString.Get("rowId");
                        if (rowId == null)
                        {
                            return Redirect("/");
                        }
                        ViewBag.Action = "edit";
                        return View("ModifySector", await db.Sectors.FindAsync(rowId));
                    }
                case "add":
                    {
                        ViewBag.Action = "add";
                        return View("ModifySector", new Sector());
                    }
                case "delete":
                    {
                        string deletedItemId = Request.QueryString.Get("rowId");
                        if (deletedItemId != null)
                        {
                            db.Entry(new Sector() { Id = deletedItemId }).State = EntityState.Deleted;
                            await db.SaveChangesAsync();
                        }
                        return Redirect("/Admin/Sectors?page=1");
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
            ViewBag.NextPageExists = await db.Sectors.CountAsync() > ITEMS_PER_PAGE * page;
            return View(await db.Sectors.OrderBy(s => s.Id).Skip((page - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE)
                .Include(s => s.Organizations).ToListAsync());
        }

        [Auth]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Sectors/{operation}")]
        public async Task<ActionResult> Sectors(string operation, Sector sector)
        {
            if ((from sect in db.Sectors where sect.Naming == sector.Naming && sect.Id!=sector.Id select sect)
                   .FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Отрасль с таким названием уже есть");
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Title = "Отрасли";
                ViewBag.url = Request.Url.AbsolutePath;
                ViewBag.authed = AuthenticationManager.User.Identity.IsAuthenticated;
                ViewBag.Action = operation;
                ViewBag.Errors = ModelState.Values.Select(ms => ms.Errors.FirstOrDefault())
                    .Where(error => error != null);
                return View("ModifySector", sector);
            }
            switch (operation)
            {
                case "edit":
                    {
                        db.Entry(sector).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        break;
                    }
                case "add":
                    {
                        db.Entry(sector).State = EntityState.Added;
                        await db.SaveChangesAsync();
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            return Redirect("/Admin/Sectors?page=1");
        }

        [Auth]
        public async Task<ActionResult> Specialties()
        {
            ViewBag.Table = "Specialties";
            ViewBag.Title = "Специальности";
            ViewBag.url = Request.Url.AbsolutePath;
            ViewBag.authed = AuthenticationManager.User.Identity.IsAuthenticated;
            switch (Request.QueryString.Get("action"))
            {
                case "edit":
                    {
                        string rowId = Request.QueryString.Get("rowId");
                        if (rowId == null)
                        {
                            return Redirect("/");
                        }
                        ViewBag.Action = "edit";
                        return View("ModifySpecialty", await db.Specialties.FindAsync(rowId));
                    }
                case "add":
                    {
                        ViewBag.Action = "add";
                        return View("ModifySpecialty", new Specialty());
                    }
                case "delete":
                    {
                        string deletedItemId = Request.QueryString.Get("rowId");
                        if (deletedItemId != null)
                        {
                            db.Entry(new Specialty() { Id = deletedItemId }).State = EntityState.Deleted;
                            await db.SaveChangesAsync();
                        }
                        return Redirect("/Admin/Specialties?page=1");
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
            ViewBag.NextPageExists = await db.Specialties.CountAsync() > ITEMS_PER_PAGE * page;
            return View(await db.Specialties.OrderBy(s => s.Id).Skip((page - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE)
                .Include(s => s.Groups).ToListAsync());
        }

        [Auth]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Specialties/{operation}")]
        public async Task<ActionResult> Specialties(string operation, Specialty specialty)
        {
            if ((from spec in db.Specialties where spec.Educational_Program == specialty.Educational_Program && spec.Id!=specialty.Id
                 select
                 spec)
                   .FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Образовательная программа должна быть уникальной");
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Title = "Специальности";
                ViewBag.url = Request.Url.AbsolutePath;
                ViewBag.authed = AuthenticationManager.User.Identity.IsAuthenticated;
                ViewBag.Action = operation;
                ViewBag.Errors = ModelState.Values.Select(ms => ms.Errors.FirstOrDefault())
                    .Where(error => error != null);
                return View("ModifySpecialty", specialty);
            }
            switch (operation)
            {
                case "edit":
                    {
                        db.Entry(specialty).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        break;
                    }
                case "add":
                    {
                        db.Entry(specialty).State = EntityState.Added;
                        await db.SaveChangesAsync();
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            return Redirect("/Admin/Specialties?page=1");
        }

        [Auth]
        public async Task<ActionResult> Students()
        {
            ViewBag.Table = "Students";
            ViewBag.Title = "Студенты";
            ViewBag.url = Request.Url.AbsolutePath;
            ViewBag.authed = AuthenticationManager.User.Identity.IsAuthenticated;
            switch (Request.QueryString.Get("action"))
            {
                case "edit":
                    {
                        string rowId = Request.QueryString.Get("rowId");
                        if (rowId == null)
                        {
                            return Redirect("/");
                        }
                        ViewBag.Action = "edit";
                        ViewBag.Groups = await db.Groups.ToListAsync();
                        ViewBag.Contracts = await db.Contracts.ToListAsync();
                        ViewBag.Leaders = await db.Leaders.ToListAsync();
                        return View("ModifyStudent", await db.Students.FindAsync(rowId));
                    }
                case "add":
                    {
                        ViewBag.Action = "add";
                        ViewBag.Groups = await db.Groups.ToListAsync();
                        ViewBag.Contracts = await db.Contracts.ToListAsync();
                        ViewBag.Leaders = await db.Leaders.ToListAsync();
                        return View("ModifyStudent", new Student());
                    }
                case "delete":
                    {
                        string deletedItemId = Request.QueryString.Get("rowId");
                        if (deletedItemId != null)
                        {
                            db.Entry(new Student() { Id = deletedItemId }).State = EntityState.Deleted;
                            await db.SaveChangesAsync();
                        }
                        return Redirect("/Admin/Students?page=1");
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
            ViewBag.NextPageExists = await db.Students.CountAsync() > ITEMS_PER_PAGE * page;
            return View(await db.Students.OrderBy(l => l.Id).Skip((page - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE)
                .Include(s => s.Contract).Include(s => s.Leader).Include(s=>s.Group).ToListAsync());
        }

        [Auth]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Students/{operation}")]
        public async Task<ActionResult> Students(string operation, Student student)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Title = "Студенты";
                ViewBag.url = Request.Url.AbsolutePath;
                ViewBag.authed = AuthenticationManager.User.Identity.IsAuthenticated;
                ViewBag.Action = operation;
                ViewBag.Errors = ModelState.Values.Select(ms => ms.Errors.FirstOrDefault())
                    .Where(error => error != null);
                ViewBag.Groups = await db.Groups.ToListAsync();
                ViewBag.Contracts = await db.Contracts.ToListAsync();
                ViewBag.Leaders = await db.Leaders.ToListAsync();
                return View("ModifyStudent", student);
            }
            switch (operation)
            {
                case "edit":
                    {
                        db.Entry(student).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        break;
                    }
                case "add":
                    {
                        db.Entry(student).State = EntityState.Added;
                        await db.SaveChangesAsync();
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            return Redirect("/Admin/Students?page=1");
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
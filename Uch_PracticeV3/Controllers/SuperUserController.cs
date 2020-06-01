using Aspose.Tasks;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Uch_PracticeV3.Models.Identity;

namespace Uch_PracticeV3.Controllers
{
    public class SuperUserController : Controller
    {
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        private UserContext db = new UserContext();
        
        [Authorize(Roles ="superuser")]
        public async Task<ActionResult> Index()
        {
            ViewBag.url = Request.Url.AbsolutePath;
            ViewBag.authed = AuthenticationManager.User.Identity.IsAuthenticated;
            ViewBag.isSuperUser = AuthenticationManager.User.Identity.GetUserRole() == "superuser";
            var users = await (from user in db.Users where user.RoleId == 2 select user).ToListAsync();
            return View(users);
        }

        [Route("SuperUser/Managers/create")]
        [Authorize(Roles = "superuser")]
        public async Task<ActionResult> Create()
        {
            ViewBag.Title = "Менеджеры УО";
            ViewBag.Action = "create";
            ViewBag.url = Request.Url.AbsolutePath;
            ViewBag.authed = AuthenticationManager.User.Identity.IsAuthenticated;
            ViewBag.isSuperUser = AuthenticationManager.User.Identity.GetUserRole() == "superuser";
            return View("Modify", new User());
        }
        [Route("SuperUser/Managers/update")]
        [Authorize(Roles = "superuser")]
        public async Task<ActionResult> Update()
        {
            int rowId;
            bool rowIsValid = Int32.TryParse(Request.QueryString.Get("rowId"), out rowId);
            if (!rowIsValid)
            {
                return View("Error");
            }
            ViewBag.Title = "Менеджеры УО";
            ViewBag.Action = "update";
            ViewBag.url = Request.Url.AbsolutePath;
            ViewBag.authed = AuthenticationManager.User.Identity.IsAuthenticated;
            ViewBag.isSuperUser = AuthenticationManager.User.Identity.GetUserRole() == "superuser";
            return View("Modify", await db.Users.FindAsync(rowId));
        }
        [Route("SuperUser/Managers/delete")]
        [Authorize(Roles = "superuser")]
        public async Task<ActionResult> Delete()
        {
            int rowId;
            bool rowIsValid = Int32.TryParse(Request.QueryString.Get("rowId"), out rowId);
            if (!rowIsValid)
            {
                return View("Error");
            }
            db.Entry(new User() { Id = rowId }).State = EntityState.Deleted;
            await db.SaveChangesAsync();
            return Redirect("/SuperUser");
        }

        [HttpPost]
        [Route("SuperUser/Managers/create")]
        [Authorize(Roles = "superuser")]
        public async Task<ActionResult> Create(User user)
        {
            if ((from _user in db.Users where _user.Email == user.Email && _user.Id != user.Id select _user)
                .FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Менеджер с таким Email уже есть");
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Title = "Менеджеры";
                ViewBag.url = Request.Url.AbsolutePath;
                ViewBag.authed = AuthenticationManager.User.Identity.IsAuthenticated;
                ViewBag.isSuperUser = AuthenticationManager.User.Identity.GetUserRole() == "superuser";
                ViewBag.Action = "create";
                ViewBag.Errors = ModelState.Values.Select(ms => ms.Errors.FirstOrDefault()).Where(error => error != null);
                return View("Modify", user);
            }
            db.Entry(user).State = EntityState.Added;
            await db.SaveChangesAsync();
            return Redirect("/SuperUser");
        }

        [HttpPost]
        [Route("SuperUser/Managers/update")]
        [Authorize(Roles = "superuser")]
        public async Task<ActionResult> Update(User user)
        {
            if ((from _user in db.Users where _user.Email == user.Email && _user.Id != user.Id select _user)
                .FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Менеджер с таким Email уже есть");
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Title = "Менеджеры";
                ViewBag.url = Request.Url.AbsolutePath;
                ViewBag.authed = AuthenticationManager.User.Identity.IsAuthenticated;
                ViewBag.isSuperUser = AuthenticationManager.User.Identity.GetUserRole() == "superuser";
                ViewBag.Action = "update";
                ViewBag.Errors = ModelState.Values.Select(ms => ms.Errors.FirstOrDefault()).Where(error => error != null);
                return View("Modify", user);
            }
            db.Entry(user).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return Redirect("/SuperUser");
        }
    }
}
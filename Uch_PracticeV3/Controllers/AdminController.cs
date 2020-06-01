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
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        [Authorize(Roles = "manager")]
        public ActionResult Index()
        {
            ViewBag.url = Request.Url.AbsolutePath;
            ViewBag.authed = AuthenticationManager.User.Identity.IsAuthenticated;
            ViewBag.isSuperUser = AuthenticationManager.User.Identity.GetUserRole() == "superuser";
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
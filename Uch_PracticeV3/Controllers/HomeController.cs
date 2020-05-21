﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
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
    public class HomeController : Controller
    {
        private ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }
        public async Task<ActionResult> Index()
        {
            //test data
            //var user = new ApplicationUser()
            //{ Email = "kirpetrosian@mail.ru", UserName = "kirpetrosian@mail.ru", Year = 2000 };
            //try
            //{
            //    await UserManager.DeleteAsync(user);
            //}
            //catch { }
            //await UserManager.CreateAsync(user, "trulala");
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Engine.Controllers.AbstractControllers;

namespace Engine.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            var str = "shi";
            var str2 = str;
            var str3 = str;
            return View();
        }

        [Authorize(Roles = "SuperUser")]
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

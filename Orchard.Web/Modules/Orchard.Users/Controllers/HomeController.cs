using Orchard.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Orchard.Users.Controllers
{
    public class HomeController : Controller
    {
        [AlwaysAccessibleAttribute]
        public ActionResult Index()
        {
            return View();
        }
    }
}
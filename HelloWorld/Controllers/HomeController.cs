using System.Web.Mvc;
using HelloWorld.Code.Security;
using System.Web.Optimization;
using UserRole = HelloWorld.Code.Security.UserRole;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using HelloWorld.Models;
using System.Linq;


namespace HelloWorld.Controllers
{
    [UserRoleAuthorize(UserRole.Administrator, UserRole.User)]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewData["Message"] = "This is a string created in the controller";

            return View();
        }       

    }

}


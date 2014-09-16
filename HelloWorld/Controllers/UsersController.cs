using System;
using System.Web.Mvc;
using HelloWorld.Code.Security;
using HelloWorld.Models;
using UserRole = HelloWorld.Code.Security.UserRole;

namespace HelloWorld.Controllers
{
  //  [Authorize(Roles = "Administrator")]
    [UserRoleAuthorize(UserRole.Administrator, UserRole.User)]
    public class UsersController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {

            return View();
        }


    }
}

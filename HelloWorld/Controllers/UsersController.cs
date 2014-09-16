using System;
using System.Web.Mvc;
using HelloWorld.Code.DataAccess;
using HelloWorld.Code.Security;
using HelloWorld.Code.Util;
using HelloWorld.Models;

namespace HelloWorld.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UsersController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {

            return View();
        }


    }
}

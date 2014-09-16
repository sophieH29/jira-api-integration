using System.Web.Mvc;
using HelloWorld.Code.DataAccess;
using HelloWorld.Code.Security;
using System.Web.Optimization;
using UserRole = HelloWorld.Code.Security.UserRole;

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

        [HttpGet]
        public ActionResult ContactUs()
        {
            return View();
        }


    }
}

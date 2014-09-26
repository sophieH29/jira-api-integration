using System.Web.Mvc;
using HelloWorld.Code.Security;
using System.Web.Optimization;
using UserRole = HelloWorld.Code.Security.UserRole;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using HelloWorld.Models;
using System.Linq;
using System.IO;


namespace HelloWorld.Controllers
{
    [UserRoleAuthorize(UserRole.Administrator, UserRole.User)]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ClearFolder();
            return View();
        }

        private void ClearFolder()
        {
            string[] fileNames = Directory.GetFiles(Server.MapPath("~/JiraAttachments/"))
                                   .Select(path => Path.GetFileName(path))
                                   .ToArray();

            foreach (string fileName in fileNames)
            {

                if (fileName.Length != 0)
                {
                    var file = Path.GetFileName(fileName);
                    var physicalPath = Path.Combine(Server.MapPath("~/JiraAttachments/"), file);

                    if (System.IO.File.Exists(physicalPath))
                    {
                        System.IO.File.Delete(physicalPath);
                    }
                }
            }
        }

    }


}


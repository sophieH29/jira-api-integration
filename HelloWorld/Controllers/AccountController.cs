using System.Web.Mvc;
using System.Web.Security;
using HelloWorld.Models;
using System.Web.Optimization;

namespace HelloWorld.Controllers
{
    public class AccountController : Controller
    {        
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl = "")
        {
            if (User.Identity.IsAuthenticated)
            {
                return LogOut();
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
    
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(Login model, string returnUrl = "")
        {
            if(ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.UserName, model.Password))
                {                   
                    FormsAuthentication.RedirectFromLoginPage(model.UserName, model.RememberMe);
                    RedirectToAction("Login", "Account", null);
                }

                ModelState.AddModelError("", "Incorrect username and/or password");
            }
                          
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account", null);
        }
    }
}

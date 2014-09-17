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

        [HttpGet]
        public ActionResult ContactUs()
        {
            return View();
        }

        //[HttpPost]
        //public JsonResult GetIssues()
        //{
            
        //    List<Issue> model = new List<Issue>();
        //    var issue = new Issue()
        //    {
        //        Key = "IECF - ",
        //        Type = "Bug",
        //        Summary = "Customer FeedBack Test",
        //        Priority = "High",
        //        Status = "InProgress",
        //        Created = DateTime.Now,
        //        Updated = DateTime.Now,
        //        DateResolved = DateTime.Now,
        //        DueDate = DateTime.Now

        //    };
        //    for (int i = 0; i < 20; i++ )
        //        model.Add(issue);


        //    return Json(model.Select(m => new
        //                    {
        //                        Key = m.Key,
        //                        Type = m.Type,
        //                        Summary = m.Summary,
        //                        Priority = m.Priority,
        //                        Status = m.Status,
        //                        Created = m.Created,
        //                        Updated = m.Updated,
        //                        DateResolved = m.DateResolved,
        //                        DueDate = m.DueDate
        //                    }).ToArray(), JsonRequestBehavior.AllowGet);
        //   }

        //public List<Issues> PrepareIssues()
        //{
        //    List<Issues> model = new List<Issues>();
        //    //for (int i = 0; i < 20; i++)
        //    //{
        //    int i = 0;
        //    model[i].Key = "IECF - ";
        //    model[i].Type = HelloWorld.Models.Type.Bug;
        //    model[i].Summary = model[i].Key + "Customer FeedBack Test";
        //    model[i].Priority = HelloWorld.Models.Priority.High;
        //    model[i].Status = HelloWorld.Models.Status.InProgress;
        //    model[i].Created = DateTime.Now.AddDays(i);
        //    model[i].Updated = DateTime.Now.AddDays(i + 1);
        //    model[i].DateResolved = DateTime.Now.AddDays(i + 2);
        //    model[i].DueDate = DateTime.Now.AddDays(i + 3);

        //    //}

        //    return model;
        //}

           
        }
        
    }


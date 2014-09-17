using System;
using System.Web.Mvc;
using HelloWorld.Code.Security;
using HelloWorld.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace HelloWorld.Controllers
{
  //  [Authorize(Roles = "Administrator")]
    [UserRoleAuthorize(UserRole.Administrator, UserRole.User)]
    public class IssueController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {

            return View();
        }

        [HttpGet]
        public ActionResult Details()
        {

            return View();
        }

         [HttpPost]
        public JsonResult GetIssues(string type, string status, string priority, string createdFrom, string createdTo)
        {

            //string query = "project = IECF AND issuetype =  " + type + " AND status = " + status + " AND priority = " + priority + " AND created >= " + createdFrom + " AND created <= " + createdTo + " AND assignee in (currentUser())";
            string query = "project = IECF ";
             if (type.Length != 0)
                 query = query + "AND issuetype = " + '"' + type + '"';
             if (status.Length != 0)
                 query = query + " AND status = " + '"' + status + '"';
             if (priority.Length != 0)
                 query = query + " AND priority = " + '"' + priority + '"';
             if (createdFrom.Length != 0) 
                 query = query + " AND created >= " + createdFrom;
             if (createdTo.Length != 0)
                 query = query + " AND created <= " + createdTo;
            string queryString = "search?jql=" + query ;
            HttpClient client = PrepareHttpClient();
            HttpResponseMessage response = client.GetAsync(queryString).Result;

            var issues = new List<Issue>();
            if (response.IsSuccessStatusCode)
            {
                dynamic jsonResponse = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
                dynamic issuesList = jsonResponse.issues;

                for (int i = 0; i != issuesList.Count; i++)
                {
                    issues.Add(new Issue { Key = issuesList[i].key.ToString(), Summary = issuesList[i].fields["summary"].ToString(), 
                        Description = issuesList[i].fields["description"].ToString(), Priority = issuesList[i].fields["priority"]["name"].ToString(),
                        Status = issuesList[i].fields["status"]["name"].ToString(), Type = issuesList[i].fields["issuetype"]["name"].ToString(),
                        Created = issuesList[i].fields["created"].ToString(), Updated = issuesList[i].fields["updated"].ToString(),
                        DueDate = issuesList[i].fields["duedate"].ToString(), DateResolved = issuesList[i].fields["resolutiondate"].ToString()
                    });
                }
            }

            return Json(issues, JsonRequestBehavior.AllowGet);
        }

        private HttpClient PrepareHttpClient()
        {
            string username = "enviuser";
            string password = "Env!user2014";
            var client = new HttpClient { BaseAddress = new Uri( "https://ioscorp.jira.com/rest/api/2/") };

            byte[] cred = Encoding.UTF8.GetBytes(username + ":" + password);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(cred));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }
    }
}

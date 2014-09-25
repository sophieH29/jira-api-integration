using System;
using System.Web;
using System.Web.Mvc;
using HelloWorld.Code.Security;
using HelloWorld.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using HelloWorld.Models.JiraModels;
using ServiceStack.Text;
using System.IO;


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

        public ActionResult FileUpload(IEnumerable<HttpPostedFileBase> files)//fileUploader name must be the name of uploader Control.
        {
            // The Name of the Upload component is "fileUploader"
            foreach (var file in files)
            {
                // Some browsers send file names with full path. We only care about the file name.
                var fileName = Path.GetFileName(file.FileName);
                var destinationPath = Path.Combine(Server.MapPath("~/JiraAttachments/"), fileName);


                file.SaveAs(destinationPath);
            }


            // Return an empty string to signify success
            return Content("");

        }

        public ActionResult Remove(string[] fileNames)
        {
            foreach (var fullName in fileNames)
            {
                var fileName = Path.GetFileName(fullName);
                var physicalPath = Path.Combine(Server.MapPath("~/JiraAttachments/"), fileName);


                // TODO: Verify user permissions


                if (System.IO.File.Exists(physicalPath))
                {
                    System.IO.File.Delete(physicalPath);
                }
            }


            // Return an empty string to signify success
            return Content("");

        }


         [HttpPost]
        public JsonResult CreateIssue(string type, string priority, string summary, string description, string[] labels)
        {
            string postBody = "";
            if (type == "New Feature")
            {
                var data = new IssueToCreate();
                data.fields.project.key = "IECF";
                data.fields.summary = summary;
                data.fields.labels = labels;
                data.fields.description = description;
                data.fields.issuetype.name = type;
                data.fields.priority.name = priority;
                data.fields.assignee.name = "enviuser";

                postBody = ServiceStack.Text.JsonSerializer.SerializeToString(data);
            }
            if (type == "Bug")
            {
                var data = new IssueToCreateBug();
                data.fields.project.key = "IECF";
                data.fields.summary = summary;
                data.fields.labels = labels;
                data.fields.description = description;
                data.fields.issuetype.name = type;
                data.fields.priority.name = priority;
                data.fields.assignee.name = "enviuser";
                data.fields.customfield_11410.value = "Rooted";

                postBody = ServiceStack.Text.JsonSerializer.SerializeToString(data);
            }
            HttpClient client = PrepareHttpClient();          

            System.Net.Http.HttpContent content = new System.Net.Http.StringContent(postBody, Encoding.UTF8, "application/json");

            System.Net.Http.HttpResponseMessage response = client.PostAsync("issue", content).Result;

            string key = "";
            if (response.IsSuccessStatusCode)
            {
                dynamic jsonResponse = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
                key = jsonResponse.key.ToString();                
                                   
            }
            string[] fileNames = Directory.GetFiles(Server.MapPath("~/JiraAttachments/"))
                                    .Select(path => Path.GetFileName(path))
                                    .ToArray();
            client.DefaultRequestHeaders.Add("X-Atlassian-Token", "nocheck");
            if (key.Length != 0)
            {
                foreach (string fileName in fileNames)
                {
                    System.Net.Http.HttpResponseMessage response2 = null;
                    MultipartFormDataContent content2 = null;
                    string filepath = "";
                    if (fileName.Length != 0)
                    {
                        filepath = Path.Combine(Server.MapPath("~/JiraAttachments/"), fileName);                       
                        content2 = new MultipartFormDataContent();
                        HttpContent fileContent = new ByteArrayContent(System.IO.File.ReadAllBytes(filepath));

                        string mimeType = System.Web.MimeMapping.GetMimeMapping(fileName);
                        //specifying MIME Type
                        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(mimeType);

                        content2.Add(fileContent, "file", fileName);

                         response2 = client.PostAsync("issue/" + key + "/attachments", content2).Result;
                        if (response2.IsSuccessStatusCode)
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

            return Json(key, JsonRequestBehavior.AllowGet);        
        }

        [HttpPost]
        public JsonResult GetIssues(string type, string status, string priority, string createdFrom, string createdTo)
        {

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
            string queryString = "search?jql=" + query;
            HttpClient client = PrepareHttpClient();
            HttpResponseMessage response = client.GetAsync(queryString).Result;

            var issues = new List<Issue>();
            if (response.IsSuccessStatusCode)
            {
                dynamic jsonResponse = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
                dynamic issuesList = jsonResponse.issues;

                for (int i = 0; i != issuesList.Count; i++)
                {
                    issues.Add(new Issue
                    {
                        Key = issuesList[i].key.ToString(),
                        Summary = issuesList[i].fields["summary"].ToString(),
                        Description = issuesList[i].fields["description"].ToString(),
                        Priority = issuesList[i].fields["priority"]["name"].ToString(),
                        Status = issuesList[i].fields["status"]["name"].ToString(),
                        Type = issuesList[i].fields["issuetype"]["name"].ToString(),
                        Created = issuesList[i].fields["created"].ToString(),
                        Updated = issuesList[i].fields["updated"].ToString(),
                        DueDate = issuesList[i].fields["duedate"].ToString(),
                        DateResolved = issuesList[i].fields["resolutiondate"].ToString(),
                        Label = issuesList[i].fields["labels"].ToString(),
                    });
                }
            }

            return Json(issues, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetComments(string key)
        {            
            string queryString = "issue/" + key + "/comment?expand";
            HttpClient client = PrepareHttpClient();
            HttpResponseMessage response = client.GetAsync(queryString).Result;

            var comments = new List<Comment>();
            if (response.IsSuccessStatusCode)
            {
                dynamic jsonResponse = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
                var issuesList = jsonResponse.comments;

                for (int i = 0; i != issuesList.Count; i++)
                {
                    comments.Add(new Comment
                    {                       
                        Created = issuesList[i].created.ToString(),
                        Updated = issuesList[i].updated.ToString(),
                        Author = issuesList[i].author["displayName"].ToString(),
                        Body = issuesList[i].body.ToString()
                    });
                }
            }

            return Json(comments, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EditIssue(string key, string priority, string summary, string description)
        {
                         
                var data = new IssueToEdit();
               // data.fields.project.key = "IECF";
                data.fields.summary = summary;
                data.fields.description = description;
                //data.fields.issuetype.name = type;
                data.fields.priority.name = priority;
                data.fields.assignee.name = "enviuser";
            
            HttpClient client = PrepareHttpClient();

            string postBody = ServiceStack.Text.JsonSerializer.SerializeToString(data);

            System.Net.Http.HttpContent content = new System.Net.Http.StringContent(postBody, Encoding.UTF8, "application/json");
            
            System.Net.Http.HttpResponseMessage response = client.PutAsync("issue/"+ key, content).Result;

            var res = "";
            if (response.IsSuccessStatusCode)
            {
                res = "Successfully edited!";
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        private HttpClient PrepareHttpClient()
        {
            string username = "enviuser";
            string password = "Env!user2014";
            var client = new HttpClient { BaseAddress = new Uri("https://ioscorp.jira.com/rest/api/2/") };

            byte[] cred = Encoding.UTF8.GetBytes(username + ":" + password);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(cred));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }

        public string key { get; set; }
    }
}

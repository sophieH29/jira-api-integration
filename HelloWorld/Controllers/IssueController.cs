﻿using System;
using System.Web;
using System.Web.Mvc;
using HelloWorld.Code.Security;
using HelloWorld.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
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
       

        public ActionResult FileUpload(IEnumerable<HttpPostedFileBase> files)//fileUploader name must be the name of uploader Control.
        {
            
            
            // The Name of the Upload component is "fileUploader"
            foreach (var file in files)
            {
                // Some browsers send file names with full path. We only care about the file name.
                string fileName = Path.GetFileName(file.FileName);
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

                if (System.IO.File.Exists(physicalPath))
                {
                    System.IO.File.Delete(physicalPath);
                }
            }
            // Return an empty string to signify success
            return Content("");

        }


        [HttpPost]
        public async Task<JsonResult> CreateIssue(string type, string priority, string summary, string description, string[] labels)
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
            HttpContent content = new StringContent(postBody, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("issue", content);

            string key = "";
            if (response.IsSuccessStatusCode)
            {
                dynamic jsonResponse = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
                key = jsonResponse.key.ToString();

            }

            else
            {
                key = "error";
            }
            string[] fileNames = Directory.GetFiles(Server.MapPath("~/JiraAttachments/"))
                                    .Select(path => Path.GetFileName(path))
                                    .ToArray();
            client.DefaultRequestHeaders.Add("X-Atlassian-Token", "nocheck");
            if (key.Length != 0)
            {
                foreach (string fileName in fileNames)
                {
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

                        var response2 = await client.PostAsync("issue/" + key + "/attachments", content2);
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
        public async Task<JsonResult> GetIssues(string status, string priority, string createdFrom, string createdTo, string type = "Epic")
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
            var response = await client.GetAsync(queryString);
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
            else
            {
                issues.Add(new Issue
                               {
                                   Key = "error"
                               });
            }
            return Json(issues, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> GetUserStoriesUnderEpic(string epicKey)
        {
            string queryString = "search?jql=cf[11010]=" + epicKey;
            HttpClient client = PrepareHttpClient();
            var response = await client.GetAsync(queryString);
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
            else
            {
                issues.Add(new Issue
                {
                    Key = "error"
                });
            }
            return Json(issues, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> GetAttachments(string key)
        {
            string queryString = "issue/" + key + "?fields=attachment";
            HttpClient client = PrepareHttpClient();
            var response = await client.GetAsync(queryString);
            
            var attachments = new List<Attachment>();
            if (response.IsSuccessStatusCode)
            {
                dynamic jsonResponse = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
                var issuesList = jsonResponse.fields.attachment;
                
                for (int i = 0; i != issuesList.Count; i++)
                {
                   string mimeType = issuesList[i].mimeType.ToString();
                    string[] type = mimeType.Split('/');
                    bool isImage = type[0] == "image" ? true : false;
                    attachments.Add(new Attachment
                    {
                        Key = key,
                        Id = issuesList[i].id.ToString(),
                        CreatedDate = issuesList[i].created.ToString(),
                        Name = issuesList[i].filename.ToString(),
                        ContentURL = issuesList[i].content.ToString(),
                        MimeType = issuesList[i].mimeType.ToString(),
                        Size = issuesList[i].size.ToString(),
                        IsImage = isImage
                    });
                }
            }

            return Json(attachments, JsonRequestBehavior.AllowGet);
        }

       [HttpGet]
        public FileContentResult ShowAttachment(string id, string fileName)
       {
           string fullUrl = "https://ioscorp.jira.com/secure/attachment/" + id + "/" + fileName;
           string mimeType = System.Web.MimeMapping.GetMimeMapping(fileName);
           // int sizeToInt = Convert.ToInt32(size);
            HttpClient client = new HttpClient();
            client.BaseAddress = new System.Uri(fullUrl);
            byte[] cred = UTF8Encoding.UTF8.GetBytes("enviuser:Env!user2014");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(cred));
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            byte[] result = new byte[100000];
            System.Net.Http.HttpResponseMessage response2 = client.GetAsync(fullUrl).Result;
            if (response2.IsSuccessStatusCode)
            {
                result = response2.Content.ReadAsByteArrayAsync().Result;

            }
            return new FileContentResult(result, mimeType);
        }

        

        [HttpPost]
        public async Task<JsonResult> DeleteAttachments(string id)
        {
            string queryString = "attachment/" + id;
            HttpClient client = PrepareHttpClient();
            var response = await client.DeleteAsync(queryString);
            var res = "";
            if (response.IsSuccessStatusCode)
            {
                res = "Successfully deleted!";
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async  Task<JsonResult> AddNewAttachments(string key)
        {
            HttpClient client = PrepareHttpClient();
            string res = "";

            string[] fileNames = Directory.GetFiles(Server.MapPath("~/JiraAttachments/"))
                                    .Select(path => Path.GetFileName(path))
                                    .ToArray();
            if (fileNames.Length == 0) res = "You shoud shoose file to attach!";
            client.DefaultRequestHeaders.Add("X-Atlassian-Token", "nocheck");
            if (key.Length != 0)
            {
                foreach (string fileName in fileNames)
                {
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

                        var response2 = await client.PostAsync("issue/" + key + "/attachments", content2);
                        if (response2.IsSuccessStatusCode)
                        {
                            var file = Path.GetFileName(fileName);
                            var physicalPath = Path.Combine(Server.MapPath("~/JiraAttachments/"), file);
                            res = "Attachment was successfully added!";
                            if (System.IO.File.Exists(physicalPath))
                            {
                                System.IO.File.Delete(physicalPath);
                            }

                        }
                    }                    
                }
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> GetComments(string key)
        {
            string queryString = "issue/" + key + "/comment?expand";
            HttpClient client = PrepareHttpClient();
            var response = await client.GetAsync(queryString);

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
        public async Task<JsonResult> EditIssue(string key, string priority, string summary, string description)
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
            HttpContent content = new System.Net.Http.StringContent(postBody, Encoding.UTF8, "application/json");
            var response = await client.PutAsync("issue/" + key, content);
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
    }
}

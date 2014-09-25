using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HelloWorld.Models.JiraModels
{
    public class FieldsForFeature
    {
        public Project project { get; set; }
        public Assignee assignee { get; set; }
        public string summary { get; set; }
        public string[] labels { get; set; }
        public string description { get; set; }
        public Priority priority { get; set; }
        public IssueType issuetype { get; set; }
        public FieldsForFeature()
        {
            priority = new Priority();
            assignee = new Assignee();
            project = new Project();
            issuetype = new IssueType();            
        }  
    }
}
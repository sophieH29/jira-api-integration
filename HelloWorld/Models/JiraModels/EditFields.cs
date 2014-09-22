using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HelloWorld.Models.JiraModels
{
    public class EditFields
    {
        
        public Assignee assignee { get; set; }
        public string summary { get; set; }
        public string description { get; set; }
        public Priority priority { get; set; }      
        public EditFields()
        {
            priority = new Priority();
            assignee = new Assignee();                        
        }  
    }
}
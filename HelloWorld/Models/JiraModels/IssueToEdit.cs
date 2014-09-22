using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HelloWorld.Models.JiraModels
{
    public class IssueToEdit
    {
        public EditFields fields { get; set; }
         public IssueToEdit()
        {
            fields = new EditFields();
        }
    }
}
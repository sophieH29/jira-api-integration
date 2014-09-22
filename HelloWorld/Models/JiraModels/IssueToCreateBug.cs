using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HelloWorld.Models.JiraModels
{
    public class IssueToCreateBug
    {
          public FieldsForBug fields { get; set; }
          public IssueToCreateBug()
        {
            fields = new FieldsForBug();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HelloWorld.Models.JiraModels
{
    public class IssueToCreate
    {
        public FieldsForFeature fields { get; set; }
        public IssueToCreate()
        {
            fields = new FieldsForFeature();
        }
    }
}
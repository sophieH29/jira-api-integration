using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HelloWorld.Models.JiraModels
{
    public class Attachment
    {
        public string Id { get; set; }

        public string Key { get; set; }

        public string Name { get; set; }

        public string CreatedDate { get; set; }

        public string Author { get; set; }

        public string ContentURL { get; set; }

        public bool IsImage { get; set; }
    }
}
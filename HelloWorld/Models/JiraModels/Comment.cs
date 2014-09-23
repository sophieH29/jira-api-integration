using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HelloWorld.Models.JiraModels
{
    public class Comment
    {
        public string Created { get; set; }

        public string Updated { get; set; }

        public string Author { get; set; }

        public string Body { get; set; }
    }
}
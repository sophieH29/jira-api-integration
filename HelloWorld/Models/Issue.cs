using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HelloWorld.Models
{
    public class Issue
    {
        public string Key { get; set; }

        public string Type { get; set; }

        public string Summary { get; set; }

        public string Description { get; set; }

        public string Priority { get; set; }

        public string Status { get; set; }

        public string Created { get; set; }

        public string Updated { get; set; }

        public string DateResolved { get; set; }

        public string DueDate { get; set; }

       
    }

   
}
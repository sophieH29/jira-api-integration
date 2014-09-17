using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HelloWorld.Models
{
    public enum Priority
    {
        High = 1,
        Low = 2,
        Middle = 3
    }

    public enum Type
    {
        Bug = 1,
        FeatureRequest = 2
    }

    public enum Status
    {
        Open = 1,
        InProgress = 2,
        Resolved = 3,
        closed = 4
    }
}
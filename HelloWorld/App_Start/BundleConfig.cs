
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web;
using System.Web.Optimization;

namespace HelloWorld.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/kendo").Include(
             "~/Scripts/kendo/kendo.core.min.js",
             "~/Scripts/kendo/kendo.grid.min.js",
            "~/Scripts/kendo/kendo.web.min.js"));

            
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
              "~/Scripts/jquery-{version}.js",
              "~/Scripts/modernizr-*",
              "~/Scripts/bootstrap.js",
              "~/Scripts/respond.js",
              "~/Scripts/jquery-1.9.1-vsdoc.js",
              "~/Scripts/jquery.validate-vsdoc.js",
              "~/Scripts/jquery.validate.js",
              "~/Scripts/jquery.validate.js",
              "~/Scripts/jquery.ui.datepicker.js"));
             bundles.Add(new ScriptBundle("~/bundles/grid").Include(
                 "~/Scripts/Grid/grid.js"
                 ));

            bundles.Add(new StyleBundle("~/Content/kendo").Include(
            "~/Content/kendo/kendo.common.min.css",
            "~/Content/kendo/kendo.default.min.css"));

          

             bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/jquery-ui-datepicker.css",
                      "~/Content/site.css"));
        }

        }
    }

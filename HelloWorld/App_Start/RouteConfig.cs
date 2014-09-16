using System.Web.Mvc;
using System.Web.Routing;

namespace HelloWorld
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default", //Must be unique
                url: "{controller}/{action}/{id}", //Define the parts of the URL
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
                //If a part of the URL is missing, the last line defines the default values to use
            );
        }
    }
}
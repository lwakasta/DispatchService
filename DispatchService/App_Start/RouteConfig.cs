using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DispatchService
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Authentication", action = "Login", id = UrlParameter.Optional }
            );

            //routes.MapRoute(
            //    name: "Creation",
            //    url: "{controller}/{action}/{street}/{house}/{flat}",
            //    defaults: new { controller = "Request", action = "CreateRequest", street = "", house = "", flat = "" }
            //);
        }
    }
}

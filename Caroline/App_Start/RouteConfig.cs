﻿using Caroline.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Caroline
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new {controller = "Game", action = "Index", id = UrlParameter.Optional}
                );
            routes.MapRoute(
                name: "Admin",
                url: "admin/{action}/{id}/",
                defaults: new {controller="Admin",action="Index",id=UrlParameter.Optional}
                );
            routes.MapRoute(
                name: "AdministrateUser",
                url: "admin/account/{id}/{command}",
                defaults: new { controller = "Admin", action = "Account" });
        }
    }
}

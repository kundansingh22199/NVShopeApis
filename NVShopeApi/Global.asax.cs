using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Thinktecture.IdentityModel.Http.Cors.WebApi;

namespace NVShopeApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        public class CorsConfig
        {
            public static void RegisterCors(HttpConfiguration httpConfiguration)
            {
                var corsConfig = new WebApiCorsConfiguration();
                corsConfig.RegisterGlobal(httpConfiguration);

                corsConfig.ForAllResources().AllowAllOriginsAllMethodsAndAllRequestHeaders();
            }
        }
    }
}

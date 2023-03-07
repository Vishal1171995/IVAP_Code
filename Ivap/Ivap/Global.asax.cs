using Ivap.App_Start;
using Ivap.ModelBinder;
using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Ivap
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ModelBinders.Binders.DefaultBinder = new TrimModelBinder();

            //RegisterWebApiFilters(GlobalConfiguration.Configuration.Filters);
            //WebApiConfig.Register(GlobalFilters.con);
            //SchedulerJob.RunProgram().GetAwaiter().GetResult();
            //System.Net.ServicePointManager.ServerCertificateValidationCallback +=(se, cert, chain, sslerror) =>{return true;};
        }


        //protected void Session_Start(Object sender, EventArgs e)
        //{
        //    if (Request.IsSecureConnection == true)
        //        Response.Cookies["ASP.NET_SessionID"].Secure = true;
        //}

        //protected void Application_PreSendRequestHeaders()
        //{
        //    // Response.Headers.Remove("Server");
        //    Response.Headers.Remove("Server");
        //    Response.Headers.Remove("X-Powered-By");
        //    Response.Headers.Remove("X-AspNet-Version");
        //    Response.Headers.Remove("X-AspNetMvc-Version");
        //    Response.AddHeader("x-frame-options", "DENY");
        //    Response.AppendHeader("Cache-Control", "no-cache, no-store, must-revalidate, pre-check=0,post-check=0"); // HTTP 1.1.
        //    Response.AppendHeader("Pragma", "no-cache"); // HTTP 1.0.
        //    Response.AppendHeader("Expires", "0"); // Proxies.
        //    try
        //    {
        //        HttpContext.Current.Response.Headers.Add("X-Frame-Options", "DENY");
        //        HttpContext.Current.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
        //        HttpContext.Current.Response.Headers.Add("X-Content-Type-Options", "nosniff");
        //    }catch
        //    { }

        //}

        //protected void Application_BeginRequest()
        //{

        //    if (!Context.Request.IsSecureConnection)
        //        Response.Redirect(Context.Request.Url.ToString().Replace("http:", "https:"));
        //}
    }

    
}

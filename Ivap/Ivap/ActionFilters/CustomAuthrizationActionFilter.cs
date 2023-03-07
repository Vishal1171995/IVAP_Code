using Ivap.Models;
using Ivap.Repository;
using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Ivap.ActionFilters
{
    public class CustomAuthrizationActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            AppUser uBo=new AppUser();
            //var AType=filterContext.ActionDescriptor.GetCustomAttributes()
            //This line of code  is for skiping filter logic for the action method that is addorn with SkipFilter
            if (filterContext.ActionDescriptor.GetCustomAttributes(typeof(Ivap.CustomAttribute.SkipAuthrization), false).Any())
                return;

            //Now Getting Action Type...Action Type Might be ADD UPDATE OR VIEW
            string ActionType = "";

            if (filterContext.ActionDescriptor.GetCustomAttributes(typeof(Ivap.CustomAttribute.ADDUpdateAction), false).Any())
                ActionType = "CreateAction";

            if (filterContext.ActionDescriptor.GetCustomAttributes(typeof(Ivap.CustomAttribute.ViewAction), false).Any())
                ActionType = "ViewAction";

            //Lets check its Authrization 
           if( HttpContext.Current.Session["Ubo"]!=null)
           {
               bool IsAuthorized = false;
               uBo = (AppUser)HttpContext.Current.Session["Ubo"];
               var descriptor = filterContext.ActionDescriptor;
               var controllerName = descriptor.ControllerDescriptor.ControllerName;
               //Its Authenticated user now lets check its Authrisation
               IsAuthorized = AuthorizationRepo.IsAuthorized(controllerName, ActionType);
                if (IsAuthorized)
                    return;
               if (filterContext.HttpContext.Request.IsAjaxRequest())
               {
                   filterContext.Result = new JsonResult
                   {
                       Data = new
                       {
                           status = "403"
                       },
                       JsonRequestBehavior = JsonRequestBehavior.AllowGet
                   };
                   filterContext.HttpContext.Response.StatusCode = 403;

                   return;
               }
               filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary 
                { 
                    { "controller", "Authorization" }, 
                    { "action", "Unauthorized" } ,
                    {"area",string.Empty }
                });
               return;
           }
           else
           {
             if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new JsonResult
                    {
                        Data = new
                        {
                            status = "403"
                        },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                    filterContext.HttpContext.Response.StatusCode = 403;

                    return;
                }
                filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary 
                { 
                    { "controller", "Account" }, 
                    { "action", "Login" } ,
                    {"area",string.Empty }
                });
               return;
           }
        }

    }
}
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
    public class CustomAuthActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Response Res = new Response();
            AccountRepo ObjARepo=new AccountRepo();
            Res.IsSuccess = true;

            //if (HttpContext.Current.Session["Ubo"] != null)
            //{
            //    var ObjU = (AppUser)(HttpContext.Current.Session["Ubo"]);
            //    Res = ObjARepo.CheckLoggedInUser(ObjU.UID, System.Web.HttpContext.Current.Session.SessionID);
            //}
            
            //System.Web.HttpContext.Current.Session.SessionID

            if (HttpContext.Current.Session["Ubo"] == null || Res.IsSuccess==false)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new JsonResult
                    {
                        Data = new
                        {
                            status = "401"
                        },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                    filterContext.HttpContext.Response.StatusCode = 401;

                    return;
                }
                filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary 
                { 
                    { "controller", "Account" }, 
                    { "action", "LogOut" } ,
                    {"area",string.Empty }
                });
                return;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ivap.Repository;
namespace Ivap.ActionFilters
{
    class MyErrorHandler : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            Log(filterContext);

            base.OnException(filterContext);
        }

        private void Log(ExceptionContext filterContext)
        {
            LogErrorRepo objRepo = new LogErrorRepo();
            var controllerName = (string)filterContext.RouteData.Values["controller"];
            var actionName = (string)filterContext.RouteData.Values["action"];
            var model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);
            objRepo.LogError(controllerName, actionName, filterContext.Exception.Message);
        }
    }
}
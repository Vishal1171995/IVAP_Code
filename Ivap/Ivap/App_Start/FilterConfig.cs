using Ivap.ActionFilters;
using System.Web;
using System.Web.Mvc;

namespace Ivap.App_Start
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new MyErrorHandler());
        }
    }
}

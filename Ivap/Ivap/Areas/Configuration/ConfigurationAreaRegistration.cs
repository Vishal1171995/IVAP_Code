using Ivap.ActionFilters;
using System.Web.Mvc;

namespace Ivap.Areas.Configuration
{
    
    public class ConfigurationAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Configuration";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Configuration_default",
                "Configuration/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
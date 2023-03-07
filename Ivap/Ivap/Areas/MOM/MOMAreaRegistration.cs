using System.Web.Mvc;

namespace Ivap.Areas.MOM
{
    public class MOMAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "MOM";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "MOM_default",
                "MOM/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
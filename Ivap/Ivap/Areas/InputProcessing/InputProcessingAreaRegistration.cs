using System.Web.Mvc;

namespace Ivap.Areas.InputProcessing
{
    public class InputProcessingAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "InputProcessing";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "InputProcessing_default",
                "InputProcessing/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
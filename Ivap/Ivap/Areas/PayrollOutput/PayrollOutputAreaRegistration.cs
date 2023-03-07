using System.Web.Mvc;

namespace Ivap.Areas.PayrollOutput
{
    public class PayrollOutputAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "PayrollOutput";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "PayrollOutput_default",
                "PayrollOutput/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
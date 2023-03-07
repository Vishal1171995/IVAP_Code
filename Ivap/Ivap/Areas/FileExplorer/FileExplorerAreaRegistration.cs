using System.Web.Mvc;

namespace Ivap.Areas.FileExplorer
{
    public class FileExplorerAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "FileExplorer";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "FileExplorer_default",
                "FileExplorer/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
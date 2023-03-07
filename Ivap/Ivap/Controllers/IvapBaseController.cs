using Ivap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Ivap.Controllers
{
    public class IvapBaseController : Controller
    {
        public AppUser IvapUser { private set; get; }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            AppUser ObjU = new AppUser();
            ObjU = (AppUser)Session["uBo"];
            IvapUser = ObjU;
        }
    }
}
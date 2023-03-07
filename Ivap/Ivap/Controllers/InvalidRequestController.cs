using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ivap.Controllers
{
    public class InvalidRequestController : Controller
    {
        // GET: InvalidRequest
        [Route("InvalidRequest")]
        public ActionResult InvalidRequest()
        {
            return View();
        }
    }
}
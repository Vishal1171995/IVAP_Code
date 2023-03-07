using Ivap.Areas.Configuration.Repository;
using Ivap.Areas.InputProcessing.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ivap.Controllers
{
    public class TestController : Controller
    {
        // GET: Test
        public ActionResult Index()
        {
            //EntityComponentRepo Obj = new EntityComponentRepo();
            //Obj.PublishTempHisTable(31, "HRDMAST");
            //Obj.PublishTempHisTable(31, "PAYMAST");
            InPutFileMovementRepo Obj = new InPutFileMovementRepo();
            //Obj.SetInputWorkFlow(91, 9, 119, 31,"Test Remarks...");


            return View();
        }
    }
}
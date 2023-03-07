using Ivap.ActionFilters;
using Ivap.Repository;
using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ivap.Controllers
{
    [CustomAuthActionFilter]
    public class AcountSetupWizardController : IvapBaseController
    {
        [Route("AcountSetupWizard", Name = "AcountSetupWizard")]
        public ActionResult AcountSetupWizard()
        {
            AcountSetupWizardRepo objRepo = new AcountSetupWizardRepo();
            try
            {
                ViewBag.Ds = objRepo.GetQuickSetup(IvapUser.EID);
                return View();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [Route("GetMasterCount", Name = "GetMasterCount")]
        public ActionResult GetMasterCount(int EID)
        {
            AcountSetupWizardRepo objRepo = new AcountSetupWizardRepo();
            Response Res = new Response();
            try
            {
                EID = IvapUser.EID;
                DataTable dt = objRepo.GetMasterCount(EID);
                Res.Data = JsonSerializer.SerializeTable(dt);
                Res.IsSuccess = true;
                return Json(Res, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                throw;
            }
        }
    }
}
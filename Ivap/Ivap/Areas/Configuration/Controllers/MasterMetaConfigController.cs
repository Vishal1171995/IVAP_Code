using Ivap.ActionFilters;
using Ivap.Controllers;
using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using Ivap.Areas.Configuration.Repository;
using Ivap.Areas.Configuration.Models;
using Ivap.CustomAttribute;

namespace Ivap.Areas.Configuration.Controllers
{
    [CustomAuthActionFilter(Order = 0)]
    [CustomAuthrizationActionFilter]
    [RouteArea("Configuration", AreaPrefix = "")]
    [RoutePrefix("Configuration")]
    public class MasterMetaConfigController : IvapBaseController
    {
        MasterMetaRepo MRepo = new MasterMetaRepo();
        // GET: Configuration/MasterMetaConfig
        [ViewAction]
        [Route("MasterMetaSetup",Name = "MasterMetaSetup")]
        public ActionResult MasterMetaSetup()
        {
            try
            {
                MasterMetaModel Model = new MasterMetaModel();
                Model.ENTITY_ID = IvapUser.EID;
                Model.SCREEN_NAMEList = DropdownUtils.ToSelectList(MRepo.GetDropdownLabel(Model), "SCREEN_NAME", "SCREEN_NAME");
                return View(Model);
            }

            catch
            {
                throw;
            }
        }
      
        
        [ViewAction]
        [HttpPost]
        [Route("GetMasterMeta", Name = "GetMasterMeta")]
        public ActionResult GetMasterMeta(string Screen_Name)
        {

            KendoGridUtils res = new KendoGridUtils();
            try
            {
                MasterMetaModel Model = new MasterMetaModel();
                Model.ENTITY_ID = IvapUser.EID;
                Model.SCREEN_NAME = Screen_Name;
                DataTable dt = MRepo.GetMasterMeta(Model);
                res.Data = JsonSerializer.SerializeTable(dt);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }



        [ADDUpdateAction]
        //[ValidateAntiForgeryToken]
        [HttpPost]
        [Route("UpdateMasterMeta", Name = "UpdateMasterMeta")]
        public ActionResult UpdateMasterMeta(List<MasterMetaModel> Model)
        {
            Response res = new Response();
            try
            {
                if (ModelState.IsValid)
                {
                    res = MRepo.UpdateMasterMeta(Model);
                    return Json(res, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return View();
                }
            }
            catch
            {
                throw;
            }
        }

       
    }
}
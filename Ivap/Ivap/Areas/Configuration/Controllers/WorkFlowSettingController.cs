using Ivap.ActionFilters;
using Ivap.Areas.Configuration.Models;
using Ivap.Areas.Configuration.Repository;
using Ivap.Controllers;
using Ivap.CustomAttribute;
using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ivap.Areas.Configuration.Controllers
{
    [CustomAuthrizationActionFilter(Order = 1)]
    [CustomAuthActionFilter(Order = 0)]
    [RouteArea("Configuration", AreaPrefix = "")]
    [RoutePrefix("Configuration")]
    public class WorkFlowSettingController : IvapBaseController
    {

        // GET: Configuration/WorkFlowSetting
        [ViewAction]
        [Route("WorkFlowSetting", Name = "WorkFlowSetting")]
        public ActionResult WorkFlowSetting()
        {
            WFSettingModel Model = new WFSettingModel();
            FileSetupModel objModel = new FileSetupModel();
            FileSetupRepo objRepo = new FileSetupRepo();
            try
            {
                //objModel.EID = IvapUser.EID;
                //Model.FileList = DropdownUtils.ToSelectList(objRepo.GetFileType(objModel,"WFS"), "TID", "WFS_FILENAME");
                return View("WorkFlowSetting", Model);
            }
            catch
            {
                throw;
            }
        }

        [ViewAction]
        [HttpGet]
        [Route("GetPAYRollFlowSetting", Name = "GetPAYRollFlowSetting")]
        public PartialViewResult GetPAYRollFlowSetting(int File_ID)
        {
            Response res = new Response();
            WorkFlowSettingRepo WFSRepo = new WorkFlowSettingRepo();
           WFSettingModel WFModel = new WFSettingModel();
            WFModel.FILE_ID = File_ID;
            WFModel.EID = IvapUser.EID;
            try
            {
                WFSRepo.GetWorkFlowSetting(WFModel);
                return PartialView("~/Areas/Configuration/Views/WorkFlowSetting/PayRollWorkFlowItem.cshtml", WFModel);
            }
            catch
            {
                throw;
            }
        }

        [ViewAction]
        [HttpGet]
        [Route("GetWFFileType", Name = "GetWFFileType")]
        public ActionResult GetFileType(string PayrollFileType)
        {
            Response Res = new Response();
            WorkFlowSettingRepo WFSRepo = new WorkFlowSettingRepo();
            WFSettingModel WFModel = new WFSettingModel();
            WFModel.EID = IvapUser.EID;
            try
            {
                FileSetupRepo objRepo = new FileSetupRepo();
                FileSetupModel objModel = new FileSetupModel();
                objModel.EID = IvapUser.EID;
                objModel.CreatedBy = IvapUser.UID;
                objModel.FILE_TYPE = PayrollFileType;
                DataTable dt= objRepo.GetFileTypeForWFSetting(objModel);
                Res.Data = JsonSerializer.SerializeTable(dt);
                Res.IsSuccess = true;
                return Json(Res, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                throw;
            }
        }


        [ViewAction]
        [HttpGet]
        [Route("GetFlowSetting", Name = "GetFlowSetting")]
        public PartialViewResult GetFlowSetting(int File_ID)
        {
            Response res = new Response();
            WorkFlowSettingRepo WFSRepo = new WorkFlowSettingRepo();
            PAYRollWFSettingModel PayWFModel = new PAYRollWFSettingModel();
            PayWFModel.FILE_ID = File_ID;
            PayWFModel.EID = IvapUser.EID;
            try
            {
                WFSRepo.GetPayRollWorkFlowSetting(PayWFModel);
                return PartialView("~/Areas/Configuration/Views/WorkFlowSetting/WorkFlowItem.cshtml", PayWFModel);
               
            }
            catch
            {
                throw;
            }
        }
        [ViewAction]
        [Route("GETUserRoleWise", Name = "GETUserRoleWise")]
        public ActionResult GETUserRoleWise(int RoleID,int FileID)
        {
            Response ret = new Response();
            try
            {
                WorkFlowSettingRepo WFSRepo = new WorkFlowSettingRepo();
                WFSettingModel WFModel = new WFSettingModel();
                WFModel.EID = IvapUser.EID;
                WFModel.FILE_ID = FileID;
                DataTable dt = WFSRepo.GETUserRoleWise(RoleID, WFModel);
                ret.Data = JsonSerializer.SerializeTable(dt);
                ret.IsSuccess = true;
                return Json(ret, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        //Payroll input workflow by vivek
        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("PayRollAddUpdateWorkFlowSetting", Name = "PayRollAddUpdateWorkFlowSetting")]
        public ActionResult PayRollAddUpdateWorkFlowSetting(WFSettingModel Model)
        {
            WorkFlowSettingRepo WFSRepo = new WorkFlowSettingRepo();
            WFSettingModel WModel = new WFSettingModel();
            Response res = new Response();
            try
            {
                if (ModelState.IsValid)
                {
                   Model.CreatedBy = IvapUser.UID;
                    Model.EID = IvapUser.EID;
                    res = WFSRepo.SetWorkFlowSetting(Model);
                    return Json(res, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var results = new List<ValidationResult>();
                    var vc = new ValidationContext(Model, null, null);
                    var isValid = Validator.TryValidateObject(Model, vc, results, true);
                    var errors = Array.ConvertAll(results.ToArray(), o => o.ErrorMessage);
                    res.IsSuccess = false;
                    res.Message = string.Join(" ", errors);
                    res.Data = string.Join(" ", errors);
                    return Json(res, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                throw;
            }
        }
        //PMS output workflow by vivek
        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("AddUpdateWorkFlow", Name = "AddUpdateWorkFlow")]
        public ActionResult AddUpdateWorkFlow(PAYRollWFSettingModel Model)
        {
            WorkFlowSettingRepo WFSRepo = new WorkFlowSettingRepo();
            PAYRollWFSettingModel WModel = new PAYRollWFSettingModel();
            Response res = new Response();
            try
            {
                if (ModelState.IsValid)
                {
                    Model.CreatedBy = IvapUser.UID;
                    Model.EID = IvapUser.EID;
                    res = WFSRepo.SetPayRollWorkFlowSetting(Model);
                    return Json(res, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var results = new List<ValidationResult>();
                    var vc = new ValidationContext(Model, null, null);
                    var isValid = Validator.TryValidateObject(Model, vc, results, true);
                    var errors = Array.ConvertAll(results.ToArray(), o => o.ErrorMessage);
                    res.IsSuccess = false;
                    res.Message = string.Join(" ", errors);
                    res.Data = string.Join(" ", errors);
                    return Json(res, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                throw;
            }
        }
    }

}
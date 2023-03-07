using Ivap.ActionFilters;
using Ivap.Areas.Master.Models;
using Ivap.Areas.Master.Repository;
using Ivap.Controllers;
using Ivap.CustomAttribute;
using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Ivap.Areas.Master.Controllers
{

    [CustomAuthActionFilter(Order = 0)]
    [CustomAuthrizationActionFilter(Order = 1)]
    [RouteArea("Master", AreaPrefix = "")]
    [RoutePrefix("Masters")]
    public class Sub_FunctionController : IvapBaseController
    {
        // GET: Master/Sub_Function
        [ViewAction]
        [Route("SubFunction", Name = "ViewSubFunction")]
        public ActionResult ViewSubFunction()
        {

            FunctionRepo objFunctionRepo = new FunctionRepo();
            FunctionModel objFuncModel = new FunctionModel();
            SubFunctionModel Model = new SubFunctionModel();
            try 
            {
                Model.EID = IvapUser.EID;
                Model.SetDisplayName();
                objFuncModel.EID = 0;
                objFuncModel.IsActive = true;
                Model.FunctionList = DropdownUtils.ToSelectList(objFunctionRepo.GetFunction(objFuncModel).Tables[0], "TID", "FUNC_NAME");
                Model.IsActive = true;

            }
            catch (Exception ex)
            {
                throw;
            }
            return View("SubFunction", Model);
        }
        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("AddUpdateSubFunction", Name = "AddUpdateSubFunction")]
        public ActionResult AddUpdateSubFunction(SubFunctionModel Model)
        {
            SubFunctionRepo objSubFunction = new SubFunctionRepo();
            Response res = new Response();
            try
            {
                if (ModelState.IsValid)
                {
                    Model.CreatedBy = IvapUser.UID;
                    Model.EID = IvapUser.EID;
                    res = objSubFunction.AddUpdateSubFunction(Model);
                    return Json(res);
                }
                else
                {
                    return View(Model);
                }
            }
            catch
            {
                throw;
            }
        }
        [ViewAction]
        [HttpGet]
        [Route("GetSubFunction", Name = "GetSubFunction")]
        public ActionResult GetSubFunction(int SID)
        {
            SubFunctionRepo SubFunctionRepo = new SubFunctionRepo();
            //KendoGridUtils
            SubFunctionModel model = new SubFunctionModel();
            DataTable dt = new DataTable();
            KendoGridUtils res = new KendoGridUtils();
            try
            {
                model.SID = SID;
                model.EID = IvapUser.EID;
                model.CreatedBy = IvapUser.UID;
                dt = SubFunctionRepo.GetSubFunction(model);
                res = res.GetCommandButtonForGrid("ViewSubFunction");
                res.Data = JsonSerializer.SerializeTable(dt);
                var UserGrid = Json(res, JsonRequestBehavior.AllowGet);
                UserGrid.MaxJsonLength = int.MaxValue;
                return UserGrid;
            }
            catch
            {
                throw;
            }
        }
        [ViewAction]
        [HttpGet]
        [Route("GetSubFunctionHis", Name = "GetSubFunctionHis")]
        public ActionResult GetSubFunctionHis(int SID)
        {
            SubFunctionRepo SubFunctionRepo = new SubFunctionRepo();
            SubFunctionModel model = new SubFunctionModel();
            DataTable dt = new DataTable();
            Response res = new Response();
            try
            {
                model.SID = SID;
                dt = SubFunctionRepo.GetSubFunctionHis(model);
                res.IsSuccess = true;
                res.Data = JsonSerializer.SerializeTable(dt);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                throw;
            }
        }
        [ViewAction]
        [HttpGet]
        [Route("DownloadAllSubFunction", Name = "DownloadAllSubFunction")]
        public ActionResult DownloadAllSubFunction()
        {
            SubFunctionRepo objRepo = new SubFunctionRepo();
            SubFunctionModel model = new SubFunctionModel();
            //model.SetDisplayName();
            DataTable dt = new DataTable();
            try
            {
                model.EID = IvapUser.EID;
                model.CreatedBy = IvapUser.UID;
                model.SetDisplayName();
                model.ERP_SUB_FUNC_CODE = "0";
                dt = objRepo.GetSubFunction(model);
                DataTable dtSubF = dt.DefaultView.ToTable(false, "ENTITY_NAME", "PAY_SUB_FUNC_CODE", "ERP_SUB_FUNC_CODE", "SUB_FUNC_NAME", "ISACTIVE");
                dtSubF.Columns["ENTITY_NAME"].ColumnName = "Entity Name";
                dtSubF.Columns["PAY_SUB_FUNC_CODE"].ColumnName = model.PAY_SUB_FUNC_CODE_TEXT;
                dtSubF.Columns["ERP_SUB_FUNC_CODE"].ColumnName = model.ERP_SUB_FUNC_CODE_TEXT;
                dtSubF.Columns["SUB_FUNC_NAME"].ColumnName = model.SUB_FUNC_NAME_TEXT;

                dtSubF.Columns["ISACTIVE"].ColumnName = "Is_Active";
                string FileName = ExcellUtils.DataTableToExcel(dtSubF);
                FileName = FileName.Replace("/", "").Replace("..", "").Replace("\\", "");
                string FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + FileName;
                byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "SubFunctionMaster.xlsx");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
       [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("UploadSubFunctionDetails", Name = "UploadSubFunctionDetails")]
        public ActionResult UploadSubFunctionDetails(string FileName)
        {
            Response ret = new Response();
            try
            {
                string[] arr = FileName.Split('.');

                if (arr[1].ToString().ToUpper() != "XLSX")
                {
                    ret.IsSuccess = false;
                    ret.Message = "Invalid file type.";
                    return Json(ret, JsonRequestBehavior.AllowGet);
                }
                SubFunctionRepo objRepo = new SubFunctionRepo();
                string FilePath = Server.MapPath("~/Docs/Temp/" + FileName);
                int SuccessCount = 0;
                int FailCount = 0;
                int CreatedBy = IvapUser.UID;
                int EID = IvapUser.EID;
                string ResultFileName = objRepo.UploadSubFunctionDetails(FilePath, CreatedBy, EID, ref SuccessCount, ref FailCount);
                if (ResultFileName == "Invalid File Format")
                {
                    ret.IsSuccess = false;
                    ret.Data = "";
                    ret.Message = "Invalid File Format";
                }
                else if (ResultFileName == "Please remove all the comma from your file.")
                {
                    ret.IsSuccess = false;
                    ret.Message = "Please remove all the commas from your file.";
                    return Json(ret, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string data = "{\"Success\":\"" + SuccessCount + "\",\"Failed\":\"" + FailCount + "\",\"FileName\":\"" + ResultFileName.Replace("\\", "\\\\") + "\"}";
                    ret.IsSuccess = true;
                    ret.Data = data;
                    //ret = ret.GetResponse("Mapping", "GetMapping", -1000, "", data, "");
                }

                return Json(ret, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                throw;
            }
        }
    }
}
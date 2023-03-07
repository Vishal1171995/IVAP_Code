using Ivap.ActionFilters;
using Ivap.Areas.Configuration.Models;
using Ivap.Areas.Configuration.Repository;
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

namespace Ivap.Areas.Configuration.Controllers
{
    [CustomAuthActionFilter(Order = 0)]
    [CustomAuthrizationActionFilter(Order = 1)]
    [RouteArea("Configuration", AreaPrefix = "")]
    [RoutePrefix("Configuration")]
    public class GlobalComponentController : IvapBaseController
    {
        GlobalComponentRepo CRepo = new GlobalComponentRepo();
        GlobalComponentModel CModel = new GlobalComponentModel();
        [ViewAction]
        [Route("GlobalComponent", Name = "ViewGlobalComponent")]
        public ActionResult ViewGlobalComponent()
        {
            FileSetupRepo objRepo = new FileSetupRepo();
            FileComponentModel RtModel = new FileComponentModel();
            try
            {
                CModel.Component_File_TypeList = DropdownUtils.ToSelectList(CRepo.GetComponentFileType(CModel, ""), "COMPONENT_FILE_TYPE", "COMPONENT_FILE_TYPE");
                CModel.COMPONENT_DATATYPEList = DropdownUtils.ToSelectList(CRepo.GetComponentFileType(CModel, "D"), "COMPONENT_DATATYPE", "COMPONENT_DATATYPE");
                CModel.Component_TableNameList = DropdownUtils.ToSelectList(CRepo.GetComponentTableName(""), "TABLE_NAME", "SCREEN_NAME");
                CModel.Validation_List = DropdownUtils.ToSelectList(objRepo.GetValidation(RtModel), "VALIDATION_FIELD", "VALIDATION_FIELD");
                return View("GlobalComponent", CModel);
            }
            catch
            {
                throw;
            }
        }


        [ViewAction]
        [HttpGet]
        [Route("getComponent", Name = "getComponent")]
        public ActionResult getComponent(int ComponentID)
        {
            //ComponentRepo CRepo = new ComponentRepo();
            //ComponentModel CModel = new ComponentModel();
            CModel.COMPONENTID = Convert.ToInt32(ComponentID);
            KendoGridUtils res = new KendoGridUtils();
            try
            {
                DataTable dt = CRepo.GetComponent(CModel);
                res.Data = JsonSerializer.SerializeTable(dt);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("AddUpdateComponent", Name = "AddUpdateComponent")]
        public ActionResult AddUpdateComponent(GlobalComponentModel Model)
        {
            Response res = new Response();
            try
            {
                if (ModelState.IsValid)
                {
                    Model.CreatedBy = IvapUser.UID;
                    if (Model.COMPONENTID > 0)
                    {
                        res = CRepo.AddUpdateComponent(Model);
                    }
                    else {
                        res = CRepo.AddComponent(Model);
                    }
                  
                    return Json(res, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
                    res.Message = messages;
                    return Json(res, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                throw;
            }
        }
        [ViewAction]
        [HttpGet]
        [Route("GetComponentHistory", Name = "GetComponentHistory")]
        public ActionResult GetComponentHistory(int ComponentID)
        {
            //ComponentRepo CRepo = new ComponentRepo();
            //ComponentModel CModel = new ComponentModel();
            Response ret = new Response();
            try
            {
                CModel.COMPONENTID = Convert.ToInt32(ComponentID);
                ret.Data = JsonSerializer.SerializeTable(CRepo.GetComponentHistory(CModel));
                ret.IsSuccess = true;
                return Json(ret, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.Message = ex.Message;
                return Json(ret, JsonRequestBehavior.AllowGet);
            }
        }

        [ViewAction]
        [HttpGet]
        [Route("getComponentFileType", Name = "getComponentFileType")]
        public ActionResult getComponentFileType(string COMPONENT_FILE_TYPE, string COMPONENT_TYPE)
        {
            Response res = new Response();
            try
            {
                CModel.COMPONENT_FILE_TYPE = COMPONENT_FILE_TYPE;
                CModel.COMPONENT_TYPE = COMPONENT_TYPE;
                DataTable dt = CRepo.GetComponentFileType(CModel,"");
                res.Data = JsonSerializer.SerializeTable(dt);
                res.IsSuccess = true;
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [ViewAction]
        [HttpGet]
        [Route("getComponentTableName", Name = "getComponentTableName")]
        public ActionResult getComponentTableName(string TableName)
        {
            Response res = new Response();
            try
            {
                DataTable dt = CRepo.GetComponentTableName(TableName);
                res.Data = JsonSerializer.SerializeTable(dt);
                res.IsSuccess = true;
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("DeleteGlobalComponent", Name = "DeleteGlobalComponent")]
        public ActionResult DeleteGlobalComponent(int GlobalCompID)
        {
            Response res = new Response();
            try
            {
                res = CRepo.DeleteGlobalComponent(GlobalCompID);
            }
            catch
            {
                res.IsSuccess = false;
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }


        [ViewAction]
        [HttpGet]
        [Route("ExportComponent", Name = "ExportComponent")]
        public ActionResult ExportComponent()
        {
            DataTable dt = new DataTable();
            try
            {
                CModel.COMPONENTID = 0;
                 dt = CRepo.GetComponent(CModel);
                DataTable dtComp = dt.DefaultView.ToTable(false, "COMPONENT_NAME", "COMPONENT_DISPLAY_NAME", "COMPONENT_TYPE", "COMPONENT_SUB_TYPE", "COMPONENT_FILE_TYPE", "COMPONENT_DATATYPE", "MIN_LENGTH", "MAX_LENGTH", "EXTRA_INPUT_VALIDATION", "MANDATORY_STATUS", "STATUS", "COMPONENT_DESCRIPTION");
                dtComp.Columns["COMPONENT_NAME"].ColumnName = "COMPONENT NAME";
                dtComp.Columns["COMPONENT_DISPLAY_NAME"].ColumnName = "COMPONENT DISPLAY NAME";
                dtComp.Columns["COMPONENT_TYPE"].ColumnName = "COMPONENT TYPE";
                dtComp.Columns["COMPONENT_SUB_TYPE"].ColumnName = "COMPONENT SUB TYPE"; 
                dtComp.Columns["COMPONENT_FILE_TYPE"].ColumnName = "FILE TYPE";
                dtComp.Columns["COMPONENT_DATATYPE"].ColumnName = "COMPONENT DATATYPE";
                dtComp.Columns["MIN_LENGTH"].ColumnName = "MIN_LENGTH";
                dtComp.Columns["MAX_LENGTH"].ColumnName = "MAX_LENGTH";
                dtComp.Columns["EXTRA_INPUT_VALIDATION"].ColumnName = "EXTRA INPUT VALIDATION";
                dtComp.Columns["MANDATORY_STATUS"].ColumnName = "MANDATORY";
                dtComp.Columns["STATUS"].ColumnName = "STATUS";
                dtComp.Columns["COMPONENT_DESCRIPTION"].ColumnName = "COMPONENT DESCRIPTION";
                string FileName = ExcellUtils.DataTableToExcel(dtComp);
                FileName = FileName.Replace("/", "").Replace("..", "").Replace("\\", "");
                string FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + FileName;
                byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "GlobalComponent.xlsx");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("UploadGlobalComponent", Name = "UploadGlobalComponent")]
        public ActionResult UploadGlobalComponent(string FileName)
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
                string FileExt = arr[arr.Length - 1].Trim().ToUpper();
                GlobalComponentRepo CompRepo = new GlobalComponentRepo();
                string FilePath = Server.MapPath("~/Docs/Temp/" + FileName);
                string SampleFileName = "";
                SampleFileName = Server.MapPath("~/Docs/Sample/Component.xlsx");
                int SuccessCount = 0;
                int FailCount = 0;
                int CreatedBy = IvapUser.UID;
                string ResultFileName = CompRepo.UploadGlobalComponentDetails(FilePath, FileExt, SampleFileName, CreatedBy, ref SuccessCount, ref FailCount);
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
                }

                return Json(ret, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                throw;
            }
        }

        [Route("DownloadComponentSample", Name = "DownloadComponentSample")]
        public ActionResult DownloadComponentSample()
        {
            Response ret = new Response();
            try
            {
                string FileName = Server.MapPath("~/Docs/Sample/Component.xlsx");

                byte[] fileBytes = System.IO.File.ReadAllBytes(FileName);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "SampleComponent.xlsx");
            }
            catch
            {
                throw;
            }
        }

        [ADDUpdateAction]
        [HttpGet]
        [Route("DownLoadResultFileComponent", Name = "DownLoadResultFileComponent")]
        public ActionResult DownLoadResultFileComponent(string FileName)
        {
            Response ret = new Response();
            try
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(FileName);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "ResultFile.csv");
            }
            catch
            {
                throw;
            }
        }
    }
}
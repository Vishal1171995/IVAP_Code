using Ivap.ActionFilters;
using Ivap.Controllers;
using Ivap.CustomAttribute;
using System;
using Ivap.Areas.Configuration.Models;
using Ivap.Areas.Configuration.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ivap.Utils;
using System.Data;
using System.Web.Hosting;

namespace Ivap.Areas.Configuration.Controllers
{
    
    [CustomAuthActionFilter(Order = 0)]
    [CustomAuthrizationActionFilter(Order = 1)]
    [RouteArea("Configuration", AreaPrefix = "")]
    [RoutePrefix("Configuration")]
    public class EntityComponentsController : IvapBaseController
    {
        EntityComponentModel EComModel = new EntityComponentModel();
        EntityComponentRepo EComRepo = new EntityComponentRepo();
        // GET: Configuration/EntityComponents
        [ViewAction]
        [Route("EntityComponentsList", Name = "EntityComponentsList")]
        public ActionResult EntityComponentsList()
        {
            try
            {
                FileSetupRepo objRepo = new FileSetupRepo();
                FileComponentModel RtModel = new FileComponentModel();
                GlobalComponentModel CModel = new GlobalComponentModel();
                GlobalComponentRepo CRepo = new GlobalComponentRepo();
                DataSet ds = new DataSet();
                ds = EComRepo.CheckPublistStatus(IvapUser.EID);
                ViewBag.Publishds = ds;
                EComModel.COMPONENT_DATATYPEList = DropdownUtils.ToSelectList(CRepo.GetComponentFileType(CModel, "D"), "COMPONENT_DATATYPE", "COMPONENT_DATATYPE");
                EComModel.Component_TableNameList = DropdownUtils.ToSelectList(CRepo.GetComponentTableName(""), "TABLE_NAME", "SCREEN_NAME");
                EComModel.Validation_List = DropdownUtils.ToSelectList(objRepo.GetValidation(RtModel), "VALIDATION_FIELD", "VALIDATION_FIELD");
                return View(EComModel);
            }
            catch
            {
                throw;
            }
        }

        [ViewAction]
        [HttpGet]
        [Route("getEntityComponent", Name = "getEntityComponent")]
        public ActionResult getEntityComponent(int EntityCompID,string COMPONENT_FILE_TYPE)
        {
            EComModel.EntityCOMPID = Convert.ToInt32(EntityCompID);
            EComModel.EID = IvapUser.EID;
            EComModel.COMPONENT_FILE_TYPE= COMPONENT_FILE_TYPE;
            KendoGridUtils res = new KendoGridUtils();
            try
            {
                DataTable dt = EComRepo.GetEntityComponent(EComModel);
                res = res.GetCommandButtonForGrid("EntityComponentsList");
                res.Data = JsonSerializer.SerializeTable(dt);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }



        [ViewAction]
        [HttpGet]
        [Route("ExportEntityComponent", Name = "ExportEntityComponent")]
        public ActionResult ExportEntityComponent(string COMPONENT_FILE_TYPE)
        {
            DataTable dt = new DataTable();
            try
            {
                EComModel.EntityCOMPID = 0;
                EComModel.EID = IvapUser.EID;
                EComModel.COMPONENT_FILE_TYPE = COMPONENT_FILE_TYPE;
                dt = EComRepo.GetEntityComponent(EComModel);
                DataTable dtComp = dt.DefaultView.ToTable(false, "COMPONENT_NAME", "COMPONENT_DISPLAY_NAME", "COMPONENT_TYPE", "COMPONENT_SUB_TYPE", "COMPONENT_FILE_TYPE", "COMPONENT_DATATYPE", "MIN_LENGTH", "MAX_LENGTH",
                                     "COMPONENT_TABLE_NAME", "COMPONENT_COLUMN_NAME", "PMS_CODE", "GL_CODE", "EXTRA_INPUT_VALIDATION", "MANDATORY_STATUS", "STATUS");
                dtComp.Columns["COMPONENT_NAME"].ColumnName = "COMPONENT NAME";
                dtComp.Columns["COMPONENT_DISPLAY_NAME"].ColumnName = "COMPONENT DISPLAY NAME";
                dtComp.Columns["COMPONENT_TYPE"].ColumnName = "COMPONENT TYPE";
                dtComp.Columns["COMPONENT_SUB_TYPE"].ColumnName = "COMPONENT SUB TYPE";
                dtComp.Columns["COMPONENT_FILE_TYPE"].ColumnName = "FILE TYPE";
                dtComp.Columns["COMPONENT_DATATYPE"].ColumnName = "COMPONENT DATATYPE";
                dtComp.Columns["MIN_LENGTH"].ColumnName = "MIN LENGTH";
                dtComp.Columns["MAX_LENGTH"].ColumnName = "MAX LENGTH";
                dtComp.Columns["COMPONENT_TABLE_NAME"].ColumnName = "COMPONENT TABLE NAME";
                dtComp.Columns["COMPONENT_COLUMN_NAME"].ColumnName = "COMPONENT FIELD NAME";
                dtComp.Columns["PMS_CODE"].ColumnName = "PMS CODE";
                dtComp.Columns["GL_CODE"].ColumnName = "GL CODE";
                dtComp.Columns["EXTRA_INPUT_VALIDATION"].ColumnName = "EXTRA INPUT VALIDATION";
                dtComp.Columns["MANDATORY_STATUS"].ColumnName = "MANDATORY";
                dtComp.Columns["STATUS"].ColumnName = "STATUS";
                string FileName = ExcellUtils.DataTableToExcel(dtComp);
                FileName = FileName.Replace("/", "").Replace("..", "").Replace("\\", "");
                string FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + FileName;
                byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                if (COMPONENT_FILE_TYPE == "HRDMAST")
                {
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "HRDComponent.xlsx");
                }
                else
                {
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "SalaryComponent.xlsx");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //[ViewAction]
        //[HttpGet]
        //[Route("ExportPAYEntityComponent", Name = "ExportPAYEntityComponent")]
        //public ActionResult ExportPAYEntityComponent()
        //{
        //    DataTable dt = new DataTable();
        //    try
        //    {
        //        EComModel.EntityCOMPID = 0;
        //        EComModel.EID = IvapUser.EID;
        //        EComModel.COMPONENT_FILE_TYPE = "PAYMAST";
        //        dt = EComRepo.GetEntityComponent(EComModel);
        //        DataTable dtComp = dt.DefaultView.ToTable(false, "COMPONENT_NAME", "COMPONENT_DISPLAY_NAME", "COMPONENT_TYPE", "COMPONENT_SUB_TYPE", "COMPONENT_FILE_TYPE", "COMPONENT_DATATYPE", "MIN_LENGTH", "MAX_LENGTH",
        //                             "COMPONENT_TABLE_NAME", "COMPONENT_COLUMN_NAME", "PMS_CODE", "GL_CODE", "EXTRA_INPUT_VALIDATION", "MANDATORY_STATUS", "STATUS");
        //        dtComp.Columns["COMPONENT_NAME"].ColumnName = "COMPONENT NAME";
        //        dtComp.Columns["COMPONENT_DISPLAY_NAME"].ColumnName = "COMPONENT DISPLAY NAME";
        //        dtComp.Columns["COMPONENT_TYPE"].ColumnName = "COMPONENT TYPE";
        //        dtComp.Columns["COMPONENT_SUB_TYPE"].ColumnName = "COMPONENT SUB TYPE";
        //        dtComp.Columns["COMPONENT_FILE_TYPE"].ColumnName = "FILE TYPE";
        //        dtComp.Columns["COMPONENT_DATATYPE"].ColumnName = "COMPONENT DATATYPE";
        //        dtComp.Columns["MIN_LENGTH"].ColumnName = "MIN LENGTH";
        //        dtComp.Columns["MAX_LENGTH"].ColumnName = "MAX LENGTH";
        //        dtComp.Columns["COMPONENT_TABLE_NAME"].ColumnName = "COMPONENT TABLE NAME";
        //        dtComp.Columns["COMPONENT_COLUMN_NAME"].ColumnName = "COMPONENT FIELD NAME";
        //        dtComp.Columns["PMS_CODE"].ColumnName = "PMS CODE";
        //        dtComp.Columns["GL_CODE"].ColumnName = "GL CODE";
        //        dtComp.Columns["EXTRA_INPUT_VALIDATION"].ColumnName = "EXTRA INPUT VALIDATION";
        //        dtComp.Columns["MANDATORY_STATUS"].ColumnName = "MANDATORY";
        //        dtComp.Columns["STATUS"].ColumnName = "STATUS";
        //        string FileName = ExcellUtils.DataTableToExcel(dtComp);
        //        FileName = FileName.Replace("/", "").Replace("..", "").Replace("\\", "");
        //        string FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + FileName;
        //        byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
        //        return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "GlobalComponent.xlsx");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}
        [ViewAction]
        [HttpGet]
        [Route("GetEntityComponentHistory", Name = "GetEntityComponentHistory")]
        public ActionResult GetEntityComponentHistory(int EntityComponentID)
        {
            Response ret = new Response();
            try
            {
                DataTable dt = new DataTable();
                EComModel.EntityCOMPID = Convert.ToInt32(EntityComponentID);
                dt = EComRepo.GetEntityComponentHistory(EComModel);
                ret.Data = JsonSerializer.SerializeTable(dt);
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
        [Route("GetGlobleComponentsList", Name = "GetGlobleComponentsList")]
        public ActionResult GetGlobleComponentsList(string COMPONENT_FILE_TYPE, string Search_Text, string EntityCompID)
        {
             string Globle_Component_ID = string.Join(",", EntityCompID);
            try
            {
                Response Res = new Response();
                EntityComponentRepo ObjERepo = new EntityComponentRepo();
                DataTable dt = ObjERepo.SearchEntityComponent(IvapUser.EID, COMPONENT_FILE_TYPE, Search_Text, EntityCompID);
                Res.Data = JsonSerializer.SerializeTable(dt);
                Res.IsSuccess = true;
                return Json(Res, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                throw;
            }
        }
 


        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("DeleteEntityComponent", Name = "DeleteEntityComponent")]
        public ActionResult DeleteEntityComponent(int EntityCompID, string COMPONENT_FILE_TYPE)
        {
            Response res = new Response();
            try
            {
                EComModel.EntityCOMPID = EntityCompID;
                EComModel.EID = IvapUser.EID;
                EComModel.File_Type = COMPONENT_FILE_TYPE;
                res = EComRepo.DeleteEntityComponent(EComModel);
            }
            catch
            {
                res.IsSuccess = false;
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("PublishHRDMaster", Name = "PublishHRDMaster")]
        public ActionResult PublishHRDMaster()
        {
            Response res = new Response();
            try
            {
                DataSet ds = new DataSet();
                //EComModel.EID = IvapUser.EID;
                res = EComRepo.PublishHRDMaster(IvapUser.EID);
                
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                throw;
            }
        }
        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("PublishPayMaster", Name = "PublishPayMaster")]
        public ActionResult PublishPayMaster()
        {
            Response res = new Response();
            try
            {
                DataSet ds = new DataSet();
                //EComModel.EID = IvapUser.EID;
                res = EComRepo.PublishPayMaster(IvapUser.EID);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                throw;
            }
        }

        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("UpdateEntityComponent", Name = "UpdateEntityComponent")]
        public ActionResult UpdateEntityComponent(EntityComponentModel Model)
        {
            Response res = new Response();
            try
            {
                if (ModelState.IsValid)
                {
                    Model.EID = IvapUser.EID;
                Model.CreatedBy = IvapUser.UID;
                res = EComRepo.SetEntityComponent(Model);
                    if (res.IsSuccess && res.Data=="True")
                    {
                        FileSetupRepo ObjERepo = new FileSetupRepo();
                        DataTable dt = new DataTable();
                        dt = ObjERepo.GetFileComponent(0, IvapUser.EID, Model.EntityCOMPID,"CheckDTL");
                        res.Data = dt.Rows.Count > 0 ? dt.Rows[0]["TotalRecord"].ToString() : "";
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

        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("AddEntityComponent", Name = "AddEntityComponent")]
        public ActionResult AddEntityComponent(List<string> EntityCompID,string COMPONENT_FILE_TYPE)
        {
            EntityComponentModel EModel = new EntityComponentModel();
           // EModel.Globle_Component_ID = string.Join(",", EntityCompID);
            Response res = new Response();
            try
            {
              
                    EModel.CreatedBy = IvapUser.UID;
                    EModel.EID = IvapUser.EID;
                    EModel.File_Type = COMPONENT_FILE_TYPE;
                    EModel.Globle_Component_ID = EntityCompID;
                    res = EComRepo.SetEntityComponent(EModel);
                    return Json(res, JsonRequestBehavior.AllowGet);
              
            }
            catch
            {
                throw;
            }
        }
        #region DropDown
        [ViewAction]
        [HttpGet]
        [Route("getEntityCompFileType", Name = "getEntityCompFileType")]
        public ActionResult getEntityCompFileType(string COMPONENT_FILE_TYPE, string COMPONENT_TYPE)
        {
            Response res = new Response();
            try
            {
                GlobalComponentModel CModel = new GlobalComponentModel();
                GlobalComponentRepo CRepo = new GlobalComponentRepo();
                CModel.COMPONENT_FILE_TYPE = COMPONENT_FILE_TYPE;
                CModel.COMPONENT_TYPE = COMPONENT_TYPE;
                DataTable dt = CRepo.GetComponentFileType(CModel, "");
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
        [Route("getEntityCompTableName", Name = "getEntityCompTableName")]
        public ActionResult getEntityCompTableName(string TableName)
        {
            Response res = new Response();
            try
            {
                GlobalComponentRepo CRepo = new GlobalComponentRepo();
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
        #endregion EndDropDown
        #region FileSetUPChange

        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("ResetEntityComponent", Name = "ResetEntityComponent")]
        public ActionResult ResetEntityComponent(int EntityCompID)
        {
            Response res = new Response();
            try
            {
                EComModel.EntityCOMPID = EntityCompID;
                EComModel.EID = IvapUser.EID;
                res = EComRepo.ResetEntityComponent(EComModel);
            }
            catch
            {
                res.IsSuccess = false;
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        [ViewAction]
        [HttpGet]
        [Route("GetFileComponentDel", Name = "GetFileComponentDel")]
        public ActionResult GetFileComponentDel(int EntityCompID)
        {
            string Globle_Component_ID = string.Join(",", EntityCompID);
            try
            {
                Response Res = new Response();
                FileSetupRepo ObjERepo = new FileSetupRepo();
                DataTable dt = ObjERepo.GetFileComponent(0, IvapUser.EID, EntityCompID);
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
        [Route("getEntityComponentTableName", Name = "getEntityComponentTableName")]
        public ActionResult getEntityComponentTableName(string TableName)
        {
            Response res = new Response();
            try
            {
                GlobalComponentRepo CRepo = new GlobalComponentRepo();
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
        [Route("UpdateFile_ComponentDetails", Name = "UpdateFile_ComponentDetails")]
        public ActionResult UpdateFile_ComponentDetails(string FileCompDelID)
        {
            EntityComponentModel EModel = new EntityComponentModel();
            // EModel.Globle_Component_ID = string.Join(",", EntityCompID);
            Response res = new Response();
            try
            {
                EModel.CreatedBy = IvapUser.UID;
                res = EComRepo.UpdateFile_ComponentDetails(EModel, FileCompDelID);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                throw;
            }
        }
        #endregion FileSetUpChange
    }
}
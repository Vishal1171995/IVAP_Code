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
    public class LevelController : IvapBaseController
    {
        [ViewAction]
        [Route("Level", Name = "ViewLevel")]
        public ActionResult Level()
        {
            LevelModel objModel = new LevelModel();
            try
            {
                objModel.EID = IvapUser.EID;
                objModel.SetDisplayName();
                objModel.IsActive = true;
                return View("Level", objModel);
            }
            catch (Exception ex) { throw; }
        }
        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("AddUpdateLevel", Name = "AddUpdateLevel")]
        public ActionResult AddUpdateLevel(LevelModel Model)
        {
            LevelRepo objrepo = new LevelRepo();
            Response res = new Response();
            try
            {
                if (ModelState.IsValid)
                {
                    Model.CreatedBy = IvapUser.UID;
                    Model.EID = IvapUser.EID;
                    res = objrepo.AddUpdateLevel(Model);
                    return Json(res, JsonRequestBehavior.AllowGet);
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
        [Route("GetLevel", Name = "GetLevel")]
        public ActionResult GetLevel(int LevelID)
        {
            LevelRepo objUBO = new LevelRepo();
            LevelModel objModel = new LevelModel();

            KendoGridUtils res = new KendoGridUtils();
            try
            {
                objModel.LevelID = LevelID;//ViewLevel
                objModel.EID = IvapUser.EID;
                objModel.CreatedBy = IvapUser.UID;
                DataTable dt = objUBO.GetLevel(objModel);
                res = res.GetCommandButtonForGrid("ViewLevel");
                res.Data = JsonSerializer.SerializeTable(dt);
                var UserGrid = Json(res, JsonRequestBehavior.AllowGet);
                UserGrid.MaxJsonLength = int.MaxValue;
                return UserGrid;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [ViewAction]
        [HttpGet]
        [Route("GetLevelHistory", Name = "GetLevelHistory")]
        public ActionResult GetLevelHistory(int LevelID)
        {
            LevelRepo objRepo = new LevelRepo();
            LevelModel objModel = new LevelModel();
            Response ret = new Response();
            try
            {
                objModel.LevelID = LevelID;
                ret.Data = JsonSerializer.SerializeTable(objRepo.GetLevelHistory(objModel));
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
        [Route("DownloadAllLevel", Name = "DownloadAllLevel")]
        public ActionResult DownloadAllLevel()
        {
            LevelRepo objRepo = new LevelRepo();
            LevelModel objModel = new LevelModel();
            
            DataTable dt = new DataTable();
            try
            {
                objModel.EID = IvapUser.EID;
                objModel.SetDisplayName();
                objModel.CreatedBy = IvapUser.UID;
                objModel.LevelID = 0;
                dt = objRepo.GetLevel(objModel);
                DataTable dtComp = dt.DefaultView.ToTable(false,"ENTITY_NAME", "PAY_LEVEL_CODE", "ERP_LEVEL_CODE", "LEVEL_NAME", "STATUS");
                dtComp.Columns["ENTITY_NAME"].ColumnName = "ENTITY NAME";
                dtComp.Columns["PAY_LEVEL_CODE"].ColumnName = objModel.PAY_LEVEL_CODE_TEXT;
                dtComp.Columns["ERP_LEVEL_CODE"].ColumnName = objModel.ERP_LEVEL_CODE_TEXT;
                dtComp.Columns["LEVEL_NAME"].ColumnName = objModel.LEVEL_NAME_TEXT;
                dtComp.Columns["STATUS"].ColumnName = objModel.ISACTIVE_TEXT;

                string FileName = ExcellUtils.DataTableToExcel(dtComp);
                FileName = FileName.Replace("/", "").Replace("..", "").Replace("\\", "");
                string FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + FileName;
                byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "LevelMaster.xlsx");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

       [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("UploadLevelDetails", Name = "UploadLevelDetails")]
        public ActionResult UploadLevelDetails(string FileName)
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
                LevelRepo objRepo = new LevelRepo();
                string FilePath = Server.MapPath("~/Docs/Temp/" + FileName);
                int SuccessCount = 0;
                int FailCount = 0;
                int CreatedBy = IvapUser.UID;
                int EID = IvapUser.EID;
                string ResultFileName = objRepo.UploadLevelDetails(FilePath, CreatedBy, EID, ref SuccessCount, ref FailCount);
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
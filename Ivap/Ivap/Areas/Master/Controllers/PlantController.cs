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
    public class PlantController : IvapBaseController
    {
        [ViewAction]
        [Route("Plant", Name = "ViewPlant")]
        public ActionResult Plant()
        {
            PlantModel objModel = new PlantModel();
            objModel.EID = IvapUser.EID;
            objModel.SetDisplayName();
            objModel.IsActive = true;
            try
            {
                return View("Plant", objModel);
            }
            catch (Exception ex) { throw; }
        }
        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("AddUpdatePlant", Name = "AddUpdatePlant")]
        public ActionResult AddUpdatePlant(PlantModel Model)
        {
            PlantRepo objrepo = new PlantRepo();
            Response res = new Response();
            try
            {
                if (ModelState.IsValid)
                {
                    Model.CreatedBy = IvapUser.UID;
                    Model.EID = IvapUser.EID;
                    res = objrepo.AddUpdatePlant(Model);
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
        [Route("GetPlant", Name = "GetPlant")]
        public ActionResult GetPlant(int PlantID)
        {
            PlantRepo objUBO = new PlantRepo();
            PlantModel objModel = new PlantModel();

            KendoGridUtils res = new KendoGridUtils();
            try
            {
                objModel.PlantID = PlantID;//ViewPlant
                objModel.EID = IvapUser.EID;
                objModel.CreatedBy = IvapUser.UID;
                DataTable dt = objUBO.GetPlant(objModel);
                res = res.GetCommandButtonForGrid("ViewPlant");
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
        [Route("GetPlantHistory", Name = "GetPlantHistory")]
        public ActionResult GetPlantHistory(int PlantID)
        {
            PlantRepo objRepo = new PlantRepo();
            PlantModel objModel = new PlantModel();
            Response ret = new Response();
            try
            {
                objModel.PlantID = PlantID;
                ret.Data = JsonSerializer.SerializeTable(objRepo.GetPlantHistory(objModel));
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
        [Route("DownloadAllPlant", Name = "DownloadAllPlant")]
        public ActionResult DownloadAllPlant()
        {
            PlantRepo objRepo = new PlantRepo();
            PlantModel objModel = new PlantModel();
            DataTable dt = new DataTable();
            try
            {
                objModel.PlantID = 0;
                objModel.EID = IvapUser.EID;
                objModel.CreatedBy = IvapUser.UID;
                objModel.SetDisplayName();
                dt = objRepo.GetPlant(objModel);
                DataTable dtComp = dt.DefaultView.ToTable(false,"ENTITY_NAME", "PAY_PLANT_CODE", "ERP_PLANT_CODE", "PLANT_NAME", "STATUS");
                dtComp.Columns["ENTITY_NAME"].ColumnName = "ENTITY NAME";
                dtComp.Columns["PAY_PLANT_CODE"].ColumnName = objModel.PAY_PLANT_CODE_TEXT;
                dtComp.Columns["ERP_PLANT_CODE"].ColumnName = objModel.ERP_PLANT_CODE_TEXT;
                dtComp.Columns["PLANT_NAME"].ColumnName = objModel.PLANT_NAME_TEXT;
                dtComp.Columns["STATUS"].ColumnName = objModel.ISACTIVE_TEXT;

                string FileName = ExcellUtils.DataTableToExcel(dtComp);
                FileName = FileName.Replace("/", "").Replace("..", "").Replace("\\", "");
                string FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + FileName;
                byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "PlantMaster.xlsx");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("UploadPlantDetails", Name = "UploadPlantDetails")]
        public ActionResult UploadPlantDetails(string FileName)
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
                PlantRepo objRepo = new PlantRepo();
                string FilePath = Server.MapPath("~/Docs/Temp/" + FileName);
                int SuccessCount = 0;
                int FailCount = 0;
                int CreatedBy = IvapUser.UID;
                int EID = IvapUser.EID;
                string ResultFileName = objRepo.UploadPlantDetails(FilePath, CreatedBy, EID, ref SuccessCount, ref FailCount);
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
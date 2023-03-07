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
    public class RegionController : IvapBaseController
    {
        // GET: Master/Designation
        // GET: Master/Company
        [ViewAction]
        [Route("Region", Name = "ViewRegion")]
        public ActionResult Region()
        {
            RegionModel objRegModel = new RegionModel();
            
            try
            {
                objRegModel.EID = IvapUser.EID;
                objRegModel.SetDisplayName();
                objRegModel.IsActive = true;
                return View("Region", objRegModel);
            }
            catch (Exception ex) { throw; }
        }
        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("AddUpdateRegion", Name = "AddUpdateRegion")]
        public ActionResult AddUpdateRegion(RegionModel Model)
        {
            RegionRepo objrepo = new RegionRepo();
            Response res = new Response();
            try
            {
                if (ModelState.IsValid)
                {
                    Model.CreatedBy = IvapUser.UID;
                    Model.EID = IvapUser.EID;
                    res = objrepo.AddUpdateRegion(Model);
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
        [Route("GetRegion", Name = "GetRegion")]
        public ActionResult GetRegion(int RegionID)
        {
            RegionRepo objUBO = new RegionRepo();
            RegionModel objModel = new RegionModel();

            KendoGridUtils res = new KendoGridUtils();
            try
            {
                objModel.RegionID = RegionID;
                objModel.EID = IvapUser.EID;
                objModel.CreatedBy = IvapUser.UID;
                DataTable dt = objUBO.GetRegion(objModel);
                res = res.GetCommandButtonForGrid("ViewRegion");
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
        [Route("GetRegionHistory", Name = "GetRegionHistory")]
        public ActionResult GetRegionHistory(int RegionID)
        {
            RegionRepo objRepo = new RegionRepo();
            RegionModel objModel = new RegionModel();
            Response ret = new Response();
            try
            {
                objModel.RegionID = RegionID;
                ret.Data = JsonSerializer.SerializeTable(objRepo.GetRegionHistory(objModel));
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
        [Route("DownloadAllRegion", Name = "DownloadAllRegion")]
        public ActionResult DownloadAllRegion()
        {
            RegionRepo objRepo = new RegionRepo();
            RegionModel objModel = new RegionModel();
            
            DataTable dt = new DataTable();
            try
            {
                objModel.EID = IvapUser.EID;
                objModel.CreatedBy = IvapUser.UID;
                objModel.SetDisplayName();
                objModel.RegionID = 0;
                dt = objRepo.GetRegion(objModel);
                DataTable dtComp = dt.DefaultView.ToTable(false,"ENTITY_NAME", "PAY_REGION_CODE", "ERP_REGION_CODE", "REGION_NAME", "STATUS");
                dtComp.Columns["ENTITY_NAME"].ColumnName = "ENTITY NAME";
                dtComp.Columns["PAY_REGION_CODE"].ColumnName = objModel.PAY_REGION_CODE_TEXT;
                dtComp.Columns["ERP_REGION_CODE"].ColumnName = objModel.ERP_REGION_CODE_TEXT;
                dtComp.Columns["REGION_NAME"].ColumnName = objModel.REGION_NAME_TEXT;
                dtComp.Columns["STATUS"].ColumnName = objModel.ISACTIVE_TEXT;

                string FileName = ExcellUtils.DataTableToExcel(dtComp);
                FileName = FileName.Replace("/", "").Replace("..", "").Replace("\\", "");
                string FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + FileName;
                byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "RegionMaster.xlsx");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    
        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("UploadRegionDetails", Name = "UploadRegionDetails")]
        public ActionResult UploadRegionDetails(string FileName)
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
                RegionRepo objRepo = new RegionRepo();
                string FilePath = Server.MapPath("~/Docs/Temp/" + FileName);
                int SuccessCount = 0;
                int FailCount = 0;
                int CreatedBy = IvapUser.UID;
                int EID = IvapUser.EID;
                string ResultFileName = objRepo.UploadRegionDetails(FilePath, CreatedBy, EID, ref SuccessCount, ref FailCount);
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
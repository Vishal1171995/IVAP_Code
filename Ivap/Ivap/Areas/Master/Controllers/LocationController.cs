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
    public class LocationController : IvapBaseController
    {
       [ViewAction]
        [Route("Location", Name = "ViewLocation")]
        public ActionResult ViewLocation()
        {
            StateRepo sr = new StateRepo();
            LocationRepo lr = new LocationRepo();
            Location Model = new Location();
            CompanyRepo ComRepo = new CompanyRepo();
            CompanyModel ComModel = new CompanyModel();
            StateModel sm = new StateModel();
            try
            {
                Model.EID = IvapUser.EID;
                Model.SetDisplayName();
                ComModel.CompID = 0;
                ComModel.IsActive = true;
                Model.CompanyList = DropdownUtils.ToSelectList(ComRepo.GetCompany(ComModel), "TID", "COMP_NAME");
                Model.StateList = DropdownUtils.ToSelectList(sr.GetState(), "TID", "STATE_NAME");
                Model.IsActive = true;
            }
            catch(Exception ex)
            {
                throw;
            }
            return View("Location", Model);
        }

        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("AddUpdateLocation", Name = "AddUpdateLocation")]
        public ActionResult AddUpdate(Location Model)
        {
            LocationRepo objLocation = new LocationRepo();
            Response res = new Response();
            try
            {
                if (ModelState.IsValid)
                {
                    Model.CreatedBy = IvapUser.UID;
                    Model.EID = IvapUser.EID;
                    res = objLocation.AddUpdateLocation(Model);
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
        [Route("GetLocation", Name = "GetLocation")]
        public ActionResult GetLocation(int LocID)

        {
            LocationRepo LocationRepo = new LocationRepo();
            //KendoGridUtils
            Location model = new Location();
            DataTable dt = new DataTable();
            KendoGridUtils res = new KendoGridUtils();
            try
            {
                model.Location_Id = LocID;
                // model.RoleID = RoleID;ViewLocation
                model.EID = IvapUser.EID;
                model.CreatedBy = IvapUser.UID;
                dt = LocationRepo.GetLocation(model);
                res = res.GetCommandButtonForGrid("ViewLocation");
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
        [Route("GetLocationHis", Name = "GetLocationHis")]
        public ActionResult GetUserRoleHis(int LocID)
        {
            LocationRepo LocationRepo = new LocationRepo();
            Location model = new Location();
            DataTable dt = new DataTable();
            Response res = new Response();
            try
            {
                model.Location_Id = LocID;
                model.EID = IvapUser.EID;
                dt = LocationRepo.GetLoationHis(model);
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
        [Route("DownloadAllLocation", Name = "DownloadAllLocation")]
        public ActionResult DownloadAllLocation()
        {
            LocationRepo LocationRepo = new LocationRepo();
            Location model = new Location();
            
            DataTable dt = new DataTable();
            try
            {
                model.EID = IvapUser.EID;
                model.CreatedBy = IvapUser.UID;
                model.SetDisplayName();
                model.Erp_Loc_Code = "0";
                dt = LocationRepo.GetLocation(model);
                DataTable dtRole = dt.DefaultView.ToTable(false, "ENTITY_NAME", "ERP_LOC_CODE","PAY_LOC_CODE", "LOC_NAME", "STATE_NAME", "METRO", "STATUS");
                dtRole.Columns["ENTITY_NAME"].ColumnName = "ENTITY NAME";
                dtRole.Columns["ERP_LOC_CODE"].ColumnName = model.Erp_Loc_Code_TEXT;
                dtRole.Columns["PAY_LOC_CODE"].ColumnName = model.Pay_Loc_Code_TEXT;
                dtRole.Columns["LOC_NAME"].ColumnName = model.Location_Name_TEXT;
                dtRole.Columns["STATE_NAME"].ColumnName = "State Name";
                dtRole.Columns["METRO"].ColumnName = model.Is_Metro_TEXT;
                dtRole.Columns["STATUS"].ColumnName = "IsActive";
                string FileName = ExcellUtils.DataTableToExcel(dtRole);
                FileName = FileName.Replace("/", "").Replace("..", "").Replace("\\", "");
                string FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + FileName;
                byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "LocationMaster.xlsx");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [ViewAction]
        [HttpGet]
        [Route("GetLocationName", Name = "GetLocationName")]
        public ActionResult GetLocationName(int StateID,string searchStr)
        {
            LocationRepo LocationRepo = new LocationRepo();
            Location model = new Location();
            Response res = new Response();
            DataTable dt = new DataTable();
            try
            {
                model.State_Id = StateID;
                model.Location_Name = searchStr;
                dt = LocationRepo.GetLoationName(model);
                res.IsSuccess = true;
                res.Data = JsonSerializer.SerializeTable(dt);
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
        [Route("UploadLocationDetails", Name = "UploadLocationDetails")]
        public ActionResult UploadLocationDetails(string FileName)
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
                LocationRepo objRepo = new LocationRepo();
                string FilePath = Server.MapPath("~/Docs/Temp/" + FileName);
                int SuccessCount = 0;
                int FailCount = 0;
                int CreatedBy = IvapUser.UID;
                int EID = IvapUser.EID;
                string ResultFileName = objRepo.UploadLocationDetails(FilePath, CreatedBy, EID, ref SuccessCount, ref FailCount);
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
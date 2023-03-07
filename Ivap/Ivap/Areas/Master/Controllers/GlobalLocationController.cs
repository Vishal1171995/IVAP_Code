using Ivap.ActionFilters;
using Ivap.Areas.Master.Models;
using Ivap.Areas.Master.Repository;
using Ivap.Controllers;
using Ivap.CustomAttribute;
using Ivap.Utils;
using IVap.Areas.Master.Repository;
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
    public class GlobalLocationController : IvapBaseController
    {
        GlobalLocationRepo GlobalRepo = new GlobalLocationRepo();
        [ViewAction]
        [Route("GlobalLocation", Name = "ViewGlobalLocation")]
        public ActionResult ViewGlobalLocation()
        {
            StateRepo ObjStateRepo = new StateRepo();
            StateModel sm = new StateModel();
            GlobalLocationModel Model = new GlobalLocationModel();
            
            StateModel objState = new StateModel();
            try
            {
                Model.EID = IvapUser.EID;
                Model.StateList = DropdownUtils.ToSelectList(ObjStateRepo.GetState(), "TID", "STATE_NAME");
                Model.IsActive = true;
            }
            catch (Exception ex)
            {
                throw;
            }
            return View("GlobalLocation", Model);
        }

        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("AddUpdateGlobalLocation", Name = "AddUpdateGlobalLocation")]
        public ActionResult AddUpdate(GlobalLocationModel Model)
        {
            GlobalLocationRepo objLocation = new GlobalLocationRepo();
            Response res = new Response();
            try
            {
                if (ModelState.IsValid)
                {
                    Model.CreatedBy = IvapUser.UID;
                    Model.EID = IvapUser.EID;
                    res = objLocation.AddUpdateGlobalLocation(Model);
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
        [HttpPost]
        [Route("GetGridGLocation", Name = "GetGridGLocation")]
        public ActionResult GetGridGLocation(int page, int pageSize, int skip, int take, List<SortDescription> sorting, FilterContainer filter)
        {
            DataSet Ds = new DataSet();
            GridGLocation gLocation = new GridGLocation();
            Response ret = new Utils.Response();
            try
            {
                int from = skip+1;  
                int to = take*page;  
                string sortingStr = "";
                #region Sorting
                if (sorting != null)
                {
                    sortingStr = GlobalRepo.sortGrid(sorting);
                }
                #endregion
                #region filtering
                string filters = "";
                if (filter != null)
                {
                    filters = GlobalRepo.FilterGrid(filter);
                }
                #endregion
                sortingStr = sortingStr.TrimStart(',');
                if (sortingStr == "") sortingStr = null;
                if (filters == "") filters = null;
                gLocation.from = from;
                gLocation.To = to;
                gLocation.FilterStr = filters;
                gLocation.SortingStr = sortingStr;
                Ds = GlobalRepo.GetGridGlobalLocation(gLocation);
                string data = JsonSerializer.SerializeTable(Ds.Tables[0]);
                ret.IsSuccess = true;
                ret.Data = "{\"Data\":" + data + ",\"Total\":" + Ds.Tables[1].Rows[0]["TotalCount"] + "}";
                ret.Message = "success";
            }
            catch
            {
                ret.IsSuccess = true;
                ret.Data = "{\"Data\":[],\"Total\":" + 0 + "}";
            }
            var jsonResult = Json(ret, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [ViewAction]
        [HttpGet]
        [Route("GetGlobalLocation", Name = "GetGlobalLocation")]
        public ActionResult GetGlobalLocation(int LID)

        {
            GlobalLocationRepo LocationRepo = new GlobalLocationRepo();
            //KendoGridUtils
            GlobalLocationModel model = new GlobalLocationModel();
            DataTable dt = new DataTable();
            KendoGridUtils res = new KendoGridUtils();
            try
            {
                model.TID = LID;
                // model.RoleID = RoleID;
                dt = LocationRepo.GetGlobalLocation(model);
                //res = res.GetCommandButtonForGrid("ViewRoles");
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
        [Route("GetGlobalLocationHis", Name = "GetGlobalLocationHis")]
        public ActionResult GetGlobalLocationHis(int LID)
        {
            GlobalLocationRepo LocationRepo = new GlobalLocationRepo();
            GlobalLocationModel model = new GlobalLocationModel();
            DataTable dt = new DataTable();
            Response res = new Response();
            try
            {
                model.TID = LID;
                dt = LocationRepo.GetGlobalLoationHis(model);
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
        [Route("DownloadAllGlobalLocation", Name = "DownloadAllGlobalLocation")]
        public ActionResult DownloadAllGlobalLocation()
        {
            GlobalLocationRepo LocationRepo = new GlobalLocationRepo();
            GlobalLocationModel model = new GlobalLocationModel();
            DataTable dt = new DataTable();
            try
            {
                model.TID = 0;
                dt = LocationRepo.GetGlobalLocation(model);
                DataTable dtRole = dt.DefaultView.ToTable(false, "LOC_CODE","LOC_NAME", "STATE_NAME", "METRO", "STATUS");
                dtRole.Columns["LOC_NAME"].ColumnName = "Location Name";
                dtRole.Columns["LOC_CODE"].ColumnName ="Location Code";
                dtRole.Columns["STATE_NAME"].ColumnName = "State Name";
                dtRole.Columns["METRO"].ColumnName = "Is Metro";
                dtRole.Columns["STATUS"].ColumnName = "IsActive";
                string FileName = ExcellUtils.DataTableToExcel(dtRole);
                FileName = FileName.Replace("/", "").Replace("..", "").Replace("\\", "");
                string FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + FileName;
                byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "GlobalLocationMaster.xlsx");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
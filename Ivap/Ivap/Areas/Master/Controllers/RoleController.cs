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
    public class RoleController : IvapBaseController
    {
        // GET: Master/Role
        [ViewAction]
        [Route("Roles", Name = "ViewRoles")]
        public ActionResult ViewRoles()
        {
            EntityModel EntModel = new EntityModel();
            RoleModel RolModel = new RoleModel();
            EntityRepo EntRep = new EntityRepo();
            RoleRepo RolRep = new RoleRepo();
            try
            {
                RolModel.EID = IvapUser.EID;
                RolModel.SetDisplayName();
                EntModel.EID = 0;
                EntModel.IsActive = true;
                RolModel.EntityList = DropdownUtils.ToSelectList(EntRep.GetEntity(EntModel), "TID", "ENTITY_NAME");
                RolModel.IsActive = true;
                return View("Roles", RolModel);
            }
            catch
            {
                throw;
            }
        }
        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("AddUpdate", Name = "AddUpdate")]
        public ActionResult AddUpdate(RoleModel Model)
        {
            RoleRepo objRepo = new RoleRepo();
            Response res = new Response();
            try
            {
                if (ModelState.IsValid)
                {
                    Model.CreatedBy = IvapUser.UID;
                    res = objRepo.AddUpdateRole(Model);
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
        [Route("GetGridRoles", Name = "GetGridRoles")]
        public ActionResult GetGridRoles(int page, int pageSize, int skip, int take, List<SortDescription> sorting, FilterContainer filter)
        {
            RoleRepo RRepo = new RoleRepo();
            DataSet Ds = new DataSet();
            GridRoles gRoles = new GridRoles();
            Response ret = new Utils.Response();
            try
            {
                int from = skip + 1; //(page - 1) * pageSize + 1;
                int to = take * page; // page * pageSize;
                string sortingStr = "";
                #region Sorting
                if (sorting != null)
                {
                    sortingStr = RRepo.sortGrid(sorting);
                }
                #endregion
                #region filtering
                string filters = "";
                if (filter != null)
                {
                    filters = RRepo.FilterGrid(filter);
                }
                #endregion
                sortingStr = sortingStr.TrimStart(',');
                if (sortingStr == "") sortingStr = null;
                if (filters == "") filters = null;
                gRoles.from = from;
                gRoles.To = to;
                gRoles.FilterStr = filters;
                gRoles.SortingStr = sortingStr;
                Ds = RRepo.GetGridRoles(gRoles);
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
        [Route("GetRoles", Name = "GetRoles")]
        public ActionResult GetRoles(int RoleID)
        {
            RoleRepo RRepo = new RoleRepo();
            //KendoGridUtils
            RoleModel model = new RoleModel();
            DataTable dt = new DataTable();
            KendoGridUtils res = new KendoGridUtils();
            try
            {
                model.RoleID = RoleID;
                dt = RRepo.GetRoles(model);
                res = res.GetCommandButtonForGrid("ViewRoles");
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
        [Route("GetUserRoleHis", Name = "GetUserRoleHis")]
        public ActionResult GetUserRoleHis(int RoleID)
        {
            RoleRepo RRepo = new RoleRepo();
            RoleModel model = new RoleModel();
            DataTable dt = new DataTable();
            Response res = new Response();
            try
            {
                model.RoleID = RoleID;
                dt = RRepo.GetUserRoleHis(model);
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
        [Route("DownloadAllRole", Name = "DownloadAllRole")]
        public ActionResult DownloadAllRole()
        {
            RoleRepo RRepo = new RoleRepo();
            RoleModel model = new RoleModel();
            DataTable dt = new DataTable();
            try
            {
                model.RoleID = 0;
                dt = RRepo.GetRoles(model);
                DataTable dtRole = dt.DefaultView.ToTable(false, "TID", "RoleName", "RoleType", "Status");
                //dtRole.Columns["ENTITY_NAME"].ColumnName = "ENTITY_NAME";
                dtRole.Columns["RoleName"].ColumnName = "RoleName";
                dtRole.Columns["RoleType"].ColumnName = "RoleType";
                dtRole.Columns["Status"].ColumnName = "Status";
                string FileName = ExcellUtils.DataTableToExcel(dtRole);
                FileName = FileName.Replace("/", "").Replace("..", "").Replace("\\", "");
                string FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + FileName;
                byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "RoleMaster.xlsx");
            }
            catch(Exception ex)
            {
                throw;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using Ivap.Areas.Master.Models;
using Ivap.Areas.Master.Repository;
using Ivap.Utils;
using Ivap.ActionFilters;
using Ivap.Controllers;
using Ivap.CustomAttribute;

namespace Ivap.Areas.Master.Controllers
{
    [CustomAuthActionFilter(Order = 0)]
    [CustomAuthrizationActionFilter(Order = 1)]
    [RouteArea("Master", AreaPrefix = "")]
    [RoutePrefix("Masters")]
    public class DepartmentController : IvapBaseController
    {
        // GET: Master/Department
        DepartmentRepo DeptRepo = new DepartmentRepo();
        DepartmentModel DeptModel = new DepartmentModel();
        [ViewAction]
        [Route("Department", Name = "ViewDepartment")]
        public ActionResult ViewDepartment()
        {
            CompanyModel ComModel = new CompanyModel();
            //DepartmentModel DeptModel = new DepartmentModel();
            CompanyRepo CompRep = new CompanyRepo();
            DeptModel.EID = IvapUser.EID;
            DeptModel.SetDisplayName();
            try
            {
                ComModel.CompID = 0;
                ComModel.IsActive = true;
                DeptModel.CompanyList = DropdownUtils.ToSelectList(CompRep.GetCompany(ComModel), "TID", "COMP_NAME");
                DeptModel.IsActive = true;
                return View("Department", DeptModel);
            }
            catch
            {
                throw;
            }
        }
        [ViewAction]
        [HttpPost]
        [Route("getDeptGrid", Name = "getDeptGrid")]
        public ActionResult getDeptGrid(int page, int pageSize, int skip, int take, List<SortDescription> sorting, FilterContainer filter)
        {
            DataSet Ds = new DataSet();
            GridCostCentre gCost = new GridCostCentre();
            Response ret = new Utils.Response();
            // CostCentreRepo CRepo = new CostCentreRepo();
            try
            {
                int from = Convert.ToInt32(skip);
                int to = Convert.ToInt32(take);
                string sortingStr = "";
                #region Sorting
                if (sorting != null)
                {
                    //sortingStr = objUBO.sortGrid(sorting);
                }
                #endregion
                #region filtering
                string filters = "";
                if (filter != null)
                {
                    //filters = objUBO.FilterGrid(filter);
                }
                #endregion
                sortingStr = sortingStr.TrimStart(',');
                if (sortingStr == "") sortingStr = null;
                if (filters == "") filters = null;
                gCost.from = from;
                gCost.To = to;
                gCost.FilterStr = filters;
                gCost.SortingStr = sortingStr;
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
        [Route("getDepartment", Name = "getDepartment")]
        public ActionResult getDepartment(int DeptID)
        {
            //DepartmentRepo DeptRepo = new DepartmentRepo();
            //DepartmentModel DeptModel = new DepartmentModel();ViewDepartment
            DeptModel.ENTITY_ID = IvapUser.EID;
            DeptModel.DEPTID = Convert.ToInt32(DeptID);
            DeptModel.CreatedBy = IvapUser.UID;
            KendoGridUtils res = new KendoGridUtils();
            try
            {
                DataTable dt = DeptRepo.GetDepartment(DeptModel);
                res = res.GetCommandButtonForGrid("ViewDepartment");
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
        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("AddUpdateDepartment", Name = "AddUpdateDepartment")]
        public ActionResult AddUpdateDepartment(DepartmentModel Model)
        {
            Response res = new Response();
            try
            {
                Model.CreatedBy = IvapUser.UID;
                Model.ENTITY_ID = IvapUser.EID;
                Model.EID = IvapUser.EID;
                res = DeptRepo.AddUpdateDepartment(Model);
                return Json(res, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                throw;
            }
        }
        [ViewAction]
        [HttpGet]
        [Route("GetDeptHistory", Name = "GetDeptHistory")]
        public ActionResult GetDeptHistory(int DeptID)
        {
            //DepartmentRepo DeptRepo = new DepartmentRepo();
            //DepartmentModel DeptModel = new DepartmentModel();
            Response ret = new Response();
            try
            {
                DeptModel.DEPTID = Convert.ToInt32(DeptID);
                ret.Data = JsonSerializer.SerializeTable(DeptRepo.GetDeptHistory(DeptModel));
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
        [Route("ExportDepartment", Name = "ExportDepartment")]
        public ActionResult ExportDepartment(string DeptID)
        {
            Response ret = new Response();
            try
            {
                DeptModel.ENTITY_ID = IvapUser.EID;
                DeptModel.CreatedBy = IvapUser.UID;
                DataTable dt = DeptRepo.GetDepartment(DeptModel);
                DeptModel.EID = IvapUser.EID;
                DeptModel.SetDisplayName();
                DataTable dtDept = dt.DefaultView.ToTable(false, "PAY_DEPT_CODE", "ERP_DEPT_CODE", "DEPT_NAME", "ENTITY_NAME", "STATUS");
                dtDept.Columns["PAY_DEPT_CODE"].ColumnName = DeptModel.PAY_DEPT_CODE_TEXT;
                dtDept.Columns["ERP_DEPT_CODE"].ColumnName = DeptModel.ERP_DEPT_CODE_TEXT;
                dtDept.Columns["DEPT_NAME"].ColumnName = DeptModel.DEPT_NAME_TEXT;
                dtDept.Columns["ENTITY_NAME"].ColumnName = DeptModel.ENTITY_ID_TEXT;
                dtDept.Columns["STATUS"].ColumnName = DeptModel.ISACTIVE_TEXT;

                string FileName = ExcellUtils.DataTableToExcel(dtDept);
                FileName = FileName.Replace("/", "").Replace("..", "").Replace("\\", "");
                string FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + FileName;
                byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "Department.xlsx");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("UploadDepartmentDetails", Name = "UploadDepartmentDetails")]
        public ActionResult UploadDepartmentDetails(string FileName)
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
                DepartmentRepo objRepo = new DepartmentRepo();
                string FilePath = Server.MapPath("~/Docs/Temp/" + FileName);
                int SuccessCount = 0;
                int FailCount = 0;
                int CreatedBy = IvapUser.UID;
                int EID = IvapUser.EID;
                string ResultFileName = objRepo.UploadDepartmentDetails(FilePath, CreatedBy, EID, ref SuccessCount, ref FailCount);
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
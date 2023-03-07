using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ivap.Areas.Master.Models;
using Ivap.Areas.Master.Repository;
using System.Data;
using System.Web.Hosting;
using Ivap.Controllers;
using Ivap.ActionFilters;
using Ivap.CustomAttribute;

namespace Ivap.Areas.Master.Controllers
{
    [CustomAuthActionFilter(Order = 0)]
    [CustomAuthrizationActionFilter(Order = 1)]
    [RouteArea("Master", AreaPrefix = "")]
    [RoutePrefix("Masters")]
    public class CostCentreController : IvapBaseController
    {
        CostCentreRepo CRepo = new CostCentreRepo();
        CostCentreModel CostModel = new CostCentreModel();
        // GET: Master/CostCenter
        [ViewAction]
        [Route("CostCentre", Name = "ViewCostCentre")]
        public ActionResult ViewCostCentre()
        {
            CompanyModel ComModel = new CompanyModel();
            //CostCentreModel  CostModel = new CostCentreModel();
            CompanyRepo CompRep = new CompanyRepo();
            try
            {
                CostModel.EID = IvapUser.EID;
                CostModel.SetDisplayName();
                ComModel.CompID = 0;
                ComModel.IsActive = true;
                CostModel.CompanyList = DropdownUtils.ToSelectList(CompRep.GetCompany(ComModel), "TID", "COMP_NAME");
                CostModel.IsActive = true;
                return View("CostCentre", CostModel);
            }
            catch
            {
                throw;
            }
        }
        [ViewAction]
        [HttpPost]
        [Route("getCostCENGrid", Name = "getCostCENGrid")]
        public ActionResult getCostCENGrid(int page, int pageSize, int skip, int take, List<SortDescription> sorting, FilterContainer filter)
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
        [Route("getCostCentre", Name = "getCostCentre")]
        public ActionResult getCostCentre(int CostCentreID)
        {
            //CostCentreRepo objCost = new CostCentreRepo();
            //CostCentreModel objModel = new CostCentreModel();ViewCostCentre
            CostModel.CostCenterID = Convert.ToInt32(CostCentreID);
            CostModel.ENTITY_ID = IvapUser.EID;
            CostModel.CreatedBy = IvapUser.UID;
            KendoGridUtils res = new KendoGridUtils();
            try
            {
                DataTable dt = CRepo.GetCostCentre(CostModel);
                res = res.GetCommandButtonForGrid("ViewCostCentre");
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
        [Route("AddUpdateCostCEN", Name = "AddUpdateCostCEN")]
        public ActionResult AddUpdateCostCEN(CostCentreModel Model)
        {
            CostCentreRepo objrepo = new CostCentreRepo();
            Response res = new Response();
            try
            {
                Model.CreatedBy = IvapUser.UID;
                Model.EID = IvapUser.EID;
                res = objrepo.AddUpdateCostCentre(Model);
                return Json(res, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                throw;
            }
        }
        [ViewAction]
        [HttpGet]
        [Route("GetCostCenHistory", Name = "GetCostCenHistory")]
        public ActionResult GetCostCenHistory(int CostCentreID)
        {
           // CostCentreRepo objRepo = new CostCentreRepo();
            //CostCentreModel objModel = new CostCentreModel();
            Response ret = new Response();
            try
            {
                CostModel.CostCenterID = Convert.ToInt32(CostCentreID);
                ret.Data = JsonSerializer.SerializeTable(CRepo.GetCostCenHistory(CostModel));
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
        [Route("ExportCostCentre", Name = "ExportCostCentre")]
        public ActionResult ExportCostCentre(string CostCentreID)
        {
            Response ret = new Response();
            try
            {
                CostModel.ENTITY_ID = IvapUser.EID;
                CostModel.CreatedBy = IvapUser.UID;
                CostModel.EID= IvapUser.EID;
                CostModel.SetDisplayName();
                DataTable dt = CRepo.GetCostCentre(CostModel);
                DataTable dtCostC = dt.DefaultView.ToTable(false,"ENTITY_NAME" , "PAY_COST_CODE", "ERP_COST_CODE", "COST_NAME", "STATUS");
                dtCostC.Columns["ENTITY_NAME"].ColumnName = "Entity Name";
                dtCostC.Columns["PAY_COST_CODE"].ColumnName = CostModel.PAY_COST_CODE_TEXT;
                dtCostC.Columns["ERP_COST_CODE"].ColumnName = CostModel.ERP_COST_CODE_TEXT;
                dtCostC.Columns["COST_NAME"].ColumnName = CostModel.COST_NAME_TEXT;
                dtCostC.Columns["STATUS"].ColumnName = CostModel.ISACTIVE_TEXT;

                string FileName = ExcellUtils.DataTableToExcel(dtCostC);
                FileName = FileName.Replace("/", "").Replace("..", "").Replace("\\", "");
                string FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + FileName;
                byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "CostCentre.xlsx");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("UploadCostCenterDetails", Name = "UploadCostCenterDetails")]
        public ActionResult UploadCostCenterDetails(string FileName)
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
                CostCentreRepo objRepo = new CostCentreRepo();
                string FilePath = Server.MapPath("~/Docs/Temp/" + FileName);
                int SuccessCount = 0;
                int FailCount = 0;
                int CreatedBy = IvapUser.UID;
                int EID = IvapUser.EID;
                string ResultFileName = objRepo.UploadCostCenterDetails(FilePath, CreatedBy, EID, ref SuccessCount, ref FailCount);
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
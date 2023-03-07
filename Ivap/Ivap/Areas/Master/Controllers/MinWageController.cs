using Ivap.ActionFilters;
using Ivap.Areas.Master.Models;
using Ivap.Areas.Master.Repository;
using Ivap.Controllers;
using Ivap.CustomAttribute;
using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    public class MinWageController : IvapBaseController
    {
        [ViewAction]
        [Route("MinWage", Name = "ViewMinWage")]
        public ActionResult MinWage()
        {
            MinWageModel objMWageModel = new MinWageModel();
            StateRepo objSRepo = new StateRepo();
            
            try
            {
                objMWageModel.EID = IvapUser.EID;
                objMWageModel.SetDisplayName();
                objMWageModel.StateList = DropdownUtils.ToSelectList(objSRepo.GetState(), "TID", "STATE_NAME");
                objMWageModel.IsActive = true;
                //objMWageModel.LocationList = DropdownUtils.ToSelectList(objLocRepo.GetLocation(objLocModel), "TID", "LOC_NAME");
                return View("MinWage", objMWageModel);
            }
            catch (Exception ex) { throw; }
        }
        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("AddUpdateMinWage", Name = "AddUpdateMinWage")]
        public ActionResult AddUpdateMinWage(MinWageModel Model)
        {
            MinWageRepo objrepo = new MinWageRepo();
            Response res = new Response();
            try
            {
                if (ModelState.IsValid)
                {
                    Model.CreatedBy = IvapUser.UID;

                    res = objrepo.AddUpdateMinWage(Model);
                    return Json(res, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var results = new List<ValidationResult>();
                    var vc = new ValidationContext(Model, null, null);
                    var isValid = Validator.TryValidateObject(Model, vc, results, true);
                    var errors = Array.ConvertAll(results.ToArray(), o => o.ErrorMessage);
                    res.IsSuccess = false;
                    res.Message = string.Join(" ", errors);
                    res.Data = string.Join(" ", errors);
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
        [Route("GetMinWage", Name = "GetMinWage")]
        public ActionResult GetMinWage(int MinWageID)
        {
            MinWageRepo objRepo = new MinWageRepo();
            MinWageModel objModel = new MinWageModel();

            KendoGridUtils res = new KendoGridUtils();
            try
            {
                objModel.MinWageID = MinWageID;
                DataTable dt = objRepo.GetMinWage(objModel);
                res = res.GetCommandButtonForGrid("ViewMinWage");
                res.Data = JsonSerializer.SerializeTable(dt);
                //return Json(res, JsonRequestBehavior.AllowGet);
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
        [Route("GetMinWageHistory", Name = "GetMinWageHistory")]
        public ActionResult GetMinWageHistory(int MinWageID)
        {
            MinWageRepo objRepo = new MinWageRepo();
            MinWageModel objModel = new MinWageModel();
            Response ret = new Response();
            try
            {
                objModel.MinWageID = MinWageID;
                ret.Data = JsonSerializer.SerializeTable(objRepo.GetMinWageHistory(objModel));
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
        [Route("DownloadAllMinWage", Name = "DownloadAllMinWage")]
        public ActionResult DownloadAllMinWage()
        {
            MinWageRepo objRepo = new MinWageRepo();
            MinWageModel objModel = new MinWageModel();
            DataTable dt = new DataTable();
            try
            {
                objModel.EID = IvapUser.EID;
                objModel.SetDisplayName();
                objModel.MinWageID = 0;
                dt = objRepo.GetMinWage(objModel);
                DataTable dtComp = dt.DefaultView.ToTable(false, "TID", "STATE_NAME", "CATEGORY", "MIN_WAGE", "EFF_DT_FROM", "EFF_DATE_TO", "Status");
                dtComp.Columns["STATE_NAME"].ColumnName = "STATE_NAME";
                dtComp.Columns["CATEGORY"].ColumnName = "Category";
                dtComp.Columns["MIN_WAGE"].ColumnName = "Min Wage";
                dtComp.Columns["EFF_DT_FROM"].ColumnName = "EFF DT From";
                dtComp.Columns["EFF_DATE_TO"].ColumnName = "EFF DT TO";
                dtComp.Columns["Status"].ColumnName = "Status";

                string FileName = ExcellUtils.DataTableToExcel(dtComp);
                FileName = FileName.Replace("/", "").Replace("..", "").Replace("\\", "");
                string FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + FileName;
                byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "MinWageMaster.xlsx");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
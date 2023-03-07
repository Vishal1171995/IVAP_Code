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
    public class PTAXController : IvapBaseController
    {
        PTaxRepo PTaxRepo = new PTaxRepo();
        PTAXModel PTaxModel = new PTAXModel();

        [ViewAction]
        [Route("PTAX", Name = "ViewPTAX")]
        public ActionResult ViewPTAX()
        {
            StateRepo StateRep = new StateRepo();
            try
            {
                PTaxModel.EID = IvapUser.EID;
                PTaxModel.SetDisplayName();
                PTaxModel.StateList = DropdownUtils.ToSelectList(StateRep.GetState(), "TID", "STATE_NAME");
                PTaxModel.DED_MONTHList = DropdownUtils.ToSelectList(PTaxRepo.MonthDropDown(), "TID", "MonthName");
                PTaxModel.YTD_MONTH_FROMList = DropdownUtils.ToSelectList(PTaxRepo.MonthDropDown(), "TID", "MonthName");
                PTaxModel.YTD_MONTH_TOList = DropdownUtils.ToSelectList(PTaxRepo.MonthDropDown(), "TID", "MonthName");
                PTaxModel.EFF_FROM_DT = System.DateTime.Now.Date;
                PTaxModel.EFF_TO_DT = System.DateTime.Now.Date;
                PTaxModel.IsActive = true;
                return View("PTAX", PTaxModel);
            }
            catch
            {
                throw;
            }
        }
        [ViewAction]
        [HttpGet]
        [Route("getPTax", Name = "getPTax")]
        public ActionResult getPTax(int PTaxID)
        {
            //PTaxRepo PTaxRepo = new PTaxRepo();
            //PTAXModel PTaxModel = new PTAXModel();
            PTaxModel.PTAXID = Convert.ToInt32(PTaxID);
            KendoGridUtils res = new KendoGridUtils();
            try
            {
                DataTable dt = PTaxRepo.GetPTax(PTaxModel);
                res = res.GetCommandButtonForGrid("ViewPTAX");
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
        [Route("AddUpdatePTAX", Name = "AddUpdatePTAX")]
        public ActionResult AddUpdatePTAX(PTAXModel Model)
        {
            Response res = new Response();
            try
            {
                if (ModelState.IsValid)
                {
                    Model.CreatedBy = IvapUser.UID;
                    res = PTaxRepo.AddUpdatePTax(Model);
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
                    res.Data= string.Join(" ", errors);
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
        [Route("GetPTaxHistory", Name = "GetPTaxHistory")]
        public ActionResult GetPTaxHistory(int PTaxID)
        {
            Response ret = new Response();
            try
            {
                PTaxModel.PTAXID = Convert.ToInt32(PTaxID);
                ret.Data = JsonSerializer.SerializeTable(PTaxRepo.GetPTaxHistory(PTaxModel));
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
        [Route("ExportALLPTAX", Name = "ExportALLPTAX")]
        public ActionResult ExportALLPTAX()
        {
            PTaxRepo PTaxRepo = new PTaxRepo();
            PTAXModel PTaxModel = new PTAXModel();
            
            DataTable dt = new DataTable();
            try
            {
                PTaxModel.EID = IvapUser.EID;
                PTaxModel.SetDisplayName();
                PTaxModel.PTAXID = 0;
                dt = PTaxRepo.GetPTax(PTaxModel);
                DataTable dtComp = dt.DefaultView.ToTable(false, "TID", "STATE_NAME", "DED_MONTHNAME", "YTD_MONTH_FROMNAME", "YTD_MONTH_TOName", "EFF_FROM_DT", "EFF_TO_DT", "PT_SAL_TO", "PT_SAL_FROM", "PERIOD_FLAG", "GENDER", "PTAX");
                dtComp.Columns["STATE_NAME"].ColumnName = "STATE_NAME";
                dtComp.Columns["DED_MONTHNAME"].ColumnName = "DED_MONTH";
                dtComp.Columns["YTD_MONTH_FROMNAME"].ColumnName = "YTD_MONTH_FROM";
                dtComp.Columns["YTD_MONTH_TOName"].ColumnName = "YTD_MONTH_TO";
                dtComp.Columns["EFF_FROM_DT"].ColumnName = "EFF_FROM_DT";
                dtComp.Columns["EFF_TO_DT"].ColumnName = "EFF_TO_DT";
                dtComp.Columns["PT_SAL_TO"].ColumnName = "PT_SAL_TO";
                dtComp.Columns["PT_SAL_FROM"].ColumnName = "PT_SAL_FROM";
                dtComp.Columns["PERIOD_FLAG"].ColumnName = "PERIOD_FLAG";
                dtComp.Columns["GENDER"].ColumnName = "GENDER";
                dtComp.Columns["PTAX"].ColumnName = "PTAX";

                string FileName = ExcellUtils.DataTableToExcel(dtComp);
                FileName = FileName.Replace("/", "").Replace("..", "").Replace("\\", "");
                string FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + FileName;
                byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "PTAXMaster.xlsx");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("UploadPTAXDetails", Name = "UploadPTAXDetails")]
        public ActionResult UploadPTAXDetails(string FileName)
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
                PTaxRepo objRepo = new PTaxRepo();
                string FilePath = Server.MapPath("~/Docs/Temp/" + FileName);
                int SuccessCount = 0;
                int FailCount = 0;
                int CreatedBy = IvapUser.UID;
                int EID = IvapUser.EID;
                string ResultFileName = objRepo.UploadPTAXDetails(FilePath, CreatedBy, EID, ref SuccessCount, ref FailCount);
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
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
    public class CompanyController : IvapBaseController
    {
        // GET: Master/Company
        [ViewAction]
        [Route("Company", Name = "ViewCompany")]
        public ActionResult Company()
        {
            ClassModel clModel = new ClassModel();
            CompanyModel compModel = new CompanyModel();
            StateRepo sRepo = new StateRepo();
            ClassRepo clRepo = new ClassRepo();
            try
            {
                compModel.EID = IvapUser.EID;
                compModel.SetDisplayName();
                compModel.StateList = DropdownUtils.ToSelectList(sRepo.GetState(), "TID", "STATE_NAME");
                clModel.CID = 0;
                clModel.IsActive = true;
                compModel.ClassList = DropdownUtils.ToSelectList(clRepo.GetClass(clModel), "TID", "CLASS_NAME");
                compModel.IsActive = true;
                return View("Company", compModel);
            }
            catch(Exception ex) { throw; }
        }
        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("AddUpdateCompany", Name = "AddUpdateCompany")]
        public ActionResult AddUpdateCompany(CompanyModel Model)
        {
            CompanyRepo objrepo = new CompanyRepo();
            Response res = new Response();
            try
            {
                if (ModelState.IsValid)
                {
                    Model.CreatedBy = IvapUser.UID;
                    Model.EID = IvapUser.EID; 
                    res = objrepo.AddUpdateCompany(Model);
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
        [Route("GetCompany", Name = "GetCompany")]
        public ActionResult GetCompany(int CompID)
        {
            CompanyRepo objUBO = new CompanyRepo();
            CompanyModel objModel = new CompanyModel();
            
            KendoGridUtils res = new KendoGridUtils();
            try
            {
                objModel.CompID = CompID;
                objModel.EID = IvapUser.EID;
                objModel.CreatedBy = IvapUser.UID;
                DataTable dt = objUBO.GetCompany(objModel);
                res = res.GetCommandButtonForGrid("ViewCompany");
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
        [Route("GetCompanyHistory", Name = "GetCompanyHistory")]
        public ActionResult GetCompanyHistory(int CompID)
        {
            CompanyRepo objUBO = new CompanyRepo();
            CompanyModel objModel = new CompanyModel();
            Response ret = new Response();
            try
            {
                objModel.CompID = Convert.ToInt32(CompID);
                ret.Data = JsonSerializer.SerializeTable(objUBO.GetCompHis(objModel));
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
        [Route("DownloadAllCompany", Name = "DownloadAllCompany")]
        public ActionResult DownloadAllCompany()
        {
            CompanyRepo objRepo = new CompanyRepo();
            CompanyModel objModel = new CompanyModel();
            DataTable dt = new DataTable();
            try
            {
                objModel.CompID = 0;
                objModel.EID = IvapUser.EID;
                objModel.CreatedBy = IvapUser.UID;
                objModel.SetDisplayName();
                dt = objRepo.GetCompany(objModel);
                DataTable dtComp = dt.DefaultView.ToTable(false, "TID", "ENTITY_NAME", "COMP_CODE", "COMP_NAME", "COMP_ADDR1", "COMP_ADDR2", "COMP_CITY", "COMPStateText", "COMP_PIN", "COMP_CLASS", "COMP_PANNO", "COMP_TANNO", "COMP_TDSCIRCLE", "SIGN_FNAME", "SIGN_LNAME", "SIGN_FATHER_NAME", "SIGN_ADDR1", "SIGN_ADDR2", "SIGN_CITY", "SIGN_DSG", "SignStateText", "SIGN_PIN", "SIGN_PLACE", "SIGN_DATE", "RETIRE_AGE", "EMP_CODE_GEN_Text", "EMP_CODE_PREFIX", "EMP_CODE_LEN", "COMP_URL", "STATUS");
                dtComp.Columns["ENTITY_NAME"].ColumnName = "ENTITY NAME";
                dtComp.Columns["COMP_CODE"].ColumnName = objModel.COMP_CODE_TEXT;
                dtComp.Columns["COMP_NAME"].ColumnName = objModel.COMP_NAME_TEXT;
                dtComp.Columns["COMP_ADDR1"].ColumnName = objModel.COMP_ADDR1_TEXT;
                dtComp.Columns["COMP_ADDR2"].ColumnName = objModel.COMP_ADDR2_TEXT;
                dtComp.Columns["COMP_CITY"].ColumnName = objModel.COMP_CITY_TEXT;
                dtComp.Columns["COMPStateText"].ColumnName = objModel.COMP_STATE_TEXT;
                dtComp.Columns["COMP_PIN"].ColumnName = objModel.COMP_PIN_TEXT;
                dtComp.Columns["COMP_CLASS"].ColumnName = objModel.COMP_CLASS_TEXT;
                dtComp.Columns["COMP_PANNO"].ColumnName = objModel.COMP_PANNO_TEXT;
                dtComp.Columns["COMP_TANNO"].ColumnName = objModel.COMP_TANNO_TEXT;
                dtComp.Columns["COMP_TDSCIRCLE"].ColumnName = objModel.COMP_TDSCIRCLE_TEXT;
                dtComp.Columns["SIGN_FNAME"].ColumnName = objModel.SIGN_FNAME_TEXT;
                dtComp.Columns["SIGN_LNAME"].ColumnName = objModel.SIGN_LNAME_TEXT;
                dtComp.Columns["SIGN_FATHER_NAME"].ColumnName = objModel.SIGN_FATHER_NAME_TEXT;
                dtComp.Columns["SIGN_ADDR1"].ColumnName = objModel.SIGN_ADDR1_TEXT;
                dtComp.Columns["SIGN_ADDR2"].ColumnName = objModel.SIGN_ADDR2_TEXT;
                dtComp.Columns["SIGN_CITY"].ColumnName = objModel.SIGN_CITY_TEXT;
                dtComp.Columns["SIGN_DSG"].ColumnName = objModel.SIGN_DSG_TEXT;
                dtComp.Columns["SignStateText"].ColumnName = objModel.SIGN_STATE_TEXT;
                dtComp.Columns["SIGN_PIN"].ColumnName = objModel.SIGN_PIN_TEXT;
                dtComp.Columns["SIGN_PLACE"].ColumnName = objModel.SIGN_PLACE_TEXT;
                dtComp.Columns["SIGN_DATE"].ColumnName = objModel.SIGN_DATE_TEXT;
                dtComp.Columns["RETIRE_AGE"].ColumnName = objModel.RETIRE_AGE_TEXT;
                dtComp.Columns["EMP_CODE_GEN_Text"].ColumnName = objModel.EMP_CODE_GEN_TEXT;
                dtComp.Columns["EMP_CODE_PREFIX"].ColumnName = objModel.EMP_CODE_PREFIX_TEXT;
                dtComp.Columns["EMP_CODE_LEN"].ColumnName = objModel.EMP_CODE_LEN_TEXT;
                dtComp.Columns["COMP_URL"].ColumnName = objModel.COMP_URL_TEXT;
                dtComp.Columns["STATUS"].ColumnName = objModel.ISACTIVE_TEXT;

                string FileName = ExcellUtils.DataTableToExcel(dtComp);
                FileName = FileName.Replace("/", "").Replace("..", "").Replace("\\", "");
                string FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + FileName;
                byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "CompanyMaster.xlsx");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("UploadCompanyDetails", Name = "UploadCompanyDetails")]
        public ActionResult UploadCompanyDetails(string FileName)
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
                CompanyRepo CompRepo = new CompanyRepo();
                string FilePath = Server.MapPath("~/Docs/Temp/" + FileName);
                int SuccessCount = 0;
                int FailCount = 0;
                int CreatedBy = IvapUser.UID;
                int EID = IvapUser.EID;
                string ResultFileName = CompRepo.UploadCompanyDetails(FilePath, CreatedBy, EID, ref SuccessCount, ref FailCount);
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
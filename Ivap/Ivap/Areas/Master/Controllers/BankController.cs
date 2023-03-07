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
    public class BankController : IvapBaseController
    {
        BankRepo BankRepo = new BankRepo();
        BankModel BankModel = new BankModel();
        [ViewAction]
        [Route("Bank", Name = "ViewBank")]
        public ActionResult ViewBank()
        {
            StateModel SModel = new StateModel();
            StateRepo SRepo = new StateRepo();
            CompanyModel ComModel = new CompanyModel();
            BankModel.EID = IvapUser.EID;
            BankModel.SetDisplayName();
            CompanyRepo CompRep = new CompanyRepo();
            try
            {
                BankModel.BankNameList = DropdownUtils.ToSelectList(BankRepo.GetGlobalBank(BankModel), "TID", "BANK_NAME");
                BankModel.BankStateList = DropdownUtils.ToSelectList(SRepo.GetState(), "TID", "STATE_NAME");
                BankModel.IsActive = true;
                return View("Bank", BankModel);
            }
            catch
            {
                throw;
            }
        }
        [ViewAction]
        [HttpGet]
        [Route("GetIFSCCode", Name = "GetIFSCCode")]
        public ActionResult GetIFSCCode(string  BANKNAME)
        {
            BankModel objModel = new BankModel();
            Response res = new Response();
            try
            {
                objModel.BANK_NAME = BANKNAME;
                DataTable dt = BankRepo.GetGlobalBank(objModel);
                res.Data = JsonSerializer.SerializeTable(dt);
                res.IsSuccess = true;
                return Json(res, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [ViewAction]
        [HttpGet]
        [Route("getBank", Name = "getBank")]
        public ActionResult getBank(int BankID)
        {
            //BankRepo BankRepo = new BankRepo();ViewBank
            //BankModel BankModel = new BankModel();
            KendoGridUtils res = new KendoGridUtils();
            try
            {
                BankModel.BANKID = Convert.ToInt32(BankID);
                BankModel.EID = IvapUser.EID;
                BankModel.CreatedBy = IvapUser.UID;
                DataTable dt = BankRepo.GetBank(BankModel);
                res = res.GetCommandButtonForGrid("ViewBank");
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
        [Route("AddUpdateBank", Name = "AddUpdateBank")]
        public ActionResult AddUpdateBank(BankModel Model)
        {
            Response res = new Response();
            try
            {
                Model.CreatedBy = IvapUser.UID;
                Model.EID = IvapUser.EID;
                res = BankRepo.AddUpdateBank(Model);
                return Json(res, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                throw;
            }
        }
        [ViewAction]
        [HttpGet]
        [Route("GetBankHistory", Name = "GetBankHistory")]
        public ActionResult GetBankHistory(int BankID)
        {
            //DepartmentRepo DeptRepo = new DepartmentRepo();
            //DepartmentModel DeptModel = new DepartmentModel();
            Response ret = new Response();
            try
            {
                BankModel.BANKID = Convert.ToInt32(BankID);
                ret.Data = JsonSerializer.SerializeTable(BankRepo.GetBankHistory(BankModel));
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
        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("UploadBankDetails", Name = "UploadBankDetails")]
        public ActionResult UploadBankDetails(string FileName)
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
                BankRepo objRepo = new BankRepo();
                string FilePath = Server.MapPath("~/Docs/Temp/" + FileName);
                int SuccessCount = 0;
                int FailCount = 0;
                int CreatedBy = IvapUser.UID;
                int EID = IvapUser.EID;
                string ResultFileName = objRepo.UploadBankDetails(FilePath, CreatedBy, EID, ref SuccessCount, ref FailCount);
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
        [ViewAction]
        [HttpGet]
        [Route("DownLoadAllBank", Name = "DownLoadAllBank")]
        public ActionResult DownLoadAllBank()
        {
            BankRepo BankRepo = new BankRepo();
            BankModel model = new BankModel();
            DataTable dt = new DataTable();
            // DataSet dT = new DataSet();
            try
            {
                model.EID = IvapUser.EID;
                model.CreatedBy = IvapUser.UID;
                model.SetDisplayName();
                dt = BankRepo.GetBank(model);
                DataTable dtRole = dt.DefaultView.ToTable(false, "ENTITY_NAME", "BANK_NAME", "BANK_ADDR", "BANK_CITY", "STATE_NAME", "ERP_BANK_CODE", "PAY_BANK_CODE", "IFSC", "STATUS");
                //dtRole.Columns["ENTITY_NAME"].ColumnName = "ENTITY_NAME";
                dtRole.Columns["ENTITY_NAME"].ColumnName = "ENTITY NAME";
                dtRole.Columns["BANK_NAME"].ColumnName = model.BANK_NAME_TEXT;
                dtRole.Columns["BANK_ADDR"].ColumnName = model.BANK_ADDR_TEXT;
                dtRole.Columns["BANK_CITY"].ColumnName = model.BANK_CITY_TEXT;
                dtRole.Columns["STATE_NAME"].ColumnName = model.BANK_STATE_TEXT;
                dtRole.Columns["ERP_BANK_CODE"].ColumnName = model.ERP_BANK_CODE_TEXT;
                dtRole.Columns["PAY_BANK_CODE"].ColumnName = model.PAY_BANK_CODE_TEXT;
                dtRole.Columns["IFSC"].ColumnName = model.IFSC_Code_TEXT;
                dtRole.Columns["STATUS"].ColumnName = "IS ACTIVE";


                string FileName = ExcellUtils.DataTableToExcel(dtRole);
                FileName = FileName.Replace("/", "").Replace("..", "").Replace("\\", "");
                string FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + FileName;
                byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "BankMaster.xlsx");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
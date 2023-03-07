using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ivap.Areas.Master.ViewModel;
using Ivap.Utils;
using Ivap.Areas.Master.Repository;
using Ivap.Controllers;
using Ivap.Areas.Master.Models;
using System.Data;
using Ivap.ActionFilters;
using Ivap.CustomAttribute;
using System.Web.Hosting;

namespace Ivap.Areas.Master.Controllers
{
    [CustomAuthActionFilter(Order = 0)]
    [CustomAuthrizationActionFilter(Order = 1)]
    [RouteArea("Master", AreaPrefix = "")]
    [RoutePrefix("Masters")]
    public class ProcessController : IvapBaseController
    {
        // GET: Master/Process
        [ViewAction]
        [Route("ProcessList", Name = "ProcessList")]
        public ActionResult ProcessList()
        {
            try
            {
                ProcessModel Model = new ProcessModel();
                Model.EID = IvapUser.EID;
                Model.SetDisplayName();
                CompanyRepo ComRepo = new CompanyRepo();
                CompanyModel ComModel = new CompanyModel();
                ComModel.CompID = 0;
                ComModel.IsActive = true;
                Model.CompanyList = DropdownUtils.ToSelectList(ComRepo.GetCompany(ComModel), "TID", "COMP_NAME");
                Model.IsActive = true;
                return View(Model);
            }
            catch
            {
                throw;
            }
        }

        [ViewAction]
        [Route("GetProcess", Name = "GetProcess")]
        public ActionResult GetProcess(int PID)
        {

            ProcessRepo PrRepo = new ProcessRepo();
            DataSet ds = new DataSet();
            Response res = new Response();
            ProcessModel PrModel = new ProcessModel();
            KendoGridUtils ObjKendoUtil = new KendoGridUtils();
            try
            {
                PrModel.EID = IvapUser.EID;
                PrModel.TID = PID;
                PrModel.CreatedBy = IvapUser.UID;
                ObjKendoUtil = ObjKendoUtil.GetCommandButtonForGrid("ProcessList");
                // PrModel.IsActive = true;
                ds = PrRepo.GetProcess(PrModel);
                res.IsSuccess = true;
                ObjKendoUtil.Data = JsonSerializer.SerializeTable(ds.Tables[0]);
                //return Json(res, JsonRequestBehavior.AllowGet);
                var UserGrid = Json(ObjKendoUtil, JsonRequestBehavior.AllowGet);
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
        [Route("GetProcessHis", Name = "GetProcessHis")]
        public ActionResult GetProcessHis(int PID)
        {
            ProcessRepo ProcessRepo = new ProcessRepo();
            ProcessModel model = new ProcessModel();
            DataTable dt = new DataTable();
            Response res = new Response();
            try
            {
                model.TID = PID;
                model.EID = IvapUser.EID;
                dt = ProcessRepo.GetProcessHis(model);
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
        [Route("AddUpdateProcess", Name = "AddUpdateProcess")]
        public ActionResult AddUpdateProcess(ProcessModel Model)
        {
            ProcessRepo objrepo = new ProcessRepo();
            Response res = new Response();
            try
            {
                if (ModelState.IsValid)
                {
                    Model.EID = IvapUser.EID;
                    Model.CreatedBy = IvapUser.UID;
                    res = objrepo.AddUpdateProcess(Model);
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
        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("UploadProcessDetails", Name = "UploadProcessDetails")]
        public ActionResult UploadProcessDetails(string FileName)
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
                ProcessRepo objRepo = new ProcessRepo();
                string FilePath = Server.MapPath("~/Docs/Temp/" + FileName);
                int SuccessCount = 0;
                int FailCount = 0;
                int CreatedBy = IvapUser.UID;
                int EID = IvapUser.EID;
                string ResultFileName = objRepo.UploadProcessDetails(FilePath, CreatedBy, EID, ref SuccessCount, ref FailCount);
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
        [Route("DownloadAllProcess", Name = "DownloadAllProcess")]
        public ActionResult DownloadAllProcess()
        {
            ProcessRepo ProcRepo = new ProcessRepo();
            ProcessModel model = new ProcessModel();
            DataTable dt = new DataTable();
            // DataSet dT = new DataSet();
            try
            {
                model.EID = IvapUser.EID;
                model.CreatedBy = IvapUser.UID;
                model.SetDisplayName();
                dt = ProcRepo.GetProcess(model).Tables[0];
                DataTable dtRole = dt.DefaultView.ToTable(false, "ENTITY_NAME", "ERP_PROC_CODE", "PAY_PROC_CODE", "PROC_NAME", "STATUS");
                //dtRole.Columns["ENTITY_NAME"].ColumnName = "ENTITY_NAME";
                dtRole.Columns["ENTITY_NAME"].ColumnName = "ENTITY NAME";
                dtRole.Columns["ERP_PROC_CODE"].ColumnName = model.ERP_PROC_CODE_Text;
                dtRole.Columns["PAY_PROC_CODE"].ColumnName = model.PAY_PROC_CODE_Text;
                dtRole.Columns["PROC_NAME"].ColumnName = model.PROC_NAME_Text;
                dtRole.Columns["STATUS"].ColumnName = model.ISACTIVE_TEXT;


                string FileName = ExcellUtils.DataTableToExcel(dtRole);
                FileName = FileName.Replace("/", "").Replace("..", "").Replace("\\", "");
                string FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + FileName;
                byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "ProcessMaster.xlsx");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
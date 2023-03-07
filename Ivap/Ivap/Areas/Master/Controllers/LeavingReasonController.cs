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
    public class LeavingReasonController : IvapBaseController
    {
        // GET: Master/LeavingReason
        [ViewAction]
        [Route("LeavingReason", Name = "ViewLeavingReason")]
        public ActionResult ViewLeavingReason()
        {
            LeavingReasonModel Model = new LeavingReasonModel();
            try
            {
                Model.EID = IvapUser.EID;
                Model.SetDisplayName();
                Model.IsActive = true;
                return View("LeavingReason", Model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("AddUpdateLeavingReason", Name = "AddUpdateLeavingReason")]
        public ActionResult AddUpdate(LeavingReasonModel Model)
        {
            LeavingReasonRepo objLeavingReason = new LeavingReasonRepo();
            Response res = new Response();
            try
            {
                if (ModelState.IsValid)
                {
                    Model.CreatedBy = IvapUser.UID;
                    Model.EID = IvapUser.EID;
                    res = objLeavingReason.AddUpdateLeavingReason(Model);
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
        [Route("GetLeavingReason", Name = "GetLeavingReason")]
        public ActionResult GetLeavingReason(int LEAVID)
        {
            LeavingReasonRepo LeavingReasonRepo = new LeavingReasonRepo();
            LeavingReasonModel model = new LeavingReasonModel();
            DataTable dt = new DataTable();
            KendoGridUtils res = new KendoGridUtils();
            try
            {
                model.TID = LEAVID;//ViewLeavingReason
                model.IsActive = true;
                model.EID = IvapUser.EID;
                model.CreatedBy = IvapUser.UID;
                dt = LeavingReasonRepo.GetLeavingReason(model);
                res = res.GetCommandButtonForGrid("ViewLeavingReason");
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
        [Route("GetLeavingReasonHis", Name = "GetLeavingReasonHis")]
        public ActionResult GetLeavingReasonHis(int LEAVID)
        {
            LeavingReasonRepo LeavingReasonRepo = new LeavingReasonRepo();
            LeavingReasonModel model = new LeavingReasonModel();
            DataTable dt = new DataTable();
            Response res = new Response();
            try
            {
                model.TID = LEAVID;
                model.EID = IvapUser.EID;
                dt = LeavingReasonRepo.GetLeavingReasonHis(model);
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
        [Route("DownloadAllLeavingReason", Name = "DownloadAllLeavingReason")]
        public ActionResult DownloadAllLeavingReason()
        {
            LeavingReasonRepo LeavingReasonRepo = new LeavingReasonRepo();
            LeavingReasonModel model = new LeavingReasonModel();
            DataTable dt = new DataTable();
            //ENTITY_ID,PAY_LEAVING_CODE,ERP_LEAVING_CODE,[VOL/NON_VOL],REASON,
            try
            {
                model.EID = IvapUser.EID;
                model.SetDisplayName();
                model.CreatedBy = IvapUser.UID;
                model.TID = 0;
                dt = LeavingReasonRepo.GetLeavingReason(model);
                DataTable dtRole = dt.DefaultView.ToTable(false, "ENTITY_NAME", "PAY_LEAVING_CODE", "ERP_LEAVING_CODE", "VOLU", "REASON", "STATUS");
                dtRole.Columns["ENTITY_NAME"].ColumnName = "Entity Name";
                dtRole.Columns["PAY_LEAVING_CODE"].ColumnName = model.PAY_LEAVING_CODE_TEXT;
                dtRole.Columns["ERP_LEAVING_CODE"].ColumnName = model.ERP_LEAVING_CODE_TEXT;
                dtRole.Columns["VOLU"].ColumnName = model.VOL_TEXT;
                dtRole.Columns["REASON"].ColumnName = model.REASON_TEXT;
                dtRole.Columns["STATUS"].ColumnName = model.ISACTIVE_TEXT;
                string FileName = ExcellUtils.DataTableToExcel(dtRole);
                FileName = FileName.Replace("/", "").Replace("..", "").Replace("\\", "");
                string FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + FileName;
                byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "LeavingReasonMaster.xlsx");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("UploadLeavingReasonDetails", Name = "UploadLeavingReasonDetails")]
        public ActionResult UploadLeavingReasonDetails(string FileName)
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
                LeavingReasonRepo objRepo = new LeavingReasonRepo();
                string FilePath = Server.MapPath("~/Docs/Temp/" + FileName);
                int SuccessCount = 0;
                int FailCount = 0;
                int CreatedBy = IvapUser.UID;
                int EID = IvapUser.EID;
                string ResultFileName = objRepo.UploadLeavingReasonDetails(FilePath, CreatedBy, EID, ref SuccessCount, ref FailCount);
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
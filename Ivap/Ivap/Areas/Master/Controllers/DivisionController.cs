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
    public class DivisionController : IvapBaseController
    {
        [ViewAction]
        [Route("Division", Name = "Division")]
        public ActionResult Division()
        {
            CompanyModel objCompModel = new CompanyModel();
            CompanyRepo objCompRepo = new CompanyRepo();
            DivisionModel objDiviModel = new DivisionModel();
            
            try
            {
                objDiviModel.EID = IvapUser.EID;
                objDiviModel.SetDisplayName();
                objDiviModel.IsActive = true;
                return View("Division", objDiviModel);
            }
            catch (Exception ex) { throw; }
        }
        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("AddUpdateDivision", Name = "AddUpdateDivision")]
        public ActionResult AddUpdateDivision(DivisionModel Model)
        {
            DivisionRepo objrepo = new DivisionRepo();
            Response res = new Response();
            try
            {
                if (ModelState.IsValid)
                {
                    Model.CreatedBy = IvapUser.UID;
                    Model.EID = IvapUser.EID;
                    res = objrepo.AddUpdateDivision(Model);
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
        [Route("GetDivision", Name = "GetDivision")]
        public ActionResult GetDivision(int DiviID)
        {
            DivisionRepo objUBO = new DivisionRepo();
            DivisionModel objModel = new DivisionModel();

            KendoGridUtils res = new KendoGridUtils();
            try
            {
                objModel.DiviID = DiviID;
                objModel.EID = IvapUser.EID;
                objModel.CreatedBy = IvapUser.UID;
                DataTable dt = objUBO.GetDivision(objModel);
                res = res.GetCommandButtonForGrid("Division");
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
        [Route("GetDivisionHistory", Name = "GetDivisionHistory")]
        public ActionResult GetDivisionHistory(int DiviID)
        {
            DivisionRepo objRepo = new DivisionRepo();
            DivisionModel objModel = new DivisionModel();
            Response ret = new Response();
            try
            {
                objModel.DiviID = DiviID;
                ret.Data = JsonSerializer.SerializeTable(objRepo.GetDivisionHistory(objModel));
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
        [Route("DownloadAllDivision", Name = "DownloadAllDivision")]
        public ActionResult DownloadAllDivision()
        {
            DivisionRepo objRepo = new DivisionRepo();
            DivisionModel objModel = new DivisionModel();
            
            DataTable dt = new DataTable();
            try
            {
                objModel.EID = IvapUser.EID;
                objModel.CreatedBy = IvapUser.UID;
                objModel.SetDisplayName();
                objModel.DiviID = 0;
                dt = objRepo.GetDivision(objModel);
                DataTable dtComp = dt.DefaultView.ToTable(false,"ENTITY_NAME", "PAY_DIVI_CODE", "ERP_DIVI_CODE", "DIVI_NAME", "STATUS");
                dtComp.Columns["ENTITY_NAME"].ColumnName = "ENTITY_NAME";
                dtComp.Columns["PAY_DIVI_CODE"].ColumnName = objModel.PAY_DIVI_CODE_TEXT;
                dtComp.Columns["ERP_DIVI_CODE"].ColumnName = objModel.ERP_DIVI_CODE_TEXT;
                dtComp.Columns["DIVI_NAME"].ColumnName = objModel.DIVI_NAME_TEXT;
                dtComp.Columns["STATUS"].ColumnName = objModel.ISACTIVE_TEXT;

                string FileName = ExcellUtils.DataTableToExcel(dtComp);
                FileName = FileName.Replace("/", "").Replace("..", "").Replace("\\", "");
                string FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + FileName;
                byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "DevisionMaster.xlsx");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("UploadDivisionDetails", Name = "UploadDivisionDetails")]
        public ActionResult UploadDivisionDetails(string FileName)
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
                DivisionRepo objRepo = new DivisionRepo();
                string FilePath = Server.MapPath("~/Docs/Temp/" + FileName);
                int SuccessCount = 0;
                int FailCount = 0;
                int CreatedBy = IvapUser.UID;
                int EID = IvapUser.EID;
                string ResultFileName = objRepo.UploadDivisionDetails(FilePath, CreatedBy, EID, ref SuccessCount, ref FailCount);
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
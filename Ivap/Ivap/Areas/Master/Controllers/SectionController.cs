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
    public class SectionController : IvapBaseController
    {
        SectionRepo SectionRepo = new SectionRepo();
        [ViewAction]
        [Route("Section", Name = "ViewSection")]
        public ActionResult ViewSection()
        {
            SectionModel model = new SectionModel();
            model.EID = IvapUser.EID;
            model.SetDisplayName();
            model.IsActive = true;
            return View("Section", model);
        }
        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("AddUpdateSection", Name = "AddUpdateSection")]
        public ActionResult AddUpdateSection(SectionModel Model)
        {
            Response res = new Response();
            try
            {
                if (ModelState.IsValid)
                {
                    Model.CreatedBy = IvapUser.UID;
                    Model.EID = IvapUser.EID;
                    res = SectionRepo.AddUpdateSection(Model);
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
        [Route("GetSection", Name = "GetSection")]
        public ActionResult GetSection(int SectionID)
        {
            
            SectionModel model = new SectionModel();
            DataTable dt = new DataTable();
            KendoGridUtils res = new KendoGridUtils();
            try
            {
                model.SECTID = SectionID;//ViewSection
                model.EID = IvapUser.EID;
                model.CreatedBy = IvapUser.UID;
                dt = SectionRepo.GetSection(model);
                res = res.GetCommandButtonForGrid("ViewSection");
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
        [Route("GetSectionHis", Name = "GetSectionHis")]
        public ActionResult GetSectionHis(int SectionID)
        {
            SectionModel model = new SectionModel();
            DataTable dt = new DataTable();
            Response res = new Response();
            try
            {
                model.SECTID = SectionID;
                dt = SectionRepo.GetSectionHis(model);
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
        [Route("ExportAllSection", Name = "ExportAllSection")]
        public ActionResult ExportAllSection()
        {
            SectionModel model = new SectionModel();
            model.EID = IvapUser.EID;
            model.CreatedBy = IvapUser.UID;
            model.SetDisplayName();
            DataTable dt = new DataTable();
            try
            {
                dt = SectionRepo.GetSection(model);
                DataTable dtRole = dt.DefaultView.ToTable(false, "ENTITY_NAME", "PAY_SECTION_CODE", "ERP_SECTION_CODE", "SECTION_NAME", "STATUS");
                dtRole.Columns["ENTITY_NAME"].ColumnName = "Entity Name";
                dtRole.Columns["PAY_SECTION_CODE"].ColumnName = model.PAY_SECTION_CODEE_TEXT;
                dtRole.Columns["ERP_SECTION_CODE"].ColumnName = model.ERP_SECTION_CODE_TEXT;
                dtRole.Columns["SECTION_NAME"].ColumnName = model.SECTION_NAME_TEXT;
                dtRole.Columns["STATUS"].ColumnName = model.ISACTIVE_TEXT;
                string FileName = ExcellUtils.DataTableToExcel(dtRole);
                FileName = FileName.Replace("/", "").Replace("..", "").Replace("\\", "");
                string FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + FileName;
                byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "SectionMaster.xlsx");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("UploadSectionDetails", Name = "UploadSectionDetails")]
        public ActionResult UploadSectionDetails(string FileName)
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
                SectionRepo objRepo = new SectionRepo();
                string FilePath = Server.MapPath("~/Docs/Temp/" + FileName);
                int SuccessCount = 0;
                int FailCount = 0;
                int CreatedBy = IvapUser.UID;
                int EID = IvapUser.EID;
                string ResultFileName = objRepo.UploadSectionDetails(FilePath, CreatedBy, EID, ref SuccessCount, ref FailCount);
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
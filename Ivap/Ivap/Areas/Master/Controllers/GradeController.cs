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
    public class GradeController : IvapBaseController
    {
        // GET: Master/Grade
        [ViewAction]
        [Route("GradeList", Name = "GradeList")]
        public ActionResult GradeList()
        {
            try
            {
                GradeModel Model = new GradeModel();
                CompanyRepo ComRepo = new CompanyRepo();
                CompanyModel ComModel = new CompanyModel();
                Model.EID = IvapUser.EID;
                Model.SetDisplayName();
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
        [HttpGet]
        [Route("GetGradHistory", Name = "GetGradHistory")]
        public ActionResult GetGradHistory(int GradID)
        {

            Response res = new Response();
            GradeRepo GrRepo = new GradeRepo();
            try
            {
                DataTable dt = GrRepo.GetGradHistory(GradID);
                res.Data = JsonSerializer.SerializeTable(dt);
                var GradG = Json(res, JsonRequestBehavior.AllowGet);
                GradG.MaxJsonLength = int.MaxValue;
                return GradG;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [ViewAction]
        [Route("GetGrade", Name = "GetGrade")]
        public ActionResult GetGrade(int GID)
        {

            GradeRepo GrRepo = new GradeRepo();
            DataTable dt = new DataTable();
            Response res = new Response();
            KendoGridUtils objKendo = new KendoGridUtils();
            GradeModel GrModel = new GradeModel();
            try
            {
                GrModel.TID = GID;
                GrModel.IsActive = true;
                GrModel.CreatedBy = IvapUser.UID; 
                GrModel.EID = IvapUser.EID;
                dt = GrRepo.GetGrade(GrModel);
                objKendo = objKendo.GetCommandButtonForGrid("GradeList");
                objKendo.Data = JsonSerializer.SerializeTable(dt);
                var UserGrid = Json(objKendo, JsonRequestBehavior.AllowGet);
                UserGrid.MaxJsonLength = int.MaxValue;
                return UserGrid;
            }
            catch
            {
                throw;
            }

        }
        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("AddUpdateGrade", Name = "AddUpdateGrade")]
        public ActionResult AddUpdateGrade(GradeModel Model)
        {
            GradeRepo objrepo = new GradeRepo();
            Response res = new Response();
            try
            {
                if (ModelState.IsValid)
                {
                    Model.CreatedBy = IvapUser.UID;
                    Model.EID = IvapUser.EID;
                    res = objrepo.AddUpdateGrade(Model);
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
        [Route("UploadGradeDetails", Name = "UploadGradeDetails")]
        public ActionResult UploadGradeDetails(string FileName)
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
                GradeRepo objRepo = new GradeRepo();
                string FilePath = Server.MapPath("~/Docs/Temp/" + FileName);
                int SuccessCount = 0;
                int FailCount = 0;
                int CreatedBy = IvapUser.UID;
                int EID = IvapUser.EID;
                string ResultFileName = objRepo.UploadGradeDetails(FilePath, CreatedBy, EID, ref SuccessCount, ref FailCount);
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
        [Route("DownloadAllGrade", Name = "DownloadAllGrade")]
        public ActionResult DownloadAllGrade()
        {
            GradeRepo GradeRepo = new GradeRepo();
            GradeModel model = new GradeModel();
            DataTable dt = new DataTable();
            try
            {
                model.EID = IvapUser.EID;
                model.CreatedBy = IvapUser.UID;
                model.SetDisplayName();
                dt = GradeRepo.GetGrade(model);
                DataTable dtRole = dt.DefaultView.ToTable(false, "ENTITY_NAME", "ERP_GRADE_CODE", "PAY_GRADE_CODE", "GARDE_NAME", "GRADE_SCALE_FROM", "GRADE_SCALE_TO", "GRADE_MIDPOINT","STATUS");
                //dtRole.Columns["ENTITY_NAME"].ColumnName = "ENTITY_NAME";
                dtRole.Columns["ENTITY_NAME"].ColumnName = "ENTITY NAME";
                dtRole.Columns["ERP_GRADE_CODE"].ColumnName = model.ERP_GRADE_CODE_TEXT;
                dtRole.Columns["PAY_GRADE_CODE"].ColumnName = model.PAY_GRADE_CODE_TEXT;
                dtRole.Columns["GARDE_NAME"].ColumnName = model.GARDE_NAME_TEXT;
                dtRole.Columns["GRADE_SCALE_FROM"].ColumnName = model.GRADE_SCALE_FROM_TEXT;
                dtRole.Columns["GRADE_SCALE_TO"].ColumnName = model.GRADE_SCALE_TO_TEXT;
                dtRole.Columns["GRADE_MIDPOINT"].ColumnName = model.GRADE_MIDPOINT_TEXT;
                dtRole.Columns["STATUS"].ColumnName = model.ISACTIVE_TEXT;
                string FileName = ExcellUtils.DataTableToExcel(dtRole);
                FileName = FileName.Replace("/", "").Replace("..", "").Replace("\\", "");
                string FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + FileName;
                byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "GradeMaster.xlsx");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
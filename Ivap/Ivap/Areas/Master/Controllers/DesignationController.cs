
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
    public class DesignationController : IvapBaseController
    {
        [ViewAction]
        [Route("Designation", Name = "ViewDesignation")]
        public ActionResult Designation()
        {
            GradeModel objGradeModel = new GradeModel();
            GradeRepo objGradeRepo = new GradeRepo();
            CompanyModel objCompModel = new CompanyModel();
            CompanyRepo objCompRepo = new CompanyRepo();
            DesignationModel objDSGModel = new DesignationModel();
            
            EntityRepo eRepo = new EntityRepo();
            try
            {
                objDSGModel.EID = IvapUser.EID;
                objDSGModel.SetDisplayName();
                objDSGModel.IsActive = true;
                return View("Designation", objDSGModel);
            }
            catch (Exception ex) { throw; }
        }
        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("AddUpdateDesignation", Name = "AddUpdateDesignation")]
        public ActionResult AddUpdateDesignation(DesignationModel Model)
        {
            DesignationRepo objrepo = new DesignationRepo();
            Response res = new Response();
            try
            {
                if (ModelState.IsValid)
                {
                    Model.EID = IvapUser.EID;
                    Model.CreatedBy = IvapUser.UID;

                    res = objrepo.AddUpdateDesignation(Model);
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
        [Route("GetDesignation", Name = "GetDesignation")] 
        public ActionResult GetDesignation(int DSGID)
        {
            DesignationRepo objUBO = new DesignationRepo();
            DesignationModel objModel = new DesignationModel();
            KendoGridUtils res = new KendoGridUtils();
            try
            {
                objModel.DSGID = DSGID;
                objModel.EID = IvapUser.EID;
                objModel.CreatedBy = IvapUser.UID;
                DataTable dt = objUBO.GetDesignation(objModel);
                res = res.GetCommandButtonForGrid("ViewDesignation");
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
        [Route("GetDSGName", Name = "GetDSGName")]
        public ActionResult GetDSGName(int EID,string searchStr)
        {
            DesignationRepo objUBO = new DesignationRepo();
            DesignationModel objModel = new DesignationModel();
            Response res = new Response();
            try
            {
                objModel.EID= EID;
                objModel.DSG_NAME = searchStr;
                DataTable dt = objUBO.GetDSGName(objModel);
                //res = res.GetCommandButtonForGrid("ViewUser");
                res.Data = JsonSerializer.SerializeTable(dt);
                return Json(res, JsonRequestBehavior.AllowGet);
                //var UserGrid = Json(res, JsonRequestBehavior.AllowGet);
                //UserGrid.MaxJsonLength = int.MaxValue;
                //return UserGrid;

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [ViewAction]
        [HttpGet]
        [Route("GetDesignationHistory", Name = "GetDesignationHistory")]
        public ActionResult GetDesignationHistory(int DSGID)
        {
            DesignationRepo objDSGRepo = new DesignationRepo();
            DesignationModel objModel = new DesignationModel();
            Response ret = new Response();
            try
            {
                objModel.DSGID = Convert.ToInt32(DSGID);
                ret.Data = JsonSerializer.SerializeTable(objDSGRepo.GetDesignationHistory(objModel));
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
        [Route("DownloadAllDesignation", Name = "DownloadAllDesignation")]
        public ActionResult DownloadAllDesignation()
        {
            DesignationRepo objRepo = new DesignationRepo();
            DesignationModel objModel = new DesignationModel();
            DataTable dt = new DataTable();
            try
            {
                objModel.EID = IvapUser.EID;
                objModel.SetDisplayName();
                objModel.DSGID = 0;
                objModel.CreatedBy = IvapUser.UID;
                dt = objRepo.GetDesignation(objModel);
                DataTable dtComp = dt.DefaultView.ToTable(false,"ENTITY_NAME", "PAY_DSG_CODE", "ERP_DSG_CODE", "DSG_NAME", "STATUS");
                dtComp.Columns["ENTITY_NAME"].ColumnName = "ENTITY NAME";
                dtComp.Columns["PAY_DSG_CODE"].ColumnName = objModel.PAY_DSG_CODE_TEXT;
                dtComp.Columns["ERP_DSG_CODE"].ColumnName = objModel.ERP_DSG_CODE_TEXT;
                dtComp.Columns["DSG_NAME"].ColumnName = objModel.DSG_NAME_TEXT;
                dtComp.Columns["STATUS"].ColumnName = objModel.ISACTIVE_TEXT;

                string FileName = ExcellUtils.DataTableToExcel(dtComp);
                FileName = FileName.Replace("/", "").Replace("..", "").Replace("\\", "");
                string FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + FileName;
                byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "DesignationMaster.xlsx");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("UploadDesignationDetails", Name = "UploadDesignationDetails")]
        public ActionResult UploadDesignationDetails(string FileName)
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
                DesignationRepo objRepo = new DesignationRepo();
                string FilePath = Server.MapPath("~/Docs/Temp/" + FileName);
                int SuccessCount = 0;
                int FailCount = 0;
                int CreatedBy = IvapUser.UID;
                int EID = IvapUser.EID;
                string ResultFileName = objRepo.UploadDesignationDetails(FilePath, CreatedBy, EID, ref SuccessCount, ref FailCount);
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
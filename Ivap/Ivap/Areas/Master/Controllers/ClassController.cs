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
    public class ClassController : IvapBaseController
    {
        [ViewAction]
        [Route("Class", Name = "ViewClass")]
        public ActionResult ViewClass()
        {
           
            CompanyRepo ComRepo = new CompanyRepo();
            CompanyModel ComModel = new CompanyModel();
            ClassModel Model = new ClassModel();
            Model.EID = IvapUser.EID;
            Model.SetDisplayName();
            try
            {
               
                ComModel.CompID = 0;
                ComModel.IsActive = true;
                Model.CompanyList = DropdownUtils.ToSelectList(ComRepo.GetCompany(ComModel), "TID", "COMP_NAME");
                Model.IsActive = true;

            }
            catch(Exception ex)
            {
                throw;
            }
            return View("Class",Model);
        }
        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("AddUpdateClass", Name = "AddUpdateClass")]
        public ActionResult AddUpdate(ClassModel Model)
        {
            ClassRepo objClass = new ClassRepo();
            Response res = new Response();
            try
            {
                if (ModelState.IsValid)
                {
                    Model.CreatedBy = IvapUser.UID;
                    Model.EID = IvapUser.EID;
                    res = objClass.AddUpdateClass(Model);
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
        [Route("GetClass", Name = "GetClass")]
        public ActionResult GetClass(int ClassID)
        {
            ClassRepo ClassRepo = new ClassRepo();
            //KendoGridUtils
            ClassModel model = new ClassModel();
            DataTable dt = new DataTable();
            KendoGridUtils res = new KendoGridUtils();
            try
            {
                model.CID = ClassID;
                // model.RoleID = RoleID;ViewClass
                model.EID = IvapUser.EID;
                model.CreatedBy = IvapUser.UID;
                dt = ClassRepo.GetClass(model);
                res = res.GetCommandButtonForGrid("ViewClass");
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
        [Route("GetClassHis", Name = "GetClassHis")]
        public ActionResult GetClassHis(int ClassID)
        {
            ClassRepo ClassRepo = new ClassRepo();
            ClassModel model = new ClassModel();
            DataTable dt = new DataTable();
            Response res = new Response();
            try
            {
                model.CID = ClassID;
                model.EID = IvapUser.EID;
                dt = ClassRepo.GetClassHis(model);
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
        [Route("DownloadAllClass", Name = "DownloadAllClass")]
        public ActionResult DownloadAllClass()
        {
            ClassRepo ClassRepo = new ClassRepo();
            ClassModel model = new ClassModel();
            
            DataTable dt = new DataTable();
            try
            {
                model.EID = IvapUser.EID;
                model.SetDisplayName();
                model.CreatedBy = IvapUser.UID;
                model.ERP_CLASS_CODE = "0";
                dt = ClassRepo.GetClass(model);
                DataTable dtRole = dt.DefaultView.ToTable(false, "ENTITY_NAME", "ERP_CLASS_CODE", "PAY_CLASS_CODE", "CLASS_NAME","STATUS");
                dtRole.Columns["ENTITY_NAME"].ColumnName = "Entity Name";
                dtRole.Columns["ERP_CLASS_CODE"].ColumnName = model.ERP_CLASS_CODE_TEXT;
                dtRole.Columns["PAY_CLASS_CODE"].ColumnName = model.PAY_CLASS_CODE_TEXT;
                dtRole.Columns["CLASS_NAME"].ColumnName = model.CLASS_NAME_TEXT;
               
                dtRole.Columns["STATUS"].ColumnName = model.ISACTIVE_TEXT;
                string FileName = ExcellUtils.DataTableToExcel(dtRole);
                FileName = FileName.Replace("/", "").Replace("..", "").Replace("\\", "");
                string FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + FileName;
                byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "ClassMaster.xlsx");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

       [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("UploadClassDetails", Name = "UploadClassDetails")]
        public ActionResult UploadClassDetails(string FileName)
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
                ClassRepo objRepo = new ClassRepo();
                string FilePath = Server.MapPath("~/Docs/Temp/" + FileName);
                int SuccessCount = 0;
                int FailCount = 0;
                int CreatedBy = IvapUser.UID;
                int EID = IvapUser.EID;
                string ResultFileName = objRepo.UploadClassDetails(FilePath, CreatedBy, EID, ref SuccessCount, ref FailCount);
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
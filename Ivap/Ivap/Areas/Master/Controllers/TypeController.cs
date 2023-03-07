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
    public class TypeController : IvapBaseController
    {
        [ViewAction]
        [Route("TypeList", Name = "TypeList")]
        // GET: Master/Type
        public ActionResult TypeList()
        {
            TypeModel TpModel = new TypeModel();
            TpModel.EID = IvapUser.EID;
            TpModel.SetDisplayName();
            TpModel.IsActive = true;
            return View("TypeList",TpModel);
        }
        [ViewAction]
        [Route("GetType", Name = "GetType")]
        public ActionResult GetType(int TPID)
        {
            TypeRepo TpRepo = new TypeRepo();
            DataSet ds = new DataSet();
            Response res = new Response();
            TypeModel TpModel = new TypeModel();
            KendoGridUtils objKendo = new KendoGridUtils();

            //TpModel.SetDisplayName();TypeList
            try
            {
                TpModel.TID = TPID;
                //TpModel.IsActive =Convert.ToBoolean(1);
                TpModel.EID = IvapUser.EID;
                TpModel.CreatedBy = IvapUser.UID;
                ds = TpRepo.GetType(TpModel);
                objKendo = objKendo.GetCommandButtonForGrid("TypeList");
                objKendo.Data = JsonSerializer.SerializeTable(ds.Tables[0]);
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
        [Route("AddUpdateType", Name = "AddUpdateType")]
        public ActionResult AddUpdateType(TypeModel Model)
        {
            TypeRepo objrepo = new TypeRepo();
            Response res = new Response();
            try
            {
                if (ModelState.IsValid)
                {
                    Model.CreatedBy = IvapUser.UID;
                    Model.EID = IvapUser.EID;
                    res = objrepo.AddUpdateType(Model);
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
        [Route("UploadTypeDetails", Name = "UploadTypeDetails")]
        public ActionResult UploadTypeDetails(string FileName)
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
                TypeRepo objRepo = new TypeRepo();
                string FilePath = Server.MapPath("~/Docs/Temp/" + FileName);
                int SuccessCount = 0;
                int FailCount = 0;
                int CreatedBy = IvapUser.UID;
                int EID = IvapUser.EID;
                string ResultFileName = objRepo.UploadTypeDetails(FilePath, CreatedBy, EID, ref SuccessCount, ref FailCount);
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
                    //ret = ret.GetResponse("Mapping", "GetMapping", -1000, "", data, "");DownLoadTypeAll
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
        [Route("DownLoadTypeAll", Name = "DownLoadTypeAll")]
        public ActionResult DownLoadTypeAll()
        {
            TypeModel model = new TypeModel();
            model.EID = IvapUser.EID;
            model.CreatedBy = IvapUser.UID;
            model.SetDisplayName();
            DataTable dt = new DataTable();
            TypeRepo objRepo = new TypeRepo();
            try
            {
                dt = objRepo.GetType(model).Tables[0];
                DataTable dtRole = dt.DefaultView.ToTable(false, "ENTITY_NAME", "PAY_TYPE_CODE", "ERP_TYPE_CODE", "TYPE_NAME", "STATUS");
                dtRole.Columns["ENTITY_NAME"].ColumnName = "Entity Name";
                dtRole.Columns["PAY_TYPE_CODE"].ColumnName = model.PAY_TYPE_CODE_TEXT;
                dtRole.Columns["ERP_TYPE_CODE"].ColumnName = model.ERP_TYPE_CODE_TEXT;
                dtRole.Columns["TYPE_NAME"].ColumnName = model.TYPE_NAME_TEXT;
                dtRole.Columns["STATUS"].ColumnName = model.ISACTIVE_TEXT;
                string FileName = ExcellUtils.DataTableToExcel(dtRole);
                FileName = FileName.Replace("/", "").Replace("..", "").Replace("\\", "");
                string FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + FileName;
                byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "TypeMaster.xlsx");
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        [ViewAction]
        [HttpGet]
        [Route("GetTypeHis", Name = "GetTypeHis")]
        public ActionResult GetTypeHis(int TPID)
        {
            TypeRepo TypeRepo = new TypeRepo();
            TypeModel model = new TypeModel();
            DataTable dt = new DataTable();
            Response res = new Response();
            try
            {
                model.TID = TPID;
                model.EID = IvapUser.EID;
                dt = TypeRepo.GetTypeHis(model);
                res.IsSuccess = true;
                res.Data = JsonSerializer.SerializeTable(dt);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                throw;
            }
        }

    }
}




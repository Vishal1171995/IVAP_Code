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
    public class FunctionController : IvapBaseController
    {
        // GET: Master/Function
       
        FunctionRepo FunctionRepo = new FunctionRepo();
        FunctionModel FunctionModel = new FunctionModel();
        [ViewAction]
        [Route("Function", Name = "ViewFunction")]
        public ActionResult ViewFunction()
        {
            CompanyModel ComModel = new CompanyModel();
            //FunctionModel FunctionModel = new FunctionModel();
            
            CompanyRepo CompRep = new CompanyRepo();
            try
            {
                FunctionModel.EID = IvapUser.EID;
                FunctionModel.SetDisplayName();
                ComModel.CompID = 0;
                ComModel.IsActive = true;
                FunctionModel.CompanyList = DropdownUtils.ToSelectList(CompRep.GetCompany(ComModel), "TID", "COMP_NAME");
                FunctionModel.IsActive = true;
                return View("Function", FunctionModel);
            }
            catch
            {
                throw;
            }
        }
        [ViewAction]
        [HttpGet]
        [Route("getFunction", Name = "getFunction")]
        public ActionResult getFunction(int FunctionID)
        {
            //FunctionRepo FunctionRepo = new FunctionRepo();
            //FunctionModel FunctionModel = new FunctionModel();
            FunctionModel.TID = Convert.ToInt32(FunctionID);
            FunctionModel.EID = IvapUser.EID;
            FunctionModel.CreatedBy = IvapUser.UID;
            KendoGridUtils res = new KendoGridUtils();
            try
            {
                DataSet ds = FunctionRepo.GetFunction(FunctionModel);
                res = res.GetCommandButtonForGrid("ViewFunction");
                res.Data = JsonSerializer.SerializeTable(ds.Tables[0]);
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
        [Route("GetFunctionHistory", Name = "GetFunctionHistory")]
        public ActionResult GetFunctionHistory(int FunctionID)
        {
          
            Response res = new Response();
            try
            {
                DataTable dt = FunctionRepo.GetFunctionHistory(FunctionID);
                res.Data = JsonSerializer.SerializeTable(dt);
                var HisFun = Json(res, JsonRequestBehavior.AllowGet);
                HisFun.MaxJsonLength = int.MaxValue;
                return HisFun;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("AddUpdateFunction", Name = "AddUpdateFunction")]
        public ActionResult AddUpdateFunction(FunctionModel Model)
        {
            Response res = new Response();
            try
            {
                Model.CreatedBy = IvapUser.UID;
                Model.EID = IvapUser.EID;
                res = FunctionRepo.AddUpdateFunction(Model);
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
        [Route("UploadFunctionDetails", Name = "UploadFunctionDetails")]
        public ActionResult UploadFunctionDetails(string FileName)
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
                FunctionRepo objRepo = new FunctionRepo();
                string FilePath = Server.MapPath("~/Docs/Temp/" + FileName);
                int SuccessCount = 0;
                int FailCount = 0;
                int CreatedBy = IvapUser.UID;
                int EID = IvapUser.EID;
                string ResultFileName = objRepo.UploadFunctionDetails(FilePath, CreatedBy, EID, ref SuccessCount, ref FailCount);
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
        [Route("DownLoadAllFunction", Name = "DownLoadAllFunction")]
        public ActionResult DownLoadAllFunction()
        {
            FunctionRepo FunRepo = new FunctionRepo();
            FunctionModel model = new FunctionModel();
            DataTable dt = new DataTable();
            // DataSet dT = new DataSet();
            try
            {
                model.EID = IvapUser.EID;
                model.SetDisplayName();
                model.CreatedBy = IvapUser.UID;
                dt = FunRepo.GetFunction(model).Tables[0];
                DataTable dtRole = dt.DefaultView.ToTable(false, "ENTITY_NAME", "FUNC_NAME", "ERP_FUNC_CODE", "PAY_FUNC_CODE", "STATUS");
                //dtRole.Columns["ENTITY_NAME"].ColumnName = "ENTITY_NAME";
                dtRole.Columns["ENTITY_NAME"].ColumnName = "ENTITY NAME";
                dtRole.Columns["FUNC_NAME"].ColumnName = model.FUNC_NAME_TEXT;
                dtRole.Columns["ERP_FUNC_CODE"].ColumnName = model.ERP_FUNC_CODE_TEXT;
                dtRole.Columns["PAY_FUNC_CODE"].ColumnName = model.PAY_FUNC_CODE_TEXT;
                dtRole.Columns["STATUS"].ColumnName = model.ISACTIVE_TEXT;



                string FileName = ExcellUtils.DataTableToExcel(dtRole);
                FileName = FileName.Replace("/", "").Replace("..", "").Replace("\\", "");
                string FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + FileName;
                byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "FunctionMaster.xlsx");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
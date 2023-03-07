using Ivap.ActionFilters;
using Ivap.Areas.Master.Models;
using Ivap.Areas.Master.Repository;
using Ivap.Controllers;
using Ivap.CustomAttribute;
using Ivap.Utils;
using System;
using System.Data;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Ivap.Areas.Master.Controllers
{
    [CustomAuthActionFilter(Order = 0)]
    [CustomAuthrizationActionFilter(Order = 1)]
    [RouteArea("Master", AreaPrefix = "")]
    [RoutePrefix("Masters")]
    public class LwfController : IvapBaseController
    {
       

        // GET: Master/Lwf
        [ViewAction]
        [Route("Lwf", Name = "ViewLwf")]
        public ActionResult ViewLwf()
        {
            StateRepo sr = new StateRepo();
            LWFModel model = new LWFModel();
            LwfRepo Lrep = new LwfRepo();
            StateModel sm = new StateModel();
            try
            {
                model.StateList = DropdownUtils.ToSelectList(sr.GetState(), "TID", "STATE_NAME");
                model.DED_MONTHList = DropdownUtils.ToSelectList(Lrep.MonthDropDown(), "TID", "MonthName");
                //model.EID = IvapUser.EID;
                model.EID = 1;
                model.IsActive = true;
                model.SetDisplayName();
            }
            catch (Exception ex)
            {
                throw;
            }
            return View("Lwf", model);
        }
    
    [ADDUpdateAction]
    [ValidateAntiForgeryToken]
    [HttpPost]
    [Route("AddUpdateLwf", Name = "AddUpdateLwf")]
    public ActionResult AddUpdate(LWFModel Model)
    {
            LWFModel objlwf = new LWFModel();
       LwfRepo objRepo = new LwfRepo();
        Response res = new Response();
        try
        {
                if (ModelState.IsValid)
                {
                    Model.CreatedBy = IvapUser.UID;
                    Model.EID = IvapUser.EID;
                    res = objRepo.AddUpdateLwf(Model);
                    return Json(res);
                }
                else
                {
                    return View("Lwf", objlwf);
                }
            }
        catch
        {
            throw;
        }
            
        }

    [ViewAction]
    [HttpGet]
    [Route("GetLwf", Name = "GetLwf")]
    public ActionResult GetLwf(int LwfID)
    {
        LwfRepo LRepo = new LwfRepo();
        //KendoGridUtils
        LWFModel model = new LWFModel();
        DataTable dt = new DataTable();
        KendoGridUtils res = new KendoGridUtils();
        try
        {
                model.LWFID = LwfID;
                dt = LRepo.GetLwf(model);
                res = res.GetCommandButtonForGrid("ViewLwf");
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
    [Route("GetLwfHis", Name = "GetLwfHis")]
    public ActionResult GetLwfHis(int LWFID)
    {
        LwfRepo LRepo = new LwfRepo();
        LWFModel model = new LWFModel();
        DataTable dt = new DataTable();
        Response res = new Response();
        try
        {
            model.LWFID = LWFID;
            dt = LRepo.GetLwfHis(model);
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
        [Route("DownloadAllLwf", Name = "DownloadAllLwf")]
        public ActionResult DownloadAllLwf()
        {
            LwfRepo LRepo = new LwfRepo();
            LWFModel model = new LWFModel();
            DataTable dt = new DataTable();
            //ENTITY_ID,PAY_LEAVING_CODE,ERP_LEAVING_CODE,[VOL/NON_VOL],REASON,
            try
            {
                //model.EID = IvapUser.EID;
                model.EID = IvapUser.EID;
                model.SetDisplayName();
                //model.TID = 0;

                dt = LRepo.GetLwf(model);
                DataTable dtRole = dt.DefaultView.ToTable(false, "LOCATION_ID", "PERIOD_FLAG", "DED_MONTH", "STATE_NAME", "LWF_EMPLOYEE", "LWF_EMPLOYER","STATUS");
                dtRole.Columns["LOCATION_ID"].ColumnName = model.Location_Id_TEXT;
                dtRole.Columns["PERIOD_FLAG"].ColumnName = model.Period_Flag_TEXT;
                dtRole.Columns["DED_MONTH"].ColumnName = model.Ded_Month_TEXT;
                dtRole.Columns["STATE_NAME"].ColumnName = model.State_Id_TEXT;
                dtRole.Columns["LWF_EMPLOYEE"].ColumnName = model.Lwf_Employee_TEXT;
                dtRole.Columns["STATUS"].ColumnName = "STATUS";
                //dtRole.Columns["ISACTIVE"].ColumnName = model.is;
                string FileName = ExcellUtils.DataTableToExcel(dtRole);
                string FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + FileName;
                FileName = FileName.Replace("/", "").Replace("..", "").Replace("\\", "");
                byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "LwfMaster.xlsx");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("UploadLWFDetails", Name = "UploadLWFDetails")]
        public ActionResult UploadLWFDetails(string FileName)
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
                LwfRepo objRepo = new LwfRepo();
                string FilePath = Server.MapPath("~/Docs/Temp/" + FileName);
                int SuccessCount = 0;
                int FailCount = 0;
                int CreatedBy = IvapUser.UID;
                int EID = IvapUser.EID;
                string ResultFileName = objRepo.UploadLWFDetails(FilePath, CreatedBy, EID, ref SuccessCount, ref FailCount);
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
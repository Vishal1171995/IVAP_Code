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
    public class CurrencyController : IvapBaseController
    {
        [ViewAction]
        [Route("Currency", Name = "ViewCurrency")]
        public ActionResult ViewCurrency()
        {
            CurrencyModel CurencyModel = new CurrencyModel();
            try
            {
                CurencyModel.EID = IvapUser.EID;
                CurencyModel.SetDisplayName();
                CurencyModel.IsActive = true;
            }
            catch
            {
                throw;
            }
            return View("Currency", CurencyModel);
        }
        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("AddUpdateCurrency", Name = "AddUpdateCurrency")]
        public ActionResult AddUpdate(CurrencyModel Model)
        {
            CurrencyRepo objCurrency = new CurrencyRepo();
            Response res = new Response();
            try
            {
                if (ModelState.IsValid)
                {
                    Model.CreatedBy = IvapUser.UID;
                    Model.EID = IvapUser.EID;
                    res = objCurrency.AddUpdateCurrency(Model);
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
        [Route("GetCurrency", Name = "GetCurrency")]
        public ActionResult GetCurrency(int CID)
        {
            CurrencyRepo CurrencyRepo = new CurrencyRepo();
            //KendoGridUtils
            CurrencyModel model = new CurrencyModel();
            DataTable dt = new DataTable();
            KendoGridUtils res = new KendoGridUtils();
            try
            {
                model.CID = CID;
                // model.RoleID = RoleID;
                dt = CurrencyRepo.GetCurrency(model);
                res = res.GetCommandButtonForGrid("ViewCurrency");
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
        [Route("GetCurrencyHis", Name = "GetCurrencyHis")]
        public ActionResult GetCurrecynHis(int CID)
        {
            CurrencyRepo CurrencyRepo = new CurrencyRepo();
            CurrencyModel model = new CurrencyModel();
            DataTable dt = new DataTable();
            Response res = new Response();
            try
            {
                model.CID = CID;
                dt = CurrencyRepo.GetCurrencyHis(model);
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
        [Route("DownloadAllCurrency", Name = "DownloadAllCurrency")]
        public ActionResult DownloadAllClass()
        {
            CurrencyRepo CurrencyRepo = new CurrencyRepo();
            CurrencyModel model = new CurrencyModel();
            
            DataTable dt = new DataTable();
            try
            {
                // model.CURRENCY_CODE = "0";
                model.EID = IvapUser.EID;
                model.SetDisplayName();
                dt = CurrencyRepo.GetCurrency(model);
                DataTable dtRole = dt.DefaultView.ToTable(false,  "CURRENCY_CODE", "CURRENCY_NAME","ISACTIVE");
               // dtRole.Columns["ENTITY_NAME"].ColumnName = "Entity Name";
                dtRole.Columns["CURRENCY_CODE"].ColumnName = model.CURRENCY_CODE_TEXT;
                dtRole.Columns["CURRENCY_NAME"].ColumnName = model.CURRENCY_NAME_TEXT;
              //  dtRole.Columns["CLASS_NAME"].ColumnName = model.CLASS_NAME_TEXT;

                dtRole.Columns["ISACTIVE"].ColumnName = "Is_Active";
                string FileName = ExcellUtils.DataTableToExcel(dtRole);
                FileName = FileName.Replace("/", "").Replace("..", "").Replace("\\", "");
                string FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + FileName;
                byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "CurrencyMaster.xlsx");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
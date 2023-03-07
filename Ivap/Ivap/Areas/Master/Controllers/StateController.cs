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
    public class StateController : IvapBaseController
    {
        // GET: Master/State
      [ViewAction]
        [Route("State", Name = "ViewState")]
        public ActionResult ViewState()
        {
            StateModel Model = new StateModel();
            Model.EID = IvapUser.EID;
            Model.SetDisplayName();
            Model.IsActive = true;
            return View("State",Model);
        }
         [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("AddUpdateState", Name = "AddUpdateState")]
        public ActionResult AddUpdateState(StateModel Model)
        {
            StateRepo objState = new StateRepo();
            Response res = new Response();
            try
            {
                if (ModelState.IsValid)
                {
                    Model.CreatedBy = IvapUser.UID;
                    res = objState.AddUpdateState(Model);
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
        [Route("GetState", Name = "GetState")]
        public ActionResult GetState(int StateId)

        {
            StateRepo StateRepo = new StateRepo();
            //KendoGridUtils
            StateModel model = new StateModel();
            DataTable dt = new DataTable();
            KendoGridUtils res = new KendoGridUtils();
            try
            {
                model.StateId = StateId;
                // model.RoleID = RoleID;
                dt = StateRepo.GetState(model);
                res = res.GetCommandButtonForGrid("ViewState");
                res.Data = JsonSerializer.SerializeTable(dt);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                throw;
            }
        }
        [ViewAction]
        [Route("GetStateHis", Name = "GetStateHis")]
        public ActionResult GetStateHis(int StateId)
        {
            StateRepo StateRepo = new StateRepo();
            StateModel model = new StateModel();
            DataTable dt = new DataTable();
            Response res = new Response();
            try
            {
                model.StateId = StateId;
                dt = StateRepo.GetStateHis(model);
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
        [Route("DownloadAllState", Name = "DownloadAllState")]
        public ActionResult DownloadAllState()
        {
            StateRepo StateRepo = new StateRepo();
            StateModel model = new StateModel();
            DataTable dt = new DataTable();
            try
            {
                model.EID = IvapUser.EID;
                model.SetDisplayName();
                model.StateId = 0;
                dt = StateRepo.GetState(model);
                DataTable dtState = dt.DefaultView.ToTable(false, "TID","COUNTRY", "STATE_CODE", "STATE_NAME", "STATUS");
                dtState.Columns["TID"].ColumnName = "StateID";
                dtState.Columns["COUNTRY"].ColumnName = "Country_Name";
                dtState.Columns["STATE_CODE"].ColumnName = "State Code";
                dtState.Columns["STATE_NAME"].ColumnName = "State Name";
                dtState.Columns["STATUS"].ColumnName = "Status";
                string FileName = ExcellUtils.DataTableToExcel(dtState);
                FileName = FileName.Replace("/", "").Replace("..", "").Replace("\\", "");
                string FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + FileName;
                byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "StateMaster.xlsx");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
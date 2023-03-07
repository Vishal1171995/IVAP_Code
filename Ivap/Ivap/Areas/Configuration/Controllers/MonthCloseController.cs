using Ivap.ActionFilters;
using Ivap.Areas.Configuration.Models;
using Ivap.Areas.Configuration.Repository;
using Ivap.Controllers;
using Ivap.CustomAttribute;
using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ivap.Areas.Configuration.Controllers
{
    [CustomAuthActionFilter(Order = 0)]
    [RouteArea("Configuration", AreaPrefix = "")]
    [RoutePrefix("Configuration")]
    public class MonthCloseController : IvapBaseController
    {
        [ViewAction]
        [Route("MonthClose", Name = "MonthClose")]
        public ActionResult MonthClose()
        {
            try
            {
                return View();
            }
            catch
            {
                throw;
            }
        }
        [ViewAction]
        [HttpGet]
        [Route("GetMonthClose", Name = "GetMonthClose")]
        public ActionResult GetMonthClose(int TID)
        {
            MonthCloseRepo objUBO = new MonthCloseRepo();
            MonthCloseModel objModel = new MonthCloseModel();

            KendoGridUtils res = new KendoGridUtils();
            try
            {
                objModel.TID = TID;
                objModel.EID = IvapUser.EID;
                objModel.CreatedBy = IvapUser.UID;
                DataTable dt = objUBO.GetMonthClose(objModel);
                res = objUBO.GetCommandButtonForGrid("MonthClose");
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
        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("SetStatus", Name = "SetStatus")]
        public ActionResult SetStatus(int TID, string Status)
        {
            MonthCloseRepo objRepo = new MonthCloseRepo();
            MonthCloseModel objModel = new MonthCloseModel();
            Response res = new Response();
            try
            {
                objModel.CreatedBy = IvapUser.UID; //TmsUser.UID;
                objModel.TID = TID;
                objModel.EID = IvapUser.EID;
                objModel.Status = Status;
                res = objRepo.SetStatus(objModel);
                return Json(res);
            }
            catch
            {
                throw;
            }
        }

        [ViewAction]
        [HttpGet]
        [Route("GetMonthCloseHistory", Name = "GetMonthCloseHistory")]
        public ActionResult GetMonthCloseHistory(int TID)
        {
            MonthCloseRepo objRepo = new MonthCloseRepo();
            MonthCloseModel objModel = new MonthCloseModel();
            Response ret = new Response();
            try
            {
                objModel.TID = TID;
                ret.Data = JsonSerializer.SerializeTable(objRepo.GetMonthCloseHistory(objModel));
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

        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("SetDefaultCurrentMonth", Name = "SetDefaultCurrentMonth")]
        public ActionResult SetDefaultCurrentMonth()
        {
            MonthCloseRepo objRepo = new MonthCloseRepo();
            MonthCloseModel objModel = new MonthCloseModel();
            Response res = new Response();
            try
            {
                objModel.CreatedBy = IvapUser.UID; //TmsUser.UID;
                objModel.EID = IvapUser.EID;
                res = objRepo.SetDefaultCurrentMonth(objModel);
                return Json(res);
            }
            catch
            {
                throw;
            }
        }

        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("SetDefaultMonth", Name = "SetDefaultMonth")]
        public ActionResult SetDefaultMonth(int TID)
        {
            MonthCloseRepo objRepo = new MonthCloseRepo();
            MonthCloseModel objModel = new MonthCloseModel();
            Response res = new Response();
            try
            {
                objModel.CreatedBy = IvapUser.UID; //TmsUser.UID;
                objModel.EID = IvapUser.EID;
                objModel.TID = TID;
                res = objRepo.SetDefaultMonth(objModel);
                return Json(res);
            }
            catch
            {
                throw;
            }
        }
    }
}
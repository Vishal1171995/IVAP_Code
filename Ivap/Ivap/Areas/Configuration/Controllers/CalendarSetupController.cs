using Ivap.ActionFilters;
using Ivap.Areas.Calendar.Models;
using Ivap.Areas.Calendar.Repository;
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
    public class CalendarSetupController : IvapBaseController
    {
        [ViewAction]
        [Route("CalendarSetup", Name = "CalendarSetup")]
        public ActionResult CalendarSetup()
        {
            CalendarSetupModel objModel = new CalendarSetupModel();
            CalendarSetupRepo objRepo = new CalendarSetupRepo();
            try
            {
                objModel.CalendarTypeList = DropdownUtils.ToSelectList(objRepo.GetCalendarType(), "TID", "NAME");
                return View(objModel);
            }
            catch
            {
                throw;
            }
        }
        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("AddUpdateCalendarSetup", Name = "AddUpdateCalendarSetup")]
        public ActionResult AddUpdateCalendarSetup(CalendarSetupModel Model)
        {
            CalendarSetupRepo objrepo = new CalendarSetupRepo();
            Response res = new Response();
            try
            {
                if (ModelState.IsValid)
                {
                    Model.CreatedBy = IvapUser.UID;
                    Model.EID = IvapUser.EID;
                    res = objrepo.AddUpdateCalendarSetup(Model);
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
        [Route("GetCalendarSetup", Name = "GetCalendarSetup")]
        public ActionResult GetCalendarSetup(int TID)
        {
            CalendarSetupRepo objUBO = new CalendarSetupRepo();
            CalendarSetupModel objModel = new CalendarSetupModel();

            KendoGridUtils res = new KendoGridUtils();
            try
            {
                objModel.TID = TID;
                objModel.EID = IvapUser.EID;
                objModel.CreatedBy = IvapUser.UID;
                DataTable dt = objUBO.GetCalendarSetup(objModel);
                res = objUBO.GetCommandButtonForGrid("CalendarSetup");
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
        [Route("GetCalendarSetupHistory", Name = "GetCalendarSetupHistory")]
        public ActionResult GetCalendarSetupHistory(int TID)
        {
            CalendarSetupRepo objRepo = new CalendarSetupRepo();
            CalendarSetupModel objModel = new CalendarSetupModel();
            Response ret = new Response();
            try
            {
                objModel.TID = TID;
                ret.Data = JsonSerializer.SerializeTable(objRepo.GetCalendarSetupHistory(objModel));
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
        [Route("DeleteCalendarSetup", Name = "DeleteCalendarSetup")]
        public ActionResult DeleteCalendarSetup(int TID)
        {
            CalendarSetupRepo objrepo = new CalendarSetupRepo();
            CalendarSetupModel objModel = new CalendarSetupModel();
            Response res = new Response();
            try
            {
                objModel.TID = TID;
                objModel.CreatedBy = IvapUser.UID;
                objModel.EID = IvapUser.EID;
                res = objrepo.DeleteCalendarSetup(objModel);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                throw;
            }
        }
        [ViewAction]
        [HttpGet]
        [Route("GetCalendarSetupData", Name = "GetCalendarSetupData")]
        public ActionResult GetCalendarSetupData(int CalendarType,string PayDate,string DueDate,string Event,string FileType)
        {
            JsonResult result = new JsonResult();
            CalendarSetupRepo objrepo = new CalendarSetupRepo();
            try
            {
                // Loading.
                List<CalendarDetailsModel> data = objrepo.GetCalendarSetupForCalendarView(IvapUser.EID, CalendarType, PayDate, DueDate, Event, FileType);

                result = this.Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }

            // Return info.
            return result;
        }
    }
}
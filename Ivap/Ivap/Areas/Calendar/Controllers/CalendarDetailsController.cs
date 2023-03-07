using Ivap.ActionFilters;
using Ivap.Areas.Calendar.Models;
using Ivap.Areas.Calendar.Repository;
using Ivap.Controllers;
using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ivap.Areas.Calendar.Controllers
{
    [CustomAuthActionFilter(Order = 0)]
    [RouteArea("Calendar", AreaPrefix = "")]
    [RoutePrefix("Calendar")]
    public class CalendarDetailsController : IvapBaseController
    {
        [Route("CalendarDetails", Name = "CalendarDetails")]
        public ActionResult CalendarDetails(int CalendarType)
        {
            ViewBag.CalendarType = CalendarType;
            return View();
        }
        [HttpGet]
        [Route("GetCalendarData", Name = "GetCalendarData")]
        public ActionResult GetCalendarData(int CalendarType,string PayDate,string DueDate,string Event,string FileType)
        {
            JsonResult result = new JsonResult();
            CalendarDetailsRepo objRepo = new CalendarDetailsRepo();
            try
            {
                // Loading.
                List<CalendarDetailsModel> data = objRepo.LoadData(IvapUser.EID, CalendarType, PayDate, DueDate, Event, FileType);

                result = this.Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }

            // Return info.
            return result;
        }

        [HttpGet]
        [Route("GetUserByRole", Name = "GetUserByRole")]
        public ActionResult GetUserByRole(string Role)
        {
            JsonResult result = new JsonResult();
            CalendarDetailsRepo objRepo = new CalendarDetailsRepo();
            DataTable dt = new DataTable();
            Response res = new Response();
            try
            {
                dt = objRepo.GetUserByRole(IvapUser.EID, Role);
                res.Data = JsonSerializer.SerializeTable(dt);
                res.IsSuccess = true;
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
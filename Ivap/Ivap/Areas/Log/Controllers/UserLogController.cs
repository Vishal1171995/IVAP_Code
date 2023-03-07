using Ivap.ActionFilters;
using Ivap.Areas.Log.Models;
using Ivap.Areas.Log.Repository;

using Ivap.Controllers;
using Ivap.CustomAttribute;
using Ivap.Models;
using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ivap.Areas.Log.Controllers
{
    [CustomAuthActionFilter(Order = 0)]
    [CustomAuthrizationActionFilter(Order = 1)]
    [RouteArea("Log", AreaPrefix = "")]
    [RoutePrefix("Log")]
    public class UserLogController : IvapBaseController
    {
        // GET: Log/UserLog
        [ViewAction]
        [Route("UserLog", Name = "UserLog")]
        public ActionResult UserLog()
        {
            return View();
        }
      

        //[Route("UserLogDetail", Name = "UserLogDetail")]
        //public ActionResult UserLogDetail()
        //{
        //    LogDetailRepo objRepo = new LogDetailRepo();
        //    int uid = ((AppUser)Session["uBo"]).UID;
        //    DataTable dt = objRepo.GetUserLog(uid);
        //    Response res = new Response();
        //    res.Data = JsonSerializer.SerializeTable(dt);
        //    res.IsSuccess = true;
        //    return Json(res, JsonRequestBehavior.AllowGet);
        //}

        [ViewAction]
        [HttpPost]
        [Route("UserLogDetail", Name = "UserLogDetail")]
        public ActionResult UserLogDetail(int page, int pageSize, int skip, int take, List<SortDescription> sorting, FilterContainer filter)
        {
            DataSet Ds = new DataSet();
            DataTable dt = new DataTable();
            UserLogRepo objRepo = new UserLogRepo();
            GridUserLogClass grd = new GridUserLogClass();

            grd.EID = IvapUser.EID;
            Response ret = new Utils.Response();
            try
            {
                int from = skip + 1;
                int to = take * page;
                string sortingStr = "";
                #region Sorting
                if (sorting != null)
                {
                    sortingStr = objRepo.sortGrid(sorting);
                }
                #endregion
                #region filtering
                string filters = "";
                if (filter != null)
                {
                    filters = objRepo.FilterGrid(filter);
                }
                #endregion
                sortingStr = sortingStr.TrimStart(',');
                if (sortingStr == "") sortingStr = null;
                if (filters == "") filters = null;
                grd.from = from;
                grd.To = to;
                grd.FilterStr = filters;
                grd.SortingStr = sortingStr;
                Ds = objRepo.GetUserLog_New(grd);
                string data = JsonSerializer.SerializeTable(Ds.Tables[0]);
                ret.IsSuccess = true;
                ret.Data = "{\"Data\":" + data + ",\"Total\":" + Ds.Tables[1].Rows[0]["TotalCount"] + "}";
                ret.Message = "success";
            }
            catch
            {
                ret.IsSuccess = true;
                ret.Data = "{\"Data\":[],\"Total\":" + 0 + "}";
            }
            var jsonResult = Json(ret, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
    }
}
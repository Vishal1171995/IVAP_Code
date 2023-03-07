using Ivap.ActionFilters;
using Ivap.Areas.Configuration.Models;
using Ivap.Areas.Master.Models;
using Ivap.CustomAttribute;
using Ivap.Utils;
using IVap.Areas.Master.Repository;
using System;
using System.Web.Mvc;
using Ivap.Areas.Configuration.Repository;
using Ivap.Controllers;
using System.Data;
using System.Collections.Generic;

namespace Ivap.Areas.Configuration.Controllers
{
    [CustomAuthActionFilter(Order = 0)]
    [CustomAuthrizationActionFilter(Order = 1)]
    [RouteArea("Configuration", AreaPrefix = "")]
    [RoutePrefix("Configuration")]
    public class DataAccessControlController : IvapBaseController
    {
        // GET: Configuration/DataAccessControl
        [ViewAction]
        [Route("DataAccessControl", Name = "DataAccessControl")]
        public ActionResult DataAccessControl()
        {
            DataAccessControlModel Model = new DataAccessControlModel();
            Repository.DataAccessControlRepo ObjAccessRepo = new Repository.DataAccessControlRepo();
            UserModel objUsermodel = new UserModel();
            UserRepo objUserRepo = new UserRepo();
            MenuModel objMenuModel = new MenuModel();
            MenuRepo objMenuRepo = new MenuRepo();
            DataTable dt = new DataTable();
            try
            {
                objUsermodel.EID = IvapUser.EID;
                //int EID = IvapUser.EID;
                DataTable Dt = objUserRepo.GetUser(objUsermodel);
                ViewBag.SelectedValue = Dt.Rows[0]["UID"];
                Model.UserList = DropdownUtils.ToSelectList(Dt, "UID", "USERID");
                //ViewBag.MenuData = DropdownUtils.ToSelectList(objMenuRepo.GetMenu(objMenuModel), "TID", "NAME");
                dt = ObjAccessRepo.GetMenuAccess(IvapUser.EID);
                DataTable dtRole = dt.DefaultView.ToTable(false, "TID", "NAME", "ROUTE");
                ViewBag.SelMaster = dtRole.Rows[0]["ROUTE"];
                ViewBag.MenuData = dtRole.AsEnumerable();
            }
            catch (Exception ex)
            {
                throw;
            }
            return View("DataAccessControl", Model);
        }

        [ViewAction]
        [HttpGet]
        [Route("GetAccessControl", Name = "GetAccessControl")]
        public ActionResult GetAccessControl(string ActionName, int UID)
        {

            Repository.DataAccessControlRepo DataAccessRepo = new Repository.DataAccessControlRepo();
            DataTable dt = new DataTable();
            KendoGridUtils res = new KendoGridUtils();
            try
            {

                dt = DataAccessRepo.GetAccessControl(ActionName, IvapUser.EID, UID);

                res.Data = JsonSerializer.SerializeTable(dt);
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
        [Route("AddUpdateDataAccess", Name = "AddUpdateDataAccess")]
        public ActionResult AddUpdateDataAccess(string AccessCheck, string ActionName, int UID)
        {

            Response res = new Response();
            AccessCheck += ",";
            Repository.DataAccessControlRepo objRepo = new Repository.DataAccessControlRepo();
            try
            {
                res = objRepo.AddUpdateDataAccess(AccessCheck, ActionName, UID, IvapUser.EID);
                return Json(res);


            }
            catch
            {
                throw;
            }
        }

        [ViewAction]
        [HttpGet]
        [Route("CopyRightUser", Name = "CopyRightUser")]
        public ActionResult CopyRightUser(int UID)
        {

            Repository.DataAccessControlRepo DataAccessRepo = new Repository.DataAccessControlRepo();
            DataTable dt = new DataTable();

            DataAccessControlModel model = new DataAccessControlModel();
            KendoGridUtils res = new KendoGridUtils();

            try
            {
                dt = DataAccessRepo.CopyRightUser(UID, IvapUser.EID);
                res.Data = JsonSerializer.SerializeTable(dt);
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
        [Route("CopyRightSubmit", Name = "CopyRightSubmit")]
        public ActionResult CopyRightSubmit(int COPYID, string UID)
        {
            DataTable dt = new DataTable();
            Response res = new Response();
            Repository.DataAccessControlRepo objRepo = new Repository.DataAccessControlRepo();
            try
            {
                res = objRepo.CopyToAnotherUser(COPYID, UID, IvapUser.EID);
                return Json(res);
            }

            
            catch (Exception ex)
            {
                throw;
            }

        }
            
        
    }
}
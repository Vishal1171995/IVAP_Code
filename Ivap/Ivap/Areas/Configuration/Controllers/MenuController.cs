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
    [CustomAuthrizationActionFilter(Order = 1)]
    [RouteArea("Configuration", AreaPrefix = "")]
    [RoutePrefix("Configurations")]
    public class MenuController : IvapBaseController
    {
        // GET: Configuration/Menu
        [ViewAction]
        [Route("Menu", Name = "ViewMenu")]
        public ActionResult Menu()
        {
            try
            {
                return View("Menu");
            }
            catch { throw; }
        }
        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("UpdateMenu", Name = "UpdateMenu")]
        public ActionResult UpdateMenu(int TID,string MenuName, string Roles)
        {
            MenuRepo objRepo = new MenuRepo();
            MenuModel objModel = new MenuModel();
            Response res = new Response();
            try
            {
                objModel.CreatedBy = IvapUser.UID; //TmsUser.UID;
                objModel.TID = TID;
                objModel.EID = IvapUser.EID;
                objModel.MenuName = MenuName;
                objModel.Roles = Roles;
                res = objRepo.UpdateMenu(objModel);
                return Json(res);
            }
            catch
            {
                throw;
            }
        }
        [ViewAction]
        [HttpGet]
        [Route("GetMenu", Name = "GetMenu")]
        public ActionResult GetMenu(int TID,string MenuName)
        {
            MenuRepo objRepo = new MenuRepo();
            //KendoGridUtils
            MenuModel model = new MenuModel();
            DataTable dt = new DataTable();
            Response res = new Response();
            //KendoGridUtils res = new KendoGridUtils();
            try
            {
                model.TID = TID;
                model.EID = IvapUser.EID;
                model.MenuName = MenuName;
                dt = objRepo.GetMenu(model);
                //res = res.GetCommandButtonForGrid("ViewRoles");
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
        [Route("GetRolesByMenu", Name = "GetRolesByMenu")]
        public ActionResult GetRolesByMenu(int RoleID)
        {
            MenuRepo objRepo = new MenuRepo();
            DataTable dt = new DataTable();
            Response res = new Response();
            try
            {
                res = objRepo.GetRoles(RoleID);
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
        [Route("SetDisplayOrder_UP", Name = "SetDisplayOrder_UP")]
        public ActionResult SetDisplayOrder_UP(int TID, string Name,int PID)
        {
            MenuRepo objRepo = new MenuRepo();
            MenuModel objModel = new MenuModel();
            Response res = new Response();
            try
            {
                objModel.CreatedBy = IvapUser.UID; //TmsUser.UID;
                objModel.TID = TID;
                objModel.EID = IvapUser.EID;
                objModel.MenuName = Name;
                objModel.PID = PID;
                res = objRepo.SetDisplayOrder_UP(objModel);
                return Json(res);
            }
            catch(Exception ex)
            {
                throw;
            }
        }
        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("SetDisplayOrder_down", Name = "SetDisplayOrder_down")]
        public ActionResult SetDisplayOrder_down(int TID, string Name, int PID)
        {
            MenuRepo objRepo = new MenuRepo();
            MenuModel objModel = new MenuModel();
            Response res = new Response();
            try
            {
                objModel.CreatedBy = IvapUser.UID; //TmsUser.UID;
                objModel.TID = TID;
                objModel.EID = IvapUser.EID;
                objModel.PID = PID;
                objModel.MenuName = Name;
                res = objRepo.SetDisplayOrder_down(objModel);
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
        [Route("ActiveInactiveMenu", Name = "ActiveInactiveMenu")]
        public ActionResult ActiveInactiveMenu(int TID, int IsAct)
        {
            MenuRepo objRepo = new MenuRepo();
            MenuModel objModel = new MenuModel();
            Response res = new Response();
            try
            {
                objModel.CreatedBy = IvapUser.UID; //TmsUser.UID;
                objModel.TID = TID;
                objModel.EID = IvapUser.EID;
                objModel.IsActive = Convert.ToBoolean(IsAct);
                res = objRepo.ActiveInactiveMenu(objModel);
                return Json(res);
            }
            catch
            {
                throw;
            }
        }
    }

}
using Ivap.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ivap.Controllers
{
    public class AuthorizationController : IvapBaseController
    {

        public ActionResult Unauthorized()
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
        // GET: RenderMenu
        [ChildActionOnly]
        public ActionResult RenderMenu()
        {

            try
            {
                AuthorizationRepo aObj = new AuthorizationRepo();
                DataTable dtMenu = new DataTable();
                dtMenu = (DataTable)Session["uMenu"];
                //if (Session["uMenu"] == null)
                //{
                //    dtMenu = aObj.GetUserMenu(TmsUser.RoleName);
                    //Session["uMenu"] = dtMenu;
                //}
                //else
                //{
                //    dtMenu = (DataTable)Session["uMenu"];
                //}
                return View(dtMenu);
            }
            catch
            {
                throw;
            }
        }

        [ChildActionOnly]
        public ActionResult RenderMasterButton(string RouteName)
        {

            try
            {
                ViewBag.RouteName = RouteName;
                return View();
            }
            catch
            {
                throw;
            }
        }
    }
}
using Ivap.ActionFilters;
using Ivap.Areas.MOM.Factory;
using Ivap.Areas.MOM.Models;
using Ivap.Areas.MOM.Repository;
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


namespace Ivap.Areas.MOM.Controllers
{
    [CustomAuthActionFilter(Order = 0)]
    [RouteArea("MOM", AreaPrefix = "")]
    [RoutePrefix("MOM")]
    public class MOMController : IvapBaseController
    {
        // GET: MOM/MOM
        [Route("ViewMom", Name = "ViewMom")]
        public ActionResult GetMom()
        {
            //FactoryMom.Create("PLANMOM");
            var model = new PlanMom();
            model.CreatedBy = IvapUser.UID;
            model.EID = IvapUser.EID;
            MomRepo objMomRepo = new MomRepo();
            DataTable dt = objMomRepo.GetMomCountDetails(EntityID: IvapUser.EID);
            if (dt.Rows.Count > 0)
            {
                ViewBag.All = dt.Rows[0]["TotalMinutes"];
                ViewBag.Pending = dt.Rows[0]["TotalPending"];
                ViewBag.Closed = dt.Rows[0]["TotalClosed"];
            }
            else {
                ViewBag.All = "0";
                ViewBag.Pending = "0";
                ViewBag.Closed = "0";
            }
            return View(model);
        }

        [ViewAction]
        [HttpPost]
        [Route("BindAddMinutes", Name = "BindAddMinutes")]
        public PartialViewResult BindAddMinutes(int MOMID)
        {
            //TripModel TripModel = new TripModel();
            //TripRepo trpRepo = new TripRepo();
            try
            {

                //ViewBag.UID = TmsUser.UID;

                //TripModel.TripIDs = TripID;
                //TripModel.CreatedBy = TmsUser.UID;
                //Ds = trpRepo.GetTripDetails(TripModel);
                return PartialView();
            }
            catch { throw; }
        }


        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("AddMom", Name = "AddMom")]
        public ActionResult AddMom(CreateMOM Model)
        {
            Response res = new Response();
            MomRepo objMomRepo = new MomRepo();
            try
            {
                if (ModelState.IsValid)
                {
                    Model.CreatedBy = IvapUser.UID;
                    Model.EID = IvapUser.EID;
                    Model.MID = 0;
                    Model.IsActive = true;
                    res = objMomRepo.AddMOM_ITEM(Model);
                    return Json(res, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return View();
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                throw;
            }
        }


        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("UpdateMom", Name = "UpdateMom")]
        public ActionResult UpdateMom(UpdateMoM Model)
        {
            Response res = new Response();
            MomRepo objMomRepo = new MomRepo();
            try
            {
                if (ModelState.IsValid)
                {
                    Model.CreatedBy = IvapUser.UID;
                    Model.EID = IvapUser.EID;

                    res = objMomRepo.UpdateMOM_ITEM(Model);
                    return Json(res, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return View();
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                throw;
            }
        }


        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("AddMinutes", Name = "AddMinutes")]
        public ActionResult AddMinutes(BaseMinutesDetails Model) {
            try
            {
                Response res= new Response();
                MomRepo objMomRepo; 
                if (ModelState.IsValid) {
                    objMomRepo= new MomRepo();
                    Model.EID = IvapUser.EID;
                    res = objMomRepo.AddMinutes(Model);
                }
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex) {
                throw ex;
            }
        }


        [HttpGet]
        [Route("UpdateMomRequest", Name = "UpdateMomRequest")]
        public ActionResult UpdateMomRequest(int MOMID)
        {
            try
            {
                UpdateMoM objModel = new UpdateMoM();
                objModel.MOMID = MOMID;
                MomRepo objMomRepo = new MomRepo();
                objModel = objMomRepo.GetUpdateRequest(MOMID: MOMID, Entity_ID: IvapUser.EID);
                return View(objModel);
            }
            catch
            {
                throw;
            }

        }


        [ViewAction]
        [HttpGet]
        [Route("GetMomDetails", Name = "GetMomDetails")]
        public ActionResult GetMomDetails(int MomID, string filter)
        {
            try
            {
                MomRepo objMomRepo = new MomRepo();
                KendoGridUtils res = new KendoGridUtils();

                var momModel = FactoryMom.Create("PLANMOM");
                momModel.MID = MomID;
                momModel.EID = IvapUser.EID;
                momModel.CreatedBy = IvapUser.UID;
                momModel.filter = filter;
                DataTable dt = objMomRepo.GetMomDetails(momModel);
                res = res.GetCommandButtonForGrid("ViewMom");
                //var ViewDataCommand = res.Command.command.SingleOrDefault(r => r.name.ToUpper() == "VIEW");
                //if (ViewDataCommand != null) { res.Command.command.Remove(ViewDataCommand); }
                res.Data = JsonSerializer.SerializeTable(dt);
                var UserGrid = Json(res, JsonRequestBehavior.AllowGet);
                UserGrid.MaxJsonLength = int.MaxValue;
                return UserGrid;
            }
            catch
            {
                throw;
            }
        }


        [ViewAction]
        [HttpGet]
        [Route("GetMomItemDetails", Name = "GetMomItemDetails")]
        public ActionResult GetMomItemDetails(int MomID)
        {
            try
            {
                MomRepo objMomRepo = new MomRepo();
                KendoGridUtils res = new KendoGridUtils();

                var momModel = FactoryMom.Create("PLANMOM");
                momModel.MID = MomID;
                momModel.EID = IvapUser.EID;
                momModel.CreatedBy = IvapUser.UID;
                DataTable dt = objMomRepo.GetMomItemDetails(momModel);
                res.Data = JsonSerializer.SerializeTable(dt);
                var UserGrid = Json(res, JsonRequestBehavior.AllowGet);
                UserGrid.MaxJsonLength = int.MaxValue;
                return UserGrid;
            }
            catch
            {
                throw;
            }
        }

        [ViewAction]
        [HttpPost]
        [Route("DeleteMomItemDetails", Name = "DeleteMomItemDetails")]
        public ActionResult DeleteMomItemDetails(int ItemID)
        {
            try
            {
                Response res = new Response();
                MomRepo objMomRepo = new MomRepo();
                res = objMomRepo.DeleteMomItemDetails(Item_ID: ItemID);
                var UserGrid = Json(res, JsonRequestBehavior.AllowGet);
                return UserGrid;
            }
            catch
            {
                throw;
            }
        }

        [ViewAction]
        [HttpGet]
        [Route("GetMOMGridCalendar", Name = "GetMOMGridCalendar")]
        public ActionResult GetMOMGridCalendar(string Status)
        {
            try
            {
                JsonResult result = new JsonResult();
                MomRepo objMomRepo = new MomRepo();
                int EID = IvapUser.EID;
                List<MOMGrid> data = objMomRepo.GetMOMCalendarData(MOMID: 0, ENTITYID: IvapUser.EID, UID: IvapUser.UID,Status: Status);

                result = this.Json(data, JsonRequestBehavior.AllowGet);

                return result;
            }
            catch
            {
                throw;
            }

        }

        [ViewAction]
        [HttpGet]
        [Route("GetAttendees", Name = "GetAttendees")]
        public ActionResult GetAttendees(int MoMID)
        {
            try
            {
                MomRepo objMomRepo = new MomRepo();
                Response res = new Response();
                int EID = IvapUser.EID;
                DataTable dt = objMomRepo.GetAttendeesData(MOMID: MoMID, EntityID: IvapUser.EID);
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
        [Route("GetTotalMOM", Name = "GetTotalMOM")]
        public ActionResult GetTotalMOM(int MoMID)
        {
            try
            {
                MomRepo objMomRepo = new MomRepo();
                Response res = new Response();
                int EID = IvapUser.EID;
                DataTable dt = objMomRepo.GetTotalMoMData(MOMID: MoMID, EntityID: IvapUser.EID,Status:"All");
                res.Data = JsonSerializer.SerializeTable(dt);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                throw;
            }

        }

        [ViewAction]
        [HttpGet]
        [Route("GetTotalPendingMOM", Name = "GetTotalPendingMOM")]
        public ActionResult GetTotalPendingMOM(int MoMID)
        {
            try
            {
                MomRepo objMomRepo = new MomRepo();
                Response res = new Response();
                int EID = IvapUser.EID;
                DataTable dt = objMomRepo.GetTotalMoMData(MOMID: MoMID, EntityID: IvapUser.EID, Status: "Pending");
                res.Data = JsonSerializer.SerializeTable(dt);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                throw;
            }

        }


        [ViewAction]
        [HttpGet]
        [Route("GetTotalClosedMOM", Name = "GetTotalClosedMOM")]
        public ActionResult GetTotalClosedMOM(int MoMID)
        {
            try
            {
                MomRepo objMomRepo = new MomRepo();
                Response res = new Response();
                int EID = IvapUser.EID;
                DataTable dt = objMomRepo.GetTotalMoMData(MOMID: MoMID, EntityID: IvapUser.EID, Status: "Closed");
                res.Data = JsonSerializer.SerializeTable(dt);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                throw;
            }

        }

        [ViewAction]
        [HttpGet]
        [Route("GetTotalDiscardedMOM", Name = "GetTotalDiscardedMOM")]
        public ActionResult GetTotalDiscardedMOM(int MoMID)
        {
            try
            {
                MomRepo objMomRepo = new MomRepo();
                Response res = new Response();
                int EID = IvapUser.EID;
                DataTable dt = objMomRepo.GetTotalMoMData(MOMID: MoMID, EntityID: IvapUser.EID, Status: "Discarded");
                res.Data = JsonSerializer.SerializeTable(dt);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                throw;
            }

        }

        [Route("CreateMOM", Name = "CreateMOM")]
        public ActionResult CreateMOM()
        {
            try
            {
                CreateMOM objModel = new CreateMOM();
                objModel.MID = 0;

                return View(objModel);
            }
            catch
            {
                throw;
            }
        }

        //[Route("CreateMOMDate", Name = "CreateMOMDate")]
        //public ActionResult CreateMOM(string MeetingDate)
        //{
        //    try
        //    {
        //        CreateMOM objModel = new CreateMOM();
        //        objModel.MID = 0;
        //        objModel.MeetingHeld = MeetingDate;
        //        return View(objModel);
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}
        [ViewAction]
        [HttpGet]
        [Route("ViewMOMRequest", Name = "ViewMOMRequest")]
        public ActionResult ViewMOMRequest(int MOMID)
        {
            try
            {
                JsonResult result = new JsonResult();
                MomRepo objMomRepo = new MomRepo();
                int EID = IvapUser.EID;
                Response res = new Response();
                res = objMomRepo.GetViewMOM(MOMID: MOMID, ENTITY_ID: EID);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                throw;
            }

        }

        //Read file from folder 
        [ViewAction]
        [HttpGet]
        [Route("DownloadFileMOM", Name = "DownloadFileMOM")]
        public ActionResult DownloadFileMOM(int MOMID, string System_File_Name,string Original_File_Name )
        {
            try
            {
                System_File_Name = System_File_Name.Replace("/", "").Replace("..", "").Replace("\\", "");
                string FilePath;
                if (MOMID == 0)
                {
                    FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + System_File_Name;
                }
                else {
                   FilePath = HostingEnvironment.MapPath("~/Docs/Entity_"+IvapUser.EID+ "/MOM/" + MOMID+"/") + System_File_Name;
                }
                
                byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, Original_File_Name);
            }
            catch {
                throw;
            }
        }

    }
}
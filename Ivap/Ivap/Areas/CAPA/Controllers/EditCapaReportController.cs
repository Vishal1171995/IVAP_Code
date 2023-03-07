using Ivap.ActionFilters;
using Ivap.Areas.CAPA.Models;
using Ivap.Areas.CAPA.Repository;
using Ivap.Controllers;
using Ivap.CustomAttribute;
using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ivap.Areas.CAPA.Controllers
{
    [CustomAuthActionFilter(Order = 0)]
    [CustomAuthrizationActionFilter(Order = 1)]
    [RouteArea("CAPA", AreaPrefix = "")]
    [RoutePrefix("CAPA")]
    public class EditCapaReportController : IvapBaseController
    {
        [ViewAction]
        [Route("EditCapaReport", Name = "EditCapaReport")]
        public ActionResult EditCapaReport()
        {
            return View();
        }

        [ViewAction]
        [HttpGet]
        [Route("GetEditCapa", Name = "GetEditCapa")]
        public ActionResult GetEditCapa(int CapaID)
        {
            EditCapaRepo objRepo = new EditCapaRepo();
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            CapaModel objCapaModel = new CapaModel();
            PreventiveDetailModel objPreventModel = new PreventiveDetailModel();
            CorrectiveDetailModel objCorrectModel = new CorrectiveDetailModel();
            try
            {

                // Model.CreatedBy = IvapUser.UID;
                int EID = IvapUser.EID;

                ds = objRepo.GetCapa(CapaID, EID);
                var CapaData = JsonSerializer.SerializeTable(ds.Tables[0]);
                var CorrectData = JsonSerializer.SerializeTable(ds.Tables[1]);
                var PreventData = JsonSerializer.SerializeTable(ds.Tables[2]);

                var CorrectConversationData = JsonSerializer.SerializeTable(ds.Tables[3]);
                var PreventConversationData = JsonSerializer.SerializeTable(ds.Tables[4]);
                KendoGridUtils res = new KendoGridUtils();
                res = res.GetCommandButtonForGrid("CapaReport");
                res.Data = "{\"CData\":" + CapaData + ",\"CorrectiveData\":" + CorrectData + ",\"PreventiveData\":" + PreventData + ",\"CorrectRemarkData\":" + CorrectConversationData + ",\"PreventRemarkData\":" + PreventConversationData + "}";
                var UserGrid = Json(res, JsonRequestBehavior.AllowGet);
                UserGrid.MaxJsonLength = int.MaxValue;
                return UserGrid;
            }
            catch
            {
                throw;
            }
        }



        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("UpdateCapa", Name = "UpdateCapa")]
        public ActionResult UpdateCapa(CapaModel Model)
        {
            CapaRepo objCapaRepo = new CapaRepo();
            Response res = new Response();
            int CapaID =Convert.ToInt32(Model.TID);
            try
            {

                Model.CreatedBy = IvapUser.UID;

                Model.EID = IvapUser.EID;

                int CapaIDs = objCapaRepo.AddUpdateCapa(Model);

                if (CapaID < 0)
                {
                    res.Message = "Failed! Please try again.";
                    res.IsSuccess = false;
                    return Json(res);
                }

                if (Model.CorrectiveAction_Detail != null)
                {
                  
                    var Correctivecollection = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CorrectiveDetailModel_Update>>(Model.CorrectiveAction_Detail);
                    if (Correctivecollection != null)
                    {
                        foreach (CorrectiveDetailModel_Update objCorrective in Correctivecollection)
                        {
                            res = objCapaRepo.AddUpdateCapaCorrective(Capa_Update:objCorrective.TID, Capa_ID: CapaID, Corrective_Action: objCorrective.Corrective_Action, Corrective_Text: objCorrective.Action_Text, Corrective_Owner: objCorrective.Action_Owner, UID: Model.CreatedBy,Email:Model.CorrectiveAction_Email);
                            if (res.IsSuccess == false)
                            {
                                break;
                            }
                        }
                    }
                }

                if (Model.PreventiveAction_Detail != null)
                {
                   
                    var Preventivecollection = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PreventiveDetailModel_Update>>(Model.PreventiveAction_Detail);
                    if (Preventivecollection != null)
                    {
                        foreach (PreventiveDetailModel_Update objPreventive in Preventivecollection)
                        {
                            res = objCapaRepo.AddUpdateCapaPreventive(Capa_Update: objPreventive.TID, Capa_ID: CapaID, Preventive_Action: objPreventive.Preventive_Action, Preventive_Text: objPreventive.Action_Text, Preventive_Owner: objPreventive.Action_Owner, UID: Model.CreatedBy,Email:Model.PreventiveAction_Email);
                            if (res.IsSuccess == false)
                            {
                                break;
                            }
                        }
                    }
                }
                
                return Json(res);

            }
            catch
            {
                throw;
            }
        }



        [ViewAction]
        [HttpGet]
        [Route("GetCapaHis", Name = "GetCapaHis")]
        public ActionResult GetCapaHis(int CapaID)
        {
            EditCapaRepo objRepo = new EditCapaRepo();
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            CapaModel objCapaModel = new CapaModel();
            PreventiveDetailModel objPreventModel = new PreventiveDetailModel();
            CorrectiveDetailModel objCorrectModel = new CorrectiveDetailModel();
            try
            {

                dt = objRepo.GetCapaHis(CapaID);
                Response res = new Response();
                var CapaDataHis = JsonSerializer.SerializeTable(dt);
                res.IsSuccess = true;
                res.Data = CapaDataHis;
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                throw;
            }
        }

        //New Code Start



        [ViewAction]
        [HttpGet]
        [Route("GetCalendarCapa", Name = "GetCalendarCapa")]
        public ActionResult GetCalendarCapa(string CapaDate)
        {
            EditCapaRepo objRepo = new EditCapaRepo();
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            CapaModel objCapaModel = new CapaModel();
            PreventiveDetailModel objPreventModel = new PreventiveDetailModel();
            CorrectiveDetailModel objCorrectModel = new CorrectiveDetailModel();
            try
            {

                // Model.CreatedBy = IvapUser.UID;
                int EID = IvapUser.EID;

                ds = objRepo.GetCapaCalendar(CapaDate, EID);
                var CapaData = JsonSerializer.SerializeTable(ds.Tables[0]);
                var CorrectData = JsonSerializer.SerializeTable(ds.Tables[1]);
                var PreventData = JsonSerializer.SerializeTable(ds.Tables[2]);
                KendoGridUtils res = new KendoGridUtils();
                res = res.GetCommandButtonForGrid("CapaReport");
                res.Data = "{\"CData\":" + CapaData + ",\"CorrectiveData\":" + CorrectData + ",\"PreventiveData\":" + PreventData + "}";
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
        [Route("GetCapaCalendarData", Name = "GetCapaCalendarData")]
        public ActionResult GetCapaCalendarData()
        {
            EditCapaRepo objRepo = new EditCapaRepo();
            DataTable dt = new DataTable();
            JsonResult result = new JsonResult();
            DataSet ds = new DataSet();
            // List<CapaCalendarModel> lst = new List<CapaCalendarModel>();

            try
            {

                // Model.CreatedBy = IvapUser.UID;
                int EID = IvapUser.EID;
                List<CapaCalendarModel> data = objRepo.GetCapaCalendarData(EID);

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
        [Route("GetCapaForCalendar", Name = "GetCapaForCalendar")]
        public ActionResult GetCapaForCalendar(int CapaID)
        {
            EditCapaRepo objRepo = new EditCapaRepo();
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            CapaModel objCapaModel = new CapaModel();
            PreventiveDetailModel objPreventModel = new PreventiveDetailModel();
            CorrectiveDetailModel objCorrectModel = new CorrectiveDetailModel();
            try
            {

                // Model.CreatedBy = IvapUser.UID;
                int EID = IvapUser.EID;

                ds = objRepo.GetCapa(CapaID, EID);
                var CapaData = JsonSerializer.SerializeTable(ds.Tables[0]);
                var CorrectData = JsonSerializer.SerializeTable(ds.Tables[1]);
                var PreventData = JsonSerializer.SerializeTable(ds.Tables[2]);
                KendoGridUtils res = new KendoGridUtils();
                res = res.GetCommandButtonForGrid("CapaReport");
                res.Data = "{\"CData\":" + CapaData + ",\"CorrectiveData\":" + CorrectData + ",\"PreventiveData\":" + PreventData + "}";
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
        [Route("GetCapaCalendarHis", Name = "GetCapaCalendarHis")]
        public ActionResult GetCapaCalendarHis(int CapaID)
        {
            EditCapaRepo objRepo = new EditCapaRepo();
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            CapaModel objCapaModel = new CapaModel();
            PreventiveDetailModel objPreventModel = new PreventiveDetailModel();
            CorrectiveDetailModel objCorrectModel = new CorrectiveDetailModel();
            try
            {

                dt = objRepo.GetCapaHis(CapaID);
                Response res = new Response();
                var CapaDataHis = JsonSerializer.SerializeTable(dt);
                res.IsSuccess = true;
                res.Data = CapaDataHis;
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                throw;
            }
        }


    }
}
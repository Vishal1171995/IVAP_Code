using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ivap.Areas.Master.ViewModel;
using Ivap.Utils;
using Ivap.Areas.Master.Repository;
using Ivap.Controllers;
using Ivap.Areas.Master.Models;
using System.Data;
using Ivap.ActionFilters;
using Ivap.CustomAttribute;
using IVap.Areas.Master.Repository;

namespace Ivap.Areas.Master.Controllers
{
    [CustomAuthActionFilter(Order = 0)]
    [CustomAuthrizationActionFilter(Order = 1)]
    [RouteArea("Master", AreaPrefix = "")]
    [RoutePrefix("Masters")]
    public class EntityController : IvapBaseController
    {
        // GET: Master/Entity

        [ViewAction]
        [Route("EntityList", Name = "EntityList")]
        public ActionResult EntityList()
        {
            EntityVM Model = new EntityVM();
            StateRepo ObjSRepo = new StateRepo();
            CountryRepo ObjCRepo = new CountryRepo();
            CurrencyRepo ObjCurrRepo = new CurrencyRepo();
            CurrencyModel CuMDL = new CurrencyModel();

            ServicesAvailedModel SAModel = new ServicesAvailedModel();
            ServiceAvailedRepo SARepo = new ServiceAvailedRepo();
            SAModel.IsActive = true;
            SAModel.SID = 0;
            CuMDL.IsActive = true;
            CuMDL.CID = 0;
          
            try
            {
                Model.CurrencyList = DropdownUtils.ToSelectList(ObjCurrRepo.GetCurrency(CuMDL), "TID", "CurrencyCode");
                Model.StateList = DropdownUtils.ToSelectList(ObjSRepo.GetState(), "TID", "StateNameAndCode");
                Model.CountryList = DropdownUtils.ToSelectList(ObjCRepo.GetCountry(), "TID", "CountryCode");
                Model.StateList = DropdownUtils.ToSelectList(ObjSRepo.GetState(), "TID", "StateNameAndCode");
                return View(Model);
            }
            catch
            {
                throw;
            }

        }
        [ViewAction]
        [Route("GetEntity", Name = "GetEntity")]
        public ActionResult GetEntityDtl(int EID)
        {

            EntityRepo enRepo = new EntityRepo();
            DataTable dt = new DataTable();
            Response res = new Response();
            EntityModel EntModel = new EntityModel();
            try
            {
                EntModel.TID = EID;
                // EntModel.IsAct = "1";
                dt = enRepo.GetEntity(EntModel);
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
        [Route("GetUserForEntity", Name = "GetUserForEntity")]
        public ActionResult GetUserForEntity(int EID)
        {
            UserRepo UserRepoObj = new UserRepo();
            UserModel model = new UserModel();
            DataTable dt = new DataTable();
            Response res = new Response();
            EntityModel EntModel = new EntityModel();
            try
            {
                model.UID = 0;
                model.EID = EID;
                model.USERID = "";
                model.Role = 0;
                dt = UserRepoObj.GetUser(model);
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
        [Route("GetEntityCity", Name = "GetEntityCity")]
        public ActionResult GetEntityCity(int STATEID)
        {
            LocationRepo locRepo = new LocationRepo();
            Location locModel = new Location();
            EntityRepo enRepo = new EntityRepo();
            EntityModel EntModel = new EntityModel();
            DataTable dt = new DataTable();
            Response res = new Response();
            try
            {
                locModel.State_Id = STATEID;
                locModel.IsActive = true;
                locModel.EID = IvapUser.EID;
                dt = locRepo.GetLocation(locModel);
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
        [Route("GetEntityCityGlobal", Name = "GetEntityCityGlobal")]
        public ActionResult GetEntityCityGlobal(int STATEID)
        {
           
            EntityRepo enRepo = new EntityRepo();
            EntityModel EntModel = new EntityModel();
            DataTable dt = new DataTable();
            Response res = new Response();
            try
            {
                
                dt = enRepo.GetEntityCityGlobal(STATEID);
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
        [Route("GetEntityServiceGlobal", Name = "GetEntityServiceGlobal")]
        public ActionResult GetEntityServiceGlobal()
        {

            EntityRepo enRepo = new EntityRepo();
            EntityModel EntModel = new EntityModel();


            ServicesAvailedModel SAModel = new ServicesAvailedModel();
            ServiceAvailedRepo SARepo = new ServiceAvailedRepo();
            SAModel.IsActive = true;
            SAModel.SID = 0;

            DataTable dt = new DataTable();
            Response res = new Response();
            try
            {

                dt = SARepo.GetServiceAvailed(SAModel);
                res.IsSuccess = true;
                res.Data = JsonSerializer.SerializeTable(dt);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                throw;
            }
        }


        [ADDUpdateAction]
        [Route("EntitySetUp", Name = "EntitySetUp")]
        public ActionResult EntitySetUp()
        {
            try
            {
                EntityRepo enRepo = new EntityRepo();
                EntityOnBoardVM Model = new EntityOnBoardVM();
                StateRepo ObjSRepo = new StateRepo();
                CountryRepo ObjCRepo = new CountryRepo();
                CurrencyRepo ObjCurrRepo = new CurrencyRepo();
                Model.StateList = DropdownUtils.ToSelectList(ObjSRepo.GetState(), "TID", "StateNameAndCode");
                Model.CountryList = DropdownUtils.ToSelectList(ObjCRepo.GetCountry(), "TID", "CountryCode");
                CurrencyModel CuMDL = new CurrencyModel();
                CuMDL.IsActive = true;
                CuMDL.CID = 0;
                // Model.CurrencyList = DropdownUtils.ToSelectList(ObjCurrRepo.GetCurrency(CuMDL), "TID", "CurrencyCode");
                Model.CurrencyList = DropdownUtils.ToSelectList(enRepo.GetCurrencyEntity(CuMDL), "TID", "CurrencyCode");
                ServicesAvailedModel SAModel = new ServicesAvailedModel();
                ServiceAvailedRepo SARepo = new ServiceAvailedRepo();
                SAModel.IsActive = true;
                SAModel.SID = 0;
                Model.Services_Availed_List = DropdownUtils.ToSelectList(SARepo.GetServiceAvailed(SAModel), "TID", "ServicesName");
                return View(Model);
            }
            catch
            {
                throw;
            }
            
        }
        [ADDUpdateAction]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateEntity(EntityOnBoardVM Model)
        {
            try
            {
                StateRepo ObjSRepo = new StateRepo();
                CurrencyRepo objCurrency = new CurrencyRepo();
                CurrencyModel crModel = new CurrencyModel();

                CountryRepo objCountry = new CountryRepo();

                ServicesAvailedModel SAModel = new ServicesAvailedModel();
                ServiceAvailedRepo SARepo = new ServiceAvailedRepo();
                SAModel.IsActive = true;
                SAModel.SID = 0;
                
                if (ModelState.IsValid)
                {
                    EntityRepo ObjERepo = new EntityRepo();
                    Model.EntityModel.CreatedBy = IvapUser.UID;
                    Model.EntityUser.Created_By = IvapUser.UID;
                    crModel.IsActive = true;

                    var Res = ObjERepo.SetUpEntity(Model);
                    if (Res.IsSuccess == false)
                    {
                        Model.StateList = DropdownUtils.ToSelectList(ObjSRepo.GetState(), "TID", "StateNameAndCode");
                        Model.CurrencyList = DropdownUtils.ToSelectList(objCurrency.GetCurrency(crModel), "TID", "CURRENCY_CODE");
                        Model.CountryList = DropdownUtils.ToSelectList(objCountry.GetCountry(), "TID", "COUNTRY_NAME");
                        Model.Services_Availed_List = DropdownUtils.ToSelectList(SARepo.GetServiceAvailed(SAModel), "TID", "ServicesName");

                        //ModelState.AddModelError(string.Empty, Res.Message);
                        //return View("EntitySetUp", Model);
                        return Json(Res);
                    }
                    else
                    {
                        return View("EntityCreated", Model);
                    }
                }
                else
                {
                    Model.StateList = DropdownUtils.ToSelectList(ObjSRepo.GetState(), "TID", "StateNameAndCode");
                    Model.CurrencyList = DropdownUtils.ToSelectList(objCurrency.GetCurrency(crModel), "TID", "CURRENCY_CODE");
                    Model.CountryList = DropdownUtils.ToSelectList(objCountry.GetCountry(), "TID", "COUNTRY_NAME");

                    Model.Services_Availed_List = DropdownUtils.ToSelectList(SARepo.GetServiceAvailed(SAModel), "TID", "ServicesName");
                    var Result = "Please fill all mandatory fields.";
                    return Json(Result);
                    //ModelState.AddModelError(string.Empty, "Please fill all mandatory fields.");
                    //return View("EntitySetUp",Model);
                }
            }
            catch
            {
                throw;
            }

        }


        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("UpdateEntity", Name = "UpdateEntity")]
        public ActionResult UpdateEntity(EntityVM Model)
        {
            Response res = new Response();
            EntityRepo enrepo = new EntityRepo();
            try
            {
                //if (ModelState.IsValid)
                //{
                Model.EntityModel.CreatedBy = IvapUser.UID;
                res = enrepo.UpdateEntity(Model);
                return Json(res);
                //}
                //else
                //{
                   //return View(Model);
                //}
            }
            catch
            {
                throw;
            }
        }
    }
}
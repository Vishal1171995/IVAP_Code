using Ivap.ActionFilters;
using Ivap.Areas.Master.Models;
using IVap.Areas.Master.Repository;
using Ivap.CustomAttribute;
using Ivap.Repository;
using Ivap.Utils;
using Ivap.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Hosting;
using Ivap.Areas.Master.Repository;

namespace Ivap.Areas.Master.Controllers
{
    [CustomAuthActionFilter(Order = 0)]
    [CustomAuthrizationActionFilter(Order = 1)]
    [RouteArea("Master", AreaPrefix = "")]
    [RoutePrefix("Masters")]
    public class UserController :IvapBaseController
    {
        [ViewAction]
        [Route("Users", Name = "ViewUser")]
        public ActionResult ViewUser()
        {
            UserModel usermdl = new UserModel();
            RoleModel RolModel = new RoleModel();
            RoleRepo roleRep = new RoleRepo();
            EntityModel EntModel = new EntityModel();
            EntityRepo EntRep = new EntityRepo();
            try
            {
                usermdl.EID = IvapUser.EID;
                usermdl.SetDisplayName();
                //RolModel.EntityID = 0;
                RolModel.RoleID = 0;
                RolModel.IsActive = true;
                EntModel.EID = 0;
                EntModel.IsActive = true; ;
                usermdl.EntityList = DropdownUtils.ToSelectList(EntRep.GetEntity(EntModel), "TID", "ENTITY_NAME");
                usermdl.RoleList = DropdownUtils.ToSelectList(roleRep.GetRoles(RolModel), "TID", "RoleName");
                usermdl.IsActive = true;
                return View("Users",usermdl);
            }
            catch(Exception Ex)
            {
                throw;
            }
        }
        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("AddUpdateUser", Name = "AddUpdateUser")]
        public ActionResult AddUpdate(UserModel Model)
        {
            UserRepo objrepo = new UserRepo();
            Response res = new Response();
            try
            {
                if (ModelState.IsValid)
                {
                    Model.EID = IvapUser.EID;
                    Model.UserType = "USER";
                    Model.CreatedBy = IvapUser.UID;
                    res = objrepo.AddUpdateUser(Model);
                    return Json(res, JsonRequestBehavior.AllowGet);
                }
                else { return View("Users", Model);  }
            }
            catch
            {
                throw;
            }
        }
        [ViewAction]
        [HttpPost]
        [Route("getGridUser", Name = "getGridUser")]
        public ActionResult getGridUser(int page, int pageSize, int skip, int take, List<SortDescription> sorting, FilterContainer filter)
        {
            DataSet Ds = new DataSet();
            GridUser gUser = new GridUser();
            Response ret = new Utils.Response();
            UserRepo objUBO = new UserRepo();
            try
            {
                int from = Convert.ToInt32(skip); //+ 1; //(page - 1) * pageSize + 1;
                int to = Convert.ToInt32(take); //* page; // page * pageSize;
                string sortingStr = "";
                #region Sorting
                if (sorting != null)
                {
                    //sortingStr = objUBO.sortGrid(sorting);
                }
                #endregion
                #region filtering
                string filters = "";
                if (filter != null)
                {
                    //filters = objUBO.FilterGrid(filter);
                }
                #endregion
                sortingStr = sortingStr.TrimStart(',');
                if (sortingStr == "") sortingStr = null;
                if (filters == "") filters = null;
                gUser.from = from;
                gUser.To = to;
                gUser.FilterStr = filters;
                gUser.SortingStr = sortingStr;
                //Ds = objUBO.GetGridUser(gUser);
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

        [ViewAction]
        [HttpGet]
        [Route("getUser", Name = "getUser")]
        public ActionResult getUser(int UID)
        {
            UserRepo objUBO = new UserRepo();
            UserModel objModel = new UserModel();
            objModel.UID = Convert.ToInt32(UID);
            KendoGridUtils res = new KendoGridUtils();
            try
            {
                objModel.UID = UID;
                objModel.EID = IvapUser.EID;
                DataTable dt = objUBO.GetUser(objModel);
                res = res.GetCommandButtonForGrid("ViewUser");
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
        //[ADDUpdateAction]
        [HttpGet]
        [Route("ChangeStatus", Name = "ChangeStatus")]
        public ActionResult ChangeStatus(string UID, string Status)
        {
            UserRepo objUBO = new UserRepo();
            UserModel objModel = new UserModel();
            objModel.UID = Convert.ToInt32(UID);
            Response ret = new Response();
            try
            {
                ret = objUBO.ChangeStatus(UID, Status);
                return Json(ret, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        
        //[ADDUpdateAction]
        [HttpGet]
        [Route("ResetPassword", Name = "ResetPassword")]
        public ActionResult ResetPassword(string UID)
        {
            AccountRepo acrepo = new AccountRepo();
            UserRepo objUBO = new UserRepo();
            Response ret = new Response();
            try
            {

                ret.Data = acrepo.ForgetPassword(UID);
                ret.IsSuccess = true;
                return Json(ret, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
      [ViewAction]
        [HttpGet]
        [Route("getUserHistory", Name = "getUserHistory")]
        public ActionResult getUserHistory(string UID)
        {
            UserRepo objUBO = new UserRepo();
            UserModel objModel = new UserModel();
            Response ret = new Response();
            try
            {
                objModel.UID = Convert.ToInt32(UID);               
                ret.Data = JsonSerializer.SerializeTable(objUBO.GetUserHistory(objModel));
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

        [ViewAction]
        [HttpGet]
        [Route("ExportUser", Name = "ExportUser")]
        public ActionResult ExportUser(string UID)
        {
            Response ret = new Response();
            try
            {
                UserRepo repo = new UserRepo();
                UserModel model = new UserModel();
                model.UID = 0;
                model.EID = IvapUser.EID;
                model.SetDisplayName();
                DataTable dt = repo.GetUser(model);

                DataTable dtUser = dt.DefaultView.ToTable(false, "ENTITY_NAME", "USERID", "USER_FIRSTNAME", "USER_LASTNAME", "USER_EMAIL", "USER_MOBILENO", "ROLENAME", "STATUS");
                dtUser.Columns["ENTITY_NAME"].ColumnName = model.EID_TEXT;
                dtUser.Columns["USER_FIRSTNAME"].ColumnName = model.FirstName_Text;
                dtUser.Columns["USER_LASTNAME"].ColumnName = model.LastName_Text;
                dtUser.Columns["USER_EMAIL"].ColumnName = model.Email_Text;
                dtUser.Columns["USER_MOBILENO"].ColumnName = model.MobileNo_Text;
                dtUser.Columns["ROLENAME"].ColumnName = model.Role_Text;
                //dtUser.Columns["Circle"].ColumnName = "Circle";
                dtUser.Columns["STATUS"].ColumnName = "Status";
               

                string FileName = ExcellUtils.DataTableToExcel(dtUser);
                FileName = FileName.Replace("/", "").Replace("..", "").Replace("\\", "");
                string FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + FileName;
                byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "ALLUser.xlsx");
            }
            catch(Exception ex)
            {
                throw;
            }
        }
        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("UploadUserDetails", Name = "UploadUserDetails")]
        public ActionResult UploadUserDetails(string FileName)
        {
            Response ret = new Response();
            try
            {
                string[] arr = FileName.Split('.');

                if (arr[1].ToString().ToUpper() != "XLSX")
                {
                    ret.IsSuccess = false;
                    ret.Message = "Invalid file type.";
                    return Json(ret, JsonRequestBehavior.AllowGet);
                }
                UserRepo USRepo = new UserRepo();
                string FilePath = Server.MapPath("~/Docs/Temp/" + FileName);
                int SuccessCount = 0;
                int FailCount = 0;
                string ResultFileName = USRepo.UploadUserDetails(FilePath, IvapUser.UID,IvapUser.EID, ref SuccessCount, ref FailCount);
                if (ResultFileName == "Invalid File Format")
                {
                    ret.IsSuccess = false;
                    ret.Data = "";
                    ret.Message = "Invalid File Format";
                }
                else if (ResultFileName == "Please remove all the comma from your file.")
                {
                    ret.IsSuccess = false;
                    ret.Message = "Please remove all the commas from your file.";
                    return Json(ret, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string data = "{\"Success\":\"" + SuccessCount + "\",\"Failed\":\"" + FailCount + "\",\"FileName\":\"" + ResultFileName.Replace("\\", "\\\\") + "\"}";
                    ret.IsSuccess = true;
                    ret.Data = data;
                    //ret = ret.GetResponse("Mapping", "GetMapping", -1000, "", data, "");
                }

                return Json(ret, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                throw;
            }
        }
    }
}
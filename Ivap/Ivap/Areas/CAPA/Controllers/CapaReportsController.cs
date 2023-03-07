using Ivap.ActionFilters;
using Ivap.Areas.CAPA.Models;
using Ivap.Areas.CAPA.Repository;
using Ivap.Areas.FileExplorer.Repository;
using Ivap.Controllers;
using Ivap.CustomAttribute;
using Ivap.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Ivap.Areas.CAPA.Controllers
{
    [CustomAuthActionFilter(Order = 0)]
    [CustomAuthrizationActionFilter(Order = 1)]
    [RouteArea("CAPA", AreaPrefix = "")]
    [RoutePrefix("CAPA")]
    public class CapaReportsController : IvapBaseController
    {
        [ViewAction]
        [Route("CapaReport", Name = "CapaReport")]
        public ActionResult CapaReport()
        {
            return View();
        }

        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("AddUpdateCapa", Name = "AddUpdateCapa")]
        public ActionResult AddUpdate(CapaModel Model)
        {
            CapaRepo objCapaRepo = new CapaRepo();
            Response res = new Response();
            Response res1 = new Response();
            try
            {

                Model.CreatedBy = IvapUser.UID;
                Model.EID = IvapUser.EID;
                res = objCapaRepo.AddUpdateCapaForEach(Model);
                return Json(res);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        [ViewAction]
        [HttpGet]
        [Route("GetEditCapaTesting", Name = "GetEditCapaTesting")]
        public ActionResult CapaReport(int CapaID)
        {
            EditCapaRepo objRepo = new EditCapaRepo();
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            CapaModel objCapaModel = new CapaModel();
            PreventiveDetailModel objPreventModel = new PreventiveDetailModel();
            CorrectiveDetailModel objCorrectModel = new CorrectiveDetailModel();
           
            try
            {

                int EID = IvapUser.EID;
                objCapaModel = objRepo.GetCapaEdit(CapaID, EID);
                return View("UpdateCAPA", objCapaModel);

            }
            catch
            {
                throw;
            }
        }

        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("UpdateCapaReport", Name = "UpdateCapaReport")]
        public ActionResult UpdateCapaReport(CapaModel Model)
        {
            CapaRepo objCapaRepo = new CapaRepo();
            Response res = new Response();
            //int CapaID = Convert.ToInt32(Model.TID);

            try
            {

                Model.CreatedBy = IvapUser.UID;
                Model.EID = IvapUser.EID;
                res = objCapaRepo.UpdateCapaForEach(Model);
                return Json(res);


            }
            catch
            {
                throw;
            }
        }
        [ViewAction]
        [HttpPost]
        [Route("DeleteCAPAItemDetails", Name = "DeleteCAPAItemDetails")]
        public ActionResult DeleteCAPAItemDetails(int ItemID, string Type)
        {
            try
            {
                Response res = new Response();
                CapaRepo objCapaRepo = new CapaRepo();
                res = objCapaRepo.DeleteCAPAItemDetails(Item_ID: ItemID, Type: Type);
                var UserGrid = Json(res, JsonRequestBehavior.AllowGet);
                return UserGrid;
            }
            catch
            {
                throw;
            }
        }

        [ViewAction]
        [HttpPost]
        [Route("AddCorrective", Name = "AddCorrective")]
        public ActionResult AddCorrective(int TID, string CORRECTIVE_ACTION, string ACTION_TEXT, string ACTION_OWNER,string Owner_Email)
        {
            try
            {
                Response res = new Response();
                CapaRepo objCapaRepo = new CapaRepo();
                int UID = IvapUser.UID;

                res = objCapaRepo.AddUpdateCapaCorrective(Capa_Update: 0, Capa_ID: TID, Corrective_Action: CORRECTIVE_ACTION, Corrective_Text: ACTION_TEXT, Corrective_Owner: ACTION_OWNER, UID: UID,Email: Owner_Email);

                return Json(res);
            }
            catch
            {
                throw;
            }
        }

        [ViewAction]
        [HttpPost]
        [Route("AddPreventive", Name = "AddPreventive")]
        public ActionResult AddPreventive(int TID, string PREVENTIVE_ACTION, string ACTION_TEXT, string ACTION_OWNER,string Owner_Email)
        {
            try
            {
                Response res = new Response();
                CapaRepo objCapaRepo = new CapaRepo();
                int UID = IvapUser.UID;

                res = objCapaRepo.AddUpdateCapaPreventive(Capa_Update: 0, Capa_ID: TID, Preventive_Action: PREVENTIVE_ACTION, Preventive_Text: ACTION_TEXT, Preventive_Owner: ACTION_OWNER, UID: UID,Email: Owner_Email);

                return Json(res);
            }
            catch
            {
                throw;
            }
        }


        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [Route("UploadFileCapaConversation", Name = "UploadFileCapaConversation")]
        [HttpPost]
        public ActionResult UploadFileCapaConversation(HttpPostedFileBase fileToUpload)
        {
            string path = "";
            string NewPath;
            Response ret = new Response();
            //FileExtUtils fileext = new FileExtUtils();
            if (fileToUpload != null && fileToUpload.ContentLength > 0)
                try
                {
                    if (fileToUpload.ContentLength <= 31457280) //20MB {1048576 *20}
                    {
                        string fileExt = "";
                        string orignalFileName = fileToUpload.FileName;
                        fileExt = Path.GetExtension(fileToUpload.FileName);
                        //if ((fileExt.ToLower() != ".exe") && (fileExt.ToLower() != ".vb") && (fileExt.ToLower() != ".js") && (fileExt.ToLower() != ".cs") && (fileExt.ToLower() != ".html") && (fileExt.ToLower() != ".htm") && (fileExt.ToLower() != ".php"))
                        if (FileExtUtils.checkFileExt(fileExt.ToLower()))
                        {
                            string FileName = DateTime.Now.Year.ToString() + DateTime.Now.Month + DateTime.Now.Date.DayOfYear + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond;
                            FileName = FileName.Replace(fileExt, string.Empty);
                            FileName = FileName + fileExt;
                            int EID = IvapUser.EID;
                          //  path = Server.MapPath("~/Docs/Entity_" + EID + "/CAPA/");
                            path = Server.MapPath("~/Docs/TEMP/");
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }
                            NewPath = path + FileName;
                            fileToUpload.SaveAs(NewPath);
                            
                            ret.Data = FileName + "$" + orignalFileName;
                            ret.IsSuccess = true;
                        }
                        else
                        {
                            ret.IsSuccess = false;
                            ret.Data = "File type not supported.";
                            ret.Message = "File type not supported";
                        }
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.Data = "File size must be less than 20mb.";
                        ret.Message = "File size must be less than 20mb.";
                    }
                }
                catch (Exception ex)
                {
                    ret.IsSuccess = false;
                    ret.Data = "Sorry!!!Something went wrong.Please try again later.";
                    ret.Message = "Sorry!!!Something went wrong.Please try again later.";
                }
            else
            {
                ret.IsSuccess = false;
                ret.Data = "Invalid File.";
                ret.Message = "Invalid File.";
            }
            return Json(ret);
        }

        [ViewAction]
        [HttpPost]
        [Route("AddUpdateConversationRemark", Name = "AddUpdateConversationRemark")]
        public ActionResult AddUpdateConversationRemark(int TID, int CAPAID, int ITEM_ID, string ITEM_NAME,string REMARK,string ATTACHMENT,string SYSTEM_ATTACHMENT, string STATUS,string CLOSURE_DATE)
        {
            try
            {
                CapaRepo objCapaRepo = new CapaRepo();
                Response res = new Response();
                int UID = IvapUser.UID;
                int EID = IvapUser.EID;


                if (TID==1)
                {
                    res = objCapaRepo.AddUpdateConversastion(Con_TID: 0, Capa_ID: CAPAID, ITEM_ID: ITEM_ID, ITEM_NAME: ITEM_NAME, REMARK: REMARK, ATTACHMENT: ATTACHMENT, SYSTEM_ATTACHMENT: SYSTEM_ATTACHMENT, STATUS: STATUS, CLOSURE_DATE: CLOSURE_DATE, UID: UID,EID: EID);

                  
                }
                else
                {
                    res = objCapaRepo.AddUpdateConversastion(Con_TID: TID, Capa_ID: CAPAID, ITEM_ID: ITEM_ID, ITEM_NAME: ITEM_NAME, REMARK: REMARK, ATTACHMENT: ATTACHMENT, SYSTEM_ATTACHMENT: SYSTEM_ATTACHMENT, STATUS: STATUS, CLOSURE_DATE: CLOSURE_DATE, UID: UID,EID: EID);
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
        [Route("DownloadFileAttachment", Name = "DownloadFileAttachment")]
        public ActionResult DownloadFileMOM(int CAPAID, string OriginalName, string SystemName)
        {
           // int EID = IvapUser.EID;
            try
            {
                SystemName = SystemName.Replace("/", "").Replace("..", "").Replace("\\", "");
                string FilePath;
                if (CAPAID == 0)
                {
                    //FilePath = HostingEnvironment.MapPath("~/Docs/Entity_" + IvapUser.EID + "/CAPA/") + SystemName;
                    FilePath = HostingEnvironment.MapPath("~/Docs/TEMP/") + SystemName;
                }
                else
                {
                    FilePath = HostingEnvironment.MapPath("~/Docs/Entity_" + IvapUser.EID + "/CAPA/") + SystemName;
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, OriginalName);
            }
            catch
            {
                throw;
            }
        }

      

    }
}
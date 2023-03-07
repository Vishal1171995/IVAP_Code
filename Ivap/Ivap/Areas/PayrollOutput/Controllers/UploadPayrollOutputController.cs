using ClosedXML.Excel;
using Ivap.ActionFilters;
using Ivap.Areas.Configuration.Repository;
using Ivap.Areas.PayrollOutput.Models;
using Ivap.Areas.PayrollOutput.Repository;
using Ivap.Controllers;
using Ivap.CustomAttribute;
using Ivap.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Ivap.Areas.PayrollOutput.Controllers
{
    [CustomAuthActionFilter(Order = 0)]
    [CustomAuthrizationActionFilter(Order = 1)]
    [RouteArea("PayrollOutPut", AreaPrefix = "")]
    [RoutePrefix("PayrollOutPut")]
    public class UploadPayrollOutputController : IvapBaseController
    {
        // GET: PayrollOutput/UploadPayrollOutput
        // GET: InputProcessing/UploadInput

        [ViewAction]
        [HttpGet]
        [Route("DownloadOutPutSampleFile", Name = "DownloadOutPutSampleFile")]
        public ActionResult DownloadOutPutSampleFile(int FileID, string File_Name)
        {
            FileSetupRepo objFileSetupRepo = new FileSetupRepo();
            DataTable dt = new DataTable();
            try
            {
                int EID = IvapUser.EID;
                dt = objFileSetupRepo.FileSetupSampleDownload(EID, FileID);
                string FileName = objFileSetupRepo.DataTableToExcel_FileSample(dt);
                string FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + FileName;
                byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, File_Name + ".xlsx");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [ViewAction]
        [Route("UploadPayrollOutPutFile", Name = "UploadPayrollOutPutFile")]
        public ActionResult UploadFile()
        {
            UploadPayrollOutPutModel Model = new UploadPayrollOutPutModel();
            DataTable Dt = new DataTable();
            PayrollOutPutFileSetupModel objModel = new PayrollOutPutFileSetupModel();
            PayrollOutPutFileSetUpRepo objRepo = new PayrollOutPutFileSetUpRepo();
            try
            {
                Session["OutpuTempTable"] = null;
                Model.PayDate = IvapUser.PayDate;
                objModel.EID = IvapUser.EID;
                objModel.FILE_TYPE = "PMS Output File";
                Model.FileList = DropdownUtils.ToSelectList(objRepo.GetFileType(objModel), "TID", "InputUploadFileName");
                return View("UploadPayrollOutPutFile", Model);
            }
            catch
            {
                throw;
            }
        }

        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("UploadPayrollOutputDetails", Name = "UploadPayrollOutputDetails")]
        public ActionResult UploadPayrollOutputDetails(string FileName, int FileID)
        {
            Response ret = new Response();
            PayrollOutputFileUploadModel OutputFileObj = new PayrollOutputFileUploadModel();
            PayrollOutPutFileSetUpRepo ObjUpload = new PayrollOutPutFileSetUpRepo();
            try
            {
                
                //Validating File Extention

                string[] ArrFileExt = FileName.Split('.');
                //At last index of array file extention resides
                if (!(ArrFileExt[ArrFileExt.Length - 1].ToString().ToUpper() == "XLSX" || ArrFileExt[ArrFileExt.Length - 1].ToString().ToUpper() == "XLS"))
                {
                    ret.IsSuccess = false;
                    ret.Message = "Invalid file type!!! File type must be XLS OR XLSX only.";
                    return Json(ret, JsonRequestBehavior.AllowGet);
                }
                //Removing Vulner path from file
                FileName = FileName.Replace("/", "").Replace("..", "").Replace("\\", "");
                OutputFileObj.FileExtention = ArrFileExt[ArrFileExt.Length - 1].ToString().Trim().ToUpper();

                string FilePath = Server.MapPath("~/Docs/Temp/" + FileName);
                OutputFileObj.CreatedBy = IvapUser.UID;
                OutputFileObj.FileID = FileID;
                OutputFileObj.File_Name = FileName;
                OutputFileObj.EID = IvapUser.EID;
                OutputFileObj.FilePath = FilePath;
                OutputFileObj.PayDate = IvapUser.PayDate;
                PayrollOutputFileResponseModel OutputFileResponse = ObjUpload.UploadOutputDetails(OutputFileObj);
                return Json(OutputFileResponse, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                throw;
            }
        }

        [ViewAction]
        [HttpGet]
        [Route("GetEffectiveDueDateOutputData", Name = "GetEffectiveDueDateOutputData")]
        public ActionResult GetEffectiveDueDateOutputData()
        {
            Response ret = new Response();
            try
            {
                PayrollOutPutFileSetUpRepo objRepo = new PayrollOutPutFileSetUpRepo();
                PayrollOutPutFileSetupModel objModel = new PayrollOutPutFileSetupModel();
                DataTable dt = new DataTable();
                objModel.EID = IvapUser.EID;
                objModel.FILE_TYPE = "PMS Output File";
                dt = objRepo.GetFileType(objModel);
                var arr = new ArrayList();
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        arr.Add(dr["tid"]);
                    }
                }


                DataTable dtOutPut = objRepo.GetEffectiveDueDateOutputData(string.Join(",", arr.ToArray()), IvapUser.UID, IvapUser.EID,IvapUser.PayDate);
                ret.Data = JsonSerializer.SerializeTable(dtOutPut);
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
        [Route("DownloadErrorLogFile", Name = "DownloadErrorLogFile")]
        public ActionResult DownloadErrorLogFile()
        {


            try
            {
                DataTable dt = (DataTable)Session["OutpuTempTable"];

                string FileName = ExcellUtils.DataTableToExcel(dt);
                FileName = FileName.Replace("/", "").Replace("..", "").Replace("\\", "");
                string FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + FileName;
                byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "OutPutErrorLog.xlsx");

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [ViewAction]
        [HttpGet]
        [Route("PayrollOutputDownload", Name = "PayrollOutputDownload")]
        public ActionResult PayrollOutputDownload(int File_ID, string Pay_Date)
        {
            try
            {
                PayrollOutPutFileSetUpRepo ObjRepo = new PayrollOutPutFileSetUpRepo();
                DataTable dt = ObjRepo.GetPayrollOutPutTable(PayDate:Pay_Date,File_ID:File_ID,UID:IvapUser.UID,EID:IvapUser.EID);
                string FileName = ExcellUtils.DataTableToExcel(dt);
                FileName = FileName.Replace("/", "").Replace("..", "").Replace("\\", "");
                string FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + FileName;
                byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "Result" + ".xlsx");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [ViewAction]
        [HttpPost]
        [Route("PayrollOutputDelete", Name = "PayrollOutputDelete")]
        public ActionResult PayrollOutputDelete(int File_ID, string Pay_Date)
        {
            try
            {
                Response res = new Response();
                PayrollOutPutFileSetUpRepo ObjRepo = new PayrollOutPutFileSetUpRepo();
                res = ObjRepo.DeletePayrollOutput(File_ID:File_ID,PayDate:Pay_Date,UID:IvapUser.UID,EID:IvapUser.EID);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
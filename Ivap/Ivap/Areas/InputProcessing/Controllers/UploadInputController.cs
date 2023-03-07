using Ivap.ActionFilters;
using Ivap.Areas.Configuration.Models;
using Ivap.Areas.Configuration.Repository;
using Ivap.Areas.InputProcessing.Models;
using Ivap.Areas.InputProcessing.Repository;
using Ivap.Controllers;
using Ivap.CustomAttribute;
using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Ivap.Areas.InputProcessing.Controllers
{
    [CustomAuthActionFilter(Order = 0)]
    [CustomAuthrizationActionFilter(Order = 1)]
    [RouteArea("InputProcessing", AreaPrefix = "")]
    [RoutePrefix("InputProcessing")]
    public class UploadInputController : IvapBaseController
    {
        // GET: InputProcessing/UploadInput
        [ViewAction]
        [Route("UploadFile", Name = "UploadFile")]
        public ActionResult UploadFile()
        {
            UploadInputModel Model = new UploadInputModel();
            DataTable Dt = new DataTable();
            FileSetupModel objModel = new FileSetupModel();
            FileSetupRepo objRepo = new FileSetupRepo();
            try
            {
                Session["TempTable"] = null;
                objModel.EID = IvapUser.EID;
                objModel.FILE_TYPE = "Payroll Input File";
                Model.FileList = DropdownUtils.ToSelectList(objRepo.GetFileType(objModel), "TID", "InputUploadFileName");
                return View("UploadInput", Model);
            }
            catch
            {
                throw;
            }
        }
        [ViewAction]
        [HttpGet]
        [Route("DownloadSampleFile", Name = "DownloadSampleFile")]
        public ActionResult DownloadSampleFile(int FileID, string File_Name)
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


        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("UploadInputDetails", Name = "UploadInputDetails")]
        public ActionResult UploadInputDetails(string FileName,int FileID)
        {
            Response ret = new Response();
            InputFileUploadModel InputFileObj = new InputFileUploadModel();
            UploadInputRepo ObjUpload = new UploadInputRepo();
            try
            {
                
                //Validating File Extention

                string[] ArrFileExt = FileName.Split('.');
                //At last index of array file extention resides
                if (!(ArrFileExt[ArrFileExt.Length-1].ToString().ToUpper() == "XLSX"|| ArrFileExt[ArrFileExt.Length - 1].ToString().ToUpper() == "XLS"))
                {
                    ret.IsSuccess = false;
                    ret.Message = "Invalid file type!!! File type must be XLS OR XLSX only.";
                    return Json(ret, JsonRequestBehavior.AllowGet);
                }
                //Removing Vulner path from file
                FileName = FileName.Replace("/", "").Replace("..", "").Replace("\\", "");
                InputFileObj.FileExtention = ArrFileExt[ArrFileExt.Length - 1].ToString().Trim().ToUpper();

                string FilePath = Server.MapPath("~/Docs/Temp/" + FileName);
                InputFileObj.CreatedBy= IvapUser.UID;
                InputFileObj.FileID = FileID;
                InputFileObj.File_Name = FileName;
                InputFileObj.EID= IvapUser.EID;
                InputFileObj.FilePath = FilePath;
                InputFileResponseModel InputFileResponse = ObjUpload.UploadInputDetails(InputFileObj);
                return Json(InputFileResponse, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                throw;
            }
        }


        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("ValidateInputData", Name = "ValidateInputData")]
        public ActionResult ValidateInputData(int FileID, string PayDate)
        {
            Response ret = new Response();
            UploadInputRepo ObjUpload = new UploadInputRepo();
            try
            {
                InputValidationVM VMValidation = new InputValidationVM();
                InputValidationModel MValidation = new InputValidationModel();
                int EID = IvapUser.EID;
                VMValidation = ObjUpload.ValidateTempDataTable(EID, FileID,PayDate, IvapUser.UID);
                DataTable dt = new DataTable();
                dt.Clear();
                dt.Columns.Add("Item");
                dt.Columns.Add("Data");
                dt.Rows.Add("Total Valid Records", VMValidation.SuccessCount);
                dt.Rows.Add("Required Field Validation Error", VMValidation.ReqValFailCount);
                dt.Rows.Add("Data Format Validation Error", VMValidation.DataFormateValFailCount);
                dt.Rows.Add("Date Format Validation Error", VMValidation.DateFormateValFailCount);
                dt.Rows.Add("Master Valued Validation Error", VMValidation.MasterValFailCount);
                dt.Rows.Add("Master Valued Data Access Voilation Error", VMValidation.MasterAccessVoilationCount);
                ret.Message = "Validate Successfully";
                string Griddata = JsonSerializer.SerializeTable(dt);
                
                VMValidation.DataFormateValFailCount = (VMValidation.DataFormateValFailCount / VMValidation.TotalCount) * 100;
                VMValidation.DateFormateValFailCount = (VMValidation.DateFormateValFailCount / VMValidation.TotalCount) * 100;
                VMValidation.MasterValFailCount = (VMValidation.MasterValFailCount / VMValidation.TotalCount) * 100;
                VMValidation.MasterAccessVoilationCount = (VMValidation.MasterAccessVoilationCount / VMValidation.TotalCount) * 100;
                VMValidation.ReqValFailCount = (VMValidation.ReqValFailCount / VMValidation.TotalCount) * 100;
                VMValidation.SuccessCount = (VMValidation.SuccessCount / VMValidation.TotalCount) * 100;
                string Chartdata = JsonSerializer.SerializeObject(VMValidation);
                ret.IsSuccess = true;
                ret.Data = "{\"CData\":" + Chartdata + ",\"GData\":" + Griddata + "}";
                ret.IsSuccess = true;
                return Json(ret, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                throw;
            }
        }
        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("SendToApproval", Name = "SendToApproval")]
        public ActionResult SendToApproval()
        {
            Response ret = new Response();
            UploadInputRepo ObjUpload = new UploadInputRepo();
            try
            {
                int Result = 0;
                DataTable ApprovalDt = new DataTable();
                ApprovalDt = (DataTable)Session["TempTable"];
                DataView dataView = ApprovalDt.DefaultView;
                dataView.RowFilter = "CkStatus = True";
                DataTable dt = dataView.ToTable();
                int EID = IvapUser.EID;
                Result = ObjUpload.SendToApproval(dt, EID);
                if (Result == 1) { ret.IsSuccess = true; }
                else { ret.IsSuccess = false; }
                    ret.Message = "Send To Approval Successfully";
                
                return Json(ret, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                throw;
            }
        }

        [ViewAction]
        [HttpGet]
        [Route("ErrorFileDownload", Name = "ErrorFileDownload")]
        public ActionResult ErrorFileDownload(string ItemName,int FileID)
        {
            try
            {
                DataTable dtResult = new DataTable();
                dtResult = (DataTable)Session["TempTable"];
                //Now Get Schema of File to change column 
                UploadInputRepo ObjR = new UploadInputRepo();
                DataTable dtSchema = ObjR.GetSchemaOfInputFile(IvapUser.EID, FileID);
                DataView dvColumns;
                StringBuilder SbDownloadColumns = new StringBuilder();
                for (int i=0;i<dtResult.Columns.Count;i++)
                {
                    try
                    {
                        dvColumns = new DataView(dtSchema);
                        dvColumns.RowFilter = "COMPONENT_NAME='" + dtResult.Columns[i].ColumnName.Trim() + "'";
                        DataTable dtColumns = dvColumns.ToTable();
                        dtResult.Columns[i].ColumnName = dtColumns.Rows[0]["COMPONENT_DISPLAY_NAME"].ToString().Trim();
                        SbDownloadColumns.Append(dtColumns.Rows[0]["COMPONENT_DISPLAY_NAME"].ToString().Trim()).Append(",");
                    }
                    catch
                    { }
                }
                //dtResult.Columns.Remove("TID");
                DataView dataView = dtResult.DefaultView;
                if (ItemName.Trim().ToUpper() == "Total Success".ToUpper() || ItemName.Trim().ToUpper() == "Total Valid Records".ToUpper())
                    dataView.RowFilter = "ValidationType = 'SUCCESS'";
                else if (ItemName.Trim().ToUpper() == "Required field Error".ToUpper() || ItemName.Trim().ToUpper() == "Required Field Validation Error".ToUpper())
                    dataView.RowFilter = "ValidationType = 'Required Validation'";

                else if (ItemName.Trim().ToUpper() == "Master Valued Error".ToUpper() || ItemName.Trim().ToUpper() == "Master Valued Validation Error".ToUpper())
                    dataView.RowFilter = "ValidationType = 'Master Valued Validation'";

                else if (ItemName.Trim().ToUpper() == "Date Format Error".ToUpper() || ItemName.Trim().ToUpper() == "Date Format Validation Error".ToUpper())
                    dataView.RowFilter = "ValidationType = 'Date Format Validation'";


                else if (ItemName.Trim().ToUpper() == "Data Format  Error".ToUpper() || ItemName.Trim().ToUpper() == "Data Format Validation Error".ToUpper())
                    dataView.RowFilter = "ValidationType = 'Data Format Validation'";


                else if (ItemName.Trim().ToUpper() == "Access Voilation Error".ToUpper() || ItemName.Trim().ToUpper() == "Master Valued Data Access Voilation Error".ToUpper())
                    dataView.RowFilter = "ValidationType = 'Master Data Access Voilation'";
                SbDownloadColumns.Append("ValidationType");
                //string StrColumns = "\"[EMP_CODE]\" \",\"[HR_PAYDATE] \",\"[Employee Name] \",\"[Date of Joining UST] \",\"[India Payroll Start] \",\"[DOB] \",\"[GENDER] \",\"[DESIGN] \",\"[GRADE] \",\"[LOCATION] \",\"[Work Location] \",\"[PAN] \",\"[Employee Remarks] \",\"[BANK NAME] \",\"[NEFT] \",\"[BANK AC NO] \",\"[PF NO] \",\"[UAN] \",\"[EMAIL ID] \",\"[BASIC] \",\"[HRA] \",\"[CONVEYANCE] \",\"[SPL_ALLOWANCE] \",\"[Education Allowance] \",\"[BONUS] \",\"[PF Employer] \",\"[Gratuity CTC] \",\"[Variable Pay] \",\"[Sodexo] \",\"[TELEPHONE Reimbursement] \",\"[CTC] \",\"[PAY_PAYDATE] \",\"[TRN_DATE] \",\"[EMP_BANK NAME] \",\"[STATUS] \",\"[TRN_WEF] \",\"[Payroll_Type] \",\"[Father_Name] \",\"[AADHAARNO] \",\"[COSTCENTRE] \",\"[DEPARTMENT] \",\"[ESI NO] \",\"[MOBILENO]";
                string[] arrColumns = SbDownloadColumns.ToString().Trim().TrimEnd(',').Split(',');
                DataTable dt = dataView.ToTable(false, arrColumns);

                //DataTable dtDownload=dt.DefaultView(false, SbDownloadColumns.ToString()

                string FileNameToDownload = dtSchema.Rows[0]["FILE_NAME"].ToString().Replace(" ", "_") + ItemName.Replace(" ", "_") + "_" + DateTime.Now.ToLongTimeString();
                //return dt;
                string FileName = ExcellUtils.DataTableToExcel(dt);
                FileName = FileName.Replace("/", "").Replace("..", "").Replace("\\", "");
                string FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + FileName;
                byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, FileNameToDownload + ".xlsx");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("DeleteValidatedData", Name = "DeleteValidatedData")]
        public ActionResult DeleteValidatedData(string ItemName)
        {
            Response ret = new Response();
            DataTable dtResult = new DataTable();
            dtResult = (DataTable)Session["TempTable"];
            //Now Get Schema of File to change column 
            UploadInputRepo ObjR = new UploadInputRepo();
            DataView DvShema = new DataView(dtResult);
            string[] selectedColumns = new[] { "TID", "ValidationType" };
            DataTable Dispalydt = DvShema.ToTable(false, selectedColumns);
            DataView dataView = Dispalydt.DefaultView;
            if (ItemName.Trim().ToUpper() == "Total Success".ToUpper() || ItemName.Trim().ToUpper() == "Total Valid Records".ToUpper())
                dataView.RowFilter = "ValidationType = 'SUCCESS'";
            else if (ItemName.Trim().ToUpper() == "Required field Error".ToUpper() || ItemName.Trim().ToUpper() == "Required Field Validation Error".ToUpper())
                dataView.RowFilter = "ValidationType = 'Required Validation'";

            else if (ItemName.Trim().ToUpper() == "Master Valued Error".ToUpper() || ItemName.Trim().ToUpper() == "Master Valued Validation Error".ToUpper())
                dataView.RowFilter = "ValidationType = 'Master Valued Validation'";

            else if (ItemName.Trim().ToUpper() == "Date Format Error".ToUpper() || ItemName.Trim().ToUpper() == "Date Format Validation Error".ToUpper())
                dataView.RowFilter = "ValidationType = 'Date Format Validation'";
            else if (ItemName.Trim().ToUpper() == "Data Format  Error".ToUpper() || ItemName.Trim().ToUpper() == "Data Format Validation Error".ToUpper())
                dataView.RowFilter = "ValidationType = 'Data Format Validation'";
            else if (ItemName.Trim().ToUpper() == "Access Voilation Error".ToUpper() || ItemName.Trim().ToUpper() == "Master Valued Data Access Voilation Error".ToUpper())
                dataView.RowFilter = "ValidationType = 'Master Data Access Voilation'";
            DataTable dt = dataView.ToTable();
            ret = ObjR.DeleteValidatedData(dt, IvapUser.EID);
            return Json(ret, JsonRequestBehavior.AllowGet);


        }
        [ViewAction]
        [HttpGet]
        [Route("GetAllTempData", Name = "GetAllTempData")]
        public ActionResult GetAllTempData(int File_ID)
        {
            Response ret = new Response();
            try
            {
                UploadInputRepo ObjUpload = new UploadInputRepo();
                DataTable dt = ObjUpload.GetAllTempData(File_ID);
                ret.Data = JsonSerializer.SerializeTable(dt);
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
        [Route("GetEffectiveDueDateData", Name = "GetEffectiveDueDateData")]
        public ActionResult GetEffectiveDueDateData(int File_ID)
        {
            Response ret = new Response();
            try
            {
                UploadInputRepo ObjUpload = new UploadInputRepo();
                DataTable dt = ObjUpload.GetEffectiveDueDateData(File_ID, IvapUser.UID , IvapUser.EID);
                ret.Data = JsonSerializer.SerializeTable(dt);
                ret.IsSuccess = true;
                return Json(ret, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("DeleteTempTableData", Name = "DeleteTempTableData")]
        public ActionResult DeleteTempTableData(int FileID, string EffectiveDate, string PayDate)
        {
            Response ret = new Response();
            UploadInputRepo ObjUpload = new UploadInputRepo();
            try
            {
                int Result = 0;
                int EID = IvapUser.EID;
                string TempTable = "Ivap_MAST_TEMP_" + EID;
                int USERID = IvapUser.UID;
                Result = ObjUpload.DeleteTempTableData(FileID,EffectiveDate, PayDate,TempTable, USERID);
                if (Result >= 1) { ret.IsSuccess = true; ret.Message = "Data Deleted Successfully"; }
                else { ret.IsSuccess = false; ret.Message = "You have not right to Delete this PayDate record."; }
                return Json(ret, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                throw;
            }
        }

        [ValidateAntiForgeryToken]
        [ADDUpdateAction]
        [HttpPost]
        [Route("ProceedClearance", Name = "ProceedClearance")]
        public ActionResult ProceedClearance(int FileID, string PayDate)
        {
            try
            {
                UploadInputRepo ObjR = new UploadInputRepo();
                int EID = IvapUser.EID;
                DataTable dtData = (DataTable)Session["TempTable"];
                DataView dataView = new DataView(dtData);
                dataView.RowFilter = "ValidationType = 'SUCCESS'";
                DataTable dtSuccess = dataView.ToTable();
                Response ret = new Response();
                ret = ObjR.ProceesClearance(dtSuccess, FileID, EID, IvapUser.UID);
                return Json(ret, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        [ViewAction]
        [HttpGet]
        [Route("TempFileDownload", Name = "TempFileDownload")]
        public ActionResult TempFileDownload(int FileID, string EffectiveDate, string PayDate)
        {
            try
            {
                UploadInputRepo ObjUpload = new UploadInputRepo();
                int EID = IvapUser.EID;
                string TempTable = "Ivap_MAST_TEMP_" + EID;
                DataTable dt = ObjUpload.GetTempTable(FileID,  PayDate,IvapUser.EID,IvapUser.UID);
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
    }
}
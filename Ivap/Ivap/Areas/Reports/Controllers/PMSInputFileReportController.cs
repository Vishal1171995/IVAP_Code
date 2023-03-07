using Ivap.ActionFilters;
using Ivap.Areas.Reports.Models;
using Ivap.Areas.Reports.Repository;
using Ivap.Controllers;
using Ivap.CustomAttribute;
using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Globalization;

namespace Ivap.Areas.Reports.Controllers
{
    [CustomAuthActionFilter(Order = 0)]
    [CustomAuthrizationActionFilter(Order = 1)]
    [RouteArea("Reports", AreaPrefix = "")]
    [RoutePrefix("Reports")]
    public class PMSInputFileReportController : IvapBaseController
    {
        // GET: Reports/PMSInputFileReport
        [ViewAction]
        [Route("GetPMSInputFileReport", Name = "GetPMSInputFileReport")]
        public ActionResult GetPMS_InputFile_Report()
        {
            PayrollInputFileReportModel objModel = new PayrollInputFileReportModel();
            PayrollInputReportRepo objRepo = new PayrollInputReportRepo();
            try
            {
                objModel.PayDateList = DropdownUtils.ToMultiSelectList(objRepo.GetPayDates(IvapUser.EID), "PayDate", "PayDate_Text");
                objModel.PayDate = IvapUser.PayDate;
                objModel.SelectedPayDate = objModel.PayDate.ToString("dd/MM/yyyy");

            }
            catch {
                throw;
            }
            return View(objModel);
        }


        [ViewAction]
        [HttpGet]
        [Route("GetInputReportFileData", Name = "GetInputReportFileData")]
        public ActionResult GetInputReportFileData(string PayDate)
        {
            Response ret = new Response();
            try
            {

                PayrollInputReportRepo objRepo = new PayrollInputReportRepo();
                PayrollInputFileReportModel objModel = new PayrollInputFileReportModel();
                objModel.EID = IvapUser.EID;
                objModel.PayDate = IvapUser.PayDate;
                objModel.SelectedPayDate = PayDate==""?objModel.PayDate.ToString("dd/MM/yyyy"):PayDate;
                DataTable dtOutPut = objRepo.GetInputFileReportData(objModel);
                ret.Data = JsonSerializer.SerializeTable(dtOutPut);
                ret.IsSuccess = true;
                return Json(ret, JsonRequestBehavior.AllowGet);

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
        [Route("PayrollInputDownload", Name = "PayrollInputDownload")]
        public ActionResult PayrollInputDownload(int  File_ID, int SubFile_ID,string Pay_Date)
        {
            try
            {
                PayrollInputFileReportModel objModel = new PayrollInputFileReportModel();
                PayrollInputReportRepo objRepo = new PayrollInputReportRepo();

                objModel.File_ID = File_ID;
                objModel.SubFile_ID = SubFile_ID;
                objModel.PayDate = DateTime.ParseExact(Pay_Date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                objModel.EID = IvapUser.EID;
                DataTable dtInputDownload= objRepo.GetInputFileReportDownload(objModel);
                //Now Replace Master valued with TIDs
                string FileName = ExcellUtils.DataTableToExcel(dtInputDownload);
                FileName = FileName.Replace("/", "").Replace("..", "").Replace("\\", "");
                string FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + FileName;
                byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "PayrollInputResult" + ".xlsx");
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                throw;
            }
        }
    }
}
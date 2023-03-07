using Ivap.ActionFilters;
using Ivap.Areas.InputProcessing.Models;
using Ivap.Areas.InputProcessing.Repository;
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

namespace Ivap.Areas.InputProcessing.Controllers
{
    [CustomAuthActionFilter(Order = 0)]
    [RouteArea("InputProcessing", AreaPrefix = "")]
    [RoutePrefix("InputProcessing")]
    public class PayrollProcessingController : IvapBaseController
    {
        // GET: InputProcessing/PayrollProcessing
        [ViewAction]
        [Route("PayRollProcessing", Name = "PayRollProcessing")]
        public ActionResult PayRollProcessing()
        {
            return View("PayrollProcessing");
        }

        [ViewAction]
        [Route("GetPayRoll", Name = "GetPayRoll")]
        public ActionResult GetRequest()
        {
            Response res = new Response();
            DataTable dt = new DataTable();
            PayrollProcessingModel model = new PayrollProcessingModel();
            PayrollProcessingRepo ObjRepo = new PayrollProcessingRepo();
            model.CreatedBy = IvapUser.UID;
            int EID = IvapUser.EID;
            string TableName = "IVAP_MAST_TEMP_" + EID;
            dt = ObjRepo.GetPayRoll(TableName, model.CreatedBy);
            res.Data = JsonSerializer.SerializeTable(dt);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [ViewAction]
        [HttpGet]
        [Route("FileDownload_Pay", Name = "FileDownload_Pay")]
        public ActionResult FileDownload_Pay(int FILE_ID)
        {
            try
            {
                int EID = IvapUser.EID;

                DataTable dt = new DataTable();
                MyRequestRepo ObjRepo = new MyRequestRepo();
                dt = ObjRepo.FileDownload(EID, FILE_ID,"APPROVED");
                //return dt;
                string FileName = ExcellUtils.DataTableToExcel(dt);
                FileName = FileName.Replace("/", "").Replace("..", "").Replace("\\", "");
                string FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + FileName;
                byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "Upload_Input_Data_PayRoll" + ".xlsx");
            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }
}
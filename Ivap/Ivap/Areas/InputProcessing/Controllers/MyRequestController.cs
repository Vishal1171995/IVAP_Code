using Ivap.ActionFilters;
using Ivap.Areas.Configuration.Repository;
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
    public class MyRequestController : IvapBaseController
    {
        // GET: InputProcessing/MyRequest
        [ViewAction]
        [Route("MyRequest", Name = "MyRequest")]
        public ActionResult MyRequest()
        {
           

            return View("MyRequest");
        }
        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("SendToApproved", Name = "SendToApproved")]
        public ActionResult SendToApproved(int FILE_ID)
        {
            Response ret = new Response();
            ApprovedRepo ObjApp = new ApprovedRepo();
            try
            {
                int Result = 0;
                int EID = IvapUser.EID;
                int ROLE = IvapUser.Role;
                string Status = "Awatting Cleint Approval";
                if (ROLE == 8)
                {
                    Status = "Approved";
                }
                Result = ObjApp.SendToApproved(FILE_ID, EID,Status);
                if (Result == 1) { ret.IsSuccess = true; }
                else { ret.IsSuccess = false; }
                ret.Message = "Approved Successfully";

                return Json(ret, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                throw;
            }
        }

        [ViewAction]
        [Route("GetRequest", Name = "GetRequest")]
        public ActionResult GetRequest()
        {
            Response res = new Response();
            DataTable dt = new DataTable();
            MyRequestRepo ObjRepo = new MyRequestRepo();
            int EID = IvapUser.EID;
            int ROLE = IvapUser.Role;
            string Status = "Awaiting Approval";
            if (ROLE == 8 )
            {
                Status = "Awatting Cleint Approval";
            }
            string TableName = "IVAP_MAST_TEMP_" + EID;
            dt = ObjRepo.MyRequestFile(TableName,IvapUser.UID, Status);
            res.Data = JsonSerializer.SerializeTable(dt);
            return Json(res, JsonRequestBehavior.AllowGet);
        }


        [ViewAction]
        [HttpGet]
        [Route("FileDownload", Name = "FileDownload")]
        public ActionResult FileDownload(int FILE_ID)
        {
            try { 
            int EID = IvapUser.EID;

            DataTable dt = new DataTable();
            MyRequestRepo ObjRepo = new MyRequestRepo();
            int ROLE = IvapUser.Role;
            string Status = "Awaiting Approval";
            if (ROLE == 8)
            {
                Status = "Awatting Cleint Approval";
            }
                dt = ObjRepo.FileDownload(EID, FILE_ID, Status);
            //return dt;
            string FileName = ExcellUtils.DataTableToExcel(dt);
            FileName = FileName.Replace("/", "").Replace("..", "").Replace("\\", "");
            string FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + FileName;
            byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet,"Upload_Input_Data" +".xlsx");
        }
            catch (Exception ex)
            {
                throw;
            }
        }



    }
}
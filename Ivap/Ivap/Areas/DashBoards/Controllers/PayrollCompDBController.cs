using Ivap.ActionFilters;
using Ivap.Areas.DashBoards.Repository;
using Ivap.Areas.InputProcessing.Repository;
using Ivap.Areas.PayrollOutput.Repository;
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
using static Ivap.Areas.DashBoards.Repository.DashBoardRepo;

namespace Ivap.Areas.DashBoards.Controllers
{
    [CustomAuthActionFilter(Order = 0)]
    [RouteArea("DashBoards", AreaPrefix = "")]
    [RoutePrefix("DashBoard")]
    public class PayrollCompDBController : IvapBaseController
    {
        // GET: DashBoards/PayrollCompDB
        // GET: Reports/WarehouseGetPayrollSummary
        [Route("PayrollCompDB", Name = "PayrollCompDB")]
        public ActionResult PayrollCompDB()


        {
            DashBoardRepo objRepo = new DashBoardRepo();
            DataSet ds = new DataSet();
            try
            {
                ViewBag.Paydate = string.Format("{0:dd-MMM-yyyy}", IvapUser.PayDate);
                ViewBag.RoleID = IvapUser.Role;
                ds = objRepo.GetFileTypeDB(IvapUser.EID, IvapUser.UID, IvapUser.RoleName, string.Format("{0:dd-MMM-yyyy}", IvapUser.PayDate));
                return View(ds);
            }
            catch
            {
                return View("~/Views/Account/DummyDashBoard.cshtml");
            }

        }

        [Route("GetPayComponentDB", Name = "GetPayComponentDB")]
        public ActionResult GetPayComponentDB(int FileTypeID, int Duration)
        {
            Response res = new Response();
            DashBoardRepo payrepo = new DashBoardRepo();
            DataSet ds = new DataSet();
            try
            {

                List<SeriesData> objList = new List<SeriesData>();
                var Res = payrepo.GetPayComponentDB(FileTypeID, IvapUser.EID, IvapUser.RoleName, IvapUser.UID, string.Format("{0:yyyy-MM-dd}", IvapUser.PayDate), Duration);
                var data = JsonSerializer.SerializeObject(Res);
                return Json(data, JsonRequestBehavior.AllowGet);
                //return Res;
            }
            catch
            {
                res.IsSuccess = false;
                return Json(res, JsonRequestBehavior.AllowGet);
            }

        }

        [Route("GetCTCReconcilationDB", Name = "GetCTCReconcilationDB")]
        public ActionResult GetCTCReconcilationDB()
        {
            Response res = new Response();
            DashBoardRepo payrepo = new DashBoardRepo();
            DataSet ds = new DataSet();
            DateTime LastMonthDay = IvapUser.PayDate.AddMonths(-1);
            int LastDays = DateTime.DaysInMonth(LastMonthDay.Year, LastMonthDay.Month);
            string PreviousMonth = new DateTime(LastMonthDay.Year, LastMonthDay.Month, LastDays).ToString("yyyy/MM/dd");
            try
            {
                ds = payrepo.GetCTCReconcilationDB(IvapUser.EID, IvapUser.PayDate.ToString("yyyy/MM/dd"), PreviousMonth);
                res.Data = JsonSerializer.SerializeObject(ds);
                res.IsSuccess = true;
                return Json(res, JsonRequestBehavior.AllowGet);


            }
            catch
            {
                res.IsSuccess = false;
                return Json(res, JsonRequestBehavior.AllowGet);
            }

        }



        [Route("GetHeadCountDB", Name = "GetHeadCountDB")]
        public ActionResult GetHeadCountDB()
        {
            Response res = new Response();
            DashBoardRepo payrepo = new DashBoardRepo();
            DataSet ds = new DataSet();
            try
            {
                DateTime LastMonthDay = IvapUser.PayDate.AddMonths(-1);
                int LastDays = DateTime.DaysInMonth(LastMonthDay.Year, LastMonthDay.Month);
                string PreviousMonth = new DateTime(LastMonthDay.Year, LastMonthDay.Month, LastDays).ToString("yyyy/MM/dd");
                ds = payrepo.GetHeadCountDB(IvapUser.EID, string.Format("{0:yyyy-MM-dd}", IvapUser.PayDate), PreviousMonth);
                res.Data = JsonSerializer.SerializeObject(ds);
                res.IsSuccess = true;
                return Json(res, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                res.IsSuccess = false;
                return Json(res, JsonRequestBehavior.AllowGet);
            }

        }

        [ViewAction]
        [HttpGet]
        [Route("DownLoadDataPayrollTrend", Name = "DownLoadDataPayrollTrend")]
        public ActionResult DownLoadDataPayrollTrend(int FILETYPEID)
        {
            DashBoardRepo payrepo = new DashBoardRepo();
            DataTable dt = new DataTable();
            try
            {
                dt = payrepo.DownLoadDataPayrollTrend(IvapUser.EID, string.Format("{0:yyyy-MM-dd}", IvapUser.PayDate), IvapUser.RoleName, FILETYPEID, IvapUser.UID);
                string FileName = ExcellUtils.DataTableToExcel(dt);
                FileName = FileName.Replace("/", "").Replace("..", "").Replace("\\", "");
                string FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + FileName;
                byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "Data.xlsx");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [ValidateAntiForgeryToken]
        [ADDUpdateAction]
        [HttpPost]
        [Route("Input_File_Approve", Name = "Input_File_Approve")]
        public ActionResult Input_File_Approve(int FILETYPEID, string Remarks)
        {
            DashBoardRepo payrepo = new DashBoardRepo();
            DataTable dt = new DataTable();
            try
            {
                InPutFileMovementRepo Obj = new InPutFileMovementRepo();
                var Res = Obj.SetInputWorkFlow(IvapUser.UID, IvapUser.Role, FILETYPEID, IvapUser.EID, Remarks, IvapUser.PayDate);
                return Json(Res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("Input_File_Download", Name = "Input_File_Download")]
        public ActionResult Input_File_Download(int FILETYPEID)
        {
            DashBoardRepo payrepo = new DashBoardRepo();
            DataTable dt = new DataTable();
            try
            {
                InPutFileMovementRepo Obj = new InPutFileMovementRepo();
                dt = Obj.Download(IvapUser, FILETYPEID);
                string FileName = ExcellUtils.DataTableToExcel(dt);
                FileName = FileName.Replace("/", "").Replace("..", "").Replace("\\", "");
                string FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + FileName;
                byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                string DownloadFileName = IvapUser.PayDate.ToString().Replace("/", "") + "_Result.xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, DownloadFileName);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [ValidateAntiForgeryToken]
        [ADDUpdateAction]
        [HttpPost]
        [Route("Input_File_Reconsider", Name = "Input_File_Reconsider")]
        public ActionResult Input_File_Reconsider(int FILETYPEID, string Remarks)
        {
            DashBoardRepo payrepo = new DashBoardRepo();
            DataTable dt = new DataTable();
            try
            {
                InPutFileMovementRepo Obj = new InPutFileMovementRepo();
                var Res = Obj.Reconsider(IvapUser.UID, IvapUser.Role, FILETYPEID, IvapUser.EID, Remarks, IvapUser.PayDate);
                return Json(Res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [ValidateAntiForgeryToken]
        [ADDUpdateAction]
        [HttpPost]
        [Route("Output_File_Approve", Name = "Output_File_Approve")]
        public ActionResult Output_File_Approve(int FILETYPEID, string Remarks)
        {
            DashBoardRepo payrepo = new DashBoardRepo();
            DataTable dt = new DataTable();
            try
            {
                PayrollOutPutFileMovementRepo Obj = new PayrollOutPutFileMovementRepo();
                var Res = Obj.SetOutputWorkFlow(IvapUser.UID, IvapUser.Role, FILETYPEID, IvapUser.EID, Remarks, IvapUser.PayDate);
                return Json(Res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [ValidateAntiForgeryToken]
        [ADDUpdateAction]
        [HttpPost]
        [Route("Output_File_Reconsider", Name = "Output_File_Reconsider")]
        public ActionResult Output_File_Reconsider(int FILETYPEID, string Remarks)
        {
            DashBoardRepo payrepo = new DashBoardRepo();
            DataTable dt = new DataTable();
            try
            {
                PayrollOutPutFileMovementRepo Obj = new PayrollOutPutFileMovementRepo();
                var Res = Obj.Reconsider(IvapUser.UID, IvapUser.Role, FILETYPEID, IvapUser.EID, Remarks, IvapUser.PayDate);
                return Json(Res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("Output_File_Download", Name = "Output_File_Download")]
        public ActionResult Output_File_Download(int FILETYPEID)
        {
            DashBoardRepo payrepo = new DashBoardRepo();
            DataTable dt = new DataTable();
            try
            {
                PayrollOutPutFileMovementRepo Obj = new PayrollOutPutFileMovementRepo();
                dt = Obj.Download(IvapUser, FILETYPEID);
                string FileName = ExcellUtils.DataTableToExcel(dt);
                FileName = FileName.Replace("/", "").Replace("..", "").Replace("\\", "");
                string FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + FileName;
                byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                string DownloadFileName = IvapUser.PayDate.ToString().Replace("/", "") + "_Result.xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, DownloadFileName);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [ViewAction]
        [Route("GetCTCReconDataOutput", Name = "GetCTCReconDataOutput")]
        public ActionResult GetCTCReconDataOutput()
        {
            DashBoardRepo payrepo = new DashBoardRepo();
            DataTable dt = new DataTable();
            Response res = new Utils.Response();
            try
            {
                DateTime LastMonthDay = IvapUser.PayDate.AddMonths(-1);
                int LastDays = DateTime.DaysInMonth(LastMonthDay.Year, LastMonthDay.Month);
                string PreviousMonth = new DateTime(LastMonthDay.Year, LastMonthDay.Month, LastDays).ToString("yyyy/MM/dd");

                dt = payrepo.GetCTCReconDataOutput(IvapUser.EID, IvapUser.PayDate.ToString("yyyy/MM/dd"), PreviousMonth);
                res.Data = JsonSerializer.SerializeTable(dt);
                res.IsSuccess = true;
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch {
                res.IsSuccess = false;
                return Json(res, JsonRequestBehavior.AllowGet);
            }
        }

        [ViewAction]
        [Route("GetHeadCountDataOutput", Name = "GetHeadCountDataOutput")]
        public ActionResult GetHeadCountDataOutput()
        {
            DashBoardRepo payrepo = new DashBoardRepo();
            DataTable dt = new DataTable();
            Response res = new Utils.Response();
            try
            {
                DateTime LastMonthDay = IvapUser.PayDate.AddMonths(-1);
                int LastDays = DateTime.DaysInMonth(LastMonthDay.Year, LastMonthDay.Month);
                string PreviousMonth = new DateTime(LastMonthDay.Year, LastMonthDay.Month, LastDays).ToString("yyyy/MM/dd");

                dt = payrepo.GetHeadCountDataOutput(IvapUser.EID, IvapUser.PayDate.ToString("yyyy/MM/dd"), PreviousMonth);
                res.Data = JsonSerializer.SerializeTable(dt);
                res.IsSuccess = true;
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch {
                res.IsSuccess = false;
                return Json(res, JsonRequestBehavior.AllowGet);
            }
        }


        [ViewAction]
        [Route("GetExceptionDataOutput", Name = "GetExceptionDataOutput")]
        public ActionResult GetExceptionDataOutput()
        {
            DashBoardRepo payrepo = new DashBoardRepo();
            DataTable dt = new DataTable();
            Response res = new Utils.Response();
            try
            {
                DateTime LastMonthDay = IvapUser.PayDate.AddMonths(-1);
                int LastDays = DateTime.DaysInMonth(LastMonthDay.Year, LastMonthDay.Month);
                string PreviousMonth = new DateTime(LastMonthDay.Year, LastMonthDay.Month, LastDays).ToString("yyyy/MM/dd");

                dt = payrepo.GetExceptionDataOutput(IvapUser.EID, IvapUser.PayDate.ToString("yyyy/MM/dd"), PreviousMonth);
                res.Data = JsonSerializer.SerializeTable(dt);
                res.IsSuccess = true;
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                res.IsSuccess = false;
                return Json(res, JsonRequestBehavior.AllowGet);
            }
        }


        [ViewAction]
        [Route("GetSDSOutput", Name = "GetSDSOutput")]
        public ActionResult GetSDSOutput()
        {
            DashBoardRepo payrepo = new DashBoardRepo();
            DataTable dt = new DataTable();
            Response res = new Utils.Response();
            try
            {
                DateTime LastMonthDay = IvapUser.PayDate.AddMonths(-1);
                int LastDays = DateTime.DaysInMonth(LastMonthDay.Year, LastMonthDay.Month);
                string PreviousMonth = new DateTime(LastMonthDay.Year, LastMonthDay.Month, LastDays).ToString("yyyy/MM/dd");

                dt = payrepo.GetSDSOutput(IvapUser.EID, IvapUser.PayDate.ToString("yyyy/MM/dd"), PreviousMonth);
                res.Data = JsonSerializer.SerializeTable(dt);
                res.IsSuccess = true;
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch {
                res.IsSuccess = false;
                return Json(res, JsonRequestBehavior.AllowGet);
            }
        }

        [Route("GetOneTimeEarningDeduction", Name = "GetOneTimeEarningDeduction")]
        public ActionResult GetOneTimeEarningDeduction()
        {
            Response res = new Response();
            DashBoardRepo payrepo = new DashBoardRepo();
            DataTable dt = new DataTable();
            try
            {
                DateTime LastMonthDay = IvapUser.PayDate.AddMonths(-1);
                int LastDays = DateTime.DaysInMonth(LastMonthDay.Year, LastMonthDay.Month);
                string PreviousMonth = new DateTime(LastMonthDay.Year, LastMonthDay.Month, LastDays).ToString("yyyy/MM/dd");

                dt = payrepo.GetOneTimeEarningDeduction(IvapUser.EID, string.Format("{0:yyyy-MM-dd}", IvapUser.PayDate), PreviousMonth);
                res.Data = JsonSerializer.SerializeTable(dt);
                res.IsSuccess = true;
                return Json(res, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                res.IsSuccess = false;
                return Json(res, JsonRequestBehavior.AllowGet);
            }

        }

        [Route("GetVariance_Output", Name = "GetVariance_Output")]
        public ActionResult GetVariance_Output()
        {
            Response res = new Response();
            DashBoardRepo payrepo = new DashBoardRepo();
            DataSet ds = new DataSet();
            try
            {

                List<SeriesData> objList = new List<SeriesData>();
                var Res = payrepo.GetVarianceDB_Output(IvapUser.EID, IvapUser.RoleName, IvapUser.UID, string.Format("{0:yyyy-MM-dd}", IvapUser.PayDate));
                var data = JsonSerializer.SerializeObject(Res);
                return Json(data, JsonRequestBehavior.AllowGet);
                //return Res;
            }
            catch
            {
                res.IsSuccess = false;
                return Json(res, JsonRequestBehavior.AllowGet);
            }

        }

    }
}